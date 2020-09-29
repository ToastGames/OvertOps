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
    // HURT: When enemy is damaged in any way, they will pause for a moment and play the hurt animation. After being hurt, enemy will always go straight into Agro state (Hurt not yet implemented)
    //
    // DYING: When health is reduced to 0, swap to this state and play dying animation (Dying not yet implemented)
    //
    // DEAD: After dying state completes, enemy goes into DEAD state. Dead state doesn't do anything, but does remain persistent in the level.
    //       Could also be a good oportunity for the player to be able to interact with the dead bodies in some way? get something from them maybe? (Dead not yet implemented)


    public enum EnemyState { Idle, Patrolling, Agro, Roaming, Shooting, Hurt, Dying, Dead }

    private GameObject enemyPrefab;
    private GameObject spritePlane;
    private EnemyPatrolPath patrolPath;
    private GameObject patrolPathParent;        
    private GameObject playerTarget;            // these 5 things are private because they cannot be assigned in te prefab, and have to be scraped out of the scene by traversing the heirarchy

    public float speed;                         // gameplay variables, tweak to taste
    public float wallcollisionRange;
    public float rayWidth;
    public float turnVariation;
    public float nodeProximityThreshold;

    private bool pathLoop;                      // this batch of private variables are more about making the enemy behaviour actually do it's thing
    private bool pathGoingForward = true;
    private int currentNode = 0;
    private Vector3 currentNodePosition;        // this probably would have mde more sense to call "destination node" --> its the next path node the enemy is moving towards
    private EnemyState state;

    void Start()
    {
        ///////// have to get Game Objects and patrol path manually now that spawner can spawn different enemy types ////////////////
        enemyPrefab = transform.parent.parent.gameObject;
        spritePlane = transform.GetChild(0).gameObject;
        patrolPath = transform.parent.parent.transform.GetChild(1).GetComponent<EnemyPatrolPath>();

        patrolPathParent = GameObject.FindGameObjectWithTag("Patrol-Paths_Parent");                     // The patrol paths for all enemies actually get detached and reparented to a master parent in the scene (I cannot remember why)
        patrolPath.gameObject.transform.SetParent(patrolPathParent.transform);                          // so after finding the parent in the line above, it is then attached to it here

        playerTarget = GameObject.FindGameObjectWithTag("Player");                                      // Set the player as the enemy target
        pathLoop = patrolPath.loop;                                                                     // if the path loops or not has to be derived from another object, so it's grabbed here
        state = EnemyState.Patrolling;                                                                  // Set initial STATE to PATROLLING
        currentNodePosition = patrolPath.pathNodes[currentNode].transform.position;                     // set "current position" -> the nex path to move to as the first node in the node path position list
    }

    void Update()
    {
        if (state == EnemyState.Roaming) // ROAMING //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);      // just move forward a bit (determined by speed value), that's literally all roam does
            DetectWall();                                                                   // this is the function that handles the "pong like" behaviour
        }
        else if (state == EnemyState.Patrolling) // PATROLLING ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            Reorient(currentNodePosition - enemyPrefab.transform.position);                 // make sure the enemy is always facing the direction they are walking towards (transform, not sprite)
            enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);      // move enemy a bit (determined by speed value) along their forward vector (why reorienting first is important)

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

                currentNodePosition = patrolPath.pathNodes[currentNode].transform.position; // grab the actual position Vec3 out of the path node game object
            }
                                // only check for player in PATROLLING state
            CheckForPlayer();   // check line if sight between enemy and player is free of walls

            // END of EPIC IF statement to determine where to go next on the path
        }

        spritePlane.transform.LookAt(playerTarget.transform, Vector3.up);                                       // billboard sprite object to player
        spritePlane.transform.eulerAngles = new Vector3(0.0f, spritePlane.transform.eulerAngles.y, 0.0f);       // zero out X and Z angles after billboarding so enemy sprite stays aligned to the vertical ( Y ) axis
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

    void CheckForPlayer()
    {
        RaycastHit hitInfo; // local variable to store raycast hit info in

        Debug.DrawLine(transform.position + Vector3.up, playerTarget.transform.position + Vector3.up, Color.magenta);
        if (Physics.Raycast(transform.position + Vector3.up, (playerTarget.transform.position - transform.position) + Vector3.up, out hitInfo))                       // check the LEFT ray for colission
        {
            //Debug.Log(hitInfo.transform.gameObject.name);
            if (hitInfo.transform.gameObject.tag == "Wall")
            {
                // do nothing?
            }
            else
            {
                //Debug.Log("######################");
                state = EnemyState.Roaming;
            }
        }
        else
        {
            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!");
            state = EnemyState.Roaming;
        }
    }

        void DetectWall()       // Does all the heavy lifting for RAOM state                  
    {
        RaycastHit hitInfo; // local variable to store raycast hit info in

        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f);                      // constructing a position that is LEFT of the enemy, by half the "width" of the enemy
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f);                          // same for the RIGHT
        offsetLeft = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetLeft;    // construct a vector that starts at the LEFT offset, and points in the same direction as the enemy is facing
        offsetRight = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetRight;  // same for the RIGHT (after rotating, these are then just stored in the same original variables)

        Debug.DrawRay(transform.position + offsetLeft, transform.forward * wallcollisionRange, Color.red);          // draw a line from the left offset, in the direction the enemy is facing, as long as the "collision range"
        Debug.DrawRay(transform.position + offsetRight, transform.forward * wallcollisionRange, Color.blue);        // same for right, and colour code them so they can be told apart easily

        if (Physics.Raycast(transform.position + offsetLeft, transform.forward, out hitInfo, wallcollisionRange))                       // check the LEFT ray for colission
        {
            if (hitInfo.collider.gameObject.tag == "Wall")                                                                              // collision is true if ray intersects colliders tagged "Wall"
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallcollisionRange, Color.yellow);    // if colission, draw a debug ray in the reflection direction
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));                                               // reorient enemy to face (and therefore start moving) down the direction of the reflected vector
            }
        }
        if (Physics.Raycast(transform.position + offsetRight, transform.forward, out hitInfo, wallcollisionRange))                      // check the RIGHT ray for colission
        {
            if (hitInfo.collider.gameObject.tag == "Wall")                                                                              // collision is true if ray intersects colliders tagged "Wall"
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallcollisionRange, Color.yellow);    // if colission, draw a debug ray in the reflection direction
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));                                               // reorient enemy to face (and therefore start moving) down the direction of the reflected vector
            }                                                                                                                           // YES, I know these two blocks of code are identical, and there's probably some more efficient
        }                                                                                                                               //      way of doing the same thing with both rays and only one block of code, but whatever
    }

    void Reorient(Vector3 newDirection)     // custom function to "reorient" the enemy prefab (rotate it to face down a new direction) because it happens all the time
    {
        float newVariation = Random.Range(-turnVariation, turnVariation);                                   // There is actually some random variation applied to the reorientation, +- some max bounds
                                                                                                            //   I guess really this probably shouldn't technically happen INSIDE the function, but that's how it is for now
        enemyPrefab.transform.rotation = Quaternion.LookRotation(newDirection);                             // rotate enemy to look EXACTLY at its new target
        enemyPrefab.transform.Rotate(0.0f, newVariation, 0.0f);                                             // this line here is where the additional rotation variation is applied

        enemyPrefab.transform.eulerAngles = new Vector3(0.0f, enemyPrefab.transform.eulerAngles.y, 0.0f);   // zero out X and Z angles after billboarding so enemy sprite stays aligned to the vertical ( Y ) axis
    }


    /////////////////////////// This function purely exists to draw the debug rays at editor time and is an exact copy and paste of the code above
    /////////////////////////// (again, it's really bad to dupe code like this, so at some point I should probably turn this into a fucntion that gets called from here and from the collision detection

    private void OnDrawGizmos()
    {
        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f);
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f);
        offsetLeft = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetLeft;
        offsetRight = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetRight;

        Debug.DrawRay(transform.position + offsetLeft, transform.forward * wallcollisionRange, Color.red);
        Debug.DrawRay(transform.position + offsetRight, transform.forward * wallcollisionRange, Color.blue);

        ////// Draw line between enemy and player (this is the ray that will be checked for line of sight               // ALSO, probably not going to do shit because the enemies only spawn at runtime now
        Debug.DrawLine(transform.position + Vector3.up, playerTarget.transform.position + Vector3.up, Color.magenta);   // ok, it does, but only at run time, so there are double magenta lines...
    }

}
