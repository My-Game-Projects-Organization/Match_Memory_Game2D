using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystemManager : Singleton<LevelSystemManager>
{
    private LevelData levelData;
    private int currentLevel;

    public LevelData LevelData { get => levelData; set => levelData = value; }
    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }

    public override void Awake()
    {
        MakeSingleton(true);
    }

    private new void Start()
    {
        List<LevelScriptableData> listLevel = LevelData.levelScriptableDatas;
        if(listLevel == null || listLevel.Count == 0 )
        {
            // call load level method from source of saveloaddata script
            SaveLoadData.Ins.GetAllLevelDataFromResources();
            Debug.Log("Load level error");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveLoadData.Ins.SaveData();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveLoadData.Ins.ClearData();
        }
    }

    private void OnEnable()
    {
        // call check state game method
        SaveLoadData.Ins.Initialize();
    }
    public void InitLevelData()
    {
        levelData = new LevelData();
        levelData.lastUnlockedLevel = 0;
        levelData.levelScriptableDatas = new List<LevelScriptableData>();
    }

    public void LevelComplete(int curLevel)
    {
        if(LevelData.lastUnlockedLevel < (curLevel + 1))
        {
            LevelData.lastUnlockedLevel = curLevel + 1;
            if(LevelData.lastUnlockedLevel > LevelData.levelScriptableDatas.Count - 1)
            {
                LevelData.lastUnlockedLevel = 0;
            }
            LevelData.levelScriptableDatas[LevelData.lastUnlockedLevel].unlocked = true;
        }
        // Call method save data level 
        SaveLoadData.Ins.SaveData();
    }
}
