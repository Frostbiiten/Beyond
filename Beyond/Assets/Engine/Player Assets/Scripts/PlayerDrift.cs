using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrift : MonoBehaviour
{
    public PlayerCore playerCore;
    public List<Vector3> pastDirs;
    public float driftDir;
    public bool drifting;

    public float driftThreshold;
    public float driftInterpolate = 0.05f;
    public float driftPower = 2f;
    float initial = 32f;
    void FixedUpdate()
    {
        if (playerCore.inputCore.LShift && playerCore.velocityMagnitude > driftThreshold && playerCore.groundedPhysics.enabled == true)
        {
            drifting = true;
 
        }
        else
        {
            drifting = false;
        }

        if (playerCore.inputCore.LShiftDown)
        {
            initial = playerCore.velocityMagnitude * 0.99f;
        }


        if (playerCore.inputCore.directionalInput.x != 0f)
        {
            driftDir = Mathf.Lerp(driftDir, playerCore.inputCore.directionalInput.x * driftPower, driftInterpolate);
        }


        if(drifting == true)
        {
            playerCore.rb.velocity = Vector3.ClampMagnitude((Quaternion.AngleAxis(driftDir, transform.up) * playerCore.velocity), initial);

        }

    }


}
