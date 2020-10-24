using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    private bool active = false;

    private GameObject hackermanPickupObjects;

    private void Awake()
    {
        hackermanPickupObjects = GameObject.FindGameObjectWithTag("Object_Parent_Hackerman");
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // for now, terminals will activate automatically until I can get my head around the fucking new input system

        if (other.tag == "Player")
        {
            active = true;

            other.transform.Translate(Vector3.up * 4);
            other.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            other.GetComponent<Player>().inHackerman = true;

            hackermanPickupObjects.SetActive(true); // activate the hackerman bonus objects (they are hidden by default so they don't render through the roof)
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            active = false;
    }



    public void Interact(InputAction.CallbackContext context)
    {
        /*
        //LRMovement = context.ReadValue<float>();
        bool temp = context.ReadValue<bool>();

        Debug.Log("fucken");
        */
    }

}
