using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunPathObj : MonoBehaviour
{
    public string colTag = "Player";
    public float triggerSize = 4f;
    void OnTriggerEnter(Collider other)
    {
        PlayerCore pc = other.GetComponent<PlayerCore>();
        if (pc.rPath.canGoOnPath == true && other.CompareTag(colTag) && pc.rPath.runningOnPath == false)
        {
            pc.rPath.exitDistance = triggerSize;
            pc.rPath.EnterPath(gameObject);
        }

    }
}
