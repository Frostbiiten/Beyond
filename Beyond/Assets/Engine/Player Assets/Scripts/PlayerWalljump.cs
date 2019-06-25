﻿using System.Collections;
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
    public Collider lastObjectJumpedOff;
    public ContactPoint[] points;
    private void Update()
    {
        if(pc.groundedPhysics.enabled == true){
            stickGo = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine("WallJump");
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
                    //Debug.Log(collisionPoints[0].otherCollider);

                    StartCoroutine(WallJump(wallJumpFreezeTime, collisionPoints[0]));
                }


            }
        }
 
    }

    public bool Check(Collider go, ContactPoint[] a)
    {
        for(int i = 0; i< a.Length; i++)
        {
            if(a[i].otherCollider == go)
            {
                return true;
            }
        }
        return false;
    }
    public IEnumerator WallJump(float stayTime, ContactPoint c)
    {
        float t;
        t = stayTime;
        bool down = false;
        Vector3 initialPosition;
        initialPosition = pc.playerAnimationManager.playerSkin.localPosition;

        if(Physics.Raycast(transform.position, c.point - transform.position, Vector3.Distance(c.point, transform.position) + 1f) && pc.grounded == false)
        {
            pc.playerAnimationManager.playerAnimator.Play("Walljump");
        }

        while (t > 0f && down == false && Physics.Raycast(transform.position, c.point - transform.position, Vector3.Distance(c.point, transform.position) + 1f) && pc.grounded == false)
        {

            yield return null;
            if (pc.inputCore.JumpKeyDown)
            {
                down = true;
            }
            pc.playerAnimationManager.playerSkin.forward = -c.normal;
            pc.playerAnimationManager.playerSkin.localPosition = initialPosition + c.normal * 0.1f;
            t -= Time.deltaTime;
            pc.rb.velocity = new Vector3(0f, -0.2f - t, 0f);
        }

        if(down == true)
        {
            lastObjectJumpedOff = c.otherCollider;
            JumpOff(c);
        }

        pc.playerAnimationManager.playerSkin.localPosition = initialPosition;

    }

    public void JumpOff(ContactPoint c)
    {
        Vector3 jumpdir;
        jumpdir = new Vector3(c.normal.x, wallJumpY, c.normal.z);
        pc.rb.AddForce(jumpdir * jumpForce, ForceMode.Impulse);
    }

}
