using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemPackage
{
    [CreateAssetMenu(fileName = "CharacterShopDatabase", menuName = "Shopping/Characters shop database")]
    public class CharacterShopDatabase : ScriptableObject
    {
        public CharacterShopVer3[] characterShopVer3s;

        public CharacterShopVer3[] CharacterShopVer3s { get => characterShopVer3s; }

        public int CharactersCount()
        {
            return CharacterShopVer3s.Length;
        }

        public CharacterShopVer3 GetCharacterShopVer3(int index)
        {
            return CharacterShopVer3s[index];
        }

        public void PurchasedCharacter(int index)
        {
            CharacterShopVer3s[index].isPurchased = true;
        }

    }

}
