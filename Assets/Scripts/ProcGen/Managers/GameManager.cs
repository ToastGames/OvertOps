using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
            ReLoadLevel();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();

    }

    public void ReLoadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        // for now, "load next level" just reloads the current level

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

