using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAffector : MonoBehaviour
{
    public List<Rigidbody> affectedRbs = new List<Rigidbody>();

    public Vector3 force;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            affectedRbs.Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            affectedRbs.Remove(rb);
        }
    }

    void FixedUpdate()
    {
        for(int i = 0; i < affectedRbs.Count; i++)
        {
            affectedRbs[i].AddForce(force);
        }
    }
}
