using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class LevelEditorWindow : EditorWindow
{
    private int curLevel = 0;
    private int levelId = 0;
    private int selectedIndex;
    private string[] NumberOfPairs = { "2", "6", "10" };
    private int selectedModeLevel;
    private string[] NumberOfModes = { "1", "2", "3" };

    private int timeLimit;
    private int startArchived;
    private bool unlocked;

    private bool isFirstTimeShowWindow = true;
    private List<LevelObjectData> listLevel;
    /* advanced feature
    private string[] ChoiceOfHelps = {"","",""};
    private string subject;
     */

    [MenuItem("Tools/Level Editor")]
    private static void ShowWindow()
    {
        var window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Level Editor");

        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("Create a New Level", EditorStyles.boldLabel);

        GetCountLevelFromServer();
        //curLevel = GetCountLevelFromResource();

        GUILayout.Label($"Current Level: {curLevel}");
        GUILayout.Space(5);
        GUILayout.Label($"ID Level: {levelId}");

        GUILayout.Space(15);
        selectedIndex = EditorGUILayout.Popup("Type Of NOPairs", selectedIndex, NumberOfPairs);
        GUILayout.Space(5);  
        GUILayout.Label($"Number Of Pairs: {NumberOfPairs[selectedIndex]}");

        GUILayout.Space(15);
        selectedModeLevel = EditorGUILayout.Popup("Type Of Modes", selectedModeLevel, NumberOfModes);
        GUILayout.Space(5);
        GUILayout.Label($"Number Of Modes: {NumberOfModes[selectedModeLevel]}");
        GUILayout.Space(5);

        timeLimit = EditorGUILayout.IntField("Time Limit", timeLimit);
        startArchived = EditorGUILayout.IntField("Start Archived", startArchived);
        unlocked = EditorGUILayout.Toggle("Unlocked", false);
        Debug.Log("noPairs " + selectedIndex);
        Debug.Log("timelimit " + timeLimit);
        Debug.Log("unlocked " + unlocked);
        if (GUILayout.Button("Create Level Data"))
        {
            /* If use ScriptableObject
            LevelScriptableData newLevel = ScriptableObject.CreateInstance<LevelScriptableData>();
            newLevel.nOPairs = Convert.ToInt32(NumberOfPairs[selectedIndex].ToString());
            newLevel.timeLimit = Convert.ToInt32(timeLimit.ToString());
            newLevel.startArchived = Convert.ToInt32(startArchived.ToString());
            newLevel.modeLevel = Convert.ToInt32(NumberOfModes[selectedModeLevel].ToString());
            newLevel.unlocked = unlocked;

            AssetDatabase.CreateAsset(newLevel, "Assets/Resources/Level_" + curLevel + ".asset");
            */

            LevelObjectData newLevel = new LevelObjectData(levelId,
                Convert.ToInt32(NumberOfPairs[selectedIndex].ToString()),
                Convert.ToInt32(timeLimit.ToString()),
                Convert.ToInt32(startArchived.ToString()),
                Convert.ToInt32(NumberOfModes[selectedModeLevel].ToString()),
                unlocked);
            listLevel.Add(newLevel);

            curLevel++;
            levelId++;
        }

        if (GUILayout.Button("Upload Level Data to Server"))
        {
            LevelData listLevelData = new LevelData();
            listLevelData.lastUnlockedLevel = 0;
            listLevelData.levelScriptableDatas = listLevel;

            string levelDataString = JsonConvert.SerializeObject(listLevelData, Formatting.Indented);
            UploadLevelsDataToServer(levelDataString);

        }
    }
    public void UploadLevelsDataToServer(string levelJson)
    {
        if (levelJson == null || levelJson == "")
        {
            return;
        }

        PlayFabServerAPI.SetTitleData(
            new PlayFab.ServerModels.SetTitleDataRequest
            {
                Key = "Levels",
                Value = levelJson
            },
            result => Debug.Log("Upload new level data successfull!"),
            error => {
                Debug.Log("Got error setting titleData:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
    }
    private void GetCountLevelFromServer()
    {
        if (!isFirstTimeShowWindow)
            return;
        LoginPlayfabServer();
        PlayFabClientAPI.GetTitleData(new PlayFab.ClientModels.GetTitleDataRequest(),
            result => {
                if (result.Data == null || !result.Data.ContainsKey("Levels")) Debug.Log("No Levels");
                else
                {
                    string levelDataJson = result.Data["Levels"];
                    // Parse JSON thành object
                    LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelDataJson);

                    levelId = levelData.levelScriptableDatas.Count;
                    curLevel = levelData.levelScriptableDatas.Count + 1;

                    listLevel = levelData.levelScriptableDatas;

                    Debug.Log("Loaded level successfull! NoLevels: " + levelData.levelScriptableDatas.Count);
                }
            },
            error => {
                Debug.Log("Got error getting titleData:");
                Debug.Log(error.GenerateErrorReport());
            }
        );
        isFirstTimeShowWindow = false;
    }
    private void LoginPlayfabServer()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,
            result => {
                Debug.Log("Successfull login/ account create!");
            },
            error =>
            {
                Debug.Log("Error while login in/ creating account!");
                Debug.Log(error.GenerateErrorReport());
            });
    }
    private void OnDestroy()
    {
        Debug.Log("Editor window is closing!");
        isFirstTimeShowWindow = true;
    }

    // If use ScriptableObject
    private int GetCountLevelFromResource()
    {
        LevelScriptableData[] listLevel = Resources.LoadAll<LevelScriptableData>("");
        if (listLevel != null && listLevel.Length > 0)
        {
            int count = 0;
            foreach (LevelScriptableData resource in listLevel)
            {
                // Check the type if necessary
                if (resource is LevelScriptableData)
                {
                    count++;
                }
            }

            Debug.Log("Number of ScriptableObjects: " + count);
            curLevel = count;
        }
        else
        {
            curLevel = 0;
            Debug.Log("No ScriptableObjects found in Resources");
        }
        curLevel++;
        return curLevel;
    }
}
