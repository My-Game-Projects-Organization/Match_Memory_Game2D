using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtraHintHelp : MonoBehaviour, IGameHelp
{
    private int remainingHints = 3;

    public bool CanUseHelp()
    {
        return remainingHints > 0;
    }

    private void ShowHint()
    {
        List<MatchItemUI> list_item = GameManager.Ins.m_matchItemUIs.Where(item => !item.IsOpened).ToList();

        if (list_item.Count >= 1)
        {
           foreach (MatchItemUI item in list_item)
            {
                if(item != null)
                {
                    item.OpenAnimTrigger();
                }
            }

            StartCoroutine(HideHint(list_item, 2.0f));
        }
        else
        {
            Debug.Log("No item to show!");
        }

    }
    private IEnumerator HideHint(List<MatchItemUI> list, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (list != null && list.Count >= 1)
        {
            foreach(MatchItemUI item in list)
            {
                if(item != null)
                {
                    item.OpenAnimTrigger();
                }
            }
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
