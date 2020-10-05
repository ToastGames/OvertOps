using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropGroup : MonoBehaviour
{
    // This script sits on each child of the child PROPS>Random of each room prefab (the ones named "Group")
    // So we have
    // ROOM
    // > PROPS
    // > > Random
    // > > > Group1
    // > > > Group2
    // > > > Group2
    //
    // This script sits on the "groups" listed above
    // Each group has multiple children, each a prop prefab (of ANY of the 4 types), and when the level is generated, only one enemy from each group is kept
    // the Group object also has a trigger box collider to define WHERE the prop will spawn (anywahere randomly XZ within the box)


    private List<GameObject> potentialProps = new List<GameObject>();
    private BoxCollider bounds;

    void Start()
    {
        bounds = GetComponent<BoxCollider>();

        int childToKeep = Random.Range(0, transform.childCount);        // pick a random number for whilch enemy child to keep

        for (int i = 0; i < transform.childCount; i++)                  // iterate through all the children and add them to a list
            potentialProps.Add(transform.GetChild(i).gameObject);


        for (int i = 0; i < transform.childCount; i++)                  // iterate through the list and turn off all the ones that don't match the chosen number
            if (i != childToKeep)
                potentialProps[i].SetActive(false);

        float newX = Random.Range(-(bounds.size.x / 2), (bounds.size.x / 2));
        float newZ = Random.Range(-(bounds.size.z / 2), (bounds.size.z / 2));
        Vector3 randomComponent = new Vector3(newX, 0.0f, newZ);
        Vector3 boundsCentreComponent = new Vector3(bounds.center.x, 0.0f, bounds.center.z);
        potentialProps[childToKeep].transform.position = transform.position + randomComponent + boundsCentreComponent;
    }
}
