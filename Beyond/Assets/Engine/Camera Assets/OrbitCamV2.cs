using UnityEngine;
using System.Collections;
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

    public PlayerCore pc;
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;


    public float x = 0.0f;
    public float y = 0.0f;

    public float rotationDamp = 0.55f;
    public float movementDamp = 0.55f;

    public float lookTurnSpeed;

    public float currentTurnSpeed;
    public float turnSpeedInterpolation;
    public AnimationCurve turnSpeedCurve;

    public bool inCameraTrigger;
    public CameraTrigger currentCameraTrigger;

    public Transform currentLook;

    public Quaternion lookRotation; // do not change

    public float lookInterpolation;

    public Transform cameraLockTrigger;

    public Vector3 lookOffset;
    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

    }

    void FixedUpdate()
    {

        if(inCameraTrigger == false)
        {
            if (target)
            {
                if(currentLook == null)
                {
                    x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                    y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                }


                y = ClampAngle(y, yMinLimit, yMaxLimit);

                x += pc.inputCore.directionalInput.x * currentTurnSpeed;

                currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, turnSpeedCurve.Evaluate(pc.velocityMagnitude), turnSpeedInterpolation);

                Quaternion rotation = Quaternion.Euler(y, x, 0f);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    //distance -= hit.distance;
                }
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;
                

                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationDamp);
                transform.position = Vector3.Lerp(transform.position, position, movementDamp);

                if (currentLook)
                {
                    transform.position = cameraLockTrigger.position + lookOffset;
                    lookRotation = Quaternion.Slerp(lookRotation, Quaternion.LookRotation((currentLook.position - cameraLockTrigger.position).normalized), lookInterpolation);
                    transform.rotation = lookRotation;
                }
            }
        }
        else
        {
            if(currentCameraTrigger.setPosition == true)
            {
                transform.position = Vector3.Lerp(transform.position, currentCameraTrigger.positionTarget.position, currentCameraTrigger.positionInterpolate);
            }

            if (currentCameraTrigger.setRotation == true)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, currentCameraTrigger.rotationTarget.rotation, currentCameraTrigger.rotationInterpolate);
            }

            if (currentCameraTrigger.setLookat == true && currentCameraTrigger.lookTarget)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((currentCameraTrigger.lookTarget.position - transform.position).normalized), currentCameraTrigger.rotationInterpolate);
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
