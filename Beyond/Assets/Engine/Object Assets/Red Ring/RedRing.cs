using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedRing : MonoBehaviour
{
    public float scoreAdd = 1000f;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerCore pc;
            
            pc = other.GetComponent<PlayerCore>();
            pc.score += scoreAdd;
            pc.UIManager.UpdateScore();
            pc.redRings++;
            pc.UIManager.RedringAnimation();
            Destroy(gameObject);
        }
    }
}
