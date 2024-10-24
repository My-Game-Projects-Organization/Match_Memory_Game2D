using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace ShopSystemPackage
{
    public class CharacterItemUI : MonoBehaviour
    {
        [SerializeField] Color itemNotSelectedColor;
        [SerializeField] Color itemSelectedColor;

        [Space(20f)]
        [SerializeField] Image characterImage;
        [SerializeField] Text characterNameText;
        [SerializeField] Image characterSpeedFill;
        [SerializeField] Image characterPowerFill;
        [SerializeField] Text characterPriceText;
        [SerializeField] Button characterPurchasedButton;

        [Space(20f)]
        [SerializeField] Button itemButton;
        [SerializeField] Image itemImage;
        [SerializeField] Outline itemOutline;

        //------------------------
        public void SetItemPosition(Vector2 position)
        {
            GetComponent<RectTransform>().anchoredPosition += position;
        }

        public void SetCharacterImage(Sprite sprite)
        {
            characterImage.sprite = sprite;
        }

        public void SetCharacterName(string name)
        {
            characterNameText.text = name;
        }

        public void SetCharacterPrice(int price)
        {
            characterPriceText.text = price.ToString();
        }

        public void SetCharacterSpeed(float speed)
        {
            characterSpeedFill.fillAmount = speed / 100;
        }
        public void SetCharacterPower(float power)
        {
            characterPowerFill.fillAmount = power / 100;
        }

        public void SetCharacterAsPurchased()
        {
            characterPurchasedButton.gameObject.SetActive(false);
            //TODO: Change item Color
            itemButton.interactable = true;

            itemImage.color = itemNotSelectedColor;
        }
        public void OnItemPurchase(int itemIndex, UnityAction<int> action)
        {
            characterPurchasedButton.onClick.RemoveAllListeners();
            characterPurchasedButton.onClick.AddListener(() => action.Invoke(itemIndex));
        }
        public void OnItemSelect(int itemIndex, UnityAction<int> action)
        {
            itemButton.interactable = true;
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => action.Invoke(itemIndex));
        }

        public void SelectItem()
        {
            itemOutline.enabled = true;
            itemImage.color = itemSelectedColor;
            itemButton.interactable = false;
        }

        public void DeselectItem()
        {
            itemOutline.enabled = false;
            itemImage.color = itemNotSelectedColor;
            itemButton.interactable = true;
        }
        public void AnimateShakeItem()
        {
            //End all animation first
            transform.DOComplete();

            transform.DOShakePosition(1f, new Vector3(9f, 0, 0), 1, 0).SetEase(Ease.Linear);
        }

    }

}
