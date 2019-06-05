using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlaySound : MonoBehaviour
{
    public AudioClip sound;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundCore.nonSpacialSource.PlayOneShot(sound);
        }
    }
}
