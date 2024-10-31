using DG.Tweening;
using ShopSystemPackage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] float itemSpacing = .5f;
    float itemHeight;

    [Space(20f)]
    [Header("UI elements")]
    [SerializeField] Transform ShopMenu;
    [SerializeField] Transform ShopItemsContainer;
    [SerializeField] GameObject itemPrefab;

    [Space(20f)]
    [SerializeField] ItemShopDatabase itemDB;

    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button openShopBtn;
    [SerializeField] Button closeShopBtn;

    [Space(20f)]
    [Header("Scroll View")]
    [SerializeField] ScrollRect scrollRect;

    [Space(20f)]
    [Header("Purchase Fx && Error Message")]
    [SerializeField] Text noEnoughCoinsText;

    // Start is called before the first frame update
    void Start()
    {
        AddShopEvents();

        GenerateShopItemsUI();
    }

    private void GenerateShopItemsUI()
    {
        //Delete itemTemplate after calculating item's Height:
        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        ShopItemsContainer.DetachChildren();

        for(int i=0;i < itemDB.ItemCount; i++)
        {
            ItemHelp item = itemDB.GetItemShop(i);
            ItemShopUI uiItem = Instantiate(itemPrefab,ShopItemsContainer).GetComponent<ItemShopUI>();

            //Move item to its position
            uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));

            //set item Name
            uiItem.gameObject.name = "Item" + i + "-" + item.itemName;

            //add info to the UI 
            uiItem.SetItemName(item.itemName);
            uiItem.SetItemIcon(item.itemHelpIcon);
            uiItem.SetItemDescription(item.itemHelpInfo);
            uiItem.SetItemPrice(item.price);

            uiItem.OnItemPurchase(i, OnItemPurchased);

            //Resize Items Container
            ShopItemsContainer.GetComponent<RectTransform>().sizeDelta =
                Vector2.up * ((itemHeight + itemSpacing) * itemDB.ItemCount + itemSpacing);

        }
    }

    private void OnItemPurchased(int index)
    {
        ItemHelp item = itemDB.GetItemShop(index);
        ItemShopUI uiItem = GetItemUI(index);

        if (Pref.startcoins >= item.price)
        {
            Pref.startcoins -= item.price;
            UpdateCountHint(index);
            //Update Coins UI Text
            GameSharedUI.Instance.UpdateCoinsUIText();
        }
        else
        {
            //No enough coins
            //AnimateNoMoreCoinsText();
            uiItem.AnimateShakeItem();
        }
    }

    private void UpdateCountHint(int index)
    {
        switch (index) {
            case 0:
                Pref.nOHintHelp++;
                break;
            case 1:
                Pref.nOExtraHintHelp++;
                break;
            case 2:
                Pref.nOExtraTimeHelp++;
                break;
            default:
                break;
        }
    }

    ItemShopUI GetItemUI(int index)
    {
        return ShopItemsContainer.GetChild(index).GetComponent<ItemShopUI>();
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

    private void AddShopEvents()
    {
        openShopBtn.onClick.RemoveAllListeners();
        openShopBtn.onClick.AddListener(OpenShop);

        closeShopBtn.onClick.RemoveAllListeners();
        closeShopBtn.onClick.AddListener(CloseShop);
    }

    private void CloseShop()
    {
        if(shopUI != null)
        {
            Time.timeScale = 1f;
            GameSharedUI.Instance.UpdateHintCountUIText();
            shopUI.SetActive(false);
        }
    }

    private void OpenShop()
    {
        if (shopUI != null)
        {
            Time.timeScale = 0f;
            GameSharedUI.Instance.UpdateCoinsUIText();
            shopUI.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Pref.startcoins += 50;
            GameSharedUI.Instance.UpdateCoinsUIText();
        }
    }
}
