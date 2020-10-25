using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /////////////////////////
    // NEEDS COMMENTS PASS //
    /////////////////////////

    public enum PlayerState { Idle, Shooting, Reloading, Walking, Hurt, Dying }

    private CharacterController playerController;

    public WeaponDefList wepDefs;
    private PlayerState playerState;
    public MeshRenderer playerSpriteObject;       // this should be obtained from dragging the child object with the spritesheet on it into this variable

    public int currentWeapon = 1;
    private bool justFired = false;
    private bool firing = false;
    private bool canFire = true;
    private float timePassed = 0.0f;

    public float moveSpeed;
    public float strafeSpeed;
    public float rotSpeed;
    public float mouseRotSpeed;

    public int health = 100;

    private Vector3 moveVector = new Vector3();
    private float LRMovement = 0.0f;
    private float FBMovement = 0.0f;
    private float YRotation = 0.0f;
    private float mouseRotation = 0.0f;

    public int rayCount = 10;

    public float rayCheckLength;
    public float collisionWidth;
    public float rayArcWidth;

    [HideInInspector] public float currentMovementSpeed;
    [HideInInspector] public bool inHackerman = false;

    ////////////////////////////////////////////// Weapon Stuff <-- Replace most of this with Weapon Def stuff

    public GameObject shotFXPistol;
    //public GameObject shotFXShotgun;
    //public GameObject shotFXPlasma;
    //public GameObject shotFXRocket;
    //public GameObject shotFXChaingun;

    public float pistolFireTime;


    //////////////////////////////////////////////
    // variables below this line need reassessing ///////////////////////////////////////////////////////////

    private float shootTime;
    private float reloadTime;
    private float hurtTime;
    private float deathTime;


    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerState = PlayerState.Idle;

        playerController = GetComponent<CharacterController>();
    }


    private void Update()
    {
        playerSpriteObject.material = wepDefs.WeaponDefs[currentWeapon].gameObject.GetComponent<WeaponDef>().spriteSheet;

        CheckMouseMovement();
        CalculateMovement();

        CheckState();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //      PLAYER IS FUUUUUUCKED..... BASICALLY GOING TO GUT IT AND START AGAIN        // <-- currently in the process of doing so, its _better_ but still has big problems
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void CheckState()
    {
        //Material tempMaterial = playerSpriteObject.GetComponent<Renderer>().material;

        // indexes currently hard coded to 1 <-- shotty , need to make this a variable at some point

        if (playerState == PlayerState.Idle)
        {
            {
                /*
                tempMaterial.SetFloat("_AnimationNumber", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleAnimNumber);
                tempMaterial.SetFloat("_FrameOffset", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleStartFrame);
                tempMaterial.SetFloat("_FrameCount", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleEndFrame - playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().idleStartFrame + 1);
                */
            }  // old fucked shit
        }
        if (playerState == PlayerState.Shooting)
        {
            timePassed += Time.deltaTime;

            if (justFired)
            {
                justFired = false;
                shotFXPistol.SetActive(true);
                firing = true;
            }
            if (firing)
            {
                if (currentWeapon == 1)
                {
                    if (timePassed >= pistolFireTime)
                    {
                        CheckHitscanShot();

                        shotFXPistol.SetActive(false);
                        timePassed = 0.0f;
                        firing = false;
                        canFire = true;
                    }
                }
            }
        }
    }

    ////////////////////////////////

    private void CheckHitscanShot()
    {
        RaycastHit hitInfo; // local variable to store raycast hit info in
        LayerMask mask = ~(13);

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * 50.0f, Color.black, 1.0f);

        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hitInfo, Mathf.Infinity, mask, QueryTriggerInteraction.Collide))
        {
            Debug.Log(hitInfo.transform.gameObject);

            if (hitInfo.transform.gameObject.tag == "Enemy")
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }


    private void CalculateMovement()
    {
        // apply movement and strafing

        Vector3 targetVelocity = transform.forward * FBMovement * moveSpeed;
        targetVelocity += transform.right * LRMovement * strafeSpeed;

        Vector3 moveDifference = targetVelocity - moveVector;

        moveVector += moveDifference * moveSpeed * Time.deltaTime;
        Vector3.ClampMagnitude(moveVector, moveSpeed);

        currentMovementSpeed = moveVector.magnitude;

        // final movement

        moveVector.y = 0.0f;

        playerController.Move(moveVector * Time.deltaTime);

        if (!inHackerman)
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);     // this is a hack to keep the player on the floor because the doors can push the player controller downwards
        else
            transform.position = new Vector3(transform.position.x, 4.0f, transform.position.z);     // yes, the height of the hackerman rooms is a total magic number (4)

        // rotation

        Vector3 playerRotation = transform.rotation.eulerAngles;
        playerRotation.y += (YRotation + mouseRotation) * rotSpeed;

        transform.rotation = Quaternion.Euler(playerRotation);
        
    }

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
        mouseRotation = mouseDelta.x * mouseRotSpeed * Time.deltaTime;
        Mathf.Clamp(mouseRotation, -mouseRotSpeed, mouseRotSpeed);

    }


    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// INPUT STUFF ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    public void Shoot(InputAction.CallbackContext context)
    {
        //Debug.Log("##########");
        if (canFire)
        {
            playerState = PlayerState.Shooting;
            justFired = true;
            canFire = false;
        }
    }

    public void Walk(InputAction.CallbackContext context)
    {
        FBMovement = context.ReadValue<float>();
    }

    public void Strafe(InputAction.CallbackContext context)
    {
        LRMovement = context.ReadValue<float>();
    }

    public void Turn(InputAction.CallbackContext context)
    {
        YRotation = context.ReadValue<float>();
    }
}