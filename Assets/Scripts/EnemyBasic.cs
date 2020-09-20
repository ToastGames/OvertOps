using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public enum EnemyState { Idle, Roaming };

    public float moveSpeed;
    public float proximityThreshold;

    private EnemyState enemyState;
    private Vector3 targetDestination;
    


    void Start()
    {
        enemyState = EnemyState.Roaming;
        targetDestination = PickNewDestination(Vector3.zero);
    }

    void Update()
    {
        if (enemyState == EnemyState.Roaming)
        {
            //transform.LookAt(targetDestination);
            //transform.Translate(transform.forward * moveSpeed);

            transform.position = Vector3.MoveTowards(transform.position, targetDestination, moveSpeed);
        }
        Debug.DrawLine(transform.position, targetDestination, Color.magenta);

        if (Vector3.Distance(transform.position, targetDestination) < proximityThreshold)
            targetDestination = PickNewDestination(Vector3.zero);
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 tempVector = PickNewDestination(targetDestination);
        targetDestination = tempVector;
    }

    private Vector3 PickNewDestination(Vector3 oldDestination)
    {
        float newX = Random.Range(-100.0f, 100.0f);
        float newY = 0.0f;
        float newZ = Random.Range(-100.0f, 100.0f);

        if (newX * (oldDestination.x - transform.position.x) > 0.0f)
            newX = newX * -1;

        if (newZ * (oldDestination.z - transform.position.z) > 0.0f)
            newZ = newZ * -1;

        Vector3 newVector = new Vector3(newX, newY, newZ);

        return newVector;
    }

}
