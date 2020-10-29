using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackermanBonus : MonoBehaviour
{
    public GameObject collectionParticles;
    public float particleDuration;

    public AudioClip pickUpSoundClip;
    public GameObject emptySound;


    private void OnTriggerEnter(Collider other)
    {
        GameObject newParticles = Instantiate(collectionParticles, transform.position, transform.rotation) as GameObject;
        Destroy(newParticles, particleDuration);

        GameObject newEmptySound = Instantiate(emptySound, transform.position, transform.rotation) as GameObject;
        newEmptySound.GetComponent<EmptySound>().soundToPlay = pickUpSoundClip;
        newEmptySound.GetComponent<EmptySound>().playSound();
        Destroy(newEmptySound, pickUpSoundClip.length);

        gameObject.SetActive(false);
    }

}
