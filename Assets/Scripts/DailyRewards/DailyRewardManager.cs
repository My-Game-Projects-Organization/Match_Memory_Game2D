using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardType
{
    Coins,
    SpecialGift
}
[Serializable] public struct Reward
{
    public RewardType type; 
    public int Amount;
}
public class DailyRewardManager : MonoBehaviour
{
    [Space]
    [Header("Rewards UI")]
    [SerializeField] GameObject rewardsCanvas;
    [SerializeField] Button openBtn;
    [SerializeField] Button closeBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Image rewardImage;
    [SerializeField] Text rewardAmountText;
    [SerializeField] Text rewardAmountSpecialGiftText;
    [SerializeField] Button claimBtn;
    [SerializeField] GameObject rewardsNotification;
    [SerializeField] GameObject noMoreRewardsPanel;
    [SerializeField] GameObject specialRewardsPanel;

    [Space]
    [Header("Rewards Images")]
    [SerializeField] Sprite iconCoinsSprite;
    [SerializeField] Sprite iconSpecialGiftSprite;

    [Space]
    [Header("FX")]
    [SerializeField] private ParticleSystem fxCoins;

    [Space]
    [Header("Rewards Database")]
    [SerializeField] private DailyRewardDBScriptableObject dailyRewardDBScriptable;

    [Space]
    [Header("Timing")]
    [SerializeField] double nextRewardDelay = 10f;
    [SerializeField] float checkForRewardDelay = 5f;

    private int nextRewardIndex;
    private bool isRewardDelay = false;
    private DailyRewardsDatabase rewardsDatabase;

    private void Awake()
    {
        InitDataIfFirstTimeStartGame();
    }

    private void InitDataIfFirstTimeStartGame()
    {
        if (!Pref.isFirstTimeStartGame)
            return;

    }

    void Start()
    {
        Initialize();
        CheckForReward();
    }
    private void CheckForReward()
    {
        DateTime lastRewardDate = DateTime.Parse(Pref.lastClaimDate);

        if(string.IsNullOrEmpty(Pref.lastClaimDate))
        {
            ActiveReward();
            Pref.lastClaimDate = DateTime.UtcNow.ToString();
            return;
        }    

        if (IsNewDay(lastRewardDate))
        {
            ActiveReward();
        }
        else
        {
            DesactiveReward();
        }
    }
    public static bool IsNewDay(DateTime lastRewardDate)
    {
        DateTime currentDate = DateTime.UtcNow.Date;

        return lastRewardDate.Date < currentDate;
    }
    private void Initialize()
    {
        nextRewardIndex = Pref.rewardIndex;

        //Add click events
        openBtn.onClick.RemoveAllListeners();
        openBtn.onClick.AddListener(() => OnOpenBtnClick());

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() => OnCloseBtnClick());

        claimBtn.onClick.RemoveAllListeners();
        claimBtn.onClick.AddListener(() => OnClaimButtonClick());

        backBtn.onClick.RemoveAllListeners();
        backBtn.onClick.AddListener(() => AactiveSpecialRewardPanel(false,0));
    }
    private void ActiveReward()
    {
        isRewardDelay = true;

        noMoreRewardsPanel.SetActive(false);
        rewardsNotification.SetActive(true);

        //Update Reward UI
        Reward reward = rewardsDatabase.GetReward(nextRewardIndex);
        if (reward.type == RewardType.SpecialGift)
        {
            rewardImage.sprite = iconSpecialGiftSprite;
        }
        else
        {
            rewardImage.sprite = iconCoinsSprite;
        }
        rewardAmountText.text = string.Format("+{0}", reward.Amount);
    }
    private void AactiveSpecialRewardPanel(bool show, int amount)
    {
        rewardAmountSpecialGiftText.text = string.Format("+{0}", amount);
        specialRewardsPanel.SetActive(show);
    }
    private void DesactiveReward()
    {
        isRewardDelay = false;

        noMoreRewardsPanel.SetActive(true);
        rewardsNotification.SetActive(false);
    }
    private void OnClaimButtonClick()
    {
        Reward reward = rewardsDatabase.GetReward(nextRewardIndex);

        //check reward type
        switch (reward.type)
        {
            case RewardType.Coins:
                Debug.Log("<color=white>" + reward.type.ToString() + " Claimed: </color>+" + reward.Amount);
                UpdateStartCoins(reward.Amount);
                //TODO : FX
                fxCoins.Play();
                break;
            case RewardType.SpecialGift:
                Debug.Log("<color=white>" + reward.type.ToString() + " Claimed: </color>+" + reward.Amount + " support items.");
                //TODO : Show detail gift
                AactiveSpecialRewardPanel(true, reward.Amount);

                UpdateSupportItem(reward.Amount);
                break;
        }
        isRewardDelay = false;

        // Save next reward index
        nextRewardIndex++;
        if (nextRewardIndex >= rewardsDatabase.rewardsCount)
            nextRewardIndex = 0;
        Pref.rewardIndex = nextRewardIndex;

        //Save DateTime of the last claim click
        Pref.lastClaimDate = DateTime.Now.ToString();

        DesactiveReward();
    }
    // Update MainMenu UI
    private void UpdateStartCoins(int amount)
    {
        Pref.startcoins += amount;
    }
    private void UpdateSupportItem(int amount)
    {
        Pref.nOExtraHintHelp += amount;
        Pref.nOHintHelp += amount;
        Pref.nOExtraTimeHelp += amount;
    }
    //Open / Close UI ------------------------
    private void OnOpenBtnClick()
    {
        rewardsCanvas.SetActive(true);
    }
    private void OnCloseBtnClick()
    {
        rewardsCanvas.SetActive(false);
    }
}