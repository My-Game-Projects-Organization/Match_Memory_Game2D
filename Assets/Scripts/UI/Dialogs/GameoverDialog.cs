using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverDialog : Dialog
{
    public Text totalMoveTxt;
    public Text bestMoveTxt;
    public Image[] starArray;

    public void PlaySoundBtn()
    {
        if (AudioController.Ins)
        {
            AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
        }
    }

    public void Show(bool isShow, int starArchived)
    {
        base.Show(isShow);

        if(totalMoveTxt && GameManager.Ins)
            totalMoveTxt.text = GameManager.Ins.TotalMoving.ToString();

        if(bestMoveTxt)
            bestMoveTxt.text = Pref.bestMove.ToString();
        if(starArray != null && starArray.Length > 0)
        {
            HideStarArchived();
            starArray[starArchived - 1].gameObject.SetActive(true);
        }
    }
    private void HideStarArchived()
    {
        for (int i = 0; i < starArray.Length; i++)             //loop through entire star array
        {
            if (starArray[i] != null)
            {
                starArray[i].gameObject.SetActive(false);
            }
        }
    }
        public void Continue()
    {
        SceneManager.sceneLoaded += OnSceneLoadedEvent;
        if (SceneController.Ins)
            SceneController.Ins.LoadCurrentScene();
    }
    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {

        //if (GameManager.Ins)
        //    GameManager.Ins.PlayGame();

        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }
}
