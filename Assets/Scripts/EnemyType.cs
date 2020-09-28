using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyType : MonoBehaviour
{
    public List<GameObject> enemyTypes;

    void Awake()
    {
        int selection = Random.Range(0, enemyTypes.Count - 1);

        GameObject newEnemy = Instantiate(enemyTypes[selection], transform.position, Quaternion.identity);
        newEnemy.transform.SetParent(transform);
    }
}
