using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPhysics : MonoBehaviour
{
    public PlayerCore playerCore;

    #region Movement
    [Header("Main Movement")]

    [Tooltip("The default speed when directional input is first entered")]
    public float defaultForce;

    [Tooltip("The current speed to be added - for acceleration")]
    public float currentForce;

    [Header("Drag and Acceleration")]


    [Tooltip("The least velocity that the player's movement force can actually increase")]
    public float leastVelocityToStartAccelerating;

    [Tooltip("Pretty much the same thing as above, but this is because if the player stops moving, and begins again, things can go crazy")]
    public float leastVelocityToStartAcceleratingNoInput;

    [Tooltip("The current amount of acceleration to 'add' ")]
    public float currentAcceleration;

    [Tooltip("This is a curve for acceleration based on how fast the player is going")]
    public AnimationCurve accelerationCurve;

    [Tooltip("The fraction of speed retained base on velocity")]
    public float deceleration;

    [Tooltip("The least speed that you can run in loops and such")]
    public float leastSpeedToAlignToGround;

    [Tooltip("extra gravity")]
    public float extraGravity;

    #endregion

    #region Slope
    [Header("Slope Physics")]

    [Tooltip("For physics")]
    public AnimationCurve slopeCurve;

    [Tooltip("For physics")]
    public AnimationCurve spinSlopeCurve;

    [Tooltip("how much force for slopes")]
    public float slopeMagnitude;

    [Tooltip("current slope force")]
    public float currentSlopeForce;

    [Tooltip("The force for going down")]
    public float downForce;

    #endregion

    #region Spindash
    public float spindashDragScalar;
    public float spindashForceScalar;
    public float spindashSlopeScalar;
    public float spinDashMomentum = 0.1f;
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerCore.ball = !playerCore.ball;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerCore.playerHomingAttack.airDashed = false;
        #region Align To Ground and Some Slope Physics

        if (playerCore.velocityMagnitude >= leastSpeedToAlignToGround || playerCore.ball == true)
        {
            Vector3 projection = Vector3.ProjectOnPlane(transform.forward, playerCore.groundNormal);
            Quaternion rotation = Quaternion.LookRotation(projection, playerCore.groundNormal);
            transform.rotation = rotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 0.1f);
        }

        #endregion

        #region Basic Movement 
        if(playerCore.ball == false)
        {
            if(playerCore.playerStompSlide.sliding == false)
            {
                if (playerCore.inputCore.directionalInput != Vector2.zero)
                {
                    playerCore.rb.AddForce(playerCore.playerForward.forward * currentForce);
                }
            }
        }
        else
        {
            if (playerCore.inputCore.directionalInput != Vector2.zero)
            {
                playerCore.rb.AddForce(playerCore.playerForward.forward * currentForce * spindashForceScalar);
            }
        }


        currentAcceleration = accelerationCurve.Evaluate(playerCore.velocityMagnitude);
        #endregion

        #region Acceleration

        if (playerCore.inputCore.directionalInput == Vector2.zero)
        {
            if (playerCore.velocityMagnitude < leastVelocityToStartAcceleratingNoInput)
            {
                currentForce = playerCore.velocityMagnitude * deceleration;
            }
        }
        else
        {
            if (playerCore.velocityMagnitude < leastVelocityToStartAccelerating)
            {
                currentForce = defaultForce;
            }
            else
            {
                currentForce += currentAcceleration;
            }
        }

        #endregion

        #region Slope

        if (playerCore.ball == false)
        {
            currentSlopeForce = slopeCurve.Evaluate(playerCore.groundNormal.y) * slopeMagnitude;
            playerCore.rb.AddForce(Vector3.down * currentSlopeForce);

            //Debug.Log(Vector3.down * currentSlopeForce * slopeMagnitude);
        }
        else
        {
            playerCore.rb.AddForce(playerCore.rb.velocity * spinDashMomentum);
            currentSlopeForce = spinSlopeCurve.Evaluate(playerCore.groundNormal.y) * slopeMagnitude;
            playerCore.rb.AddForce(Vector3.down * currentSlopeForce * spindashSlopeScalar);
            //Debug.Log();
        }

        if(playerCore.velocityMagnitude <= 1f)
        {
            playerCore.ball = false;
        }


        #endregion

        #region Down Force
        if (!Input.GetButton("Jump"))
        {
            playerCore.rb.AddForce(-transform.up * playerCore.velocityMagnitude * downForce);
            playerCore.rb.AddForce(Vector3.down * extraGravity);
        }
        #endregion
    }

    void OnEnable()
    {
        playerCore.ball = false;
    }
}
