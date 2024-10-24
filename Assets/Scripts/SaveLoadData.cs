using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveLoadData : Singleton<SaveLoadData>
{
    public override void Awake()
    {
        MakeSingleton(true);
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveData();
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
    public void Initialize()
    {
        if(Pref.isFirstTimeStartGame)
        {
            GetAllLevelDataFromResources();
            SaveData();
            Pref.isFirstTimeStartGame = false;
        }
        else
        {
            LoadData();
        }
    }
    private int ExtractLevelNumber(string name)
    {
        var match = Regex.Match(name, @"\d+");
        return match.Success ? int.Parse(match.Value) : 0;
    }
    public void GetAllLevelDataFromResources()
    {
        LevelScriptableData[] levelInResources = Resources.LoadAll<LevelScriptableData>("");
        // Sort List level
        //LevelScriptableData[] levelInResourcesAfterSort = levelInResources.OrderBy(level => ExtractLevelNumber(level.name)).ToArray();
        LevelSystemManager.Ins.InitLevelData();
        LevelSystemManager.Ins.LevelData.levelScriptableDatas = new List<LevelScriptableData>(levelInResources);

        Debug.Log("Number of ScriptableObjects In Resource: " + levelInResources.Length);
    }
    public void SaveData()
    {
        string levelDataString = JsonConvert.SerializeObject(LevelSystemManager.Ins.LevelData, Formatting.Indented);
        try
        {
            System.IO.File.WriteAllText(Application.persistentDataPath + "/LevelData.json", levelDataString);
            Debug.Log("<color=green>[Level Data] Saved.</color>");
        }
        catch (System.Exception e)
        {
            Debug.Log("Error Saving data" + e);
            throw;
        }
    }
    private void LoadData()
    {
        try
        {
            string levelDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/LevelData.json");
            Debug.Log("Log json: " + levelDataString);
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelDataString);
            if (levelData != null)
            {
                for (int i = 0; i < levelData.levelScriptableDatas.Count; i++)
                {
                    if (i > levelData.lastUnlockedLevel)
                        break;
                    else
                        levelData.levelScriptableDatas[i].unlocked = true;

                }
                LevelSystemManager.Ins.InitLevelData();
                LevelSystemManager.Ins.LevelData.levelScriptableDatas = new List<LevelScriptableData>(levelData.levelScriptableDatas);
                LevelSystemManager.Ins.LevelData.lastUnlockedLevel = levelData.lastUnlockedLevel;

                Debug.Log(LevelSystemManager.Ins.LevelData.lastUnlockedLevel + "");
            }
            Debug.Log("<color=green>[Level Data] Loaded.</color>");
            CheckStateLevelDataFromLevelSystem();
        }
        catch (MissingReferenceException e)
        {
            Debug.Log("Error Loading Data" + e);
        }catch (NullReferenceException e1)
        {
            Debug.Log("Level is null" + e1);
        }
    }
    public void ClearData()
    {
        Debug.Log("Data Cleared");
        GetAllLevelDataFromResources();
        LevelSystemManager.Ins.LevelData.lastUnlockedLevel = 0;
        for (int i = 1; i < LevelSystemManager.Ins.LevelData.levelScriptableDatas.Count; i++)
        {
            LevelSystemManager.Ins.LevelData.levelScriptableDatas[i].unlocked = false;
        }
        SaveData();
        Pref.nOHintHelp = 3;
        Pref.nOExtraHintHelp = 3;
        Pref.nOExtraTimeHelp = 3;
        Pref.isFirstTimeStartGame = true;
    }
    void CheckStateLevelDataFromLevelSystem()
    {
        List<LevelScriptableData> list = LevelSystemManager.Ins.LevelData.levelScriptableDatas;
        if (list != null && list.Count > 0)
        {
            int count = 0;
            foreach (LevelScriptableData resource in list)
            {
                // Check the type if necessary
                if (resource is LevelScriptableData)
                {
                    count++;
                    Debug.Log(count + " - " + resource.unlocked);
                }
            }
            Debug.Log("Number of ScriptableObjects In Resource: " + count);
        }
        else
        {
            Debug.Log("No ScriptableObjects found in Resources");
        }
    }
}
