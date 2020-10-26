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
        hackermanPickupObjects = GameObject.FindGameObjectWithTag("Object_Parent_Hackerman");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if ((active == true) && (player.interactPressed))
        {
            player.terminalReturnPosition = playerObject.transform.position;
            player.terminalReturnRotation = playerObject.transform.rotation;

            playerObject.transform.Translate(Vector3.up * 4);
            playerObject.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            playerObject.GetComponent<Player>().inHackerman = true;

            hackermanPickupObjects.SetActive(true); // activate the hackerman bonus objects (they are hidden by default so they don't render through the roof)
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // for now, terminals will activate automatically until I can get my head around the fucking new input system

        if (other.tag == "Player")
        {
            playerObject = other.gameObject;
            active = true;
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            active = false;
    }
    */
}
