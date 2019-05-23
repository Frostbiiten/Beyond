using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.playerHpManager.lives++;
            pc.playerHpManager.UpdateLives();
            Destroy(gameObject);
        }
    }
}
