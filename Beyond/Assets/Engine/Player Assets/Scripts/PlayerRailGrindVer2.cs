using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINDED_AM_ME;

public class PlayerRailGrindVer2 : MonoBehaviour
{
    public PlayerCore playerCore;
    public bool grinding;
    public bool canGrind;
    public Path_Comp currentPath;
    public float currentRailCompletion;
    public float currentGrindSpeed;
    public float baseGrindSpeedFrac;
    public float yOffset;
    public float gravityForce;
    public float jumpForce;
    public ParticleSystem ps;
    public float camXRot;
    public float railLookaheadAmount;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rail") && grinding == false && canGrind == true)
        {
            EnterRailGrind(other.gameObject);
        }
    }
    
    void EnterRailGrind(GameObject other)
    {
        currentPath = other.GetComponent<Path_Comp>();

        currentRailCompletion = currentPath.GetNearestPoint(transform.position, currentPath);

        float dot = Vector3.Dot(playerCore.rb.velocity.normalized, currentPath.GetTangent(currentRailCompletion, currentPath));

        /*
        if (dot > 0f)
        {
            currentGrindSpeed = baseGrindSpeedFrac * playerCore.velocityMagnitude;
        }

        if (dot < 0f)
        {
            currentGrindSpeed = -baseGrindSpeedFrac * playerCore.velocityMagnitude;
        }
        */

        currentGrindSpeed = baseGrindSpeedFrac * playerCore.velocityMagnitude * dot;

        grinding = true;
        playerCore.playerHomingAttack.airDashed = true;
        //Debug.Log("Player is at " + currentRailCompletion + " Of the rail's normalizedtime");
        playerCore.playerAnimationManager.playerAnimator.Play("Grind");
        playerCore.playerHomingAttack.airDashed = true;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && grinding == true)
        {
            grinding = false;
            
            StartCoroutine(ExitRail());
            playerCore.rb.velocity = currentPath.GetUpTangent(currentRailCompletion, currentPath) * jumpForce;
        }
    }
    void FixedUpdate()
    {
        if (grinding == true)
        {
            //Debug.Log("Grinding");
            //playerCore.rb.isKinematic = true;
            RailGrind();
        }
    }

    public void RailGrind()
    {
        
        playerCore.orbitCam.x = camXRot;
        currentRailCompletion += currentGrindSpeed;
        //playerCore.rb.MovePosition(currentPath.GetPoint(currentRailCompletion, currentPath) * currentGrindSpeed);
        playerCore.rb.MovePosition(currentPath.GetPoint(currentRailCompletion, currentPath) + currentPath.GetUpTangent(currentRailCompletion, currentPath) * yOffset);
        transform.position = currentPath.GetPoint(currentRailCompletion, currentPath) + currentPath.GetUpTangent(currentRailCompletion, currentPath) * yOffset;
        RigidbodyConstraints locks = RigidbodyConstraints.FreezeAll;
        playerCore.rb.constraints = locks;
        if (currentGrindSpeed > 0f)
        {
            camXRot = Mathf.Lerp(camXRot, Quaternion.LookRotation(currentPath.GetTangent(currentRailCompletion + railLookaheadAmount, currentPath)).eulerAngles.y, 0.02f);
            Vector3 cross = Vector3.Cross(currentPath.GetTangent(currentRailCompletion, currentPath), currentPath.GetRightTangent(currentRailCompletion, currentPath));
            playerCore.playerAnimationManager.playerSkin.rotation = Quaternion.LookRotation(currentPath.GetTangent(currentRailCompletion, currentPath), cross);
        }
        else
        {
            camXRot = Mathf.Lerp(camXRot, Quaternion.LookRotation(-currentPath.GetTangent(currentRailCompletion - railLookaheadAmount, currentPath)).eulerAngles.y, 0.02f);
            Vector3 cross = Vector3.Cross(currentPath.GetTangent(currentRailCompletion, currentPath), currentPath.GetRightTangent(currentRailCompletion, currentPath));
            playerCore.playerAnimationManager.playerSkin.rotation = Quaternion.LookRotation(-currentPath.GetTangent(currentRailCompletion, currentPath), cross);
        }

        ps.Play();
        ps.gameObject.SetActive(true);

        //set player rotation

        //playerCore.playerAnimationManager.playerSkin.rotation = Quaternion.LookRotation(tangentReference.forward, tangentUp.up);
        currentGrindSpeed = currentGrindSpeed + currentPath.GetTangent(currentRailCompletion, currentPath).y * -gravityForce;

        if (Mathf.Clamp(currentRailCompletion, 0f, currentPath.TotalDistance) != currentRailCompletion)
        {
            grinding = false;
            currentRailCompletion = 0f;
            StartCoroutine(ExitRail());
        }

        //Debug.DrawRay(transform.position, bs.GetTangent(enterRailNormalizedTime), Color.red);
    }

    public IEnumerator ExitRail()
    {
        ps.Stop();
        ps.gameObject.SetActive(false);
        grinding = false;
        canGrind = false;
        RigidbodyConstraints locks;
        locks = RigidbodyConstraints.None;
        locks = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        playerCore.rb.constraints = locks;
        yield return new WaitForSeconds(1f);
        canGrind = true;
    }
}


