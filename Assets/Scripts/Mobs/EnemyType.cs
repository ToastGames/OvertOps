using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : MonoBehaviour
{
    // This script goes on the first child of all enemy prefabs
    // The "ENEMY" prefab is just a wrapper prefab that is parent to the enemy "Guts" and the enemy patrol path
    //
    // This script contains a list off all potential enemy "Guts" that could potentially be assigned to this enemy to randomly choose between


    public List<GameObject> enemyTypes;

    void Awake()                                                                                            // Done on Awake so it happens before all the other level set up
    {
        int selection = Random.Range(0, enemyTypes.Count);                                                  // randomly choose from the list of possible enemy "Guts"

        GameObject newEnemy = Instantiate(enemyTypes[selection], transform.position, transform.parent.parent.parent.parent.rotation);   //Quaternion.identity);  // instantiate randomly chosen enemy prefab
        newEnemy.transform.SetParent(transform);                                                            // parent new prefab to this object
    }
}
