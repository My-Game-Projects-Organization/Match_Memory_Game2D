using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : Singleton<DialogManager>
{
    public Image timeBar;
    public PauseDialog pauseDialog;
    public TimeoutDialog timeoutDialog;
    public GameoverDialog gameoverDialog;

    public override void Awake()
    {
        MakeSingleton(false);
    }

    public void UpdateTimeBar(float curTime, float totalTime)
    {
        float rate = curTime / totalTime;
        if (timeBar)
            timeBar.fillAmount = rate;
    }
}
