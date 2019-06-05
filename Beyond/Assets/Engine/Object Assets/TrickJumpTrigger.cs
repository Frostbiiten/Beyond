using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickJumpTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.playerAnimationManager.playerAnimator.SetFloat("Trick#", (int)Random.Range(0, 10));
            pc.playerAnimationManager.playerAnimator.Play("Trick");
        }
    }
}
