using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintHelp : MonoBehaviour ,IGameHelp
{
    private int remainingHints = 3;

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
}
