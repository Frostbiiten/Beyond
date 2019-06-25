using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotation;
    public bool ignoreTime = false;

    void Update()
    {
        if (ignoreTime == false)
        {
            transform.Rotate(rotation * Time.deltaTime * 60f);
        }

        if (ignoreTime == true)
        {
            transform.Rotate(rotation);
        }

    }
}
