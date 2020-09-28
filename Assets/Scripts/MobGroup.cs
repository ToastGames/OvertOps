using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGroup : MonoBehaviour
{
    private List<GameObject> potentialMobs = new List<GameObject>();
    
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            potentialMobs.Add(transform.GetChild(i).gameObject);
        }

        int childToKeep = Random.Range(0, transform.childCount);

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != childToKeep)
                potentialMobs[i].SetActive(false);
        }

    }
}
