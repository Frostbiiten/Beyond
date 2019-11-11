using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;
using UnityEngine.SceneManagement;
using UnityEditor;
using NaughtyAttributes;


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

    [BoxGroup("Player Abilities")]
    public InputCore inputCore;

    [BoxGroup("Player Abilities")]
    public AirbornePhysics airbornePhysics;

    [BoxGroup("Player Abilities")]
    public GroundPhysics groundedPhysics;

    [BoxGroup("Player Abilities")]
    public PlayerHpManager playerHpManager;

    [BoxGroup("Player Abilities")]
    public PlayerDragCore playerDragCore;

    [BoxGroup("Player Abilities")]
    public PlayerAnimationManager playerAnimationManager;

    [BoxGroup("Player Abilities")]
    public PlayerJump playerJump;

    [BoxGroup("Player Abilities")]
    public PlayerHomingAttack playerHomingAttack;

    [BoxGroup("Player Abilities")]
    public UIManager UIManager;

    [BoxGroup("Player Abilities")]
    public PlayerStompSlide playerStompSlide;

    [BoxGroup("Player Abilities")]
    public PlayerSideStep PlayerSideStep;

    [BoxGroup("Player Abilities")]
    public PlayerRailGrindVer2 railGrind;

    [BoxGroup("Player Abilities")]
    public OrbitCamV2 orbitCam;

    [BoxGroup("Player Abilities")]
    public PlayerSoundManager playerSoundCore;

    [BoxGroup("Player Abilities")]
    public RunPath rPath;

    [BoxGroup("Player Abilities")]
    public PlayerDrift playerDrift;

    #endregion

    #region Main Variables
    [Header("Main Variables")]

    [BoxGroup("Main Variables")]
    [Tooltip("Is the player grounded?")]
    public bool grounded; // Is the player Grounded

    [BoxGroup("Main Variables")]
    [Tooltip("How fast is the player moving in each dimension")]
    public Vector3 velocity; // The velocity of the player's Rigidbody (how fast) * In Vector 3 

    [BoxGroup("Main Variables")]
    [Tooltip("The magnitude of the players velocity")]
    public float velocityMagnitude; // The player's velocity in a single number

    [BoxGroup("Main Variables")]
    [Tooltip("How far should the player check for ground")]
    public float groundDetectionDistance;

    [BoxGroup("Main Variables")]
    [Tooltip("Is the player in a  spinball")]
    public bool ball;

    [BoxGroup("Main Variables")]
    [Tooltip("The normal of the ground - the 'reflection' of the face")]
    public Vector3 groundNormal;

    public RaycastHit playerGroundHit; //This will not be visible in the editor

    // T H I S   I S   A   V E R Y   L A Z Y   A P P R O A C H

    [BoxGroup("360 system")]
    public Transform playerForward;

    [BoxGroup("360 system")]
    public Transform playerForwardDummy;

    [BoxGroup("360 system")]
    public Transform playerForwardParent;

    [BoxGroup("Other")]
    public Camera playerCam;

    #endregion

    #region Unity Components
    [BoxGroup("Other")]
    public Rigidbody rb;

    [BoxGroup("Other")]
    public Animator fadeAnimator;
    #endregion

    [BoxGroup("Other")]
    public int redRings;

    [BoxGroup("Other")]
    public float score;

    [BoxGroup("Other")]
    public string menuScene;

    int groundDetectionMask;

    Collider[] explosionObjs = new Collider[32];

    [BoxGroup("Other")]
    public AnimationCurve turnSpeedCurve;

    [BoxGroup("Experimental")]
    public bool isSuperSonic; // INCOMPLETE

    [BoxGroup("Experimental")]
    public KeyCode transformKey;

    [BoxGroup("Experimental")]
    public float transformTime;

    [BoxGroup("Experimental")]
    public Light superSonicLight;

    [BoxGroup("Experimental")]
    public ParticleSystem superSonicParticle;

    [BoxGroup("Experimental")]
    public GameObject chaosEmeralds;

    [BoxGroup("Experimental")]
    public float hpToTurnSuper = 50f;

    bool transforming;

    [BoxGroup("Other")]
    public GameObject defaultJumpBall;

    [BoxGroup("Other")]
    public GameObject superJumpBall;


    void Start()
    {
        //oh =~ is opposit for mask
        groundDetectionMask |= ~(int)PlayerLayerHelper.Layers.Homeable;

        //lives playerprefs
        if (PlayerPrefs.GetString("LastSceneLoaded") == SceneManager.GetActiveScene().name)
        {

        }
        else
        {

        }

        UIManager.UpdateLives();


    }
    // Update is called once per frame
    void Update()
    {
        playerAnimationManager.superSonicPlayerSkinRenderer.gameObject.SetActive(isSuperSonic);
        playerAnimationManager.playerSkinRenderer.gameObject.SetActive(!isSuperSonic);

        defaultJumpBall.SetActive(!isSuperSonic);
        superJumpBall.SetActive(isSuperSonic);

        superSonicLight.gameObject.SetActive(isSuperSonic);
        if (isSuperSonic)
        {
            superSonicParticle.Play();
        }
        else
        {
            superSonicParticle.Stop();
        }


        if (Input.GetKeyDown(transformKey))
        {
            StartCoroutine(Transform());
        }


    }

    IEnumerator Transform()
    {
        if (isSuperSonic)
        {
            if(transforming == false)
            {
                playerAnimationManager.superPlayerAnimatorInstance.Play("Transform");
                playerAnimationManager.playerAnimator.Play("Transform");
                transforming = true;
                chaosEmeralds.SetActive(true);
                yield return new WaitForSeconds(transformTime / 2f);
                isSuperSonic = false;
                yield return new WaitForSeconds(transformTime / 2f);
                chaosEmeralds.SetActive(false);
                transforming = false;
            }
        }
        else
        {
            if (transforming == false && playerHpManager.hp >= hpToTurnSuper)
            {
                
                playerAnimationManager.superPlayerAnimatorInstance.Play("Transform");
                playerAnimationManager.playerAnimator.Play("Transform");
                transforming = true;
                chaosEmeralds.SetActive(true);
                yield return new WaitForSeconds(transformTime / 2f);
                isSuperSonic = true;
                yield return new WaitForSeconds(transformTime / 2f);
                chaosEmeralds.SetActive(false);
                transforming = false;
                StartCoroutine(ringDrain());
            }
        }

    }

    IEnumerator ringDrain()
    {
        while (isSuperSonic == true)
        {
            yield return new WaitForSeconds(1f);
            playerHpManager.hp--;
            playerHpManager.UpdateRings();

            if(playerHpManager.hp == 0)
            {
                isSuperSonic = false;
            }
        }
    }


    // FixedUpdate is called once per physics "frame" we do all physics related things here
    void FixedUpdate()
    {
        if (transforming)
        {
            rb.velocity = new Vector3(0f, 0.5f, 0f);
        }

        #region Lazy System
        playerForwardParent.eulerAngles = playerCam.transform.eulerAngles;
        playerForwardParent.localRotation = Quaternion.Euler(0f, playerForwardParent.localEulerAngles.y, 0f);
        if (playerDrift.drifting == true)
        {
            playerForwardDummy.localPosition = new Vector3(inputCore.directionalInput.x * 5f, 0f, inputCore.directionalInput.y * 5f);
        }
        else
        {
            playerForwardDummy.localPosition = new Vector3(inputCore.directionalInput.x, 0f, inputCore.directionalInput.y);
        }

        //playerForward.LookAt(playerForwardDummy);
        playerForward.rotation = Quaternion.Slerp(playerForward.rotation, Quaternion.LookRotation((playerForwardDummy.position - playerForward.position).normalized), turnSpeedCurve.Evaluate(velocityMagnitude));

        #endregion

        #region Simple Stuff
        velocity = rb.velocity;
        velocityMagnitude = velocity.magnitude;

        #endregion

        #region Detection

        grounded = Physics.Raycast(transform.position, -transform.up, out playerGroundHit, groundDetectionDistance, groundDetectionMask, QueryTriggerInteraction.Ignore);

        if (grounded == true)
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

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        PlayerPrefs.SetInt("TimesLoaded", PlayerPrefs.GetInt("TimesLoaded") + 1);
        PlayerPrefs.SetString("LastSceneLoaded", scene.name);
        SceneManager.LoadScene(scene.name);

    }

    public void Explode(Vector3 position, float radius, float force)
    {
        explosionObjs = Physics.OverlapSphere(position, radius);
        for (int i = 0; i < explosionObjs.Length; i++)
        {
            Rigidbody r = explosionObjs[i].GetComponent<Rigidbody>();
            if (r != null)
            {
                if (r != rb)
                    r.AddExplosionForce(force, position, radius, 0f, ForceMode.Impulse);
            }
        }
    }
}
