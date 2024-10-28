using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShopSystemPackage
{

    public class GameSharedUI : MonoBehaviour
    {
        #region Singleton class : GameSharedUI

        public static GameSharedUI Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        #endregion

        [SerializeField] Text[] coinsUIText;
        [SerializeField] Text[] hintCountUIText;
        private void Start()
        {
            UpdateCoinsUIText();
        }

        public void UpdateHintCountUIText()
        {
            SetCoinsText(hintCountUIText[0], Pref.nOHintHelp);
            SetCoinsText(hintCountUIText[1], Pref.nOExtraHintHelp);
            SetCoinsText(hintCountUIText[2], Pref.nOExtraTimeHelp);

        }

        public void UpdateCoinsUIText()
        {
            for (int i = 0; i < coinsUIText.Length; i++)
            {
                SetCoinsText(coinsUIText[i], Pref.startcoins);
            }
        }

        void SetCoinsText(Text textMesh, int val)
        {
            if (val >= 1000)
                textMesh.text = string.Format("{0}K.{1}", (val / 1000), GetFirstDigitFromNumber(val % 1000));
            else
                textMesh.text = val.ToString();
        }

        private object GetFirstDigitFromNumber(int v)
        {
            return int.Parse(v.ToString()[0].ToString());
        }
    }

}