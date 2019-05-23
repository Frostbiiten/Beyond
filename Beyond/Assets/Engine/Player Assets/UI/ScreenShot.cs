using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    public int supersize = 1;
    public TMP_InputField supersizeinput;

    void Update()
    {
        if (Input.GetButtonDown("Screenshot"))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        int.TryParse(supersizeinput.text, out supersize);
        Debug.Log(supersize);
        StartCoroutine(CaptureScreen());
    }


    public IEnumerator CaptureScreen()
    {
        // Wait till the last possible moment before screen rendering to hide the UI
        yield return null;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

        // Wait for screen rendering to complete
        yield return new WaitForEndOfFrame();

        // Take screenshot
        //ScreenCapture.CaptureScreenshot(ScreenShotName());
        Capture();

        yield return null;

        // Show UI after we're done
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
    }

    public void Capture()
    {
        if (!Directory.Exists(Application.dataPath + "/Captures"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Captures");
        }
        string date = System.DateTime.Now.ToString();
        date = date.Replace("/", "-");
        date = date.Replace(" ", "_");
        date = date.Replace(":", "-");

        Debug.Log(Application.dataPath + "/Captures/" + date + ".png" + supersize);
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Captures/" + date + ".png", supersize);
    }
}
