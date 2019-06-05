using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMover : MonoBehaviour
{
    public Vector3 move;
    public Vector3 rot;
    public Vector3 moveAmpl;
    public Vector3 rotAmpl;


    void FixedUpdate()
    {
        transform.Translate(new Vector3(moveAmpl.x * Mathf.Sin(move.x * Time.time), moveAmpl.y * Mathf.Sin(move.y * Time.time), moveAmpl.z * Mathf.Sin(move.z * Time.time)), Space.Self);
        transform.Rotate(new Vector3(rotAmpl.x * Mathf.Sin(rot.x * Time.time), rotAmpl.y * Mathf.Sin(rot.y * Time.time), rotAmpl.z * Mathf.Sin(rot.z * Time.time)));
    }
}
