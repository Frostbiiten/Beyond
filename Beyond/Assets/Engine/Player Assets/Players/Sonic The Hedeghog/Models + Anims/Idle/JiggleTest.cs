using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JiggleTest : MonoBehaviour
{
    Quaternion oldRotation;

    public Quaternion oldRot;
    public Quaternion rot;

    public Vector3 acceleration;
    public Quaternion smoothAccel;

    public float strength;

    public float interpolation = 0.6f;

    public bool global = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (global)
        {
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(oldRot);

            oldRot = transform.rotation;

            float angle = 0.0f;
            Vector3 axis = Vector3.zero;

            deltaRotation.ToAngleAxis(out angle, out axis);

            angle *= Mathf.Deg2Rad;

            acceleration = axis * angle * (1.0f / Time.deltaTime);

            smoothAccel = Quaternion.Slerp(smoothAccel, Quaternion.Euler(acceleration * strength), interpolation);

            transform.rotation *= smoothAccel;
        }
        else
        {
            Quaternion deltaRotation = transform.localRotation * Quaternion.Inverse(oldRot);

            oldRot = transform.localRotation;

            float angle = 0.0f;
            Vector3 axis = Vector3.zero;

            deltaRotation.ToAngleAxis(out angle, out axis);

            angle *= Mathf.Deg2Rad;

            acceleration = axis * angle * (1.0f / Time.deltaTime);

            smoothAccel = Quaternion.Slerp(smoothAccel, Quaternion.Euler(acceleration * strength), interpolation);

            transform.localRotation *= smoothAccel;
        }

    }
}
