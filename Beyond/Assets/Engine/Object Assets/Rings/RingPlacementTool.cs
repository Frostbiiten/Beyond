using BLINDED_AM_ME;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingPlacementTool : MonoBehaviour
{
    public float ringOffset;
    public Path_Comp path;
    public float step;
    public GameObject placementObjects;
    public Transform ringParent;
    //for now, needs to be tweaked
    // add capsule version
    // and a version that checks how much tangent has changed
    void Start()
    {
        path = gameObject.GetComponent<Path_Comp>();
        for (float q = 0f; q < path.TotalDistance; q += step)
        {
            Instantiate(placementObjects, path.GetPoint(q + ringOffset, path), Quaternion.LookRotation(path.GetTangent(q + ringOffset, path)), ringParent);
        }
    }
}
