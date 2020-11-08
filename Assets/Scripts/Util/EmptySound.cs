using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySound : MonoBehaviour
{
    private AudioSource s;

    [HideInInspector] public AudioClip soundToPlay;

    void Awake()
    {
        s = GetComponent<AudioSource>();
    }

    public void playSound()
    {
        s.clip = soundToPlay;
        s.Play();
    }

}
