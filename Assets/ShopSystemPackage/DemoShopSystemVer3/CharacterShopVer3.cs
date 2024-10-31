using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemPackage
{
    [System.Serializable]
    public struct CharacterShopVer3
    {
        public Sprite image;
        public string name;
        [Range(0f, 100)] public float speed;
        [Range(0f, 100)] public float power;
        public int price;

        public bool isPurchased;
    }
}
