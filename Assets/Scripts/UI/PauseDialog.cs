using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseDialog : Dialog
{
    public override void Show(bool isShow)
    {
        base.Show(isShow);
        Time.timeScale = 0f;
    }
    public void PlaySoundBtn()
    {
        if (AudioController.Ins)
        {
            AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
        }
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        Close();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1;

        if(SceneController.Ins)
            SceneController.Ins.LoadCurrentScene();
    }
}