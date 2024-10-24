using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    public GameObject mainMenu;
    //public GameObject gameplay;
    public GameObject levelMenu;
    //public Image timeBar;
    //public PauseDialog pauseDialog;
    //public TimeoutDialog timeoutDialog;
    //public GameoverDialog gameoverDialog;
    public Button btnStart;
    //public Button btnMenu;

    [SerializeField] private GameObject leveBtnGridHolder;
    [SerializeField] private LevelButtonScript levelBtnPrefab;

    public override void Awake()
    {
        MakeSingleton(true);
    }
    public override void Start()
    {
        if (AudioController.Ins)
            AudioController.Ins.PlayBackgroundMusic();
        btnStart.onClick.RemoveAllListeners();
        btnStart.onClick.AddListener(() =>
        {
            ShowLevelMenu(true);
            if (AudioController.Ins)
            {
                AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
            }
        });

        //btnMenu.onClick.RemoveAllListeners();
        //btnMenu.onClick.AddListener(() =>
        //{
        //    if (AudioController.Ins)
        //    {
        //        AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
        //    }
        //});

        InitializeUILevel();
    }

    private void InitializeUILevel()
    {
        List<LevelScriptableData> levelScriptableDatas = LevelSystemManager.Ins.LevelData.levelScriptableDatas;
        for (int i = 0; i < levelScriptableDatas.Count; i++)
        {
            LevelButtonScript levelBtn = Instantiate(levelBtnPrefab, leveBtnGridHolder.transform);
            levelBtn.SetLevelButton(levelScriptableDatas[i], i);
        }
    }
    public void ShowLevelMenu(bool isShow)
    {
        if (mainMenu)
            mainMenu.SetActive(!isShow);

        if (levelMenu)
            levelMenu.SetActive(isShow);
    }
    //public void ShowGameplay(bool isShow)
    //{
    //    if (gameplay) 
    //        gameplay.SetActive(isShow);

    //    if(levelMenu)
    //        levelMenu.SetActive(!isShow);
    //}

    //public void UpdateTimeBar(float curTime,float totalTime)
    //{
    //    float rate = curTime / totalTime;
    //    if (timeBar)
    //        timeBar.fillAmount = rate;
    //}
}
