using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalljump : MonoBehaviour
{
    public float wallJumpFreezeTime;
    public ContactPoint[] collisionPoints;
    public PlayerCore pc;
    public float wallJumpY;
    public float jumpForce;
    public bool stickGo;

    private void Update()
    {
        if(pc.groundedPhysics.enabled == true){
            stickGo = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy"))
        {
           collisionPoints = collision.contacts;

            if (Vector3.Dot(pc.playerForward.forward, -collisionPoints[0].normal) > -0.1f && pc.airbornePhysics.enabled == true)
            {
                stickGo = true;
            }

            if (stickGo == true)
            {
                if (collisionPoints[0].normal.y > -0.05f && collisionPoints[0].normal.y < 0.05f)
                {
                    pc.playerAnimationManager.playerAnimator.Play("Walljump");
                    StartCoroutine(WallJump(wallJumpFreezeTime, collisionPoints[0]));
                }
            }
        }
 
    }

    public IEnumerator WallJump(float stayTime, ContactPoint c)
    {
        float t;
        t = stayTime;
        bool down = false;
        
        while (t > 0f && down == false)
        {
            yield return null;
            if (pc.inputCore.JumpKeyDown)
            {
                down = true;
            }
            pc.playerAnimationManager.playerSkin.forward = -c.normal;
            t -= Time.deltaTime;
            pc.rb.velocity = new Vector3(0f, 0.1f, 0f);
        }

        if(down == true)
        {
            JumpOff(c);
        }

    }

    public void JumpOff(ContactPoint c)
    {
        Vector3 jumpdir;
        jumpdir = new Vector3(c.normal.x, wallJumpY, c.normal.z);
        pc.rb.AddForce(jumpdir * jumpForce, ForceMode.Impulse);
    }

}
