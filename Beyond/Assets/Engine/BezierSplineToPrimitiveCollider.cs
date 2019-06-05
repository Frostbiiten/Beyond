using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class BezierSplineToPrimitiveCollider : MonoBehaviour
{
    public float sphereSize;
    public float sphereOffset;
    public BezierSpline bs;
    public float step;
    //for now, needs to be tweaked
    // add capsule version
    // and a version that checks how much tangent has changed
    void Start()
    {
        bs = gameObject.GetComponent<BezierSpline>();
        for(float q = 0f; q < 1f; q += step)
        {
            SphereCollider m_Collider = gameObject.AddComponent<SphereCollider>();
            //m_Collider.center = Vector3.zero; // the center must be in local coordinates
            m_Collider.center = transform.InverseTransformPoint(bs.GetPoint(q + sphereOffset));
            m_Collider.radius = sphereSize;
            m_Collider.isTrigger = true;
        }
    }



}
