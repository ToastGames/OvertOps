using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorMovingBit;
    public GameObject doorDestination;
    public float doorMoveTime;

    private float timePassed;

    private bool isMoving;
    private bool isTriggered;

    private Vector3 startPos;
    private Vector3 endPos;

    private void Start()
    {
        startPos = transform.position;
        endPos = doorDestination.transform.position;        
    }

    void Update()
    {
        if (isMoving)
        {
            timePassed += Time.deltaTime;
            doorMovingBit.transform.position = Vector3.Lerp(startPos, endPos, timePassed / doorMoveTime);

            if (timePassed >= doorMoveTime)
                isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player") && (!isTriggered))
        {
            isTriggered = true;
            isMoving = true;
        }
    }
}
