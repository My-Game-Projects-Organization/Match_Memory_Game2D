using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class LevelEditorWindow : EditorWindow
{
    private int curLevel = 0;
    private int selectedIndex;
    private string[] NumberOfPairs = { "2", "6", "10" };

    private int timeLimit;
    private bool unlocked;
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

        curLevel = GetCountLevelFromResource();
        GUILayout.Label($"Current Level: {curLevel}");

        GUILayout.Space(15);
        Rect rect = new Rect(5, 5, 30, 20);
        // Tạo Dropdown với EditorGUI.Popup
        selectedIndex = EditorGUILayout.Popup("Type Of NOPairs", selectedIndex, NumberOfPairs);
        GUILayout.Space(5);  
        GUILayout.Label($"Number Of Pairs: {NumberOfPairs[selectedIndex]}");
        GUILayout.Space(5);
        timeLimit = EditorGUILayout.IntField("Time Limit", timeLimit);
        unlocked = EditorGUILayout.Toggle("Unlocked", false);
        Debug.Log("noPairs " + selectedIndex);
        Debug.Log("timelimit " + timeLimit);
        Debug.Log("unlocked " + unlocked);
        if (GUILayout.Button("Create Level Data"))
        {
            LevelScriptableData newLevel = ScriptableObject.CreateInstance<LevelScriptableData>();
            newLevel.nOPairs = Convert.ToInt32(NumberOfPairs[selectedIndex].ToString());
            newLevel.timeLimit = Convert.ToInt32(timeLimit.ToString());
            newLevel.unlocked = unlocked;

            AssetDatabase.CreateAsset(newLevel, "Assets/Resources/Level_" + curLevel + ".asset");
            curLevel++;
        }
    }

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

    private void CreateLevel()
    {
        
    }
}
