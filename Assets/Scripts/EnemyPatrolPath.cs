using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolPath : MonoBehaviour
{
    public List<GameObject> pathNodes;
    public bool loop;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathNodes.Count-1; i++)
        {
            Debug.DrawLine(pathNodes[i].transform.position, pathNodes[i + 1].transform.position, Color.white);
        }
        if (loop)
            Debug.DrawLine(pathNodes[pathNodes.Count-1].transform.position, pathNodes[0].transform.position, Color.yellow);

    }
}
