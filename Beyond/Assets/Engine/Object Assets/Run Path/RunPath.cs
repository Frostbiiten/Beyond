using BLINDED_AM_ME;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;

public class RunPath : MonoBehaviour
{
    public PlayerCore playerCore;
    public Path_Comp currentPath;
    public float currentPathCompletion;
    public bool runningOnPath;
    public bool canGoOnPath;
    public float leastRunSpeed;
    public float playerHeight = 1.5f;
    public bool raycastToGround;
    public Transform pathRef;
    string colTag = "RunPath";
    public float exitDistance = 4f;
    public float tightness = 0.075f;



    public void Start()
    {
        InvokeRepeating("SlowUpdate", 0f, 0.1f);
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if(canGoOnPath == true && other.CompareTag(colTag)){
            EnterPath(other.gameObject);
        }

    }
    */

    public void EnterPath(GameObject other)
    {
        currentPath = other.GetComponent<Path_Comp>();
        currentPathCompletion = currentPath.GetNearestPoint(transform.position, currentPath);

        //float dot = Vector3.Dot(playerCore.rb.velocity.normalized, currentPath.GetTangent(currentRailCompletion, currentPath));

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

        //currentGrindSpeed = baseGrindSpeedFrac * playerCore.velocityMagnitude * dot;

        runningOnPath = true;
        //playerCore.playerHomingAttack.airDashed = true;
        //Debug.Log("Player is at " + currentRailCompletion + " Of the rail's normalizedtime");
        //playerCore.playerAnimationManager.playerAnimator.Play("Grind");
        //playerCore.playerHomingAttack.airDashed = true;
    }

    void FixedUpdate()
    {

        if (runningOnPath == true)
        {
            //playerCore.rb.velocity = Mathf.Clamp(playerCore.velocityMagnitude, leastRunSpeed, 256f) * currentPath.GetTangent(currentPathCompletion, currentPath);

            currentPathCompletion = currentPath.GetNearestPoint(transform.position, currentPath, 1.5f); // < causing lag but the bigger the step the less lag and accuracy

            PathRun();



        }


    }

    void PathRun()
    {
        //normalize velocity and point it in path dir
        /*
        playerCore.rb.velocity = Mathf.Clamp(playerCore.velocityMagnitude, leastRunSpeed, 256f)  * currentPath.GetTangent(currentPathCompletion, currentPath);
        if(raycastToGround == true)
        {
            RaycastHit hit;
            if(Physics.Raycast(currentPath.GetPoint(currentPathCompletion, currentPath), -currentPath.GetUpTangent(currentPathCompletion, currentPath), out hit, Mathf.Infinity, (int)PlayerLayerHelper.Layers.Everything, QueryTriggerInteraction.Ignore)){
                transform.position = hit.point + hit.normal * playerHeight;
            }

        }
        else
        {
            transform.position = currentPath.GetPoint(currentPathCompletion, currentPath);
        }
        */
        if (raycastToGround == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(currentPath.GetPoint(currentPathCompletion, currentPath), -currentPath.GetUpTangent(currentPathCompletion, currentPath), out hit, Mathf.Infinity, (int)PlayerLayerHelper.Layers.Everything, QueryTriggerInteraction.Ignore))
            {
                //transform.position = hit.point + hit.normal * playerHeight;
            }

        }

        //pathRef.forward = currentPath.GetTangent(currentPathCompletion, currentPath);
        pathRef.rotation = Quaternion.LookRotation(currentPath.GetTangent(currentPathCompletion, currentPath), currentPath.GetUpTangent(currentPathCompletion, currentPath));
        Vector3 localVelocity = pathRef.InverseTransformDirection(playerCore.rb.velocity);
        localVelocity.x = 0;// lock local x
        //localVelocity.y = 0;// lock local y

        playerCore.rb.velocity = Vector3.Lerp(playerCore.rb.velocity, pathRef.TransformDirection(localVelocity), tightness);


        if(Vector3.Distance(transform.position, currentPath.GetPoint(currentPathCompletion, currentPath)) > exitDistance)
        {
            StartCoroutine(ExitPath());
        }


    }

    public IEnumerator ExitPath()
    {
        runningOnPath = false;
        canGoOnPath = false;
        yield return new WaitForSeconds(1f);
        canGoOnPath = true;
    }



    void SlowUpdate()
    {


    }

  
}
