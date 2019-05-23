using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotation;
    public bool ignoreTime = false;
    void FixedUpdate()
    {
        if(ignoreTime == false)
        {
            transform.Rotate(rotation);
        }
        
    }

    void Update()
    {
        if (ignoreTime == true)
        {
            transform.Rotate(rotation);
        }

    }
}
