using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;

public class PlayerHomingAttack : MonoBehaviour
{
    public PlayerCore playerCore;
    public List<Collider> sortedTargets = new List<Collider>();
    public bool homing;
    public RaycastHit homingRay;
    public Transform currentTarget;
    public float maxHomingDistance;
    public float homingSpeed;
    public float homingExplosionY;
    public float airDashPower;
    public bool utopiaStyleBounce;
    public bool airDashed;
    private Transform oldTarget;
    public Transform homingTarget;
    public Animator homingIconAnim;
    public Collider[] objectsWithinRange;
    public Vector3 capsuleOffset;
    public Transform cam;
    public float homingShakeAmount = 5f;
    public float shakeTime;
    public Transform faceVelocity;

    int homingDetectionMask;
    void Start()
    {
        //oh =~ is opposite for mask
        //groundDetectionMask |= (int)PlayerLayerHelper.Layers.Homeable;
        homingDetectionMask = (int)PlayerLayerHelper.Layers.Homeable;

        //Calculate layermask to Raycast to. (Ignore "cube" && "sphere" layers)
        //int layerMask = ~((1 << cubeLayerIndex) | (1 << sphereLayerIndex));

        ////Calculate layermask to Raycast to. (Raycast to "cube" && "sphere" layers only)
        //int layerMask = (1 << cubeLayerIndex) | (1 << sphereLayerIndex);
    }
    void FixedUpdate()
    {
        #region Custom things
        faceVelocity.LookAt(faceVelocity.position + playerCore.velocity);
        faceVelocity.rotation = Quaternion.Euler(new Vector3(0f, faceVelocity.eulerAngles.z, 0f));
        #endregion

        #region Homing Target Main
        homingTarget.gameObject.SetActive(!playerCore.grounded);
        homingTarget.LookAt(cam);
        oldTarget = currentTarget;

        #endregion

        #region Main Raycast And Sort
        Physics.OverlapSphereNonAlloc(transform.position, maxHomingDistance, objectsWithinRange, homingDetectionMask, QueryTriggerInteraction.Collide);
        sortedTargets = SortForHoming(objectsWithinRange);
        #endregion

        #region Homing Logic p1
        if (sortedTargets.Count > 0 && playerCore.airbornePhysics.enabled == true)
        {

            homingTarget.position = sortedTargets[0].transform.position;
            currentTarget = sortedTargets[0].transform;
            homingIconAnim.enabled = true;
            if (currentTarget != oldTarget)
            {
                homingIconAnim.Play("Zoom");
            }
        }
        else
        {

            currentTarget = null;
            homingIconAnim.enabled = false;
        }
        #endregion

        #region When Homing
        if (homing == true && currentTarget)
        {
            playerCore.ball = true; // I have to do this for some reason
            playerCore.rb.velocity = ((currentTarget.position + capsuleOffset) - transform.position).normalized * homingSpeed;
        }
        #endregion

        // BIND TO INPUTCORE
        #region MAIN HOMING LOGIC P2
        if (playerCore.airbornePhysics.enabled == true)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (currentTarget)
                {
                    if (Physics.Raycast(transform.position, (currentTarget.position - transform.position).normalized, out homingRay, maxHomingDistance))
                    {
                        if (homingRay.collider.CompareTag("Homing Target") || homingRay.collider.CompareTag("Enemy"))
                        {
                            homing = true;
                        }
                        else
                        {
                            homing = false;
                        }
                    }
                    else
                    {
                        homing = false;
                    }
                }

                if (homing == false && airDashed == false)
                {
                    airDashed = true;
                    playerCore.ball = true;
                    playerCore.rb.AddForce(playerCore.playerForward.forward * airDashPower, ForceMode.Impulse);
                    //playerCore.SoundCore.mainPlayerAudio.PlayOneShot(playerCore.SoundCore.jump[Random.Range(0, playerCore.SoundCore.jump.Count - 1)]);
                }
            }
        }
        #endregion

    }

    void OnTriggerEnter(Collider other)
    {
        #region Main Trigger Enter

        if (other.CompareTag("Enemy") && (playerCore.playerHpManager.attacking == true || playerCore.playerHpManager.invincible == true))
        {
            //playerCore.Score.score = playerCore.Score.enemyScore + scoreEnemyGain;

            StartCoroutine(playerCore.camShake.Shake(homingShakeAmount, shakeTime));
            if (utopiaStyleBounce == true)
            {
                playerCore.rb.velocity = new Vector3(playerCore.rb.velocity.x, homingExplosionY, playerCore.rb.velocity.z);
            }
            else
            {
                playerCore.rb.velocity = new Vector3(0f, homingExplosionY, 0f);
            }
            //playerCore.SoundCore.mainPlayerAudio.PlayOneShot(playerCore.SoundCore.homing[Random.Range(0, playerCore.SoundCore.homing.Count - 1)]);
            if(homing == true)
            {
                StartCoroutine(SetBallFalse());
                playerCore.playerAnimationManager.playerAnimator.SetFloat("HomingAnim#", Mathf.RoundToInt(Random.Range(0f, 5f)));
                playerCore.playerAnimationManager.playerAnimator.Play("Homing");
            }

        }

        homing = false;

        #endregion
    }

    #region Workarounds
    IEnumerator SetBallFalse() // Ugh because of damage problems
    {
        yield return null;
        playerCore.ball = false;
    }

    void OnTriggerStay(Collider other)
    {
        homing = false;
    }

    #endregion

    #region Other Methods
    public List<Collider> SortForHoming(Collider[] input)
    {
        List<Collider> outputNew = new List<Collider>();

        for (int e = 0; e < input.Length; e++)
        {
            if (input[e])
            {
                if (input[e].CompareTag("Enemy") || input[e].CompareTag("Homing Target"))
                {
                    Vector3 dir = (new Vector3(input[e].transform.position.x, transform.position.y, input[e].transform.position.z) - transform.position).normalized;
                    if (Vector3.Dot(dir, playerCore.playerForward.forward) > 0)
                    {
                        outputNew.Add(input[e]);
                    }
                }
            }

        }

        outputNew.Sort(delegate (Collider a, Collider b)
        {
            return Vector3.Distance(transform.position, a.transform.position)
        .CompareTo(
              Vector3.Distance(transform.position, b.transform.position));
        });

        return outputNew;
    }

    #endregion
}
