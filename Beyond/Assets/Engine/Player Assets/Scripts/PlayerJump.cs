using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    //TODO : SWITCH IMPUT STUFF TO INPUTCORE

    public float initialJumpForce;
    public float jumpHeldForce;
    public PlayerCore pCore;
    public bool jumpTakenUp;
    public bool canAddHeldForce;
    public float jumpHeldVelocityThreshold;
    public float timeHeld;
    public float timeHeldForBall;
    public bool wentDown;
    void Update()
    {
        if(pCore.groundedPhysics.enabled == true)
        {
            timeHeld = 0f;
            jumpTakenUp = false;
            wentDown = false;
            if (pCore.inputCore.JumpKeyDown)
            {
                pCore.playerAnimationManager.playerAnimator.SetBool("OldBall", false);
                pCore.playerSoundCore.voiceSource.PlayOneShot(pCore.playerSoundCore.initialJumpSound);
                pCore.playerSoundCore.AltJump();
                jumpTakenUp = false;
                pCore.playerAnimationManager.PlayLeap();
                pCore.rb.AddForce(transform.up * initialJumpForce, ForceMode.Impulse);
            }
        }
        
    }

    void FixedUpdate()
    {
        
        if (pCore.airbornePhysics.enabled == true)
        {
            if (pCore.inputCore.JumpKeyUp)
            {
                jumpTakenUp = true;
            }

            if(pCore.velocity.y < 0f)
            {
                wentDown = true;
            }

            if (pCore.velocity.y > jumpHeldVelocityThreshold && jumpTakenUp == false && pCore.inputCore.JumpKey && wentDown == false)
            {
                timeHeld += Time.deltaTime;
                pCore.rb.AddForce(transform.up * jumpHeldForce);
            }

            if (timeHeld >= timeHeldForBall)
            {
                pCore.playerAnimationManager.playerAnimator.SetBool("OldBall", true);
                pCore.ball = true;
            }
        }

    }
}
