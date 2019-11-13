using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class UIManager : MonoBehaviour
{
    [Required("PlayerCore is Required")]
    public PlayerCore playerCore;

    [BoxGroup("Main")]
    public TextMeshProUGUI timeText;

    [BoxGroup("Main")]
    public TextMeshProUGUI ringsText;

    [BoxGroup("Main")]
    public TextMeshProUGUI livesText;

    [BoxGroup("Main")]
    public TextMeshProUGUI fpsCounter;

    [BoxGroup("Main")]
    public TextMeshProUGUI scoreText;

    [BoxGroup("Main")]
    public Image speedBar;

    [BoxGroup("Main")]
    public float speedBarTopSpeedReference;

    [BoxGroup("Main")]
    public float time;

    [BoxGroup("Main")]
    public bool paused;
    bool oldPause;

    [ProgressBar("Speed", 100, ProgressBarColor.Blue)]
    public float speedRef;

    [ReorderableList]
    [BoxGroup("Pause")]
    public List<GameObject> objectsToRemovePause = new List<GameObject>();

    [ReorderableList]
    [BoxGroup("Pause")]
    public List<GameObject> objectsToRemoveUnpause = new List<GameObject>();

    [ReorderableList]
    [BoxGroup("Pause")]
    public List<GameObject> objectsToAddPause = new List<GameObject>();

    [ReorderableList]
    [BoxGroup("Pause")]
    public List<GameObject> itemsThatStopUnpause = new List<GameObject>();

    [ReorderableList]
    [BoxGroup("Pause")]
    public List<MonoBehaviour> monosToDisablePause = new List<MonoBehaviour>();

    [BoxGroup("Other")]
    public Animator redRingAnim;

    [BoxGroup("Other")]
    public Image[] redRings;

    [BoxGroup("Other")]
    public Image sonicEye;

    [BoxGroup("Other")]
    public Sprite defaultEye;

    [BoxGroup("Other")]
    public Sprite deadEye;

    [BoxGroup("Pause")]
    public Material blur;
    int o = 0;

    [BoxGroup("Other")]
    public Texture2D normalCursor;

    private void Start()
    {
        StartCoroutine(UnPause());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        UpdateRings();
        UpdateLives();
        UpdateScore();
        InvokeRepeating("UpdateFPS", 0f, 0.5f);
    }
    void Update()
    {
        
        time += Time.deltaTime;
        timeText.text = FormatTime(time);
        speedBar.fillAmount = playerCore.velocityMagnitude / speedBarTopSpeedReference;
        oldPause = paused;
        speedRef = playerCore.velocityMagnitude;
        if (Input.GetKeyDown("p"))
        {
            paused = !paused;
        }

        if(oldPause != paused)
        {
            if(paused == true && o == 0)
            {
                for(int b = 0; b < monosToDisablePause.Count; b++)
                {
                    if (monosToDisablePause[b])
                    monosToDisablePause[b].enabled = false;
                }
                for(int q = 0; q < objectsToRemovePause.Count; q++)
                {
                    if(objectsToRemovePause[q])
                    objectsToRemovePause[q].SetActive(false);
                }

                for (int w = 0; w < objectsToAddPause.Count; w++)
                {
                    if(objectsToAddPause[w])
                    objectsToAddPause[w].SetActive(true);
                }
                StopCoroutine(Pause());
                StartCoroutine(Pause());
            }

            if(paused == false)
            {
                o = 0;
                for(int p = 0; p < itemsThatStopUnpause.Count; p++)
                {
                    if(itemsThatStopUnpause[p].activeSelf == true)
                    {
                        o++;
                    }
                }

                if(o == 0)
                {
                    for (int q = 0; q < objectsToRemoveUnpause.Count; q++)
                    {
                        objectsToRemoveUnpause[q].SetActive(false);
                    }

                    for (int b = 0; b < monosToDisablePause.Count; b++)
                    {
                        if(monosToDisablePause[b])
                        monosToDisablePause[b].enabled = true;
                    }

                    for (int e = 0; e < objectsToRemovePause.Count; e++)
                    {
                        if(objectsToRemovePause[e])
                        objectsToRemovePause[e].SetActive(true);
                    }

                    for (int r = 0; r < objectsToAddPause.Count; r++)
                    {
                        if(objectsToAddPause[r])
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
        playerCore.playerAnimationManager.playerAnimator.enabled = false;
        Time.timeScale = 0f;
        StartCoroutine(Blur());

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return null;
    }

    IEnumerator Blur()
    {
        blur.SetFloat("_Smoothness", 1f);
        while (blur.GetFloat("_Smoothness") > 0.25f)
        {

            

            blur.SetFloat("_Smoothness", Mathf.Lerp(blur.GetFloat("_Smoothness"), 0f, 0.1f));
            yield return null;
        }

    }

    IEnumerator UnPause()
    {

        playerCore.playerAnimationManager.playerAnimator.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        fpsCounter.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }

    public void UpdateScore()
    {
        scoreText.text = playerCore.score.ToString();
    }

    public void UpdateRings()
    {
        ringsText.text = playerCore.playerHpManager.hp.ToString();
    }

    public void UpdateLives()
    {
        livesText.text = Mathf.Clamp(playerCore.playerHpManager.lives, 0, 999).ToString();
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
