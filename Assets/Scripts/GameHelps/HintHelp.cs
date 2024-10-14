using System;
using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("Showing a hint...");
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
