using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunPathObj : MonoBehaviour
{
    public string colTag = "Player";
    public float triggerSize = 4f;
    public float tightness = 0.075f;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(colTag))
        {
            PlayerCore pc = other.GetComponent<PlayerCore>();
            if (pc.rPath.canGoOnPath == true && pc.rPath.runningOnPath == false)
            {
                pc.rPath.tightness = tightness;
                pc.rPath.exitDistance = triggerSize;
                pc.rPath.EnterPath(gameObject);
            }
        }


    }
}
