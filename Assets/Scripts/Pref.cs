using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pref 
{
    public static int bestMove
    {
        set
        {
            int oldMove = PlayerPrefs.GetInt(PrefKey.BestScore.ToString(), 0);
            if(oldMove > value || oldMove == 0)
                PlayerPrefs.SetInt(PrefKey.BestScore.ToString(), value);


        }
        get => PlayerPrefs.GetInt(PrefKey.BestScore.ToString(), 0);
    }

    public static bool isFirstTimeStartGame
    {
        set
        {
            if (!value)
                PlayerPrefs.SetInt(PrefKey.IsFirstTimeStartGame.ToString(), 0);
            else
                PlayerPrefs.SetInt(PrefKey.IsFirstTimeStartGame.ToString(), 1);
        }
        get
        {
            bool res = PlayerPrefs.GetInt(PrefKey.IsFirstTimeStartGame.ToString(),0) == 0 ? false : true;
            return res;
        }
    }

    public static int nOExtraHintHelp
    {
        set
        {
            PlayerPrefs.SetInt(PrefKey.NOExtraHintHelp.ToString(), value);
        }
        get => PlayerPrefs.GetInt(PrefKey.NOExtraHintHelp.ToString(), 0);
    }

    public static int nOExtraTimeHelp
    {
        set
        {
            PlayerPrefs.SetInt(PrefKey.NOExtraTimeHelp.ToString(), value);
        }
        get => PlayerPrefs.GetInt(PrefKey.NOExtraTimeHelp.ToString(), 0);
    }

    public static int nOHintHelp
    {
        set
        {
            PlayerPrefs.SetInt(PrefKey.NOHintHelp.ToString(), value);
        }
        get => PlayerPrefs.GetInt(PrefKey.NOHintHelp.ToString(), 0);
    }
}
