using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    /* This holds base player stuff like
        -The RigidBody Component
            -velocity
            -drag
            -etc
        -This also has code to detect if the player is grounded or not.
        -Other MonoBehavior References
            -For example inputCore
    */

    #region MonoBehaviors
    public InputCore inputCore;
    public AirbornePhysics airbornePhysics;
    public GroundPhysics groundedPhysics;

    #endregion

    #region Main Variables
    public bool grounded; // Is the player Grounded
    public Vector3 velocity; // The velocity of the player's Rigidbody (how fast) * In Vector 3 
    public float velocityMagnitude; // The player's velocity in a single number

    public float groundDetectionDistance;
    public RaycastHit playerGroundHit; //This will not be visible in the editor
    #endregion

    #region Unity Components
    public Rigidbody rb;

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called once per physics "frame" we do all physics related things here
    void FixedUpdate()
    {
        velocity = rb.velocity;
        velocityMagnitude = velocity.magnitude;

        grounded = Physics.Raycast(transform.position, -transform.up, groundDetectionDistance, 1, QueryTriggerInteraction.Ignore);
        //Debug
        Debug.DrawRay(transform.position, -transform.up * groundDetectionDistance, Color.blue);
    }
}
