using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void QuitToMainMenu()
    {
        Time.timeScale = 1;

        if(SpriteLoader.Ins)
            SpriteLoader.Ins.ReleaseAssets();

        DestroyPersistentObjects(); 
        SceneManager.LoadScene("MainScene");
    }

    void DestroyPersistentObjects()
    {
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("Persistent");
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
    }


}
