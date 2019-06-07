using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public Collider defaultCollider;

    public PlayerCore playerCore;

    public float threshHold;

    public bool playerIn;
    public bool running;
    void OnTriggerEnter()
    {
        if (playerCore.velocityMagnitude >= threshHold)
        {
            defaultCollider.isTrigger = false;
        }
        else
        {
            defaultCollider.isTrigger = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (playerCore.velocityMagnitude < threshHold)
        {
            defaultCollider.isTrigger = true;
        }
    }

}
