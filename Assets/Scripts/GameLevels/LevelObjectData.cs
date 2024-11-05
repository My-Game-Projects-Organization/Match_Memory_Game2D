using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelObjectData
{
    public int levelId;
    public int nOPairs;
    public int timeLimit;
    public int startArchived;
    public int modeLevel;
    public bool unlocked;

    public LevelObjectData(int levelId, int nOPairs, int timeLimit, int startArchived, int modeLevel, bool unlocked)
    {
        this.levelId = levelId;
        this.nOPairs = nOPairs;
        this.timeLimit = timeLimit;
        this.startArchived = startArchived;
        this.modeLevel = modeLevel;
        this.unlocked = unlocked;
    }
    /* advanced features
    public int nOColumns;
    public int itemSize;
    public string subject;
    */

}
