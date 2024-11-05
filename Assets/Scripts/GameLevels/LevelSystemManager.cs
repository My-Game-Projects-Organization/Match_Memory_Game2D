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
    public void InitData()
    {
        SaveLoadData.Ins.Initialize();
    }
    public void InitLevelData()
    {
        levelData = new LevelData();
        levelData.lastUnlockedLevel = 0;
        levelData.levelScriptableDatas = new List<LevelObjectData>();
    }

    public void LevelComplete(int curLevel, int star)
    {
        if(star >= LevelData.levelScriptableDatas[curLevel].startArchived)
            LevelData.levelScriptableDatas[curLevel].startArchived = star;
        if (LevelData.lastUnlockedLevel < (curLevel + 1))
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
