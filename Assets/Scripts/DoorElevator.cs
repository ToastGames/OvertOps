using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorElevator : MonoBehaviour
{
    // this is honestly almost an exact dupe of the "door" script. it has a few changes after copy and pasting the code
    // to account for the fact that what's special about elevator doors is that there are two of them that move left and right
    // rather than a single one that moves up


    public GameObject doorMovingBitLeft;        // there are now 2 moving bits
    public GameObject doorMovingBitRight;       // left and right
    public GameObject doorDestinationLeft;      // and 2 destinations
    public GameObject doorDestinationRight;     // one for each of the left and right moving bits
    public GameObject LightFadeObject;          // there is also an object that get's turned on just as the door is opened (this has a bright additive overlay to do a fake HDR dazzle thing)
    public float doorPauseTime;                 // there will be a short delay before the door actually opens
    public float doorMoveTime;                  // how long it takes the door to open

    public Texture altTexture;                  // the door will change texture to this the instant it is trigeered

    private float timePassed;                   // a bunch of housekeeping variables
    private bool isMoving;
    private bool isTriggered;
    private bool arrived = false;

    private Vector3 startPosLeft;               // more housekeeping variables
    private Vector3 startPosRight;              // a start and an end position for each side of the door
    private Vector3 endPosLeft;
    private Vector3 endPosRight;

    private void Start()
    {
        startPosLeft = doorMovingBitLeft.transform.position;        // set up al lthe start and end positions, same as in "DOOR"
        startPosRight = doorMovingBitRight.transform.position;
        endPosLeft = doorDestinationLeft.transform.position;
        endPosRight = doorDestinationRight.transform.position;
    }

    void Update()   // update basically just moves the door (if it needs to)
    {
        if ((isTriggered) && (!isMoving) && (!arrived)) // I have 3 bools to keep track of this thing? that seems like overkill
        {
            timePassed += Time.deltaTime;               // simple timer (co routine free because fuck those things)
            if (timePassed >= doorPauseTime)            // once the pause time has passed, door is now "opening" (I mean "moving")
            {
                isMoving = true;                        // set moving to true
                timePassed = 0.0f;                      // reset the timer
            }
        }
        if (isMoving)                                   // if the door is moving (which may have just been set by the previous if statement)
        {
            timePassed += Time.deltaTime;               // start counting up time again
            doorMovingBitLeft.transform.position = Vector3.Lerp(startPosLeft, endPosLeft, timePassed / doorMoveTime);       // lerp the LEFT door's position between start pos and end pos (speed is determined by doorMoveTime set in inspector)
            doorMovingBitRight.transform.position = Vector3.Lerp(startPosRight, endPosRight, timePassed / doorMoveTime);    // lerp the RIGHT door's position between start pos and end pos (speed is determined by doorMoveTime set in inspector)

            if (timePassed >= doorMoveTime)             // once the timer has ticked over how long it should take the door to move, the door should be at it's destination
            {
                isMoving = false;                       // which means it is no longer moving
                arrived = true;                         // and it has "arrived" <-- this bool exists so the door doesn't retrigger over and over, and is checked against in the initial if statement to get the door going
                LightFadeObject.SetActive(false);       // also turn off the "light dazzle effect" object, otherwise it's sprite sheet animation is going to keep playing over and over
            }
        }
    }

    private void OnTriggerEnter(Collider other)         // the door object will have a box collider the side of the doorway it sits within
    {
        if ((other.tag == "Player") && (!isTriggered))  // it will only be triggerable by the player
        {
            //transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);         // these are currently commented out because they were fucked for some reason
            //transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);         // which is weird, because they should work exactly the same as in "DOOR"
                                                                                                                                        // maybe there was no "altTexture" defined?
            isTriggered = true;
            LightFadeObject.SetActive(true);    // turn on the sweet "bright light" effect object
        }
    }
}
