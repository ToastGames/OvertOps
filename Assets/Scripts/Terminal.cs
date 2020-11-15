using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    private bool active = false;

    private GameObject hackermanPickupObjects;
    private Player player;
    private GameObject playerObject; 

    private void Awake()
    {
        hackermanPickupObjects = GameObject.FindGameObjectWithTag("Object_Parent_Hackerman");   // scrape hackerman objects out of scene so they can be unhidden later
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();     // get the player component and the player object separately apparently
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if ((active == true) && (player.interactPressed))
        {
            player.terminalReturnPosition = playerObject.transform.position;    // this is supposed to se the player return loaction and rotation, but I'm not convinced it's 100% working properly
            player.terminalReturnRotation = playerObject.transform.rotation;    // which is weird, beause there's nothing to it, it's just setting a variable with the current pos and rot

            playerObject.transform.Translate(Vector3.up * 4);                   // teleport the player 4 units upwards into hackerland
            playerObject.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));     // rotate the player 180 so they aren't facing the wall when they enter hackerland
            playerObject.GetComponent<Player>().inHackerman = true;             // setting on the player script that they are in hackerland, so it can hide objects and do whatever else

            hackermanPickupObjects.SetActive(true);                             // unhide hackerman objects
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerObject = other.gameObject;            // if player enters the Terminal trigger, bool active is set so that it can be checked agains in update in conjunction with player input
            active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            active = false;
        }
    }

}
