using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ShopSystemPackage
{
    public class CharacterShopUI : MonoBehaviour
    {
        [Header("Layout Settings")]
        [SerializeField] float itemSpacing = .5f;
        float itemHeight;

        [Space(20f)]
        [Header("UI elements")]
        [SerializeField] Image selectedCharacterIcon;
        [SerializeField] Transform ShopMenu;
        [SerializeField] Transform ShopItemsContainer;
        [SerializeField] GameObject itemPrefab;

        [Space(20f)]
        [SerializeField] CharacterShopDatabase CharacterDB;


        [Header("Shop Events")]
        [SerializeField] GameObject shopUI;
        [SerializeField] Button openShopBtn;
        [SerializeField] Button closeShopBtn;
        [SerializeField] Button scrollUpButton;

        [Space(20f)]
        [Header("Main Menu")]
        [SerializeField] Image mainMenuCharacterImage;
        [SerializeField] Text mainMenuCharacterName;

        [Space(20f)]
        [Header("Scroll View")]
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] GameObject topScrollFade;
        [SerializeField] GameObject bottomScrollFade;

        [Space(20f)]
        [Header("Purchase Fx && Error Message")]
        [SerializeField] ParticleSystem purchaseFx;
        [SerializeField] Transform purchaseFxPos;
        [SerializeField] Text noEnoughCoinsText;


        int newSelectedItemIndex = 0;
        int previousSelectedItemIndex = 0;

        private void Start()
        {
            purchaseFx.transform.position = purchaseFxPos.position;
            AddShopEvents();
            GenerateShopItemsUI();

            //Set selected character in the PlayerDataManager
            SetSelectedCharacter();

            //Select Ui Item
            SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

            //Update playerSkin
            ChangePlayerSkin();

            //Auto scroll to selected character in the shop
            AutoScrollShopList(GameDataManager.GetSelectedCharacterIndex());
        }

        private void AutoScrollShopList(int itemIndex)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - (itemHeight / (float)(CharacterDB.CharactersCount() - 1)));
        }

        void SetSelectedCharacter()
        {
            // Get saved index
            int index = GameDataManager.GetSelectedCharacterIndex();

            //Set selected character
            GameDataManager.SetSelectedCharacter(CharacterDB.GetCharacterShopVer3(index), index);
        }

        private void GenerateShopItemsUI()
        {
            //Loop throw save purchased items and make them as Purchased in the Database Array
            for (int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
            {
                int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
                CharacterDB.PurchasedCharacter(purchasedCharacterIndex);
            }

            //Delete itemTemplate after calculating item's Height:
            itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
            Destroy(ShopItemsContainer.GetChild(0).gameObject);
            ShopItemsContainer.DetachChildren();

            //generate Items
            for (int i = 0; i < CharacterDB.CharactersCount(); i++)
            {
                CharacterShopVer3 character = CharacterDB.GetCharacterShopVer3(i);
                CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();

                //Move item to its position
                uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));

                //set item name in hierarchy
                uiItem.gameObject.name = "Item" + i + "-" + character.name;

                // Add information to the UI (one item)
                uiItem.SetCharacterName(character.name);
                uiItem.SetCharacterImage(character.image);
                uiItem.SetCharacterPower(character.power);
                uiItem.SetCharacterSpeed(character.speed);
                uiItem.SetCharacterPrice(character.price);

                if (character.isPurchased)
                {
                    //character is Purchased
                    uiItem.SetCharacterAsPurchased();
                    uiItem.OnItemSelect(i, OnItemSelected);
                }
                else
                {
                    //Character is not Purchased yet
                    uiItem.SetCharacterPrice(character.price);
                    uiItem.OnItemPurchase(i, OnItemPurchased);
                }

                //Resize Items Container
                ShopItemsContainer.GetComponent<RectTransform>().sizeDelta =
                    Vector2.up * ((itemHeight + itemSpacing) * CharacterDB.CharactersCount() + itemSpacing);

            }
        }

        private void OnItemPurchased(int index)
        {
            CharacterShopVer3 character = CharacterDB.GetCharacterShopVer3(index);
            CharacterItemUI uiItem = GetItemUI(index);

            if (GameDataManager.CanSpendCoins(character.price))
            {
                //Proceed with the purchase operation
                GameDataManager.SpendCoins(character.price);
                //Play purchase FX
                purchaseFx.Play();

                //Update Coins UI Text
                GameSharedUI.Instance.UpdateCoinsUIText();

                CharacterDB.PurchasedCharacter(index);

                uiItem.SetCharacterAsPurchased();
                uiItem.OnItemSelect(index, OnItemSelected);

                //Add purchased item to Shop Data
                GameDataManager.AddPurchasedCharacter(index);

            }
            else
            {
                //No enough coins
                AnimateNoMoreCoinsText();
                uiItem.AnimateShakeItem();
            }
        }

        private void AnimateNoMoreCoinsText()
        {
            //COmplete animations (if it's running)
            noEnoughCoinsText.DOComplete();
            noEnoughCoinsText.transform.DOComplete();

            noEnoughCoinsText.transform.DOShakePosition(3f, new Vector3(5f, 0f, 0f), 10, 0);
            noEnoughCoinsText.DOFade(1f, 3f).From(0f).OnComplete(() => {
                noEnoughCoinsText.DOFade(0f, 1f);
            });
        }

        private void OnItemSelected(int index)
        {
            // Select item in the UI
            SelectItemUI(index);

            //Save Data
            GameDataManager.SetSelectedCharacter(CharacterDB.GetCharacterShopVer3(index), index);

            //Change PlayerSkin
            ChangePlayerSkin();
        }

        private void ChangePlayerSkin()
        {
            CharacterShopVer3 character = GameDataManager.GetSelectedCharacter();

            if (character.image != null)
            {
                mainMenuCharacterImage.sprite = character.image;
                mainMenuCharacterName.text = character.name;

                // Set selected Character Image at the top of shop menu
                selectedCharacterIcon.sprite = GameDataManager.GetSelectedCharacter().image;
            }
        }

        private void SelectItemUI(int itemIndex)
        {
            previousSelectedItemIndex = newSelectedItemIndex;
            newSelectedItemIndex = itemIndex;

            CharacterItemUI prevuiItem = GetItemUI(previousSelectedItemIndex);
            CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

            prevuiItem.DeselectItem();
            newUiItem.SelectItem();
        }

        CharacterItemUI GetItemUI(int index)
        {
            return ShopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
        }

        void AddShopEvents()
        {
            openShopBtn.onClick.RemoveAllListeners();
            openShopBtn.onClick.AddListener(OpenShop);

            closeShopBtn.onClick.RemoveAllListeners();
            closeShopBtn.onClick.AddListener(CloseShop);

            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect.onValueChanged.AddListener(OnShopListScroll);

            scrollUpButton.onClick.RemoveAllListeners();
            scrollUpButton.onClick.AddListener(OnScrollUpClicked);
        }

        private void OnScrollUpClicked()
        {
            scrollRect.DOVerticalNormalizedPos(1f, .5f).SetEase(Ease.OutBack);
        }

        void OnShopListScroll(Vector2 value)
        {
            float scrollY = value.y;

            //Top Fade
            if (scrollY < 1f)
            {
                topScrollFade.SetActive(true);
            }
            else
            {
                topScrollFade.SetActive(false);
            }

            //Bottom fade
            if (scrollY > 0f)
            {
                bottomScrollFade.SetActive(true);
            }
            else
            {
                bottomScrollFade.SetActive(false);
            }

            //Scroll Up Button
            if (scrollY < 0.7f)
            {
                scrollUpButton.gameObject.SetActive(true);
            }
            else
            {
                scrollUpButton.gameObject.SetActive(false);
            }
        }

        private void CloseShop()
        {
            shopUI.SetActive(false);
        }

        private void OpenShop()
        {
            shopUI.SetActive(true);
        }
    }

}