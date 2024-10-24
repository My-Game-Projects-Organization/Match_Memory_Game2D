using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeoutDialog : Dialog
{
    public void QuitToMainMenu()
    {
        if (SpriteLoader.Ins)
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
    public void PlaySoundBtn()
    {
        if (AudioController.Ins)
        {
            AudioController.Ins.PlaySound(AudioController.Ins.btnClick);
        }
    }
    public void Replay()
    {
        SceneManager.sceneLoaded += OnSceneLoadedEvent;
        if (SceneController.Ins)
            SceneController.Ins.LoadCurrentScene();
    }

    private void OnSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {

        if (GameManager.Ins)
            GameManager.Ins.PlayGame();

        SceneManager.sceneLoaded -= OnSceneLoadedEvent;
    }
}
