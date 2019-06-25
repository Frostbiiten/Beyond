using LayerHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    public RaycastHit footLeft;
    public RaycastHit footRight;
    public Transform lFoot;
    public bool leftCollide;
    bool oldLeft;
    public Transform rFoot;
    public bool rightCollide;
    bool oldRight;
    public float detectionDistance;
    public int detectionLayer;
    public AudioSource footStepSource;
    public AudioClip defaultStep;
    public AudioClip metalStep;
    public AudioClip grassStep;
    public AudioClip sandStep;
    public AudioClip waterStep;
    void Start()
    {
        detectionLayer = ~(
           (int)PlayerLayerHelper.Layers.Player |
           (int)PlayerLayerHelper.Layers.CameraTrigger |
           (int)PlayerLayerHelper.Layers.Homeable 
         );

    }

    void Update()
    {
        oldLeft = leftCollide;
        leftCollide = Physics.Raycast(lFoot.position, -lFoot.right, out footLeft, detectionDistance, detectionLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(lFoot.position, -lFoot.right * detectionDistance, Color.red);

        oldRight = rightCollide;
        rightCollide = Physics.Raycast(rFoot.position, -rFoot.right, out footRight, detectionDistance, detectionLayer, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(rFoot.position, -rFoot.right * detectionDistance, Color.red);

        if (oldLeft != leftCollide)
        {
            if(leftCollide == true)
            {
                PlayFootStep(footLeft);
            }
        }

        if (oldRight != rightCollide)
        {
            if (rightCollide == true)
            {
                PlayFootStep(footRight);
            }
        }
    }

    void PlayFootStep(RaycastHit info)
    {
        bool q = false;

        if(info.collider.gameObject.layer != 4) // water
        {
            if (info.collider.gameObject.CompareTag("Grass"))
            {
                footStepSource.PlayOneShot(grassStep);
                q = true;
            }

            if (info.collider.gameObject.CompareTag("Sand"))
            {
                footStepSource.PlayOneShot(sandStep);
                q = true;
            }

            if (info.collider.gameObject.CompareTag("Metal"))
            {
                footStepSource.PlayOneShot(metalStep);
                q = true;
            }

            if (q == false)
            {
                footStepSource.PlayOneShot(defaultStep);
            }
        }
        else
        {
            footStepSource.PlayOneShot(waterStep);
        }


    }
}
