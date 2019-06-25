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

    public float waterForce;
    public float playerXZResistance;

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

    void OnCollisionStay(Collision collision)
    {
        if (playerCore.velocityMagnitude < threshHold)
        {
            defaultCollider.isTrigger = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            playerCore.rb.AddForce(new Vector3(-playerCore.velocity.x, 0f, -playerCore.velocity.z) * playerXZResistance);
            playerCore.rb.AddForce(new Vector3(0f, Mathf.Abs(playerCore.velocity.y) * waterForce, 0f));

        }
    }

}
