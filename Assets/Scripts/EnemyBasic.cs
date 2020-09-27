using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spritePlane;

    public float speed;
    public float wallcollisionRange;
    public float rayWidth;
    public float turnVariation;

    private GameObject playerTarget;


    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        enemyPrefab.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        DetectWall();

        spritePlane.transform.LookAt(playerTarget.transform, Vector3.up);
        spritePlane.transform.eulerAngles = new Vector3(0.0f, spritePlane.transform.eulerAngles.y, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.tag == "Player")
            other.gameObject.GetComponent<Player>().isDead = true;

        if (other.tag == "PlayerProjectile")
            Kill();
        */
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    void DetectWall()
    {
        RaycastHit hitInfo;

        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f);
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f);
        offsetLeft = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetLeft;
        offsetRight = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetRight;

        Debug.DrawRay(transform.position + offsetLeft, transform.forward * wallcollisionRange, Color.red);
        Debug.DrawRay(transform.position + offsetRight, transform.forward * wallcollisionRange, Color.blue);
        //Debug.DrawRay(transform.position, transform.forward * wallcollisionRange, Color.blue);

        if (Physics.Raycast(transform.position + offsetLeft, transform.forward, out hitInfo, wallcollisionRange))
        {
            if (hitInfo.collider.gameObject.tag == "Wall")
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallcollisionRange, Color.yellow);
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));
            }
        }
        if (Physics.Raycast(transform.position + offsetRight, transform.forward, out hitInfo, wallcollisionRange))
        {
            if (hitInfo.collider.gameObject.tag == "Wall")
            {
                Debug.DrawRay(hitInfo.point, Vector3.Reflect(transform.forward, hitInfo.normal) * wallcollisionRange, Color.yellow);
                Reorient(Vector3.Reflect(enemyPrefab.transform.forward, hitInfo.normal));
            }
        }
    }

    void Reorient(Vector3 newDirection)
    {
        float newVariation = Random.Range(-turnVariation, turnVariation);

        enemyPrefab.transform.rotation = Quaternion.LookRotation(newDirection);
        enemyPrefab.transform.Rotate(0.0f, newVariation, 0.0f);

        //this is honestly a bit of a hack to stop the "getting out of the world" bug
        //for some reason "LookRotation" sometimes results in non-zero values on the x or z axes
        enemyPrefab.transform.eulerAngles = new Vector3(0.0f, enemyPrefab.transform.eulerAngles.y, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Vector3 offsetLeft = new Vector3((rayWidth / 2) * -1, 0.0f, 0.0f);
        Vector3 offsetRight = new Vector3((rayWidth / 2), 0.0f, 0.0f);
        offsetLeft = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetLeft;
        offsetRight = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * offsetRight;

        Debug.DrawRay(transform.position + offsetLeft, transform.forward * wallcollisionRange, Color.red);
        Debug.DrawRay(transform.position + offsetRight, transform.forward * wallcollisionRange, Color.blue);
    }

}
