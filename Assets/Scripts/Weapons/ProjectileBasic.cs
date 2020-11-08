using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasic : MonoBehaviour
{
    public int damage;
    public float movementSpeed;
    public float collisionRadius;
    public GameObject hitFXEnv;
    public GameObject hitFXPlayer;

    private Vector3 lastPos;

    void Start()
    {
    }

    void Update()
    {
        lastPos = transform.position;
        transform.position += transform.forward * movementSpeed * Time.deltaTime;

        Vector3 newRayDirection = transform.position - lastPos;
        Ray newRay = new Ray(lastPos, newRayDirection);
        float rayLength = Vector3.Magnitude(newRayDirection);

        //Debug.DrawLine(transform.position, lastPos, Color.yellow);
        Debug.DrawRay(lastPos, newRayDirection, Color.white);

        if (Physics.SphereCast(newRay, collisionRadius, out RaycastHit hitInfo, rayLength))
        {
            //Debug.Log(rayHitInfo);
            if(hitInfo.transform.gameObject.tag == "Wall")                                         // if it hit a wall, play some particles and destroy self
            {
                Instantiate(hitFXEnv, hitInfo.point, Quaternion.Euler(hitInfo.normal));
                Destroy(gameObject);
            }
            if (hitInfo.transform.gameObject.tag == "Player")                                      // if it hit the player, play some particles and destroy self, and deduct health from player
            {
                Instantiate(hitFXPlayer, hitInfo.point, Quaternion.Euler(hitInfo.normal));
                hitInfo.transform.gameObject.GetComponent<Player>().health -= damage;

                Destroy(gameObject);
            }
        }
    }
}
