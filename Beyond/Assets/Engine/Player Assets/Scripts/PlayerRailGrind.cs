using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class PlayerRailGrind : MonoBehaviour
{
    public PlayerCore playerCore;
    public List<BezierPoint> currentBPoints = new List<BezierPoint>();
    public BezierSpline bs;
    public float enterRailNormalizedTime;
    public bool grinding;
    public float grindBaseSpeed;
    public float railJumpOffSpeed;
    public float grindOffset = 1.5f;
    public bool goingBackwards;
    public float speedMultiple;
    public float leastSpeedMultiple;
    public float maxSpeedMultiple;
    public float currentGrindSpeed;
    public Transform tangentReference;
    public Transform tangentUp;
    public bool canGrind = true;
    public float gravityForce = 0.1f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail") && grinding == false && canGrind == true)
        {
            EnterRailGrind(other.gameObject);
        }
    }

    public void EnterRailGrind(GameObject other)
    {
        #region Init
        bs = other.GetComponent<BezierSpline>();
        #endregion

        bs.FindNearestPointTo(transform.position, out enterRailNormalizedTime);
        //Debug.Log(Vector3.Dot(playerRb.velocity.normalized, bs.GetTangent(enterRailNormalizedTime).normalized));
        float dot = Vector3.Dot(playerCore.rb.velocity.normalized, bs.GetTangent(enterRailNormalizedTime).normalized);
        Debug.Log(dot);
        if (dot > 0f)
        {
            goingBackwards = false;
        }

        if (dot < 0f)
        {
            goingBackwards = true;
        }
        playerCore.playerHomingAttack.airDashed = true;
        Debug.Log("Player is at " + enterRailNormalizedTime + " Of the rail's normalizedtime");
        playerCore.playerAnimationManager.playerAnimator.Play("Grind");
        grinding = true;
        currentGrindSpeed = grindBaseSpeed * Mathf.Clamp(playerCore.rb.velocity.magnitude, leastSpeedMultiple, maxSpeedMultiple);
    }

    void FixedUpdate()
    {
        if (grinding == true)
        {
            //playerCore.rb.isKinematic = true;
            RailGrind();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if(grinding == true)
            {
                playerCore.playerHomingAttack.airDashed = true;
                grinding = false;
                playerCore.rb.isKinematic = false;
                playerCore.rb.AddForce(transform.up * railJumpOffSpeed, ForceMode.Impulse);
                transform.position += tangentReference.up * 1f;
            }
        }
    }

    IEnumerator JumpOffRail()
    {
        yield return null;
        playerCore.rb.isKinematic = false;
        playerCore.rb.AddForce(transform.up * railJumpOffSpeed);
        transform.position += tangentReference.up * 1f;
        canGrind = false;
        yield return new WaitForSeconds(0.5f);
        canGrind = true;
    }

    public void RailGrind()
    {
        if (goingBackwards == false)
        {
            tangentReference.forward = bs.GetTangent(enterRailNormalizedTime);
            Debug.Log(-tangentReference.forward.y * gravityForce);
            playerCore.rb.MovePosition(bs.MoveAlongSpline(ref enterRailNormalizedTime, currentGrindSpeed + playerCore.playerAnimationManager.playerSkin.forward.y * gravityForce) + tangentReference.up * grindOffset);
            //transform.position = bs.MoveAlongSpline(ref enterRailNormalizedTime, currentGrindSpeed) + tangentReference.up * grindOffset;

        }
        else
        {
            tangentReference.forward = -bs.GetTangent(enterRailNormalizedTime);
            Debug.Log(tangentReference.forward.y * gravityForce);
            playerCore.rb.MovePosition(bs.MoveAlongSpline(ref enterRailNormalizedTime, -currentGrindSpeed - playerCore.playerAnimationManager.playerSkin.forward.y * gravityForce) + tangentReference.up * grindOffset);
            //transform.position = bs.MoveAlongSpline(ref enterRailNormalizedTime, -currentGrindSpeed) + tangentReference.up * grindOffset;

        }

        if(tangentReference.up.y < 0f)
        {
            tangentReference.up = new Vector3(tangentReference.up.x, -tangentReference.up.y, tangentReference.up.z);
        }

        playerCore.playerAnimationManager.playerSkin.rotation = Quaternion.LookRotation(tangentReference.forward, tangentUp.up);


        if (Mathf.Clamp(enterRailNormalizedTime, 0f, 1f) != enterRailNormalizedTime)
        {
            grinding = false;
            enterRailNormalizedTime = 0f;
        }

        //Debug.DrawRay(transform.position, bs.GetTangent(enterRailNormalizedTime), Color.red);
    }
}
