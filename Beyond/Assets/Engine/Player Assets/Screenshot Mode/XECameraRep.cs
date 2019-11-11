using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XECameraRep : MonoBehaviour
{
    public PlayerCore playerCore;

    public float rotateSpeed;

    public float moveSpeed;

    public float moveScalar;

    public float maxMoveScalar;

    Vector3 lerped;

    Vector3 lerped2;

    public float InitialTouch;

    public Camera cam;

    public float shiftMultiplier;

    public float currentShiftMultiplier;

    float speed;

    Ray ray;
    RaycastHit hit;

    public float mouseScroll;
    public float scrollSensitivity = 1f;
    void Update()
    {
        //Cursor.lockState = CursorLockMode.Confined;

        mouseScroll = Mathf.Lerp(mouseScroll, Input.mouseScrollDelta.y, 0.1f);

        cam.fieldOfView -= mouseScroll * scrollSensitivity;

        if (playerCore.inputCore.LShift){
            currentShiftMultiplier = shiftMultiplier;
        }else{
            currentShiftMultiplier = 1f;
        }

        if(playerCore.inputCore.rightClick)
        {
            moveScalar = Mathf.Lerp(moveScalar, maxMoveScalar, 0.075f);
            lerped = Vector3.Lerp(lerped, new Vector3(playerCore.inputCore.directionalInput.x, 0f, playerCore.inputCore.directionalInput.y), 0.075f);
            lerped2 = Vector3.Lerp(lerped2, new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f), 0.5f);
            transform.Translate(lerped * moveSpeed * currentShiftMultiplier, Space.Self);
            transform.Rotate(lerped2 * rotateSpeed);
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f));
        }
        else
        {
            moveScalar = 0.5f;
        }

        if (playerCore.inputCore.middleClickDown)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                StartCoroutine(gotoObject(hit.transform));
            }
        }


    }

    IEnumerator gotoObject(Transform target){
        while(Vector3.Distance(transform.position, target.position) > hit.collider.bounds.size.magnitude * 0.75f){
            speed = 0.001f;
            Debug.Log("Going");
            speed += 0.1f;
            transform.position = Vector3.Lerp(transform.position, target.position, speed);
            
            yield return null;
        }

        yield return null;
    }
}
