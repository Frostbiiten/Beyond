using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDive : MonoBehaviour
{
    public PlayerCore playerCore;
    public bool skyDiving;

    public bool diveFast;

    public float diveFastAdditive;
    public float diveSlowDrag;
    public Vector3 diveRot;
    public float yVelClamp = 32f;

    public void FixedUpdate()
    {
        if(skyDiving == true)
        {
            if(playerCore.grounded == true)
            {
                skyDiving = false;
            }

            diveFast = playerCore.inputCore.LShift;

            if (diveFast == true)
            {
                diveRot = Vector3.Lerp(diveRot, new Vector3(playerCore.inputCore.directionalInput.y * 0.75f, 0f, playerCore.inputCore.directionalInput.x) * 30f, 0.1f);
                //playerCore.playerAnimationManager.playerSkin.Rotate(diveRot);
                playerCore.rb.AddForce(Vector3.down * diveFastAdditive);
            }
            else
            {
                diveRot = Vector3.Lerp(diveRot, new Vector3(playerCore.inputCore.directionalInput.y * 0.75f, 0f, -playerCore.inputCore.directionalInput.x) * 30f, 0.1f);
                

                if(playerCore.velocity.y < -yVelClamp)
                {
                    playerCore.rb.AddForce(0f, -playerCore.velocity.y, 0f);
                }
                //playerCore.playerAnimationManager.playerSkin.Rotate(diveRot);
                playerCore.rb.AddForce(Vector3.up * diveSlowDrag);
            }
        }


    }

    public void Update()
    {
        if(playerCore != null)
        {
            playerCore.playerAnimationManager.playerAnimator.SetBool("SkyDive", skyDiving);
            playerCore.playerAnimationManager.playerAnimator.SetBool("DiveFast", diveFast);
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCore = other.GetComponent<PlayerCore>();
            skyDiving = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            skyDiving = false;
        }
    }

}
