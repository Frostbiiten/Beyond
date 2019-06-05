using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionSetter : MonoBehaviour
{
    public TMP_InputField xInput;

    public TMP_InputField yInput;

    public Toggle fullScreen;

    int x;

    int y;

    void Start()
    {
        x = Screen.width;
        y = Screen.height;
    }

    void Update()
    {
        
    }

    public void SetResolutionS()
    {
        int.TryParse(xInput.text, out x);
        int.TryParse(yInput.text, out y);
        Screen.SetResolution(x, y, fullScreen.isOn);
    }
}
