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

    #region Input bools
    public bool JumpKeyDown;
    public bool JumpKey;
    public bool inputLock;
    #endregion
    // Start is called before the first frame update
    void Update()
    {
        #region main conditions
        if (inputLock == false)
        {
            JumpKey = Input.GetButton("Jump");
            JumpKeyDown = Input.GetButtonDown("Jump");
            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            JumpKeyDown = false;
            JumpKey = false;
            directionalInput = Vector2.zero;
        }
        #endregion
    }

    #region Input Lock
    public IEnumerator InputLock(float time)
    {
        inputLock = true;
        yield return new WaitForSeconds(time);
        inputLock = false;
    }

    #endregion
}
