using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AirbornePhysics : MonoBehaviour
{
    [Required("PlayerCore is required")]
    public PlayerCore playerCore;

    [Header("Air Movement")]

    [BoxGroup("Main")]
    [Tooltip("Movement speed in the air")]
    public float airMovementSpeed;

    [BoxGroup("Main")]
    [Tooltip("Is the player in a 'ball' ")]
    public bool ball;

    [BoxGroup("Main")]
    [Tooltip("The least velocity that the player's movement force can actually increase")]
    public float groundSpeedScalar;

    [BoxGroup("Main")]
    [Tooltip("extra gravity to be applied")]
    public float extraGravity;

    [BoxGroup("Main")]
    public bool allowPlayerToTurnIntoBall = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        playerCore.groundedPhysics.currentForce = playerCore.velocityMagnitude * groundSpeedScalar;

        playerCore.rb.AddForce(Vector3.down * extraGravity);

        if (playerCore.inputCore.directionalInput != Vector2.zero)
        {
            playerCore.rb.AddForce(playerCore.playerForward.forward * airMovementSpeed);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 0.05f);
    }

    void Update()
    {
        if(allowPlayerToTurnIntoBall == true)
        {
            if (playerCore.inputCore.JumpKey)
            {
                ball = true;
            }

            if (playerCore.inputCore.JumpKeyDown) // To make sure
            {
                ball = true;
            }
        }
    }
}
