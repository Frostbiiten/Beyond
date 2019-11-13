using BLINDED_AM_ME;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerV2 : MonoBehaviour
{
    public PlayerCore currentCam;

    public Path_Comp path;//optional
    public Vector3 pathPositionOffset;

    public Transform cameraTransformReference;
    public bool cameraLook;
    public Transform cameraLookReference;

    public float triggerSize;

    public bool active;

    public float postionInterpolationSpeed;
    public float rotationInterpolationSpeed;


    public Vector3 point;

    public Quaternion rot;

    public float pathTimeOffset;

    float checkRadius;

    public bool lookLock;
    public float lookLockDistance;
    public Vector3 lookLockOffset;

    public AnimationCurve speedCurve;

    private void Start()
    {
        if (path)
        {
            checkRadius = Vector3.Distance(path.GetPoint(0, path), path.GetPoint(path.TotalDistance, path));
        }
    }
    void FixedUpdate()
    {
        if (currentCam)
        {
            if (currentCam.orbitCam.currentTrigger == this)
            {
                active = true;
            }
            else
            {
                active = false;
            }

            if (path)
            {
                //float distance = path.GetNearestPoint(currentCam.transform.position, path, 0.5f);
                float distance = path.GetNearestPointCustom(currentCam.transform.position, path, speedCurve.Evaluate(currentCam.velocityMagnitude), 0.5f);
                point = path.GetPoint(distance + pathTimeOffset, path);

                if (Vector3.Distance((path.GetPoint(0, path) + path.GetPoint(path.TotalDistance, path) / 2f), currentCam.transform.position) < checkRadius)
                {
                    if (Vector3.Distance(point, currentCam.transform.position) < triggerSize)
                    {
                        EnterTrigger();
                    }
                    else
                    {
                        ExitTrigger();
                    }
                }


                if(active == true)
                {
                    Vector3 dir = path.GetTangent(distance, path);
                    Vector3 offsetFixed = pathPositionOffset;

                

                    rot = Quaternion.Slerp(rot, Quaternion.LookRotation(dir), rotationInterpolationSpeed);
                    currentCam.orbitCam.transform.rotation = rot;

                    Vector3 camoffset = Quaternion.LookRotation(dir) * offsetFixed; //thats to local space
                    currentCam.orbitCam.transform.position = Vector3.Lerp(currentCam.orbitCam.transform.position, point + camoffset, postionInterpolationSpeed);

                }
            }
            else
            {
                if (active == true)
                {
                    if(lookLock == true)
                    {
                        Vector3 dir = (cameraLookReference.position - currentCam.transform.position).normalized;
                        currentCam.orbitCam.cameraLookatLock.position = currentCam.transform.position - dir * lookLockDistance;

                        Vector3 offset = new Vector3(dir.x * lookLockOffset.x, dir.y * lookLockOffset.y, dir.z * lookLockOffset.z);
                        currentCam.orbitCam.transform.position = currentCam.orbitCam.cameraLookatLock.position + offset;

                        Vector3 dir2 = (cameraLookReference.position - currentCam.transform.position).normalized;
                        currentCam.orbitCam.transform.rotation = Quaternion.Slerp(currentCam.orbitCam.transform.rotation, Quaternion.LookRotation(dir2, currentCam.transform.up), rotationInterpolationSpeed);

                    }
                    else
                    {
                        currentCam.orbitCam.transform.position = Vector3.Lerp(currentCam.orbitCam.transform.position, cameraTransformReference.position, postionInterpolationSpeed);
                        if(cameraLook == true)
                        {
                            rot = Quaternion.Slerp(rot, Quaternion.LookRotation(cameraLookReference.position - currentCam.orbitCam.transform.position), rotationInterpolationSpeed);
                        }
                        else
                        {
                            rot = Quaternion.Slerp(rot, cameraTransformReference.rotation, rotationInterpolationSpeed);
                        }

                        currentCam.orbitCam.transform.rotation = rot;
                    }

                }
            }
        }




    }

    void OnTriggerEnter(Collider other)
    {
        if (path == null)
        {
            if (other.CompareTag("Player"))
            {
                if (!currentCam)
                {
                    currentCam = other.GetComponent<PlayerCore>();
                }
                currentCam.orbitCam.currentTrigger = this;
            }

        }

    }

    void OnTriggerExit(Collider other)
    {
        if (path == null)
        {
            if (other.CompareTag("Player"))
            {
                if (other.GetComponent<PlayerCore>() == currentCam)
                {
                    if (currentCam.orbitCam.currentTrigger == this)
                    {
                        currentCam.orbitCam.currentTrigger = null;
                    }
                }

            }

        }

    }

    void EnterTrigger()
    {
        //rot = currentCam.orbitCam.transform.rotation;
        //point = currentCam.orbitCam.transform.position;
        currentCam.orbitCam.currentTrigger = this;
    }

    void ExitTrigger()
    {
        if (currentCam.orbitCam.currentTrigger == this)
        {
            currentCam.orbitCam.currentTrigger = null;
        }
    }




}
