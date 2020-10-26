using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    private bool active = false;

    private GameManager gameManager;
    private Player player;
    private GameObject playerObject;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if ((active == true) && (player.interactPressed))
        {
            gameManager.LoadNextLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerObject = other.gameObject;
            active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            active = false;
    }
}
