using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevel : MonoBehaviour
{
    private bool active = false;            // boolean to track if the player is standing in the elevator or not (only operate when the player is in it)

    private GameManager gameManager;        // reference to the Game Manager object in the scene
    private Player player;                  // reference to player, so it's public variables can be accessed
    private GameObject playerObject;        // this is probably a really stupid way of doing this, but this variable is so we can keep a reference to the object that triggered Enter and Exit ouside those functions
                                            // in fact, on re reading over this function, I don't seem to actually DO anything with this, I can't remember why it's here

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();      // find Game Manager in scene
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();                     // find Player in scene
        playerObject = GameObject.FindGameObjectWithTag("Player");                                      // find player object in scene. As mentioned above, this isn't actually USED for anything yet for some reason
    }

    void Update()
    {
        if ((active == true) && (player.interactPressed))   // if player can be used and player presses interact key (F)
        {
            gameManager.LoadNextLevel();                    // load whichever level is queued up next in the game manager
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")                          // make sure this stuff only happens if the player enters the elevator, nothing else
        {
            playerObject = other.gameObject;                // Why the hell am I doing this?
            active = true;                                  // The player is in the elevator
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")                          // if the player leaves the elevator
            active = false;                                 // it can no longer be used (it is no longer "active")
    }
}
