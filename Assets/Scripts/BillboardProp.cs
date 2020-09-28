using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardProp : MonoBehaviour
{
    // this script is the simplest type of prop
    // literally all it does is billboard to face the player object at all times

    private GameObject playerTarget;        // player object is private because it can't be assigned to the prefab from the scene
    
    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");      // grab the player object by it's tag
    }

    void Update()
    {
        transform.LookAt(playerTarget.transform, Vector3.up);           // every update just turn it to face the player
                                                                        // weirdly I don't have the "clamp to Y rotation only" line of code in here for some reason, will I need to add it later?
    }
}
