using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DailyRewardsDatabase
{
    public List<Reward> rewards;

    public int rewardsCount
    {
        get { return rewards.Count; }
    }

    public Reward GetReward(int index)
    {
        return rewards[index];
    }
}
