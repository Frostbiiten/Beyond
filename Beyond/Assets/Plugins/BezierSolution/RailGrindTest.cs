using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class RailGrindTest : MonoBehaviour
{
    public List<BezierPoint> currentBPoints = new List<BezierPoint>();
    public BezierSpline bs;
    public float enterRailNormalizedTime;
    public bool grinding;
    public float grindBaseSpeed;
    public Rigidbody playerRb;
    public float railJumpOffSpeed;
    public Vector3 grindOffset;
    public bool goingBackwards;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail"))
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
        float dot = Vector3.Dot(playerRb.velocity.normalized, bs.GetTangent(enterRailNormalizedTime).normalized);
        Debug.Log(dot);
        if (dot > 0f)
        {
            goingBackwards = false;
        }

        if(dot < 0f)
        {
            goingBackwards = true;
        }
        
        Debug.Log("Player is at " + enterRailNormalizedTime + " Of the rail's normalizedtime");

        grinding = true;

    }

    void FixedUpdate()
    {
        if(grinding == true)
        {
            playerRb.isKinematic = true;
            RailGrind();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            grinding = false;
            playerRb.isKinematic = false;
            playerRb.AddForce(transform.up * railJumpOffSpeed);
        }
    }

    public void RailGrind()
    {
        if(goingBackwards == false)
        {
            transform.position = bs.MoveAlongSpline(ref enterRailNormalizedTime, grindBaseSpeed) + grindOffset;
        }
        else
        {
            transform.position = bs.MoveAlongSpline(ref enterRailNormalizedTime, -grindBaseSpeed) + grindOffset;
        }

        //Debug.DrawRay(transform.position, bs.GetTangent(enterRailNormalizedTime), Color.red);
    }

}
