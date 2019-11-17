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
    public Transform altCurrentTarget;
    public float maxHomingDistance;
    public float homingSpeed;
    public float homingExplosionY;
    public float airDashPower;
    public float doubleJumpPower;
    public float fireAirDashMultiplier;
    public bool utopiaStyleBounce;
    public bool airDashed;
    private Transform oldTarget;
    public SpriteRenderer homingTarget;
    public Animator homingIconAnim;
    public Collider[] objectsWithinRange;
    public Vector3 capsuleOffset;
    public Transform cam;
    public float homingShakeAmount = 5f;
    public float shakeTime;
    public Transform faceVelocity;
    public float homingDirectionThreshold = 0.5f;

    
    int homingDetectionMask;

    public ShakeTransformEventData shake;
    public ShakeTransform shaker;

    int layerMask;
    void Start()
    {
        //oh =~ is opposite for mask
        //groundDetectionMask |= (int)PlayerLayerHelper.Layers.Homeable;
        homingDetectionMask = (int)PlayerLayerHelper.Layers.Homeable;

        layerMask = ~((int)PlayerLayerHelper.Layers.CameraTrigger | (int)PlayerLayerHelper.Layers.NoPlayerCollide);

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
        if(!playerCore.grounded && currentTarget)
        {
            Vector3 directionToTarget = transform.position - currentTarget.position;
            directionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z);
            float angle = Vector3.Angle(-playerCore.playerAnimationManager.playerSkin.forward, directionToTarget);
            if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
            {
                homingTarget.enabled = false;
            }
            else
            {
                homingTarget.enabled = true;
            }
            
        }
        else
        {
            homingTarget.enabled = false;
        }

        homingTarget.transform.LookAt(cam);

        #endregion

        #region Main Raycast And Sort
        Physics.OverlapSphereNonAlloc(transform.position, maxHomingDistance, objectsWithinRange, homingDetectionMask, QueryTriggerInteraction.Collide);
        sortedTargets = SortForHoming(objectsWithinRange);
        #endregion

        #region Homing Logic p1
        oldTarget = currentTarget;
        if (sortedTargets.Count > 0)
        {
            if (playerCore.airbornePhysics.enabled == true)
            {
                homingTarget.transform.position = sortedTargets[0].transform.position;
                currentTarget = sortedTargets[0].transform;


            }
        }

        if (playerCore.airbornePhysics.enabled == false)
        {
            currentTarget = null;


        }

        if (playerCore.airbornePhysics.enabled == false || currentTarget == null)
        {
            currentTarget = null;
        }

        if (currentTarget != oldTarget)
        {
            if (currentTarget)
            {
                Vector3 directionToTarget = transform.position - currentTarget.position;
                directionToTarget = new Vector3(directionToTarget.x, 0f, directionToTarget.z);
                float angle = Vector3.Angle(-playerCore.playerAnimationManager.playerSkin.forward, directionToTarget);
                if (Mathf.Abs(angle) > 90 && Mathf.Abs(angle) < 270)
                {
                    //Debug.Log("!");

                }
                else
                {
                    homingIconAnim.Play("Zoom");
                }
            }

            
        }
        #endregion

        #region When Homing
        if (homing == true && currentTarget)
        {
            playerCore.ball = true; // I have to do this for some reason
            playerCore.rb.velocity = ((currentTarget.position + capsuleOffset) - transform.position).normalized * homingSpeed;
        }
        #endregion

        #region MAIN HOMING LOGIC P2
        if (playerCore.airbornePhysics.enabled == true)
        {

            //FixSort(out currentTarget);


            if (playerCore.inputCore.FixedUpdateKeyDown)
            {

                playerCore.playerAnimationManager.playerAnimator.Play("Air Ball");


                if (currentTarget)
                {
                    if (Physics.Raycast(transform.position, (currentTarget.position - transform.position).normalized, out homingRay, maxHomingDistance, layerMask, QueryTriggerInteraction.Collide))
                    {
                        if (homingRay.collider.CompareTag("Homing Target") || homingRay.collider.CompareTag("Enemy") || homingRay.collider.CompareTag("ElectricCapsule") || homingRay.collider.CompareTag("FireCapsule") || homingRay.collider.CompareTag("WaterCapsule") || homingRay.collider.CompareTag("CapsuleNormalShield") || homingRay.collider.CompareTag("CapsuleRings"))
                        {
                            playerCore.playerSoundCore.PlayHome();
                            
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
                    if(playerCore.playerHpManager.shield == PlayerHpManager.Shield.none || playerCore.playerHpManager.shield == PlayerHpManager.Shield.normal)
                    {
                        playerCore.rb.AddForce(playerCore.playerForward.forward * airDashPower, ForceMode.Impulse);
                    }
                    else
                    {
                        if (playerCore.playerHpManager.shield == PlayerHpManager.Shield.electric)
                        {
                            playerCore.rb.velocity = new Vector3(playerCore.velocity.x, doubleJumpPower, playerCore.velocity.z);
                        }

                        if (playerCore.playerHpManager.shield == PlayerHpManager.Shield.fire)
                        {
                            playerCore.rb.AddForce(playerCore.playerForward.forward * airDashPower * fireAirDashMultiplier, ForceMode.Impulse);
                        }
                        
                    }

                    //playerCore.SoundCore.mainPlayerAudio.PlayOneShot(playerCore.SoundCore.jump[Random.Range(0, playerCore.SoundCore.jump.Count - 1)]);
                }
                
            }
        }

        #endregion

    }

    void FixSort(out Transform colz)
    {
        colz = null;
        for (int i = 0; i < sortedTargets.Count; i++)
        {
            //Vector3 dir = (sortedTargets[i].transform.position - new Vector3(transform.position.x, sortedTargets[i].transform.position.y, transform.position.z)).normalized;
            //float dot = Vector3.Dot(dir, playerCore.playerAnimationManager.playerSkin.forward)

            Vector3 pos = playerCore.playerAnimationManager.playerSkin.InverseTransformPoint(sortedTargets[i].transform.position);
            if (pos.z >= -0.5f)
            {

                colz = sortedTargets[i].transform;
                break;
                return;

            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        #region Main Trigger Enter

        if (other.CompareTag("Enemy") && (playerCore.playerHpManager.attacking == true || playerCore.playerHpManager.invincible == true))
        {
            //playerCore.Score.score = playerCore.Score.enemyScore + scoreEnemyGain;

            shaker.AddShakeEvent(shake);
            if (utopiaStyleBounce == true)
            {
                playerCore.rb.velocity = new Vector3(playerCore.rb.velocity.x, homingExplosionY, playerCore.rb.velocity.z);
            }
            else
            {
                if(playerCore.grounded == false)
                {
                    playerCore.rb.velocity = new Vector3(0f, homingExplosionY, 0f);
                }

            }
            //playerCore.SoundCore.mainPlayerAudio.PlayOneShot(playerCore.SoundCore.homing[Random.Range(0, playerCore.SoundCore.homing.Count - 1)]);
            if(homing == true)
            {
                playerCore.playerAnimationManager.playerAnimator.SetFloat("HomingAnim#", Mathf.RoundToInt(Random.Range(0f, 5f)));
                playerCore.playerAnimationManager.playerAnimator.Play("Homing");
            }

        }

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
        //this is temp
        if (other.gameObject.layer != 15 && other.gameObject.layer != 2 && other.gameObject.layer != 4 && playerCore.groundedPhysics.enabled == false)
        {
            //StartCoroutine(SetBallFalse()); instead, check  if homing trick is playing and if so set ball to false
            homing = false;

        }

    }

    #endregion

    #region Other Methods
    public List<Collider> SortForHoming(Collider[] input)
    {
        List<Collider> outputNew = new List<Collider>();
        List<Collider> outPutalt = new List<Collider>();

        for (int e = 0; e < input.Length; e++)
        {
            if (input[e])
            {
                if (input[e].CompareTag("Enemy") || input[e].CompareTag("Homing Target") || input[e].CompareTag("ElectricCapsule") || input[e].CompareTag("FireCapsule") || input[e].CompareTag("WaterCapsule") || input[e].CompareTag("CapsuleNormalShield") || input[e].CompareTag("CapsuleRings"))
                {

                    Vector3 dirToTarget = (transform.position - input[e].transform.position).normalized;
                    dirToTarget.y = 0f;

                    Vector3 v = new Vector3(playerCore.playerAnimationManager.playerSkin.forward.x, 0f, playerCore.playerAnimationManager.playerSkin.forward.z).normalized;
                    v.y = 0f;

                    if (Vector3.Dot(dirToTarget, -v) > 0)
                    {
                        outputNew.Add(input[e]);
                    }
                        

                    outPutalt.Add(input[e]);
                }
            }

        }

        outputNew.Sort(delegate (Collider a, Collider b)
        {
            return Vector3.Distance(transform.position, a.transform.position)
        .CompareTo(
              Vector3.Distance(transform.position, b.transform.position));
        });

        outPutalt.Sort(delegate (Collider a, Collider b)
        {
            return Vector3.Distance(transform.position, a.transform.position)
        .CompareTo(
              Vector3.Distance(transform.position, b.transform.position));
        });
        return outputNew;
    }

    #endregion
}
