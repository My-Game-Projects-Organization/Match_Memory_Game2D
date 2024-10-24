using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "LevelData", menuName = "ScriptableObjects/Create LevelData", order = 1)]
public class LevelScriptableData : ScriptableObject
{
    public int nOPairs;
    public int timeLimit;
    public bool unlocked;
    /* advanced features
    public int nOColumns;
    public int itemSize;
    public string subject;
    */
}
