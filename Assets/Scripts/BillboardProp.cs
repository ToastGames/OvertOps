using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardProp : MonoBehaviour
{
    private GameObject playerTarget;
    
    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.LookAt(playerTarget.transform, Vector3.up);
    }
}
