using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XECameraRep : MonoBehaviour
{
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
    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if(Input.GetKey(KeyCode.LeftShift)){
            currentShiftMultiplier = shiftMultiplier;
        }else{
            currentShiftMultiplier = 1f;
        }

        if(Input.GetMouseButton(1)){
            moveScalar = Mathf.Lerp(moveScalar, maxMoveScalar, 0.075f);
            lerped = Vector3.Lerp(lerped, new Vector3(Input.GetAxisRaw("X"), 0f, Input.GetAxisRaw("Y")), 0.075f);
            lerped2 = Vector3.Lerp(lerped2, new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f), 0.5f);
            transform.Translate(lerped * moveSpeed * currentShiftMultiplier, Space.Self);
            transform.Rotate(lerped2 * rotateSpeed);
            transform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f));
        }
        else
        {
            moveScalar = 0.5f;
        }

        if (Input.GetMouseButtonDown(2))
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
