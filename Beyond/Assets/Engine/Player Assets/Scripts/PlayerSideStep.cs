using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideStep : MonoBehaviour
{
    public PlayerCore playerCore;
    public float sideStepSpeed;
    public float sideStepTime;
    public bool sideStepping;
    void Update()
    {
        if (Input.GetButtonDown("SideStep") && playerCore.groundedPhysics.enabled && sideStepping == false)
        {
            StartCoroutine(SideStep(playerCore.playerForward.right * Input.GetAxis("SideStep"), sideStepTime));
        }

        if(sideStepping == true)
        {
            playerCore.playerAnimationManager.playerSkin.forward = playerCore.playerForward.forward;
        }
    }

    IEnumerator SideStep(Vector3 sideStepDirection, float time)
    {
        sideStepping = true;
        float tLeft = time;
        Vector3 tmpv = playerCore.rb.velocity;
        playerCore.playerAnimationManager.playerAnimator.Play("SideStep");
        playerCore.playerAnimationManager.playerAnimator.SetFloat("SideStepDir", Input.GetAxis("SideStep"));
        playerCore.playerAnimationManager.playerAnimator.SetFloat("SideStepVel", playerCore.velocityMagnitude);

        while (tLeft > 0f) {
            tLeft -= Time.deltaTime;
            //playerCore.rb.velocity = sideStepDirection * sideStepSpeed;
            playerCore.rb.velocity = tmpv + sideStepDirection * sideStepSpeed * Time.deltaTime * 60f;
            yield return null;
        }
        playerCore.rb.velocity = tmpv;
        yield return null;
        yield return new WaitForSeconds(0.1f);
        sideStepping = false;
    }
}
