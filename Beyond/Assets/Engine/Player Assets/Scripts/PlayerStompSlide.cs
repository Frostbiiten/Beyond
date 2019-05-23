using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStompSlide : MonoBehaviour
{
    public PlayerCore PlayerCore;
    public bool stomping;
    public float stompSpeed;
    public bool sliding;
    public bool stompLand;
    public bool crouch;

    void Update()
    {

        if (Input.GetButtonDown("Slide"))
        {
            if(PlayerCore.airbornePhysics.enabled == true)
            {
                PlayerCore.playerAnimationManager.playerAnimator.Play("Stomp");
                stomping = true;
                stompLand = true;
            }
            if(PlayerCore.groundedPhysics.enabled == true)
            {
                if(PlayerCore.velocityMagnitude >= 0.5f)
                PlayerCore.playerAnimationManager.playerAnimator.Play("Slide");
                sliding = true;

            }
        }
        crouch = Input.GetButton("Slide") && PlayerCore.groundedPhysics.enabled == true&& PlayerCore.velocityMagnitude < 0.5f;
        if (Input.GetButtonUp("Slide") || !Input.GetButton("Slide") || PlayerCore.velocityMagnitude < 0.5f)
        {
            sliding = false;
        }

        if (PlayerCore.groundedPhysics.enabled == true)
        {
            stomping = false;
        }

        if (PlayerCore.airbornePhysics.enabled == true)
        {
            sliding = false;
        }



        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Stomping", stompLand);
        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Sliding", sliding);
        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Crouching", crouch);
    }



    void FixedUpdate()
    {
        if(stomping == true)
        {
            PlayerCore.rb.velocity = Vector3.down * stompSpeed;
        }

        if(sliding == true)
        {
            // shorter collider?
        }

        if (PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Stompland"))
        {
            PlayerCore.rb.velocity = Vector3.zero;
        }
    }
}
