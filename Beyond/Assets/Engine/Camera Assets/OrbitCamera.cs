using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class OrbitCamera : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;

    public bool clampDistance;

    public float LerpMovement;
    public float LerpRotation;

    public Transform camForward;

    public float driftTurnSpeed;
    public float topDriftSpeed;

    public bool allowMouseFreeMovement;
    public float mouseFreeMovementSpeed;

    public PlayerCore PlayerCore;
    public int state;

    public float movementInterpolation;
    public float rotationInterpolation;
    public bool goToPosition;
    public Transform transformTarget;
    public bool rotateToRotation;
    public bool lookAt;
    public Transform lookTarget;

    public float freeDistance;
    public Transform realLookTarget;
    public bool playStart;
    public Transform startTarget;




    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void FixedUpdate()
    {
        if (state == 0)
        {
            if (target && !PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast") || !PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartNormal"))
            {
                if (Vector3.Distance(transform.position, target.position) >= freeDistance)
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                    //x = x + Input.GetAxis("X"); 2 player mode
                    if (allowMouseFreeMovement == true)
                    {
                        //Debug.Log(PlayerCore.playerAnimationManager.playerSkin.InverseTransformVector(PlayerCore.velocity).z);
                        //x = x + * mouseFreeMovementSpeed;
                        //Quaternion.LookRotation(PlayerCore.velocity);
                    }
                    realLookTarget.position = target.position;


                    if (Input.GetAxis("Drift") != 0f)
                    {
                        if (PlayerCore.airbornePhysics.enabled == false)
                        {
                            PlayerCore.rb.velocity = Vector3.ClampMagnitude(PlayerCore.rb.velocity, topDriftSpeed);
                            x = x + Input.GetAxis("Drift") * driftTurnSpeed;
                        }

                    }

                    y = ClampAngle(y, yMinLimit, yMaxLimit);


                    Quaternion rotation = Quaternion.Euler(y, x, 0);

                    distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                    RaycastHit hit;
                    if (clampDistance == true)
                    {
                        if (Physics.Linecast(target.position, transform.position, out hit))
                        {
                            distance -= hit.distance;
                        }
                    }

                    Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                    Vector3 position = rotation * negDistance + realLookTarget.position;

                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, LerpRotation);
                    //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + baseForward.eulerAngles.y, transform.eulerAngles.z);
                    transform.position = Vector3.Lerp(transform.position, position, LerpMovement);
                }
                else
                {
                    transform.LookAt(realLookTarget);
                }
            }

            if(PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast") || PlayerCore.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartNormal"))
            {
                transform.position = startTarget.position;
                transform.LookAt(target);
            }

        }

        Vector3 dir;

        if (state == 1)
        {
            if (goToPosition == true)
            {
                transform.position = Vector3.Slerp(transform.position, transformTarget.position, movementInterpolation);
            }
            if (rotateToRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, transformTarget.rotation, rotationInterpolation);
            }
            if (lookAt == true)
            {
                dir = lookTarget.position - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), rotationInterpolation);

            }
        }


    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}