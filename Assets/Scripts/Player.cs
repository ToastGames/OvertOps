using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(transform.up, -rotSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(transform.up, rotSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            transform.position += transform.forward * -moveSpeed * Time.deltaTime;
    }
}
