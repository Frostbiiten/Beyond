using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerCore playerCore;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI ringsText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI fpsCounter;
    public Image speedBar;
    public float speedBarTopSpeedReference;
    public float time;
    public bool paused;
    bool oldPause;
    public List<GameObject> objectsToRemovePause = new List<GameObject>();
    public List<GameObject> objectsToAddPause = new List<GameObject>();
    public Texture2D normalCursor;

    public List<GameObject> itemsThatStopUnpause = new List<GameObject>();

    public Animator redRingAnim;
    public Image[] redRings;


    private void Start()
    {
        StartCoroutine(UnPause());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        UpdateRings();
        InvokeRepeating("UpdateFPS", 0f, 0.5f);
    }
    void Update()
    {
        time += Time.deltaTime;
        timeText.text = FormatTime(time);
        speedBar.fillAmount = playerCore.velocityMagnitude / speedBarTopSpeedReference;
        oldPause = paused;

        if (Input.GetKeyDown("p"))
        {
            paused = !paused;
        }

        if(oldPause != paused)
        {
            if(paused == true)
            {
                for(int q = 0; q < objectsToRemovePause.Count; q++)
                {
                    objectsToRemovePause[q].SetActive(false);
                }

                for (int w = 0; w < objectsToAddPause.Count; w++)
                {
                    objectsToAddPause[w].SetActive(true);
                }
                StopCoroutine(Pause());
                StartCoroutine(Pause());
            }

            if(paused == false)
            {
                int o = 0;
                for(int p = 0; p < itemsThatStopUnpause.Count; p++)
                {
                    if(itemsThatStopUnpause[p].activeSelf == true)
                    {
                        o++;
                    }
                }

                if(o == 0)
                {
                    for (int e = 0; e < objectsToRemovePause.Count; e++)
                    {
                        objectsToRemovePause[e].SetActive(true);
                    }

                    for (int r = 0; r < objectsToAddPause.Count; r++)
                    {
                        objectsToAddPause[r].SetActive(false);
                    }

                    StartCoroutine(UnPause());
                }
            }
        }

        for(int e = 0; e < redRings.Length; e++)
        {
            if(e < playerCore.redRings)
            {
                redRings[e].color = Color.white;
            }
            else
            {
                redRings[e].color = Color.black;
            }
        }
    }

    public void RedringAnimation()
    {
        redRingAnim.Play("RedRingIn");
    }

    IEnumerator Pause()
    {
        StopCoroutine(UnPause());
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        yield return null;
    }

    IEnumerator UnPause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.015f * Time.timeScale;
        if(paused == false)
        {
            Time.timeScale = 1f;
        }

        yield return null;
    }

    void UpdateFPS()
    {
        fpsCounter.text = (1f / Time.deltaTime).ToString("N0");
    }

    public void UpdateRings()
    {
        ringsText.text = playerCore.playerHpManager.hp.ToString();
    }

    public void UpdateLives()
    {
        livesText.text = playerCore.playerHpManager.lives.ToString();
    }


    string FormatTime(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }
}
