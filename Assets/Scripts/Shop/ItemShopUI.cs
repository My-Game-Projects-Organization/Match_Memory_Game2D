using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemShopUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDescription;
    [SerializeField] private Text itemPrice;
    [SerializeField] private Button buyButton;

    //------------------------
    public void SetItemPosition(Vector2 position)
    {
        GetComponent<RectTransform>().anchoredPosition += position;
    }

    public void OnItemPurchase(int itemIndex, UnityAction<int> action)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => action.Invoke(itemIndex));
    }

    public void SetItemIcon(Sprite icon)
    {
        if (itemIcon != null && icon != null)
        {
            this.itemIcon.sprite = icon;
        } 
    }
    public void SetItemName(string name)
    {
        if(itemName != null)
            itemName.text = name;
    }
    public void SetItemDescription(string description)
    {
        if(itemDescription != null) 
            itemDescription.text = description;
    }
    public void SetItemPrice(int price)
    {
        if(itemPrice != null)
            itemPrice.text = price.ToString();
    }
    public void AnimateShakeItem()
    {
        //End all animation first
        transform.DOComplete();

        transform.DOShakePosition(1f, new Vector3(9f, 0, 0), 1, 0).SetEase(Ease.Linear);
    }
}
