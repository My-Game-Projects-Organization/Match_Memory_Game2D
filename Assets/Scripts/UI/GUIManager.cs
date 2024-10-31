using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : Singleton<GUIManager>
{
    public GameObject mainMenu;
    public GameObject levelMenu;
    public Button btnStart;
    public SceneTransition sceneTransition;

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
            if (AddressableManager.Ins)
                StartCoroutine(AddressableManager.Ins.LoadSprites("subject1"));
                
            ShowLevelMenu(true);
            if (AudioController.Ins)
            {
                AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
            }
        });

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
}
