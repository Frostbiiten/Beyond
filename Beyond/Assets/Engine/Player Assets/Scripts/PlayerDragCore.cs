using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDragCore : MonoBehaviour
{

    public PlayerCore playerCore;

    [Tooltip("Current drag")]
    public float currentDrag;

    #region Drag Amounts
    [Header("Grounded")]

    [Tooltip("The amount of drag to be applied based on speed (ground)")]
    public AnimationCurve groundedDragCurve;

    [Tooltip("The magnitude of ground drag")]
    public float groundDragMagnitude;

    [Tooltip("The amount of slide drag")]
    public float slideDrag = 0.1f;

    [Header("Airborne")]

    [Tooltip("Drag in the air")]
    public float airDrag;
    #endregion


    void FixedUpdate()
    {
        /*
        Currentdrag is always overrided by the lower section/ region, so the closest update below this message has least priority
        *Always check for script enabled, not grounded or something
        */

        #region Grounded Base Drag
        if(playerCore.groundedPhysics.enabled == true)
        {
            if(playerCore.ball == false)
            {
                currentDrag = groundedDragCurve.Evaluate(playerCore.velocityMagnitude) * groundDragMagnitude;
            }
            else
            {
                currentDrag = groundedDragCurve.Evaluate(playerCore.velocityMagnitude) * groundDragMagnitude * playerCore.groundedPhysics.spindashDragScalar;
            }

        }

        if(playerCore.playerStompSlide.sliding == true)
        {
            currentDrag = slideDrag;
        }
        #endregion

        #region Airborne Base Drag
        if (playerCore.airbornePhysics.enabled == true)
        {
            currentDrag = airDrag;
        }
        #endregion

        playerCore.rb.drag = currentDrag;
    }
}
