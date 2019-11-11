using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class InputCore : MonoBehaviour
{
    //Here, we calculate input stuff
    //I decided to do this in one class because doing it in multiple would be less "centralized", and changes would break individual classes

    #region Input Vectors
    [ReadOnly]
    public Vector2 directionalInput;

    #endregion

    #region Input bools
    [ReadOnly]
    [BoxGroup("Input")]
    public bool JumpKeyUp;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool JumpKeyDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool JumpKey;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool inputLock;
    bool JumpKeyFix;
    bool JKey;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool FixedUpdateKeyDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool LShift;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool LShiftDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool LCtrl;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool LCtrlUp;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool LCtrlDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool rightClick;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool leftClick;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool middleClick;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool rightClickDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool leftClickDown;

    [ReadOnly]
    [BoxGroup("Input")]
    public bool middleClickDown;


    #endregion

    [BoxGroup("Experimental")]
    public bool player2;

    void Update()
    {
        #region main conditions
        if (inputLock == false)
        {
            if(player2 == false)
            {
                JumpKey = Input.GetButton("Jump");
                JumpKeyDown = Input.GetButtonDown("Jump");
                JumpKeyUp = Input.GetButtonUp("Jump");

                directionalInput = new Vector2(Input.GetAxisRaw("X"), Input.GetAxisRaw("Y"));

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
                JumpKey = Input.GetButton("Jump2");
                JumpKeyDown = Input.GetButtonDown("Jump2");
                JumpKeyUp = Input.GetButtonUp("Jump2");

                directionalInput = new Vector2(Input.GetAxisRaw("X2"), Input.GetAxisRaw("Y2"));

                LShift = Input.GetButton("Boost2");
                LShiftDown = Input.GetButtonDown("Boost2");
                LCtrl = Input.GetButton("Slide2");
                LCtrlUp = Input.GetButtonUp("Slide2");
                LCtrlDown = Input.GetButtonDown("Slide2");

                leftClick = Input.GetMouseButton(0);
                rightClick = Input.GetMouseButton(1);
                middleClick = Input.GetMouseButton(2);

                leftClickDown = Input.GetMouseButtonDown(0);
                rightClickDown = Input.GetMouseButtonDown(1);
                middleClickDown = Input.GetMouseButtonDown(2);
            }

        }
        else
        {
            if(player2 == false)
            {
                JumpKeyDown = false;
                JumpKey = false;
                directionalInput = Vector2.zero;
            }

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
