using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /////////////////////////
    // NEEDS COMMENTS PASS //
    /////////////////////////

    public enum PlayerState { Idle, Shooting, Reloading, Walking, Hurt, Dying, Dead }       // different player states. not all are used yet, and more may be added later

    public GameObject testObject;

    private PlayerState playerState;                // self explanatory

    private GameManager gameManager;                // basically everything needs a reference to the game manager

    private CharacterController playerController;   // component scraped off object --> this basically handles all of our collision with colliders, but outside of the physics system

    public WeaponDefList wepDefs;                   // list of all weapon definitions (currently not being used for anything)
    public MeshRenderer playerSpriteObject;         // this should be obtained from dragging the child object with the spritesheet on it into this variable

    public int currentWeapon = 1;                   // weapons are -1 = Hackerman
                                                    //              0 = Melee
                                                    //              1 = Pistol
                                                    //              2 = Shotgun
                                                    //              3 = Plasma
                                                    //              4 = Rocket
                                                    //              5 = Chaingun (secret - not a regular pick up)

    private bool justFired = false;                 // used to start a timer from the moment the player shoots to check for collisions from this point onwards
    private bool firing = false;                    // bools to manage the state of the current phase of firing
    private bool canFire = true;                    // bools to manage the state of the current phase of firing
    private float timePassed = 0.0f;                // timer used to manage the state of the current phase of firing <-- this really needs a better variable name, as it does not indicate that it is for timing "firing"

    public float moveSpeed;                         // tweakable values for all the player's various movement speeds
    public float strafeSpeed;                       //
    public float rotSpeed;                          //
    public float mouseRotSpeed;                     //

    public int health = 100;                        // player start health

    private Vector3 moveVector = new Vector3();     // zero out a new vector for use in movement calculations
    private float LRMovement = 0.0f;                // these variables are going to be used to hold the input from movement axes
    private float FBMovement = 0.0f;                //
    private float YRotation = 0.0f;                 //
    private float mouseRotation = 0.0f;             //

    //public int rayCount = 10; // not actually used, this was going to be used for how many times the raycast is checked for shooting, but not it's done every frame over a duration. probably delete this

    //public float rayCheckLength;  // delete these too
    //public float collisionWidth;
    //public float rayArcWidth;

    [HideInInspector] public float currentMovementSpeed;            // public but hidden in the inspector so the gunSine script can access it
    [HideInInspector] public bool inHackerman = false;              // if the player is in hackerland also needs to be accessible from other scripts

    private float hackermanTimer = 0.0f;                            // how long has the player been in hackerland
    public float hackermanDuration;                                 // how long SHOULD the player stay in hackerland <-- this value will get modified by powerups
    [HideInInspector] public Vector3 terminalReturnPosition;        // where to dump the player back out after returning from hackerland (public so it can be set by the terminal)
    [HideInInspector] public Quaternion terminalReturnRotation;     // where to dump the player back out after returning from hackerland (public so it can be set by the terminal)
    private GameObject hackermanPickupObjects;                      // a reference to the hackerman objects so they can be turned off when not in hackerland (otherwise they can bee seen through the walls of the real world)


    [HideInInspector] public bool interactPressed = false;          // ARE YOU PRESSING F?

    public List<AudioClip> weaponSounds = new List<AudioClip>();    // list of weapon sounds
    private AudioSource playerAudio;                                // audio source for playing sounds (scraped off the player object)

    ////////////////////////////////////////////// Weapon Stuff <-- Replace with WEAPON DEF stuff

    public GameObject shotFXPistol;
    //public GameObject shotFXShotgun;
    //public GameObject shotFXPlasma;
    //public GameObject shotFXRocket;
    //public GameObject shotFXChaingun;

    public float pistolFireTime;


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // variables below this line need reassessing ///////////////////////////////////////////////////////////
    // (they currently aren't used for anything)

    private float shootTime;
    private float reloadTime;
    private float hurtTime;
    private float deathTime;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void Awake()
    {
        hackermanPickupObjects = GameObject.FindGameObjectWithTag("Object_Parent_Hackerman");       // scrape a reference to the hackerman parent object out of the scene so it can be toggled later
    }


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();  // basically everything needs a reference to the game manager

        Cursor.visible = false;                         // turn of cursor
        Cursor.lockState = CursorLockMode.Locked;       // and lock it to the centre of the screeen
        playerState = PlayerState.Idle;                 // set initial player state as idle

        playerController = GetComponent<CharacterController>();     // scrape character controller component off player object
        playerAudio = GetComponent<AudioSource>();                  // same with audio source

        ////// currently just always set the shoot sound to audio 1 --> later change the audio clip when the weapon changes (placeholder, change this later)
        playerAudio.clip = weaponSounds[0];
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////
    // TRYING TO KEEP UPDATE AS CLEAN AS POSSIBLE, BEING BASICALLY JUST A LIST OF FUNCTION CALLS ///
    ////////////////////////////////////////////////////////////////////////////////////////////////


    private void Update()
    {
        playerSpriteObject.material = wepDefs.WeaponDefs[currentWeapon].gameObject.GetComponent<WeaponDef>().spriteSheet;   // this probably needs to get rolled into a fucntion at some point to deal with weapon stuff

        CheckMouseMovement();
        CalculateMovement();

        CheckInput();
        CheckState();

        CheckHackerman();   // if in hackerland, do hackerman stuff

        CheckHealth();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void CheckHealth()
    {
        if (health <= 0)
            KillPlayer();   // this is currently very stubbed out, and we need a more detailed death and restart loop probably
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void CheckHackerman()
    {
        if (inHackerman)
            hackermanTimer += Time.deltaTime;               // if in hackerlad, start the timer
        else
            hackermanTimer = 0.0f;                          // otherwise, it gets reset to 0

        if (hackermanTimer >= hackermanDuration)            // if hackerman timer goes over the allowed duration then
        {
            inHackerman = false;                            // set bool to no longer be in hackerland
            hackermanPickupObjects.SetActive(false);        // turn off the hackerman objects so they aren't visible through the walls from the real world
            transform.position = terminalReturnPosition;    // return the player to their position when they entered from the terminal
            transform.rotation = terminalReturnRotation;    // return the player to their rotation when they entered from the terminal
                                                            // NEED TO ADD SOME SORT OF BRIEF DELAY HERE WHERE THE PLAYER CAN"T MOVE AND AN EFFECT IS PLAYED TO GIVE THEM TIME TO PROCESS WHAT'S HAPPENED
        }
    }


    private void CheckState()
    {
        if (playerState == PlayerState.Idle)                ////////////////// IDLE
        {
            {
                /*
                tempMaterial.SetFloat("_AnimationNumber", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleAnimNumber);
                tempMaterial.SetFloat("_FrameOffset", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleStartFrame);
                tempMaterial.SetFloat("_FrameCount", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleEndFrame - playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleStartFrame + 1);
                */
            }  // old fucked shit
        }
        if (playerState == PlayerState.Shooting)            ////////////////// SHOOTING
        {
            timePassed += Time.deltaTime;                   // used to track how long "FIRING" should go for

            if (justFired)                                  // at the moment the weapon is fired, do some one-off stuff
            {
                justFired = false;                          // flip the bool so we don't end up back in this if
                shotFXPistol.SetActive(true);               // play the pistol FX (this def needs some fancying up)
                playerAudio.Play();                         // play the shoot sound
                firing = true;                              // now flip on the next bool state "firing"
            }
            if (firing)                                     // after the one-of "just fired" now do this stuff
            {
                if (currentWeapon == 1)                     // currently only provisions here for the pistol. should probably turn this into a case statement when I have to start checking 7 weapons
                {
                    CheckHitscanShot();                     // do the raycast to check if you hit anything or not

                    if (timePassed >= pistolFireTime)       // once the duration of the shot (currently determined by pistolFireTime, BUT SHOULD come from the WEAPON DEF)
                    {
                        shotFXPistol.SetActive(false);      // turn off the particles
                        timePassed = 0.0f;                  // reset the timer
                        firing = false;                     // no longer firing
                        canFire = true;                     // can fire again
                    }
                }
            }
        }
    }

    ////////////////////////////////

    private void CheckHitscanShot() // HAS A PRETTY MAJOR BUG WHERE YOU CAN'T SHOOT THROUGH OPEN DOORS FOR SOME F'N REASON
    {
        RaycastHit hitInfo;         // local variable to store raycast hit info in
        //LayerMask mask = ~(13);   // This is SUPPOSED to ensure that only Enemies (things on the "Enemies" Layer get tested in the raycast)
        LayerMask mask;
        mask = LayerMask.GetMask("Enemies");    // this method ACTUALLY works instead

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 50.0f, Color.black, 1.0f);   // draw a debug ray (black line)

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hitInfo, Mathf.Infinity, mask, QueryTriggerInteraction.Collide))
        {
            //Debug.Log(hitInfo.transform.gameObject);
            //Instantiate(testObject, hitInfo.point, Quaternion.identity);

            if (hitInfo.transform.gameObject.tag == "Enemy")    // WHY am I also checking the object's tag if I'm only checking the enemy layer?
            {
                //Destroy(hitInfo.transform.gameObject);          // Currently when an enemy is hit, their game object is just destroyed, but we want to change this to setting their state to DYING, then DEAD
                hitInfo.transform.gameObject.GetComponent<EnemyBasic>().Kill();
            }
        }
    }


    private void CalculateMovement()    // and also rotation
    {
        // apply movement and strafing

        Vector3 targetVelocity = transform.forward * FBMovement * moveSpeed;    // set TARGET velocity made of the player's forward vector * FBMovement(which is the component derived from the foward/backward axis) * player speed set in the inspector
        targetVelocity += transform.right * LRMovement * strafeSpeed;           // this target vector then has added to it another vecotor calculated as above, but using input from the Left/Right axis and the stafe speed

        Vector3 moveDifference = targetVelocity - moveVector;                   // moveVector is how much was moved last turn, and so the amount we want to move is reduced by that. This code is made overly confusing by misused units

        moveVector += moveDifference * moveSpeed * Time.deltaTime;              // so the final amount we want to move is the above calculation * deltatime to keep it framrate independent * speed again (for some reason)
        Vector3.ClampMagnitude(moveVector, moveSpeed);                          // we then clamp the magnitude of the movement vector so the player can't move faster than their movement speed
                                                                                // moveSpeed will have to be changed to a new variable MAXSPEED once it starts getting modified by items
        // USED BY GUNSINE SCRIPT
        currentMovementSpeed = moveVector.magnitude;                            // for a minute there I was wondering wtf this variable was for since it never gets used (in this script), it is public though, so gunSine can use it

        // final movement

        moveVector.y = 0.0f;                                                    // clamp Y movement to 0

        playerController.Move(moveVector * Time.deltaTime);                     // us the magic PLAYER CONTROLLER component to work shit out for us when we feed it our movement vector

        if (!inHackerman)
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);     // this is a hack to keep the player on the floor because the doors can push the player controller downwards
        else
            transform.position = new Vector3(transform.position.x, 4.0f, transform.position.z);     // yes, the height of the hackerman rooms is a total magic number (4)

        // rotation

        Vector3 playerRotation = transform.rotation.eulerAngles;                // grab the current player rotation
        playerRotation.y += (YRotation + mouseRotation) * rotSpeed;             // modify the Y rotation by whatever's coming in from the keyboard axis + whatever's coming in from the mouse * rotation speed as set in the inspector

        transform.rotation = Quaternion.Euler(playerRotation);                  // set the player's rotation to the new modified value
    }

    /////////////////////////////////////////////////////////////////////////////
    // the contents of these two functions all pretty placeholder atm just to ///
    // get a game loop happening. they need much more fleshing out //////////////
    /////////////////////////////////////////////////////////////////////////////

    private void KillPlayer()
    {
        playerState = PlayerState.Dead;
        transform.GetChild(1).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
    }

    private void UnKillPlayer()
    {
        playerState = PlayerState.Idle;
        transform.GetChild(1).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
        gameManager.ReLoadLevel();
    }


    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// INPUT STUFF ////////////////////////////////


    private void CheckMouseMovement()
    {
        // Why the FUCK does this work?
        // this was copy and pasted from the unity online help basically;
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Actions.html
        //
        // and it works NOTHING like all the other "new inputs system" fucking bullshit I've found online. this is SO MUCH SIMPLER!
        // maybe look into a way to do ALL INPUT like this
        // also wtf is "var"?

        var mouse = Mouse.current;
        Vector2 mouseDelta = mouse.delta.ReadValue();

        // ok, this stuff I added myself;
        if (playerState != PlayerState.Dead)
        {
            mouseRotation = mouseDelta.x * mouseRotSpeed * Time.deltaTime;  // calculate the value that gets used in the rotation of the CalculateMovement() function above
            Mathf.Clamp(mouseRotation, -mouseRotSpeed, mouseRotSpeed);
        }
    }


    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// THE REAL INPUT STUFF ///////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// BETTER THAN THE OTHER SHIT /////////////////
    /////////////////////////////////////////////////////////////////////////////

    // basically using the new input system like the old one

    private void CheckInput()
    {
        var mouse = Mouse.current;

        if (playerState != PlayerState.Dead)
        {
            if (mouse.leftButton.isPressed)
            {
                if (canFire)
                {
                    playerState = PlayerState.Shooting;
                    justFired = true;
                    canFire = false;
                }
            }

            if (Keyboard.current.fKey.wasPressedThisFrame)
                interactPressed = true;
            else
                interactPressed = false;
        }
        else
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                UnKillPlayer();
        }


        if (Keyboard.current.kKey.wasPressedThisFrame)
            KillPlayer();
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// INPUT STUFF ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// KINDA FUCKED ///////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    /*
    public void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log("##########");
        if (canFire)
        {
            playerState = PlayerState.Shooting;
            justFired = true;
            canFire = false;
        }
    }

    */  // DELETE ME

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Want to convert this crap back to the simple "get single key input" like I am now doing for everything else
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void Walk(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Dead)
            FBMovement = context.ReadValue<float>();
    }

    public void Strafe(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Dead)
            LRMovement = context.ReadValue<float>();
    }

    public void Turn(InputAction.CallbackContext context)
    {
        if (playerState != PlayerState.Dead)
            YRotation = context.ReadValue<float>();
    }
}