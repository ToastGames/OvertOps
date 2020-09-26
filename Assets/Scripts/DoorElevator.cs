using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorElevator : MonoBehaviour
{
    public GameObject doorMovingBitLeft;
    public GameObject doorMovingBitRight;
    public GameObject doorDestinationLeft;
    public GameObject doorDestinationRight;
    public GameObject LightFadeObject;
    public float doorPauseTime;
    public float doorMoveTime;

    public Texture altTexture;

    private float timePassed;

    private bool isMoving;
    private bool isTriggered;
    private bool arrived = false;

    private Vector3 startPosLeft;
    private Vector3 startPosRight;
    private Vector3 endPosLeft;
    private Vector3 endPosRight;

    private void Start()
    {
        startPosLeft = doorMovingBitLeft.transform.position;
        startPosRight = doorMovingBitRight.transform.position;
        endPosLeft = doorDestinationLeft.transform.position;
        endPosRight = doorDestinationRight.transform.position;
    }

    void Update()
    {
        if ((isTriggered) && (!isMoving) && (!arrived))
        {
            timePassed += Time.deltaTime;
            if (timePassed >= doorPauseTime)
            {
                isMoving = true;
                timePassed = 0.0f;
            }
        }
        if (isMoving)
        {
            timePassed += Time.deltaTime;
            doorMovingBitLeft.transform.position = Vector3.Lerp(startPosLeft, endPosLeft, timePassed / doorMoveTime);
            doorMovingBitRight.transform.position = Vector3.Lerp(startPosRight, endPosRight, timePassed / doorMoveTime);

            if (timePassed >= doorMoveTime)
            {
                isMoving = false;
                arrived = true;
                LightFadeObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player") && (!isTriggered))
        {
            //transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);
            //transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);
            isTriggered = true;
            LightFadeObject.SetActive(true);
        }
    }
}
