using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakaway : MonoBehaviour
{
    public float percentageChance;
    public bool randomiseOrientation = false;
    public List<Material> materialOptions;

    void Start()
    {
        if (Random.Range(0.0f, 100.0f) <= percentageChance)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MeshRenderer>().material = materialOptions[Random.Range(0, materialOptions.Count)];
                if (randomiseOrientation)
                    transform.GetChild(i).GetComponent<MeshRenderer>().material.SetFloat("_RotationOffset", Mathf.Floor(Random.Range(0.0f, 4.0f)));
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
