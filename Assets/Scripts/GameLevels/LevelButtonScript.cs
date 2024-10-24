using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonScript : BaseButton
{
    [SerializeField] private GameObject btnLockObj, btnUnlockObj;
    [SerializeField] private Text levelIndexText;

    private int levelIndex;

    public void SetLevelButton(LevelScriptableData value, int indexLevel)
    {
        bool activeLevel = value.unlocked;
        if (value.unlocked)
        {
            if(levelIndexText != null)
            {
                levelIndex = indexLevel + 1;
                base.button.interactable = true;
                btnUnlockObj.SetActive(true);
                btnLockObj.SetActive(false);
                levelIndexText.text = levelIndex.ToString();
            }
        }
        else
        {
            base.button.interactable = false;
            btnLockObj.SetActive(true);
            btnUnlockObj.SetActive(false);
        }
    }

    protected override void OnClick()
    {
        // set value for curr level && show gameplay panel
        LevelSystemManager.Ins.CurrentLevel = levelIndex - 1;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex - 1);

        //if (GameManager.Ins)
        //    GameManager.Ins.PlayGame();
        SceneManager.LoadScene("GamePlay");
    }
}
