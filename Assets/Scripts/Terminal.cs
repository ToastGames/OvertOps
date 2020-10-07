using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Terminal : MonoBehaviour
{
    private bool active = false;

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // for now, terminals will activate automatically until I can get my head around the fucking new input system

        other.transform.Translate(Vector3.up * 4);
        other.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (other.tag == "Player")
            active = true;
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
