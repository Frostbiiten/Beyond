using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCore : MonoBehaviour
{
    //Here, we calculate input stuff
    //I decided to do this in one class because doing it in multiple would be less "centralized", and changes would break individual classes

    #region Input Vectors
    public Vector2 directionalInput;

    #endregion
    // Start is called before the first frame update
    void Update()
    {
        directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    //For future ref
    void InputLock()
    {
        //reset all input
        directionalInput = Vector2.zero;
    }
}
