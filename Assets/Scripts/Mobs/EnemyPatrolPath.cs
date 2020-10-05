using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolPath : MonoBehaviour
{
    // This script exists purely as a container to hold all the path nodes (in sequence) for an enemy
    // The script doesn't actually DO anything


    public List<GameObject> pathNodes;      // path nodes are public so they can be dragged in from the heirarchy of the prefab to allow creative authorial control
    public bool loop;                       // does the path loop or ping pong? (this variable's value is extracted by the enemy script to determine correct enemy movement later

    // The script may not DO anything, but it still has this funciont here to draw debug lines connecting all the path nodes together
    // to visualise the path in the scene view at editor time. This makes it much easier to edit paths

    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathNodes.Count-1; i++)
        {
            Debug.DrawLine(pathNodes[i].transform.position, pathNodes[i + 1].transform.position, Color.white);              // regular lines are drawn in WHITE
        }
        if (loop)
            Debug.DrawLine(pathNodes[pathNodes.Count-1].transform.position, pathNodes[0].transform.position, Color.yellow); // if "looping" is on, then an extra line is drawn in YELLOW connecting the start and end nodes
    }
}
