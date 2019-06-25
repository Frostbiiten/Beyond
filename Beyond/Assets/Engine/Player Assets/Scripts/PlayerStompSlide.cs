using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStompSlide : MonoBehaviour
{
    public PlayerCore PlayerCore;
    public bool stomping;
    public float stompSpeed;
    public bool sliding;
    public bool crouch;

    public float dTap;
    public float tapSpeed = 2f;

    public float stompExplosionForce;
    public float stompExplosionRadius;

    bool old;
    bool cur;

    public GameObject stompingFx;
    public ParticleSystem stompLandParticles;
    public float stompShakeAmount = 1.5f;
    public float shakeTime = 0.01f;
    public CapsuleCollider defaultCollider;
    float defHeight;
    public float slideMomentum = 0.33f;
    private void Start()
    {
        defHeight = defaultCollider.height;
    }

    void Update()
    {
        if(dTap > 0f)
        {
            dTap -= Time.deltaTime * tapSpeed;
        }
        
        if (Input.GetButtonDown("Slide"))
        {
            dTap++;
            if(PlayerCore.airbornePhysics.enabled == true)
            {
                PlayerCore.playerAnimationManager.playerAnimator.Play("Stomp");
                stomping = true;
            }
            if(PlayerCore.groundedPhysics.enabled == true)
            {
                if(PlayerCore.velocityMagnitude >= 0.5f)
                PlayerCore.playerAnimationManager.playerAnimator.Play("Slide");
                sliding = true;
                defaultCollider.height = 0.5f;
                defaultCollider.center = new Vector3(0f, -0.5f, 0f);
            }
        }

        if(dTap > 1.5f && PlayerCore.groundedPhysics.enabled == true)
        {
            dTap = 0f;
            PlayerCore.playerAnimationManager.playerAnimator.Play("Slide Kick");
        }
        crouch = Input.GetButton("Slide") && PlayerCore.groundedPhysics.enabled == true && PlayerCore.velocityMagnitude < 0.5f;

        if (Input.GetButtonUp("Slide") || !Input.GetButton("Slide") || PlayerCore.velocityMagnitude < 0.5f)
        {
            defaultCollider.height = defHeight;
            sliding = false;
            defaultCollider.center = Vector3.zero;
        }

        if (PlayerCore.groundedPhysics.enabled == true)
        {
            stomping = false;
        }

        if (PlayerCore.airbornePhysics.enabled == true)
        {
            sliding = false;
        }

        if(sliding == true)
        {
            PlayerCore.rb.AddForce(PlayerCore.velocity * slideMomentum);
        }


        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Stomping", stomping);
        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Stomping", stomping);
        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Sliding", sliding);
        PlayerCore.playerAnimationManager.playerAnimator.SetBool("Crouching", crouch);
        stompingFx.SetActive(PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Stomp"));
    }

    void OnStompLand()
    {
        stompLandParticles.Play();
        PlayerCore.Explode(transform.position, stompExplosionRadius, stompExplosionForce);
        StartCoroutine(PlayerCore.camShake.Shake(stompShakeAmount, shakeTime));
        PlayerCore.playerAnimationManager.playerAnimator.Play("StompLand");

    }

    void FixedUpdate()
    {
        if(stomping == true)
        {
            PlayerCore.rb.velocity = Vector3.down * stompSpeed;
        }

        old = cur;
        if(PlayerCore.grounded == false && stomping == true)
        {
            cur = true;
        }
        else
        {
            cur = false;
        }

        if(old != cur && old == true)
        {
            OnStompLand();
            stompLandParticles.Play();
        }

        if (PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StompLand"))
        {
            PlayerCore.rb.velocity = Vector3.zero;
        }
    }
}
