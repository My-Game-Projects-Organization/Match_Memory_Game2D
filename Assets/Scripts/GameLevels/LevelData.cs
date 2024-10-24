using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelData
{
    public int lastUnlockedLevel = 0;
    public List<LevelScriptableData> levelScriptableDatas;
}
