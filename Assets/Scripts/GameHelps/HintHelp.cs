using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HintHelp : MonoBehaviour ,IGameHelp
{
    [SerializeField] private Text numberOfTxt;
    
    private int remainingHints = 3;

    private void Awake()
    {
        UpdateNumberOfHelp();
    }

    public bool CanUseHelp()
    {
        return remainingHints > 0;
    }

    private void ShowHint()
    {
        List<MatchItemUI> list_item = GameManager.Ins.m_matchItemUIs.Where(item => !item.IsOpened).ToList();

        if (list_item.Count >= 2)
        {
            MatchItemUI itemFirst = list_item[UnityEngine.Random.Range(0, list_item.Count)];
            list_item.Remove(itemFirst);

            int idItem = itemFirst.Id;
            MatchItemUI itemSecond = list_item.Where(item => item.Id == idItem).First();

            itemFirst.OpenAnimTrigger();
            itemSecond.OpenAnimTrigger();

            StartCoroutine(HideHint(itemFirst, itemSecond, 2.0f));
        }
        else
        {
            Debug.Log("No item to show!");
        }

    }
    private IEnumerator HideHint(MatchItemUI itemFirst, MatchItemUI itemSecond, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (itemFirst != null && itemSecond != null)
        {
            itemFirst.OpenAnimTrigger();
            itemSecond.OpenAnimTrigger();
        }
    }

    public void ExecuteHelp()
    {
        if (CanUseHelp())
        {
            ShowHint();
            remainingHints--;
            UpdateNumberOfHelp();
        }
        else
        {
            GameObject gridObj = GameObject.Find("MatchArea");
            if(gridObj != null)
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

    private IEnumerator HideMessageImg(GameObject messImg, GameObject gridGameObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(gridGameObj != null)
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
        if(numberOfTxt != null)
        {
            numberOfTxt.text = remainingHints.ToString();
        }
    }

    public void PlaySoundBtn()
    {
        if (AudioController.Ins)
        {
            AudioController.Ins.PlaySound(AudioController.Ins.helpBtnClick);
        }
    }
}
