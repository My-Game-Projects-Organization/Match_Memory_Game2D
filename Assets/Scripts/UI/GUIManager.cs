using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    public GameObject mainMenu;
    public GameObject gameplay;
    public Image timeBar;
    public PauseDialog pauseDialog;
    public TimeoutDialog timeoutDialog;
    public GameoverDialog gameoverDialog;
    public Button btnStart;
    public Button btnMenu;

    public override void Awake()
    {
        MakeSingleton(false);
    }
    public override void Start()
    {
        btnStart.onClick.RemoveAllListeners();
        btnStart.onClick.AddListener(() =>
        {
            if (AudioController.Ins)
            {
                AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
            }
        });

        btnMenu.onClick.RemoveAllListeners();
        btnMenu.onClick.AddListener(() =>
        {
            if (AudioController.Ins)
            {
                AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
            }
        });
    }

    public void ShowGameplay(bool isShow)
    {
        if (gameplay) 
            gameplay.SetActive(isShow);

        if(mainMenu)
            mainMenu.SetActive(!isShow);
    }

    public void UpdateTimeBar(float curTime,float totalTime)
    {
        float rate = curTime / totalTime;
        if (timeBar)
            timeBar.fillAmount = rate;
    }
}
