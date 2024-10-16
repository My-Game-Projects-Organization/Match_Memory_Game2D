using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtraTimeHelp : MonoBehaviour, IGameHelp
{
    [SerializeField] private Text numberOfTxt;
    [SerializeField] private GameObject bonusTimeTxt;
    
    private int remainingHints = 3;

    private void Awake()
    {
        UpdateNumberOfHelp();
    }

    public bool CanUseHelp()
    {
        return remainingHints > 0;
    }

    public void ExecuteHelp()
    {
        if (CanUseHelp())
        {
            GameManager.Ins.m_timeCounting += 5.0f;
            if (GUIManager.Ins)
                GUIManager.Ins.UpdateTimeBar((float)GameManager.Ins.m_timeCounting, (float)GameManager.Ins.timeLimit);

            remainingHints--;
            if(bonusTimeTxt != null)
            {
                bonusTimeTxt.SetActive(true);
                StartCoroutine(HideBonusTimeTxt(5.0f));
            }
            UpdateNumberOfHelp();
        }
        else
        {
            GameObject gridObj = GameObject.Find("MatchArea");
            if (gridObj != null)
            {
                gridObj.SetActive(false);
            }

            if (GameManager.Ins.messImg != null)
            {
                GameManager.Ins.messImg.SetActive(true);

                StartCoroutine(HideMessageImg(GameManager.Ins.messImg, gridObj, 1.0f));
            }
        }
    }

    private IEnumerator HideBonusTimeTxt(float v)
    {
        yield return new WaitForSeconds(v);

        if(bonusTimeTxt != null)
        {
            bonusTimeTxt.SetActive(false);
        }  
    }

    private IEnumerator HideMessageImg(GameObject messImg, GameObject gridGameObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gridGameObj != null)
        {
            gridGameObj.SetActive(true);
        }

        if (messImg != null)
        {
            messImg.SetActive(false);
        }
    }


    public void ShowHelpInfo()
    {
        if (CanUseHelp())
        {
            Debug.Log("You can use a hint. Remaining hints: " + remainingHints);
        }
        else
        {
            Debug.Log("No hints remaining.");
        }
    }

    public void UpdateNumberOfHelp()
    {
        if (numberOfTxt != null)
        {
            numberOfTxt.text = remainingHints.ToString();
        }
    }
    public void PlaySoundBtn()
    {
        if (AudioController.Ins)
        {
            AudioController.Ins.PlaySound(AudioController.Ins.bonusTime);
        }
    }
}
