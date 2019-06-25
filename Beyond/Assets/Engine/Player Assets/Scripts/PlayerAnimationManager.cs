using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;

public class PlayerAnimationManager : MonoBehaviour
{
    public PlayerCore playerCore;

    #region Base Variables
    public Animator playerAnimator;
    public Transform playerSkin;
    #endregion

    #region Properties to set
    public float playerTilt;
    public Vector3 skinOffset;
    Quaternion oldRotation;
    public Vector3 angularVelocity;
    public float acceleration;
    float oldvel;
    float vel;

    #endregion

    #region Landing Animations
    public bool canPlayLandAnimation;
    bool landCollideHelper;
    bool landCollideColliding;
    RaycastHit landDetection;
    RaycastHit hurtLandDetection;
    public float landCollideDistance;
    public float hurtLandCollideDistance;
    public int landLayerMask;
    #endregion

    #region Workarounds
    public Transform[] weirdMouths;
    #endregion

    #region FX
    public GameObject jumpBall;
    public Material jumpBallMat;
    public ParticleSystem dustFx;
    public ParticleSystem speedLineFx;
    public float dustThreshold;
    public float sppedLineFXThreshold;
    #endregion

    public bool playStartAnimation;
    public bool playNormal;
    public float startFastSpeed;
    float waitTime;

    public TrailRenderer trail;

    public GameObject realPlayerSkin;

    public Transform driftBall;
    public float driftTilt;
    float driftRot;
    public float driftThreshold;

    public GameObject stomping;
    Vector3 diveRot;

    void Start()
    {

        //Calculate layermask to Raycast to. (Ignore "cube" && "sphere" layers)
        //int layerMask = ~((1 << cubeLayerIndex) | (1 << sphereLayerIndex));

        ////Calculate layermask to Raycast to. (Raycast to "cube" && "sphere" layers only)
        //int layerMask = (1 << cubeLayerIndex) | (1 << sphereLayerIndex);
        //landLayerMask = ~0; nvm // Set to all layers
        //now remove some:
        landLayerMask = ~(
            (int)PlayerLayerHelper.Layers.Homeable |
            (int)PlayerLayerHelper.Layers.Ring |
            (int)PlayerLayerHelper.Layers.StaticRing |
            (int)PlayerLayerHelper.Layers.NoPlayerCollide |
            (int)PlayerLayerHelper.Layers.CameraTrigger |
            (int)PlayerLayerHelper.Layers.IgnoreRaycast | //?
            (int)PlayerLayerHelper.Layers.Water |
            (int)PlayerLayerHelper.Layers.NonPlayerLand
         );

        StartCoroutine(PlayStart());
        
    }

