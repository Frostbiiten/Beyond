using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Vector3 force;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            pc.rb.velocity = transform.TransformVector(force);
            pc.ball = false;
        }
    }
}
