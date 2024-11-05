using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonScript : BaseButton
{
    [SerializeField] private Image[] starsArray;
    [SerializeField] private GameObject btnLockObj, btnUnlockObj, startBegin;
    [SerializeField] private Text levelIndexText;

    private int levelIndex;

    public void SetLevelButton(LevelObjectData value, int indexLevel)
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

                if(value.startArchived == 0)
                {
                    HideStarArchived();
                    startBegin.SetActive(true);
                }
                else
                {
                    HideStarArchived();
                    startBegin.SetActive(false);
                    starsArray[value.startArchived - 1].gameObject.SetActive(true);
                }
            }
        }
        else
        {
            base.button.interactable = false;
            btnLockObj.SetActive(true);
            btnUnlockObj.SetActive(false);
            startBegin.SetActive(true);
        }
    }
    private void HideStarArchived()
    {
        for (int i = 0; i < starsArray.Length; i++)             //loop through entire star array
        {
            if (starsArray[i] != null)
            {
                starsArray[i].gameObject.SetActive(false);
            }
        }
    }
    protected override void OnClick()
    {
        // set value for curr level && show gameplay panel
        LevelSystemManager.Ins.CurrentLevel = levelIndex - 1;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex - 1);

        if (GUIManager.Ins)
            GUIManager.Ins.sceneTransition.ChangeScene("GamePlay");
        //SceneManager.LoadScene("GamePlay");
    }
}
