using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirbornePhysics : MonoBehaviour
{

    public PlayerCore playerCore;

    [Header("Air Movement")]

    [Tooltip("Movement speed in the air")]
    public float airMovementSpeed;

    [Tooltip("Is the player in a 'ball' ")]
    public bool ball;

    [Tooltip("The least velocity that the player's movement force can actually increase")]
    public float groundSpeedScalar;

    [Tooltip("extra gravity to be applied")]
    public float extraGravity;

    [Tooltip("Drag in the air")]
    public float airDrag;


    // Update is called once per frame
    void FixedUpdate()
    {
        playerCore.rb.drag = airDrag;
        playerCore.groundedPhysics.currentForce = playerCore.velocityMagnitude * groundSpeedScalar;

        if (playerCore.inputCore.directionalInput != Vector2.zero)
        {
            playerCore.rb.AddForce(playerCore.playerForward.forward * airMovementSpeed);
        }

        if (Input.GetButton("Jump"))
        {
            ball = true;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.zero), 0.1f);
    }

    void Update()
    {
        if (Input.GetButton("Jump"))
        {
            ball = true;
        }
    }
}
