using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlayerAnimation : MonoBehaviour
{
    public string animationToPlay;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.playerAnimationManager.playerAnimator.Play(animationToPlay);
        }
    }
}
