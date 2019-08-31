using UnityEngine;
using System.Collections;
using BLINDED_AM_ME;

public class OrbitCamV2 : MonoBehaviour
{
    [System.Serializable]
    public struct CameraTrigger
    {
        public bool setPosition;
        public bool setRotation;
        public bool setLookat;

        public Transform positionTarget;
        public Transform rotationTarget;
        public Transform lookTarget;

        public float rotationInterpolate;
        public float positionInterpolate;

        CameraTrigger(bool setPositioni, bool setRotationi, bool setLookati, Transform positionTargeti, Transform rotationTargeti, Transform lookTargeti, float rotationInterpolatei, float positionInterpolatei)
        {
            setPosition = setPositioni;
            setRotation = setRotationi;
            setLookat = setLookati;

            positionTarget = positionTargeti;
            rotationTarget = rotationTargeti;
            lookTarget = lookTargeti;

            rotationInterpolate = rotationInterpolatei;
            positionInterpolate = positionInterpolatei;
        }
    }

    public CameraTriggerV2 currentTrigger;

    public PlayerCore pc;
    public Transform target;

    public float rotationDamp = 0.55f;
    public float movementDamp = 0.55f;

    public bool inCameraTrigger;
    public CameraTrigger currentCameraTrigger;

    public Transform currentLook;

    public Transform cameraLockTrigger;

    public float driftSpeed;
    public AnimationCurve driftSpeedCurve;

    [System.Serializable]
    public struct camAnim
    {
        public AnimationCurve x;
        public AnimationCurve y;
        public AnimationCurve z;
    }

    public camAnim StartA;

    public camAnim StartB;

    public Transform startPositionReference;
    public Transform startLookatReference;

    //public bool betaV2;

    public Vector3 dir;

    public float distance = 10f;
    public AnimationCurve distanceCurve;
    float currentDistance;

    public Transform cameraLookatLock;
    Vector3 upVector;

    void FixedUpdate()
    {
        if (inCameraTrigger == false)
        {
            if (target)
            {
                if (!currentTrigger)
                {
                    /*
                     * OLD
                    currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, v2TurnSpeedCurve.Evaluate(pc.velocityMagnitude) * pc.inputCore.directionalInput.x, v2TurnSpeedInterpolation);
                    v2Dir = Quaternion.Euler(0, currentTurnSpeed, 0) * v2Dir;

                    v2CurrentSideLookOffset = Mathf.Lerp(v2CurrentSideLookOffset, pc.inputCore.directionalInput.x * v2SideTurnLook, 0.1f);
                    Debug.Log(v2CurrentSideLookOffset);
                    Vector3 d = v2Dir * v2Distance;
                    Vector3 pos1;
                    pos1 = (pc.transform.position) - d;
                    Quaternion look;
                    Vector3 lookpos;
                    lookpos = target.position;
                    lookpos.y = transform.position.y;
                    look = Quaternion.LookRotation((lookpos - transform.position).normalized);



                    transform.rotation = look;
                    transform.position = pos1;
                    */

                    currentDistance = distance * distanceCurve.Evaluate(pc.velocityMagnitude);
                    Vector3 targetpos = target.position;// add offset etc here

                    dir = (transform.position - targetpos).normalized;

                    dir.y = Mathf.Clamp(dir.y, 0.15f, 99f);

                    Quaternion look;
                    

                    if (pc.grounded == false)
                    {

                        look = Quaternion.LookRotation((targetpos - transform.position).normalized);

                        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationDamp);
                        transform.position = Vector3.Lerp(transform.position, targetpos + (dir * currentDistance), movementDamp);
                    }
                    else
                    {
                        upVector = Vector3.Lerp(upVector, pc.transform.up, 0.05f);
                        look = Quaternion.LookRotation((targetpos - transform.position).normalized, pc.transform.up);
                        //look = Quaternion.Euler(look.eulerAngles.x, look.eulerAngles.y, look.eulerAngles.z);

                        transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationDamp);
                        transform.position = Vector3.Lerp(transform.position, targetpos + (dir * currentDistance), movementDamp);
                    }

                    /*
                    if(pc.velocityMagnitude >= pc.playerAnimationManager.driftThreshold)
                    {
                        transform.RotateAround(pc.transform.position, pc.transform.up, Input.GetAxis("Drift") * driftSpeedCurve.Evaluate(pc.velocityMagnitude));
                    }
                    */

                }
            }
        }

        if (pc.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartFast"))
        {
            float t = pc.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Vector3 pos = new Vector3(StartB.x.Evaluate(t), StartB.y.Evaluate(t), StartB.z.Evaluate(t));
            startPositionReference.localPosition = pos;
            transform.position = startPositionReference.position;
            transform.LookAt(startLookatReference);
        }

        if (pc.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("StartNormal"))
        {
            float t = pc.playerAnimationManager.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            Vector3 pos = new Vector3(StartA.x.Evaluate(t), StartA.y.Evaluate(t), StartA.z.Evaluate(t));
            startPositionReference.localPosition = pos;
            transform.position = startPositionReference.position;
            transform.LookAt(startLookatReference);
        }
    }
}