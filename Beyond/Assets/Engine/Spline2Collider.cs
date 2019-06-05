using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINDED_AM_ME;

public class Spline2Collider : MonoBehaviour
{
    public float sphereSize;
    public float sphereOffset;
    public Path_Comp path;
    public float step;
    //for now, needs to be tweaked
    // add capsule version
    // and a version that checks how much tangent has changed
    void Start()
    {
        path = gameObject.GetComponent<Path_Comp>();
        for (float q = 0f; q < path.TotalDistance; q += step)
        {
            SphereCollider m_Collider = gameObject.AddComponent<SphereCollider>();
            m_Collider.center = transform.InverseTransformPoint(path.GetPoint(q + sphereOffset, path));
            m_Collider.radius = sphereSize;
            m_Collider.isTrigger = true;
        }
    }
}
