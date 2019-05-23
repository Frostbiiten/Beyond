using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedRing : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCore pc;
            pc = other.GetComponent<PlayerCore>();
            pc.redRings++;
            pc.UIManager.RedringAnimation();
            Destroy(gameObject);
        }
    }
}
