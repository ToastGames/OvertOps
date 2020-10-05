using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGroup : MonoBehaviour
{
    // This script sits on each child of the child MOBS of each room prefab (the ones named "Group")
    // So we have
    // ROOM
    // > MOBS
    // > > Group1
    // > > Group2
    // > > Group2
    //
    // This script sits on the "groups" listed above
    // Each group has multiple children, each an enemy, and when the level is generated, only one enemy from each group is kept


    private List<GameObject> potentialMobs = new List<GameObject>();
    
    void Start()
    {
        int childToKeep = Random.Range(0, transform.childCount);        // pick a random number for whilch enemy child to keep

        for (int i = 0; i < transform.childCount; i++)                  // iterate through all the children and add them to a list
            potentialMobs.Add(transform.GetChild(i).gameObject);


        for (int i = 0; i < transform.childCount; i++)                  // iterate through the list and turn off all the ones that don't match the chosen number
            if (i != childToKeep)
                potentialMobs[i].SetActive(false);
    }
}
