using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    Vector3 targetScale = Vector3.one;
    public Transform target;
    public void big()
    {
        targetScale = new Vector3(1.1F, 1.1f, 1.1f);
    }


    public void normal()
    {
        targetScale = Vector3.one;
    }

    public void Update()
    {
        target.localScale = Vector3.Lerp(target.localScale, targetScale, 0.1f);
    }
}
