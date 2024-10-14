using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTimeHelp : MonoBehaviour, IGameHelp
{
    private int remainingHints = 3;

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
