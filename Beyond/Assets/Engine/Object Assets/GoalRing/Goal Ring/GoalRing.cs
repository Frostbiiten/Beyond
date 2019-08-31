using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LayerHelper;
using TMPro;
using UnityEngine.UI;
public class GoalRing : MonoBehaviour
{
    public float CThresh;
    public float BThresh;
    public float AThresh;
    public float SThresh;
    public SkinnedMeshRenderer sonicSkin;
    public SkinnedMeshRenderer goalSkin;
    public Animator goal;
    bool ran;
    public Transform playerLock;

    public GameObject Canvas;

    public float totalScore;
    public float ringScore;
    public float timeScore;

    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI ringsScoreText;
    public TextMeshProUGUI timeScoreText;

    public Image rankSpot;

    public Sprite S;
    public Sprite A;
    public Sprite B;
    public Sprite C;
    public Sprite D;

    public float goalTime = 150f;

    public Animator RankAnimator;

    public Collider cameraTrigger;

    [System.Serializable]
    public enum Rank{
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        S = 4
    }

    public Rank rank;

    private void Start()
    {
        cameraTrigger.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(ran == false)
            {
                goal.Play("Check");
                Goal(other.GetComponent<PlayerCore>());
            }

        }
    }

    IEnumerator Small(PlayerCore pc)
    {
        while(Vector3.Distance(transform.localScale, Vector3.zero) > 0f)
        {
            yield return null;
            pc.transform.position = playerLock.position;
            goalSkin.transform.parent.rotation = playerLock.rotation;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.05f);

        }
    }

    void Goal(PlayerCore pc)
    {
        cameraTrigger.enabled = true;
        for (int i = 0; i < pc.UIManager.objectsToRemovePause.Count; i++)
        {
            pc.UIManager.objectsToRemovePause[i].SetActive(false);
        }

        ringScore = pc.playerHpManager.hp * 100f;
        timeScore = (int)Mathf.Clamp(goalTime - pc.UIManager.time, 0f, 75000f) * 15f;
        totalScore = pc.score + ringScore + timeScore;

        Canvas.SetActive(true);

        totalScoreText.text = totalScore.ToString();
        ringsScoreText.text = ringScore.ToString();
        timeScoreText.text = timeScore.ToString();

        ran = true;
        StartCoroutine(Small(pc));
        pc.rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        sonicSkin.enabled = false;
        goalSkin.enabled = true;

        rank = Rank.D;

        if(totalScore > CThresh)
        {
            rank = Rank.C;
            
        }

        if (totalScore > BThresh)
        {
            rank = Rank.B;

        }

        if (totalScore > AThresh)
        {
            rank = Rank.A;

        }

        if (totalScore > SThresh)
        {
            rank = Rank.S;

        }

        if(rank == Rank.D)
        {
            rankSpot.sprite = D;
            goal.SetBool("D", true);
        }

        if (rank == Rank.C)
        {
            rankSpot.sprite = C;
            goal.SetBool("C", true);
        }

        if (rank == Rank.B)
        {
            rankSpot.sprite = B;
            goal.SetBool("B", true);
        }

        if (rank == Rank.A)
        {
            rankSpot.sprite = A;
            goal.SetBool("A", true);
        }

        if (rank == Rank.S)
        {
            rankSpot.sprite = S;
            goal.SetBool("S", true);
        }

        StartCoroutine(set(pc));


    }

    IEnumerator set(PlayerCore pc)
    {
        yield return null;
        RankAnimator.SetBool("Done", true);
        yield return new WaitForSeconds(10f);
        pc.fadeAnimator.Play("FadeOut");
        yield return new WaitForSeconds(1.75f);
        pc.LoadMenu();
    }
}
