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
    public bool JumpKeyUp;
    public bool JumpKeyDown;
    public bool JumpKey;
    public bool inputLock;
    bool JumpKeyFix;
    bool JKey;
    public bool FixedUpdateKeyDown;

    public bool LShift;
    public bool LShiftDown;
    public bool LCtrl;
    public bool LCtrlUp;
    public bool LCtrlDown;

    public bool rightClick;
    public bool leftClick;
    public bool middleClick;

    public bool rightClickDown;
    public bool leftClickDown;
    public bool middleClickDown;

    
    #endregion

    void Update()
    {
        #region main conditions
        if (inputLock == false)
        {
            JumpKey = Input.GetButton("Jump");
            JumpKeyDown = Input.GetButtonDown("Jump");
            JumpKeyUp = Input.GetButtonUp("Jump");

            directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            LShift = Input.GetButton("Boost");
            LShiftDown = Input.GetButtonDown("Boost");
            LCtrl = Input.GetButton("Slide");
            LCtrlUp = Input.GetButtonUp("Slide");
            LCtrlDown = Input.GetButtonDown("Slide");

            leftClick = Input.GetMouseButton(0);
            rightClick = Input.GetMouseButton(1);
            middleClick = Input.GetMouseButton(2);

            leftClickDown = Input.GetMouseButtonDown(0);
            rightClickDown = Input.GetMouseButtonDown(1);
            middleClickDown = Input.GetMouseButtonDown(2);
        }
        else
        {
            JumpKeyDown = false;
            JumpKey = false;
            directionalInput = Vector2.zero;
        }
        #endregion
    }

    void FixedUpdate()
    {
        JumpKeyFix = JumpKey;
        JumpKey = Input.GetButton("Jump");
        FixedUpdateKeyDown = false;
        if(JumpKey != JumpKeyFix)
        {
            if (JumpKey == true)
            {
                FixedUpdateKeyDown = true;
            }

        }
    }

    #region Input Lock
    public IEnumerator InputLock(float time)
    {
        inputLock = true;
        yield return new WaitForSeconds(time);
        inputLock = false;
    }

    public void InputLockForFrame()
    {
        JumpKeyDown = false;
        JumpKey = false;
        directionalInput = Vector2.zero;
    }
    #endregion
}
