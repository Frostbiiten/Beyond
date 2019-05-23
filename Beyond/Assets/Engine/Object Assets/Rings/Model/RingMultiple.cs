using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMultiple : MonoBehaviour
{
    public int hpAmount;
    public GameObject fx;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.playerHpManager.hp += hpAmount - 1;
            pc.playerHpManager.UpdateRings();
            Instantiate(fx, transform.position, transform.rotation);
        }
    }
}