    IEnumerator PlayStart()
    {
        yield return new WaitForSeconds(0.1f);
        bool happened = false;
        playerCore.inputCore.InputLock(4f);
        if (playStartAnimation == true)
        {
            if (playNormal == true)
            {
                playerAnimator.Play("StartNormal");
                yield return null;
            }
            else
            {
                playerAnimator.Play("StartFast");
                yield return null;
            }
            yield return null;
        }

        while (happened == false)
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartNormal"))
            {
                playerCore.rb.isKinematic = true;
            }
            else
            {
                playerCore.rb.isKinematic = false;
            }
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f)
            {
                happened = true;
                playerCore.rb.isKinematic = false;
                playerCore.rb.velocity = playerCore.playerAnimationManager.playerSkin.forward * startFastSpeed;
            }
            yield return null;
        }
    }

    void FixedUpdate()
    {

        #region Jumpballs & drift
        jumpBall.SetActive(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Air Ball"));

        if(Input.GetAxis("Drift") != 0f && playerCore.groundedPhysics.enabled == true && playerCore.velocityMagnitude > driftThreshold){
            driftBall.gameObject.SetActive(true);
            realPlayerSkin.SetActive(false);
        }else
        {
            realPlayerSkin.SetActive(true);
            driftBall.gameObject.SetActive(false);
        }

        driftRot = Mathf.Lerp(driftRot, Input.GetAxis("Drift") * driftTilt, 0.1f);
        driftBall.localRotation = Quaternion.Euler(0f, 0f, driftRot);
        #endregion

        #region Tilt

        var deltaRot = playerSkin.localRotation * Quaternion.Inverse(oldRotation);
        var eulerRot = new Vector3(Mathf.DeltaAngle(0, deltaRot.eulerAngles.x), Mathf.DeltaAngle(0, deltaRot.eulerAngles.y), Mathf.DeltaAngle(0, deltaRot.eulerAngles.z));
        oldRotation = playerSkin.localRotation;
        angularVelocity = eulerRot / Time.fixedDeltaTime;
        playerTilt = Mathf.Lerp(playerTilt, angularVelocity.y / 45f, 0.075f);

        if (float.IsNaN(playerTilt))
        {
            playerTilt = 0f;
        }
        #endregion

        #region Script Dependent

        if (playerCore.airbornePhysics.enabled == true && !playerCore.railGrind.grinding == true)
        {
            if(playerCore.PlayerSideStep.sideStepping == false && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DashRing"))
            {
                playerSkin.localRotation = Quaternion.LookRotation(playerCore.velocity, playerCore.groundNormal);
                playerSkin.localRotation = Quaternion.Euler(0f, playerSkin.localEulerAngles.y, 0f);
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DashRing"))
            {
                transform.up = playerCore.velocity.normalized;
            }

        }

        if (playerCore.groundedPhysics == true && !playerCore.railGrind.grinding == true)
        {
            playerSkin.position = transform.position + skinOffset;
            if (playerCore.velocityMagnitude > 0.5f)
            {
                playerSkin.rotation = Quaternion.Slerp(playerSkin.rotation, Quaternion.LookRotation(playerCore.velocity, playerCore.groundNormal), 0.8f);
                playerSkin.localRotation = Quaternion.Euler(0f, playerSkin.localEulerAngles.y, 0f);
            }



        }
        #endregion

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("SkyDive"))
        {
            playerSkin.localRotation = Quaternion.LookRotation(playerCore.orbitCam.transform.up);

            if (playerAnimator.GetBool("DiveFast") == true)
            {
                diveRot = Vector3.Lerp(diveRot, new Vector3(-Input.GetAxis("Y") * 0.75f, 0f, -Input.GetAxis("X")) * 30f, 0.1f);
                playerCore.playerAnimationManager.playerSkin.Rotate(diveRot);
            }
            else
            {
                diveRot = Vector3.Lerp(diveRot, new Vector3(Input.GetAxis("Y") * 0.75f, 0f, Input.GetAxis("X")) * 30f, 0.1f);
                playerCore.playerAnimationManager.playerSkin.Rotate(diveRot);
            }
            //diveRot = Vector3.Lerp(diveRot, new Vector3(Input.GetAxis("Y") * 2.5f, 0f, Input.GetAxis("X")) * 30f, 0.1f);
            //playerSkin.Rotate(diveRot);
            
        }

        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast") && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartNormal"))
        {
            canPlayLandAnimation = true;
        }
        else
        {
            canPlayLandAnimation = false;
        }



        #region Land Animations
        playerAnimator.SetBool("HurtLand", Physics.Raycast(transform.position, -transform.up, hurtLandCollideDistance, landLayerMask, QueryTriggerInteraction.Ignore));

        if (canPlayLandAnimation == true)
        {
            landCollideHelper = landCollideColliding;
            // fix below
            if (Physics.Raycast(transform.position, -transform.up, out landDetection, landCollideDistance, (int)PlayerLayerHelper.Layers.Everything, QueryTriggerInteraction.Ignore) && !landDetection.collider.gameObject.CompareTag("Enemy"))
            {
                if (landLayerMask == (landLayerMask | (1 << landDetection.collider.gameObject.layer))) // check if layer hit is not spring or something (specified in void start)
                {
                    if (landDetection.collider.CompareTag("Homing Target") || landDetection.collider.CompareTag("Enemy"))
                    {
                        //playerCore.ball = false; // BALL IS BEING SET TO FALSE HERE. BEWARE
                    }
                    else
                    {
                        landCollideColliding = false;
                    }
                    landCollideColliding = true;
                }
            }
            else
            {
                landCollideColliding = false;
            }

            if (landCollideHelper != landCollideColliding && landCollideColliding == true && playerCore.playerHpManager.recovering == false)
            {

                playerAnimator.Play("Land");
            }


        }
        #endregion

        #region acceleration
        oldvel = vel;
        vel = playerCore.velocityMagnitude;
        acceleration = vel - oldvel;
        #endregion

        #region FX
        if(playerCore.velocityMagnitude >= dustThreshold && playerCore.groundedPhysics.enabled == true)
        {
            dustFx.Play();
        }
        else
        {
            dustFx.Stop();
        }

        if (playerCore.velocityMagnitude >= sppedLineFXThreshold && playerCore.groundedPhysics.enabled == true)
        {
            speedLineFx.Play();
        }
        else
        {
            speedLineFx.Stop();
        }

        
        if(playerCore.ball == true)
        {
            trail.gameObject.SetActive(true);
            trail.time = 0.3f;
        }
        else
        {
            trail.time = 0f;
            trail.gameObject.SetActive(false);
        }
        #endregion

        UpdateAnimator();
    }

    void LateUpdate()
    {
        #region workarounds
        for (int i = 0; i < weirdMouths.Length; i++)
        {
            weirdMouths[i].localScale = Vector3.zero;
        }
        #endregion

        if(playerCore.inputCore.directionalInput == Vector2.zero)
        {
            waitTime += Time.deltaTime;
        }
        else
        {
            waitTime = 0f;
        }
    }

    #region Workarounds

    public void PlayLeap()
    {
        playerAnimator.Play("Leap");
    }

    #endregion

    void UpdateAnimator()
    {
        #region Setting Base Variables

        playerAnimator.SetBool("Grounded", playerCore.grounded);
        playerAnimator.SetFloat("Local X+Z Velocity", playerSkin.InverseTransformVector(playerCore.velocity).x + playerSkin.InverseTransformVector(playerCore.velocity).z);
        playerAnimator.SetFloat("Y Velocity", playerCore.velocity.y);
        playerAnimator.SetFloat("Tilt", playerTilt);
        playerAnimator.SetFloat("Acceleration", acceleration);
        playerAnimator.SetFloat("WaitTime", waitTime);
        playerAnimator.SetBool("Ball", playerCore.ball);
        playerAnimator.SetBool("Grinding", playerCore.railGrind.grinding);
        playerAnimator.SetFloat("DirectionalDot", Vector3.Dot(playerCore.playerForward.forward, playerCore.velocity.normalized));
        #endregion
    }
}
