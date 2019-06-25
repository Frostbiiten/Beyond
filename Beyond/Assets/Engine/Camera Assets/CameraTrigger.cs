using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public PlayerCore currentCam;
    public bool active;
    public bool activeOnlyLook;

    public OrbitCamV2.CameraTrigger ct;

    public bool onlyLook;
    public Transform onlyLookTarget;
    public Transform refp;
    public float lookOnlyInterpolation;
    public Transform lockPositionTarget;
    public float lookDistance;

    void FixedUpdate()
    {
        active = false;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (!currentCam)
            {
                currentCam = other.GetComponent<PlayerCore>();
            }

            lockPositionTarget = currentCam.orbitCam.cameraLockTrigger;
            refp = currentCam.orbitCam.transform;
            if(onlyLook == false)
                active = true;

            if (onlyLook == true)
                activeOnlyLook = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeOnlyLook = false;
            currentCam.orbitCam.currentLook = null;


            active = false;

            if (onlyLook == true)
                activeOnlyLook = false;

            currentCam.orbitCam.inCameraTrigger = false;
        }
    }



    void Update()
    {
        if(active == true && currentCam)
        {
            currentCam.orbitCam.currentCameraTrigger = ct;
            currentCam.orbitCam.inCameraTrigger = active;
        }

        if (activeOnlyLook == true && currentCam)
        {
            currentCam.orbitCam.currentLook = onlyLookTarget;
            currentCam.orbitCam.lookInterpolation = lookOnlyInterpolation;
        }

        if (onlyLookTarget && lockPositionTarget)
        {
            lockPositionTarget.localPosition = Vector3.Lerp(lockPositionTarget.localPosition, (onlyLookTarget.position - refp.position).normalized * lookDistance, 0.1f);
        }

        
    }
}
