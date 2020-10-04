using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    // This is the ememy created for the game. It is a starting point of the "main" sort of basic behaviours we will want most (maybe all) enemies to have
    // all other enemies will be base off this one, but not using inheretence. I know that would be doing it properly, but I don't want to go quite that far outside my comfort zone
    //
    // This enemy has 3 main state;
    //
    // PATROLLING: This is the state it starts in, and won't come back to this state once it leaves it
    //             when patrolling, the enemy will follow a set path of nodes. patrol paths can be loop or ping pong (just denoted as "not loop"
    //
    // AGRO: When the enemy is agro, it will make a bee line for the player, periodically stopping to shoot
    //       If the enemy loses line of sight on the player, then the state goes from Agro to Roam (Agro is not yet impllemented)
    //
    // ROAM: When roaming, the enemy is basically the DVD screen saver, ping ponging around the level.
    //       If the enemy gets line of sight on the player while roaming, then state will change to agro
    //
    // Other States:
    //
    // IDLE: Honestly not sure if idle will be used or not. Will probably mix up the "Roam" state by having the enemy periodically stop and Idle for a bit (Idle not yet implemented)
    //
    // SHOOTING: When in Agro mode, enemy will periodically stop to shoot at player (Shooting not yet implemented)
    //
    // MELEE: If within a certain range, will just stop still and start wailing on the player regardless of whatever else was going on (Melee not yet implemented)
    //
    // HURT: When enemy is damaged in any way, they will pause for a moment and play the hurt animation. After being hurt, enemy will always go straight into Agro state (Hurt not yet implemented)
    //
    // DYING: When health is reduced to 0, swap to this state and play dying animation (Dying not yet implemented)
    //
    // DEAD: After dying state completes, enemy goes into DEAD state. Dead state doesn't do anything, but does remain persistent in the level.
    //       Could also be a good oportunity for the player to be able to interact with the dead bodies in some way? get something from them maybe? (Dead not yet implemented)


    public enum EnemyState { Idle, Patrolling, Agro, Roaming, Shooting, Melee, Hurt, Dying, Dead }

    private GameObject enemyPrefab;
    private GameObject spritePlane;
    private EnemyPatrolPath patrolPath;
    private GameObject patrolPathParent;        
    private GameObject playerTarget;            // these 5 things are private because they cannot be assigned in te prefab, and have to be scraped out of the scene by traversing the heirarchy

    public GameObject shootParticles;

    public float wallCollisionRange;            // gameplay variables, tweak to taste
    public float rayWidth;
    public float turnVariation;
    public float nodeProximityThreshold;
    public float viewAngleThreshold;            // this is the "are they looking at me" angle for ilne of sight
    public float viewRadius;
    public float minimumAlertRadius;
    public float forwardAngle = 20.0f;          // pre loading these with some defaults
    public float backAngle = 90.0f;

    ////////////////////////////////

    public int shootDamage;
    public int meleeDamage;

    ////////////////////////////////

    public float patrolSpeed;
    public float roamSpeed;
    public float agroSpeed;
    public float agroShootCyclePeriod;          // float to mod time by to alternate between standing+shooting and walking towards player

    private float speed;                        // speed will change based on state
    private bool pathLoop;                      // this batch of private variables are more about making the enemy behaviour actually do it's thing
    private bool pathGoingForward = true;
    private int currentNode = 0;
    private Vector3 currentNodePosition;        // this probably would have mde more sense to call "destination node" --> its the next path node the enemy is moving towards
    private EnemyState state;
    private float angleToPlayer = 0.0f;
    private int curentAnimation = 1;

    private float stateTimePassed = 0.0f;
    private bool canShoot = false;

    void Start()
    {
        ///////// have to get Game Objects and patrol path manually now that spawner can spawn different enemy types ////////////////
        enemyPrefab = transform.parent.parent.gameObject;
        enemyPrefab.transform.rotation = transform.rotation;
        spritePlane = transform.GetChild(0).gameObject;
        patrolPath = transform.parent.parent.transform.GetChild(1).GetComponent<EnemyPatrolPath>();

        patrolPathParent = GameObject.FindGameObjectWithTag("Patrol-Paths_Parent");                     // The patrol paths for all enemies actually get detached and reparented to a master parent in the scene (I cannot remember why)
        patrolPath.gameObject.transform.SetParent(patrolPathParent.transform);                          // so after finding the parent in the line above, it is then attached to it here

        playerTarget = GameObject.FindGameObjectWithTag("Player");                                      // Set the player as the enemy target
        pathLoop = patrolPath.loop;                                                                     // if the path loops or not has to be derived from another object, so it's grabbed here
        state = EnemyState.Patrolling;                                                                  // Set initial STATE to PATROLLING
        //state = EnemyState.Roaming;
        currentNodePosition = patrolPath.pathNodes[currentNode].transform.position;                     // set "current position" -> the nex path to move to as the first node in the node path position list
        speed = patrolSpeed;
    }

    void Update()
    {
        CalculateAngleToPlayer();
        UpdateAnimation();

        ///////////////////////////////////////////////
        //         STATE specific behaviour          //
        ///////////////////////////////////////////////

        if (state == EnemyState.Roaming) // ROAMING //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);      // just move forward a bit (determined by speed value), that's literally all roam does
            DetectWall();                   // ^ why is this Vector3 and not transform?     // this is the function that handles the "pong like" behaviour
            CheckForPlayer();               // check line if sight between enemy and player is free of walls
        }
        else if (state == EnemyState.Agro)  // AGRO //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            stateTimePassed += Time.deltaTime;
            if (stateTimePassed > (agroShootCyclePeriod / 2))
                canShoot = true;

            if ((Time.time % agroShootCyclePeriod) < (agroShootCyclePeriod / 2))            // periodically alternate between 2 sub-states (shooting and moving)
            {
                //Debug.Log("111111111111111");
                Reorient(playerTarget.transform.position - enemyPrefab.transform.position);                   // make sure the enemy is always facing the PLAYER (transform, not sprite)
                enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);                    // just move forward a bit (determined by speed value)
                transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_AnimationNumber", 1);      // change to walking straight forward anim
            }
            else
            {
                //Debug.Log("222222222222222");
                transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_AnimationNumber", 6);      // change to shooting anim

                if (canShoot)
                {
                    Instantiate(shootParticles, enemyPrefab.transform.position, Quaternion.identity);
                    playerTarget.GetComponent<Player>().health -= shootDamage;
                    stateTimePassed = 0.0f;
                    canShoot = false;
                }
            }
            if (CheckForPlayer() == false)
                state = EnemyState.Roaming;
        }
        else if (state == EnemyState.Patrolling) // PATROLLING ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            Reorient(currentNodePosition - enemyPrefab.transform.position);                               // make sure the enemy is always facing the direction they are walking towards (transform, not sprite)
            enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);                    // move enemy a bit (determined by speed value) along their forward vector (why reorienting first is important)
                                            // ^ why is this Vector3 and not transform?
            // this IF statement is a funcking nightmare to understand. It's all just to pick the next path node

            if (Mathf.Abs(Vector3.Magnitude(enemyPrefab.transform.position - currentNodePosition)) < nodeProximityThreshold)  // it's fired off if the enemy gets close enough (withing "proximity" of the node)
            {
                if (currentNode == patrolPath.pathNodes.Count - 1)  // if we have hit the end of the path (reached the final node in the list)
                {
                    if (pathLoop)                                   // if it's a looping path,
                        currentNode = 0;                            // then the next node is the first one in the list
                    else
                    {                                               // however, it it's not a looping path
                        pathGoingForward = false;                   // then we need to set the path traversal direction to backwards
                        currentNode = currentNode - 1;              // and set the destination node to be the one prior to the final node in the list
                    }
                }
                else                                                // if however, we have not yet hit the end of the path
                {
                    if (pathGoingForward)                           // and we are traversing the path in the forward direction
                        currentNode++;                              // then our next node will be the current node +1
                    else
                    {                                               // if however, we are traversing the path backwards
                        if (currentNode != 0)                       // and we haven't yet hit the BEGINNING of the path (and therefore need to turn around)
                            currentNode--;                          // then the next node to move to will be the current node -1
                        else
                        {                                           // BUT, if we HAVE hit the first node in the list (and we are going backwards remember)
                            currentNode = 1;                        // then the next node we want to move to is the second in the list (because looping isn't an option going backwards)
                            pathGoingForward = true;                // and we are now traversing the path forwards once more.
                        }
                    }
                }
                currentNodePosition = patrolPath.pathNodes[currentNode].transform.position;     // grab the actual position Vec3 out of the path node game object
            }
            // only check for player in PATROLLING state
            CheckForPlayer();   // check line if sight between enemy and player is free of walls

            // END of EPIC IF statement to determine where to go next on the path
        }

        //spritePlane.transform.LookAt(playerTarget.transform, Vector3.up);                                       // billboard sprite object to player
        //spritePlane.transform.eulerAngles = new Vector3(0.0f, spritePlane.transform.eulerAngles.y, 0.0f);       // zero out X and Z angles after billboarding so enemy sprite stays aligned to the vertical ( Y ) axis

        // instead of BILLBOARDING TO THE PLAYER, we want him PERPENDICULAR TO THE PLAYER
        spritePlane.transform.eulerAngles = new Vector3(0.0f, playerTarget.transform.eulerAngles.y, 0.0f);
    }

    private void OnTriggerEnter(Collider other)                             // Doesn't do anything yet, but we're going to need this when it comes to getting shot and damaging the player etc
    {
        /*
        if (other.tag == "Player")
            other.gameObject.GetComponent<Player>().isDead = true;

        if (other.tag == "PlayerProjectile")
            Kill();
        */
    }

    public void Kill()                                                      // This function is also never called, it's here as a legacy from when I copy and pasted the initial basic enemy behaviour from Shape Wars
    {
        Destroy(gameObject);                                                // Maybe we want to just turn it off rather than destroy it?
    }

    void UpdateAnimation()
    {
        // ALL THE NUMBERS (angles) ARE HARD CODED AS FUCK.......... PROBABLY FIX THIS AS SOME POINT

        if ((angleToPlayer > -forwardAngle) && (angleToPlayer < forwardAngle))
        {
            curentAnimation = 1;
            transform.GetChild(0).localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }
        else if ((angleToPlayer > -backAngle) && (angleToPlayer < -forwardAngle))
        {
            curentAnimation = 2;
            transform.GetChild(0).localScale = new Vector3(-2.0f, 2.0f, 2.0f);      // X flipped version of anim 2
        }
        else if ((angleToPlayer < 180.0f) && (angleToPlayer > backAngle))
        {
            curentAnimation = 3;
            transform.GetChild(0).localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }
        else if ((angleToPlayer < backAngle) && (angleToPlayer > forwardAngle))
        {
            curentAnimation = 2;
            transform.GetChild(0).localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }
        else if ((angleToPlayer > -180.0f) && (angleToPlayer < -backAngle))
        {
            curentAnimation = 3;
            transform.GetChild(0).localScale = new Vector3(-2.0f, 2.0f, 2.0f);      // X flipped version of anim 3
        }

        transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_AnimationNumber", curentAnimation);
    }

    void CalculateAngleToPlayer()
    {
        // honestly, this conversion of dot product to angle came from here:
        // https://community.khronos.org/t/deriving-angles-from-0-to-360-from-dot-product/49391/2

        Vector3 rayDirection = Vector3.Normalize(playerTarget.transform.position - enemyPrefab.transform.position);
        Debug.DrawRay(enemyPrefab.transform.position + Vector3.up, rayDirection * viewRadius, Color.magenta);
        Debug.DrawRay(enemyPrefab.transform.position + Vector3.up, enemyPrefab.transform.forward, Color.cyan);

        float d = Vector3.Dot(rayDirection, enemyPrefab.transform.forward);
        Vector3 c = Vector3.Cross(rayDirection, enemyPrefab.transform.forward);
        float tempAngle = Mathf.Acos(d);
        float tempDir = Vector3.Dot(c, Vector3.up);

        if (tempDir < 0)
            angleToPlayer = -tempAngle;
        else
            angleToPlayer = tempAngle;

        angleToPlayer = (angleToPlayer / Mathf.PI) * 180.0f;

        //Debug.Log(angleToPlayer);
    }


    bool CheckForPlayer()
    {
        RaycastHit hitInfo; // local variable to store raycast hit info in

        //float tempFloat = Vector3.Dot(Vector3.Normalize(playerTarget.transform.position - enemyPrefab.transform.position), enemyPrefab.transform.forward);  // calculate the dot between the enemy facing and the line between the enemy and the player
        Vector3 rayDirection = Vector3.Normalize(playerTarget.transform.position - enemyPrefab.transform.position);
        float distanceToPlayer = Vector3.Magnitude(enemyPrefab.transform.position - playerTarget.transform.position);
        bool playerSeen = false;

        bool rayHit = Physics.Raycast(enemyPrefab.transform.position + Vector3.up, rayDirection, out hitInfo);

        // before checking ANYTHING else, check if the angle (via the dot product) between the enemies view direction and the vector connecting the enemy and the player is less than the threshold
        // and if the player is within the view radius or not

        if (distanceToPlayer < viewRadius)
            if (Mathf.Abs(angleToPlayer) < viewAngleThreshold)
            {
                playerSeen = true;

                if ((rayHit) && (hitInfo.transform.gameObject.tag == "Wall"))  // check line of sight
                {
                    playerSeen = false;
                }
            }

        if (distanceToPlayer < minimumAlertRadius)      // even if there's no line of sight, if you are closer than this, you alert them anyway
            playerSeen = true;

        if (playerSeen)
        {
            state = EnemyState.Agro;                    // This is where a PATROLLING enemy, when seen chenges state to AGRO (enemies cannot ever go back to patrolling)
            speed = agroSpeed;
        }

        return playerSeen;
    }

    void DetectWall()       // Does all the heavy lifting for RAOM state                  
    {
        RaycastHit hitInfo; // local variable to store raycast hit info in

        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f) + Vector3.up;                      // constructing a position that is LEFT of the enemy, by half the "width" of the enemy
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f) + Vector3.up;                          // same for the RIGHT
        offsetLeft = Quaternion.AngleAxis(enemyPrefab.transform.eulerAngles.y, Vector3.up) * offsetLeft;    // construct a vector that starts at the LEFT offset, and points in the same direction as the enemy is facing
        offsetRight = Quaternion.AngleAxis(enemyPrefab.transform.eulerAngles.y, Vector3.up) * offsetRight;  // same for the RIGHT (after rotating, these are then just stored in the same original variables)

        Debug.DrawRay(enemyPrefab.transform.position + offsetLeft, enemyPrefab.transform.forward * wallCollisionRange, Color.red);          // draw a line from the left offset, in the direction the enemy is facing, as long as the "collision range"
        Debug.DrawRay(enemyPrefab.transform.position + offsetRight, enemyPrefab.transform.forward * wallCollisionRange, Color.blue);        // same for right, and colour code them so they can be told apart easily

        if (Physics.Raycast(enemyPrefab.transform.position + offsetLeft, enemyPrefab.transform.forward, out hitInfo, wallCollisionRange))                       // check the LEFT ray for colission
        {
            if (hitInfo.collider.gameObject.tag == "Wall")                                                                              // collision is true if ray intersects colliders tagged "Wall"
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallCollisionRange, Color.yellow);    // if colission, draw a debug ray in the reflection direction
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));                                               // reorient enemy to face (and therefore start moving) down the direction of the reflected vector
            }
        }
        if (Physics.Raycast(enemyPrefab.transform.position + offsetRight, enemyPrefab.transform.forward, out hitInfo, wallCollisionRange))                      // check the RIGHT ray for colission
        {
            if (hitInfo.collider.gameObject.tag == "Wall")                                                                              // collision is true if ray intersects colliders tagged "Wall"
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallCollisionRange, Color.yellow);    // if colission, draw a debug ray in the reflection direction
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));                                               // reorient enemy to face (and therefore start moving) down the direction of the reflected vector
            }                                                                                                                           // YES, I know these two blocks of code are identical, and there's probably some more efficient
        }                                                                                                                               //      way of doing the same thing with both rays and only one block of code, but whatever
    }

    void Reorient(Vector3 newDirection)     // custom function to "reorient" the enemy prefab (rotate it to face down a new direction) because it happens all the time
    {
        float newVariation = Random.Range(-turnVariation, turnVariation);                                   // There is actually some random variation applied to the reorientation, +- some max bounds
                                                                                                            //   I guess really this probably shouldn't technically happen INSIDE the function, but that's how it is for now
        enemyPrefab.transform.rotation = Quaternion.LookRotation(newDirection);                             // rotate enemy to look EXACTLY at its new target

        if (state == EnemyState.Roaming)
            enemyPrefab.transform.Rotate(0.0f, newVariation, 0.0f);                                         // this line here is where the additional rotation variation is applied

        enemyPrefab.transform.eulerAngles = new Vector3(0.0f, enemyPrefab.transform.eulerAngles.y, 0.0f);   // zero out X and Z angles after billboarding so enemy sprite stays aligned to the vertical ( Y ) axis
    }


    /////////////////////////// This function purely exists to draw the debug rays at editor time and is an exact copy and paste of the code above
    /////////////////////////// (again, it's really bad to dupe code like this, so at some point I should probably turn this into a fucntion that gets called from here and from the collision detection

    private void OnDrawGizmos()
    {
        /*
        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f) + Vector3.up;
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f) + Vector3.up;
        offsetLeft = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetLeft;
        offsetRight = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetRight;

        Debug.DrawRay(enemyPrefab.transform.position + offsetLeft, enemyPrefab.transform.forward * wallCollisionRange, Color.red);
        Debug.DrawRay(enemyPrefab.transform.position + offsetRight, enemyPrefab.transform.forward * wallCollisionRange, Color.blue);

        ////// Draw line between enemy and player (this is the ray that will be checked for line of sight               // ALSO, probably not going to do shit because the enemies only spawn at runtime now
        Debug.DrawLine(transform.position + Vector3.up, playerTarget.transform.position + Vector3.up, Color.magenta);   // ok, it does, but only at run time, so there are double magenta lines...
        */
    }

}
