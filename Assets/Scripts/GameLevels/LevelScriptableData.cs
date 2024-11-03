using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "LevelData", menuName = "ScriptableObjects/Create LevelData", order = 1)]
public class LevelScriptableData : ScriptableObject
{
    public int nOPairs;
    public int timeLimit;
    public int startArchived;
    public int modeLevel;
    public bool unlocked;
    /* advanced features
    public int nOColumns;
    public int itemSize;
    public string subject;
    */

    public void UpdateData(LevelObjectData newData)
    {
        nOPairs = newData.nOPairs;
        timeLimit = newData.timeLimit;
        unlocked = newData.unlocked;
    }
}
