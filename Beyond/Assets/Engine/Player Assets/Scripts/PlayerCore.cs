using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;

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
    [Header("Player Scripts")]

    public InputCore inputCore;
    public AirbornePhysics airbornePhysics;
    public GroundPhysics groundedPhysics;
    public PlayerHpManager playerHpManager;
    public PlayerDragCore playerDragCore;
    public PlayerAnimationManager playerAnimationManager;
    public PlayerJump playerJump;
    public PlayerHomingAttack playerHomingAttack;
    public UIManager UIManager;
    public CameraShake camShake;
    public PlayerStompSlide playerStompSlide;
    public PlayerSideStep PlayerSideStep;
    public PlayerRailGrindVer2 railGrind;
    public OrbitCamV2 orbitCam;
    public PlayerSoundManager playerSoundCore;

    #endregion

    #region Main Variables
    [Header("Main Variables")]

    [Tooltip("Is the player grounded?")]
    public bool grounded; // Is the player Grounded

    [Tooltip("How fast is the player moving in each dimension")]
    public Vector3 velocity; // The velocity of the player's Rigidbody (how fast) * In Vector 3 

    [Tooltip("The magnitude of the players velocity")]
    public float velocityMagnitude; // The player's velocity in a single number

    [Tooltip("How far should the player check for ground")]
    public float groundDetectionDistance;

    [Tooltip("Is the player a ball")]
    public bool ball;

    [Tooltip("The normal of the ground - the 'reflection' of the face")]
    public Vector3 groundNormal;

    public RaycastHit playerGroundHit; //This will not be visible in the editor

    // T H I S   I S   A   V E R Y   L A Z Y   A P P R O A C H
    public Transform playerForward;
    public Transform playerForwardDummy;
    public Transform playerForwardParent;
    public Camera playerCam;

    #endregion

    #region Unity Components
    public Rigidbody rb;
    public Animator fadeAnimator;
    #endregion

    public int redRings;

    int groundDetectionMask;
    void Start()
    {
        //oh =~ is opposit for mask
        groundDetectionMask |= ~(int)PlayerLayerHelper.Layers.Homeable;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate is called once per physics "frame" we do all physics related things here
    void FixedUpdate()
    {

        #region Lazy System
        playerForwardParent.localEulerAngles = playerCam.transform.eulerAngles;
        playerForwardParent.localRotation = Quaternion.Euler(0f, playerForwardParent.localEulerAngles.y, 0f);
        playerForwardDummy.localPosition = new Vector3(inputCore.directionalInput.x, 0f, inputCore.directionalInput.y);
        playerForward.LookAt(playerForwardDummy);

        #endregion

        #region Simple Stuff
        velocity = rb.velocity;
        velocityMagnitude = velocity.magnitude;

        #endregion

        #region Detection

        grounded = Physics.Raycast(transform.position, -transform.up, out playerGroundHit, groundDetectionDistance, groundDetectionMask, QueryTriggerInteraction.Ignore);

        if(grounded == true)
        {
            groundedPhysics.enabled = true;
            airbornePhysics.enabled = false;

            groundNormal = playerGroundHit.normal;
        }

        if (grounded == false)
        {
            groundedPhysics.enabled = false;
            airbornePhysics.enabled = true;

        }
        #endregion

        #region Debug
        //Debug
        Debug.DrawRay(transform.position, -transform.up * groundDetectionDistance, Color.blue);
        #endregion

    }
}
