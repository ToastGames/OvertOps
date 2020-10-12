using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Door prefabs are made up of 2 parts; the Doorway which contains all the walls and bits that have their textures modified by the proc gen
    //      and the "ACTUAL DOOR" the moving bit that sits in the center of the doorway
    //
    // This script goes on the "Actual Door", as it is the bit that get's triggered and moves some stuff around (ie, opens the door)

    public GameObject doorMovingBit;        // the top level node of the bit of heirarchy this script is going to move out of the way
    public GameObject doorDestination;      // where does it move to (is set to 0.01 off grid to prevent Z-fighting)
    public float doorPauseTime;             // there will be a short delay before the door actually opens
    public float doorMoveTime;              // how long it takes the door to open

    public Texture altTexture;              // the door will change texture to this the instant it is trigeered

    private float timePassed;               // a bunch of housekeeping variables
    private bool isMoving;
    private bool isTriggered;
    private bool arrived = false;

    private Vector3 startPos;               // more housekeeping variables
    private Vector3 endPos;                 // start pos and end pos are extracted rather than assigned

    private void Start()
    {
        startPos = transform.position;                  // start from the initial position of this object's transform
        endPos = doorDestination.transform.position;    // end at wherever the destination object's position is set to
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
            doorMovingBit.transform.position = Vector3.Lerp(startPos, endPos, timePassed / doorMoveTime);   // lerp the door's position between start pos and end pos (speed is determined by doorMoveTime set in inspector)

            if (timePassed >= doorMoveTime)             // once the timer has ticked over how long it should take the door to move, the door should be at it's destination
            {
                isMoving = false;                       // which means it is no longer moving
                arrived = true;                         // and it has "arrived" <-- this bool exists so the door doesn't retrigger over and over, and is checked against in the initial if statement to get the door going
            }
        }
    }

    private void OnTriggerEnter(Collider other)         // the door object will have a box collider the side of the doorway it sits within
    {
        if ((other.tag == "Player") && (!isTriggered))  // it will only be triggerable by the player
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);       // when triggered the two "faces" of the door have their texture changed
            transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);       // to whatever texture is set in the inspector
            isTriggered = true;                                                                                                     // triggering the door sets this bool to true so the movement stuff above knows the door's been triggered
        }
    }
}
