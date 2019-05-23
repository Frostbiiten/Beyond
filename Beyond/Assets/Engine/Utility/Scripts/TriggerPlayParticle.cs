using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayParticle : MonoBehaviour
{
    public ParticleSystem particles;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            particles.Play();
        }
    }
}
