using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunPathObj : MonoBehaviour
{
    public string colTag = "Player";
    void OnTriggerEnter(Collider other)
    {
        PlayerCore pc = other.GetComponent<PlayerCore>();
        if (pc.rPath.canGoOnPath == true && other.CompareTag(colTag) && pc.rPath.runningOnPath == false)
        {
            pc.rPath.EnterPath(gameObject);
        }

    }
}
