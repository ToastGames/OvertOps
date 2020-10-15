using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackermanGroup : MonoBehaviour
{
    // This script sits on each child of the child HACKERMAN>Fixed of each room prefab (the ones named "Group")
    // So we have
    // ROOM
    // > HACKERMAN
    // > > Fixed        <-- fixed referring to their location, as opposed to random on other items, which chooses a random location within given bounds
    // > > > Group1
    // > > > Group2
    // > > > Group3
    //
    // This script sits on the "groups" listed above
    // Each group has multiple children, each a collection of bonus pickups for hackerman world
    // so ONE of the many "GroupContainer" objects will be selected to remain active
    // each "GroupContainer" object has many children (pickups), that are all either on or off with the parent "GroupContainer"


    private List<GameObject> potentialObjects = new List<GameObject>();

    void Awake()
    {
        //  TURNING IT ALL OFF FOR NOW WHILE I TRY AND DUBUG THE FUCKING LINES BEING IN THE WRONG PLACE

        // Y'KNOW, i THINK I'M GOING TO LEAVE IT OFF FOR NOW. I KIND OF LIKE THESE BEING NICE AND ORDERLY AND NOT RANDOM


        /*


        int childToKeep = Random.Range(0, transform.childCount);        // pick a random number for whilch enemy child to keep

        for (int i = 0; i < transform.childCount; i++)                  // iterate through all the children and add them to a list
            potentialObjects.Add(transform.GetChild(i).gameObject);


        for (int i = 0; i < transform.childCount; i++)                  // iterate through the list and turn off all the ones that don't match the chosen number
            if (i != childToKeep)
                potentialObjects[i].SetActive(false);

        */
    }
}
