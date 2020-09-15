using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorMovingBit;
    public GameObject doorDestination;
    public float doorPauseTime;
    public float doorMoveTime;

    public Texture altTexture;

    private float timePassed;

    private bool isMoving;
    private bool isTriggered;
    private bool arrived = false;

    private Vector3 startPos;
    private Vector3 endPos;

    private void Start()
    {
        startPos = transform.position;
        endPos = doorDestination.transform.position;        
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
            doorMovingBit.transform.position = Vector3.Lerp(startPos, endPos, timePassed / doorMoveTime);

            if (timePassed >= doorMoveTime)
            {
                isMoving = false;
                arrived = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player") && (!isTriggered))
        {
            transform.GetChild(1).transform.GetChild(0).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);
            transform.GetChild(1).transform.GetChild(1).GetComponent<Renderer>().material.SetTexture("_BaseMap", altTexture);
            isTriggered = true;
        }
    }
}
