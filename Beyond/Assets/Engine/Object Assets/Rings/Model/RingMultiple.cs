using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingMultiple : MonoBehaviour
{
    public int hpAmount;
    public GameObject fx;
    public AudioClip sound;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundCore.nonSpacialSource.PlayOneShot(sound, 0.4f);
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.playerHpManager.hp += hpAmount - 1;
            pc.playerHpManager.UpdateRings();
            Instantiate(fx, transform.position, transform.rotation);
        }
    }
}
