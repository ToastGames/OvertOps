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


    public WeaponDefList wepDefs;
    private PlayerState playerState;
    public MeshRenderer playerSpriteObject;       // this should be obtained from dragging the child object with the spritesheet on it into this variable

    public int currentWeapon;

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

    // variables below this line need reassessing ///////////////////////////////////////////////////////////

    private float shootTime;
    private float reloadTime;
    private float hurtTime;
    private float deathTime;


    private void Start()
    {
        Cursor.visible = false;
        playerState = PlayerState.Idle;

        {
            //Instantiate(collisionMarker, transform.position, transform.rotation);

            //shootTime = (1 / playerSpriteObject.GetComponent<Renderer>().material.GetFloat("_FrameRate")) * (playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingEndFrame - playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingStartFrame);
            //Debug.Log(shootTime);
        } // old fucked shit
    }


    private void Update()
    {
        playerSpriteObject.material = wepDefs.WeaponDefs[currentWeapon].gameObject.GetComponent<WeaponDef>().spriteSheet;

        CheckMouseMovement();
        CalculateMovement();

        //Debug.Log("Player Health: " + health);

        CheckState();
    }

    /* // probably delete this function
    void LateUpdate()
    {
        CalculateMovement();
        CheckCollision();

        transform.Translate(moveVector + adjustmentVector);                         // Let's do this differently --> see Richard's code
        adjustmentVector = Vector3.zero;                                            //
        transform.Rotate(transform.up, (YRotation * rotSpeed * Time.deltaTime));    //
    }
    */

    ////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //      PLAYER IS FUUUUUUCKED..... BASICALLY GOING TO GUT IT AND START AGAIN
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
            {
                /*
                tempMaterial.SetFloat("_AnimationNumber", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingAnimNumber);
                tempMaterial.SetFloat("_FrameOffset", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingStartFrame);
                tempMaterial.SetFloat("_FrameCount", playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingEndFrame - playerSpriteDefList.GetComponent<SpriteDefList>().SpriteFramesDefs[1].GetComponent<SpriteFamesDef>().shootingStartFrame + 1);

                accumulatedShootTime += Time.deltaTime;

                //Debug.Log(accumulatedShootTime);

                if (accumulatedShootTime >= shootTime)
                {
                    accumulatedShootTime = 0.0f;
                    playerState = PlayerState.Idle;
                }
                */
            } // old fucked shit
        }
    }

    ////////////////////////////////


    private void CalculateMovement()
    {
        {   
        //Vector3 tempVector = new Vector3(LRMovement * moveSpeed * Time.deltaTime, 0.0f, FBMovement * moveSpeed * Time.deltaTime);
        //Vector3 rotatedTempVector = Quaternion.AngleAxis(transform.rotation.y, transform.up) * tempVector;

        //moveVector = rotatedTempVector;
        }   // this is my first attempt, bleow is very simlar, but better code adapted from Richard


        // apply movement and strafing

        Vector3 targetVelocity = transform.forward * FBMovement * moveSpeed;
        targetVelocity += transform.right * LRMovement * strafeSpeed;

        Vector3 moveDifference = targetVelocity - moveVector;

        moveVector += moveDifference * moveSpeed * Time.deltaTime;
        Vector3.ClampMagnitude(moveVector, moveSpeed);

        // final movement

        CheckCollision();

        moveVector.y = 0.0f;
        transform.position += moveVector * Time.deltaTime;

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
        //Debug.Log(currentMouseDelta);

        // ok, this stuff I added myself;
        mouseRotation = mouseDelta.x * mouseRotSpeed * Time.deltaTime;
        Mathf.Clamp(mouseRotation, -mouseRotSpeed, mouseRotSpeed);
    }


    private void CheckCollision()
    {
        {
            // Basically doing the exact same thing 5 times with all rays in this function because I am lazy
            // (I should really try and turn this into a list or array at some point and use a for loop)

            // COLLISION DISABLED FOR NOW BECAUSE IT'S FUUUUUUCKED AND GETS IN THE WAY OF TESTING OTHER STUFF
            /*

            // forward facing
            RaycastHit hit1; // right
            RaycastHit hit2; // middle
            RaycastHit hit3; // left

            // sideways facing
            RaycastHit hit4; // right
            RaycastHit hit5; // middle

            Vector3 ray1Start = new Vector3();
            Vector3 ray1End = new Vector3();
            Vector3 ray2Start = new Vector3();
            Vector3 ray2End = new Vector3();
            Vector3 ray3Start = new Vector3();
            Vector3 ray3End = new Vector3();
            ///////
            Vector3 ray4Start = new Vector3();
            Vector3 ray4End = new Vector3();
            Vector3 ray5Start = new Vector3();
            Vector3 ray5End = new Vector3();

            ray1Start = transform.position + (transform.right * collisionWidth) + transform.up;
            ray1End = transform.position + (transform.right * collisionWidth) + (transform.forward * rayCheckLength) + transform.up;
            ray2Start = transform.position + transform.up;
            ray2End = transform.position + (transform.forward * rayCheckLength) + transform.up;
            ray3Start = transform.position - (transform.right * collisionWidth) + transform.up;
            ray3End = transform.position - (transform.right * collisionWidth) + (transform.forward * rayCheckLength) + transform.up;
            ///////
            ray4Start = transform.position + transform.up;
            ray4End = transform.position + (transform.right * rayCheckLength) + transform.up;
            ray5Start = transform.position + transform.up;
            ray5End = transform.position - (transform.right * rayCheckLength) + transform.up;

            Debug.DrawLine(ray1Start, ray1End, Color.red);
            Debug.DrawLine(ray2Start, ray2End, Color.yellow);
            Debug.DrawLine(ray3Start, ray3End, Color.red);
            ///////
            Debug.DrawLine(ray4Start, ray4End, Color.blue);
            Debug.DrawLine(ray5Start, ray5End, Color.blue);

            ////////////////////////////////////// Literally everything above this line is just to construct and visualise the raycasts

            if (Physics.Raycast(ray1Start, transform.forward, out hit1, rayCheckLength))
            {
                if (hit1.transform.gameObject.tag == "Wall")
                {
                    collisionMarker.transform.position = hit1.point;
                    Debug.DrawLine(hit1.point, hit1.point + hit1.normal, Color.white);

                    //adjustmentVector = ((hit1.point - transform.position) * -1) + transform.up; //// old test thing
                    Vector3 tempVector = (ray1End - hit1.point);
                    adjustmentVector = Vector3.Reflect(tempVector, hit1.normal);

                    Debug.DrawLine(hit1.point, hit1.point + adjustmentVector, Color.magenta);
                }
            }
            else if (Physics.Raycast(ray2Start, transform.forward, out hit2, rayCheckLength))
            {
                if (hit2.transform.gameObject.tag == "Wall")
                {
                    collisionMarker.transform.position = hit2.point;
                    Debug.DrawLine(hit2.point, hit2.point + hit2.normal, Color.white);

                    Vector3 tempVector = (ray2End - hit2.point);
                    adjustmentVector = Vector3.Reflect(tempVector, hit2.normal);

                    Debug.DrawLine(hit2.point, hit2.point + adjustmentVector, Color.magenta);
                }
            }
            else if(Physics.Raycast(ray3Start, transform.forward, out hit3, rayCheckLength))
            {
                if (hit3.transform.gameObject.tag == "Wall")
                {
                    collisionMarker.transform.position = hit3.point;
                    Debug.DrawLine(hit3.point, hit3.point + hit3.normal, Color.white);

                    Vector3 tempVector = (ray3End - hit3.point);
                    adjustmentVector = Vector3.Reflect(tempVector, hit3.normal);

                    Debug.DrawLine(hit3.point, hit3.point + adjustmentVector, Color.magenta);
                }
            }
            else if (Physics.Raycast(ray4Start, transform.forward, out hit4, rayCheckLength))
            {
                if (hit4.transform.gameObject.tag == "Wall")
                {
                    collisionMarker.transform.position = hit4.point;
                    Debug.DrawLine(hit4.point, hit4.point + hit4.normal, Color.white);

                    Vector3 tempVector = (ray4End - hit4.point);
                    adjustmentVector = Vector3.Reflect(tempVector, hit4.normal);

                    Debug.DrawLine(hit4.point, hit4.point + adjustmentVector, Color.magenta);
                }
            }
            else if (Physics.Raycast(ray5Start, transform.forward, out hit5, rayCheckLength))
            {
                if (hit5.transform.gameObject.tag == "Wall")
                {
                    collisionMarker.transform.position = hit5.point;
                    Debug.DrawLine(hit5.point, hit5.point + hit5.normal, Color.white);

                    Vector3 tempVector = (ray5End - hit5.point);
                    adjustmentVector = Vector3.Reflect(tempVector, hit5.normal);

                    Debug.DrawLine(hit5.point, hit5.point + adjustmentVector, Color.magenta);
                }
            }
            */
        } // old fucked shit

        // ok, let's try this again

        for (int i = 0; i < rayCount; i++)
        {
            float rayIncrement = (rayArcWidth * 2) / rayCount;

            RaycastHit hitInfo;
            //Vector3 checkVector = moveVector;
            //checkVector.x -= rayArcWidth;
            //checkVector.x += rayIncrement * i;
            //Vector3 normalisedMoveVector = Vector3.Normalize(checkVector);
            Vector3 normalisedMoveVector = Vector3.Normalize(moveVector);

            ////Quaternion tempQuat = Quaternion.LookRotation(Vector3.Normalize(moveVector + new Vector3(rayIncrement * i, 0.0f, 0.0f)));
            ////Vector3 normalisedMoveVector = Vector3.RotateTowards(moveVector, tempQuat.eulerAngles, 1000.0f, 1000.0f);

            

            Debug.DrawRay(transform.position + Vector3.up, normalisedMoveVector + (transform.forward * rayCheckLength), new Color(1.0f, 0.5f, 0.0f));
            Debug.DrawRay(transform.position + Vector3.up + (transform.right * collisionWidth), normalisedMoveVector, new Color(1.0f, 0.5f, 0.0f));
            Debug.DrawRay(transform.position + Vector3.up - (transform.right * collisionWidth), normalisedMoveVector, new Color(1.0f, 0.5f, 0.0f));

            if (Physics.Raycast(transform.position + Vector3.up, normalisedMoveVector + (transform.forward * rayCheckLength), out hitInfo, rayCheckLength))
            {
                if ((hitInfo.transform.gameObject.tag == "Wall") || (hitInfo.transform.gameObject.tag == "WallPlayerOnly"))
                {
                    Debug.DrawRay(hitInfo.point, hitInfo.normal, new Color(1.0f, 0.5f, 0.5f));

                    {
                        /*
                        if (Mathf.Abs(hitInfo.normal.z) > 0.0f)
                        {
                            ////Vector3 temp = transform.TransformPoint(moveVector);
                            ////temp.z = 0.0f;
                            ////moveVector = transform.InverseTransformPoint(temp);

                            //moveVector.z = 0.0f;


                            //////Vector3 temp = transform.TransformPoint(moveVector);
                            //////float newMagnitude = temp.x;
                            //////moveVector = new Vector3(newMagnitude, 0.0f, 0.0f);
                            //moveVector = Vector3.Normalize(moveVector);

                            //////Debug.DrawRay(transform.position, moveVector, new Color(0.5f, 0.0f, 1.0f));


                            moveVector = hitInfo.normal;
                        }   // several fucked attempts in here
                        if (Mathf.Abs(hitInfo.normal.x) > 0.0f)
                            moveVector.x = 0.0f;
                        */
                    }   // several fucked attempts in here

                    //moveVector = hitInfo.normal * rayCheckLength;
                    moveVector = Vector3.Reflect(moveVector, hitInfo.normal);
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up + (transform.right * collisionWidth), normalisedMoveVector, out hitInfo, rayCheckLength))
            {
                if ((hitInfo.transform.gameObject.tag == "Wall") || (hitInfo.transform.gameObject.tag == "WallPlayerOnly"))
                {
                    Debug.DrawRay(hitInfo.point, hitInfo.normal, new Color(1.0f, 0.5f, 0.5f));
                    //moveVector = hitInfo.normal * rayCheckLength;
                    moveVector = Vector3.Reflect(moveVector, hitInfo.normal);
                }
            }
            if (Physics.Raycast(transform.position + Vector3.up - (transform.right * collisionWidth), normalisedMoveVector, out hitInfo, rayCheckLength))
            {
                if ((hitInfo.transform.gameObject.tag == "Wall") || (hitInfo.transform.gameObject.tag == "WallPlayerOnly"))
                {
                    Debug.DrawRay(hitInfo.point, hitInfo.normal, new Color(1.0f, 0.5f, 0.5f));
                    //moveVector = hitInfo.normal * rayCheckLength;
                    moveVector = Vector3.Reflect(moveVector, hitInfo.normal);
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //////////////////////////////// INPUT STUFF ////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    public void Shoot(InputAction.CallbackContext context)
    {
        //Debug.Log("##########");
        playerState = PlayerState.Shooting;
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