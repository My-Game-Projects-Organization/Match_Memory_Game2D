using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShopSystemPackage
{

    //Shop Data Holder
    [System.Serializable]
    public class CharacterShopData
    {
        public List<int> purchasedCharacterIndexes = new List<int>();
    }

    // Player Data Holder
    [System.Serializable]
    public class PlayerData
    {
        public int coins = 0;
        public int selectedCharacterIndex = 0;
    }
    public static class GameDataManager
    {
        static PlayerData playerData = new PlayerData();
        static CharacterShopData characterShopData = new CharacterShopData();

        static CharacterShopVer3 selectedCharacter;

        static GameDataManager()
        {
            LoadPlayerData();
            LoadCharacterShopData();
        }

        // Player Data Method -----------------------------------------

        public static CharacterShopVer3 GetSelectedCharacter()
        {
            return selectedCharacter;
        }
        public static void SetSelectedCharacter(CharacterShopVer3 character, int index)
        {
            selectedCharacter = character;
            playerData.selectedCharacterIndex = index;
            SavePlayerData();
        }

        public static int GetSelectedCharacterIndex()
        {
            return playerData.selectedCharacterIndex;
        }

        public static int GetCOins()
        {
            return playerData.coins;
        }
        public static void AddCoins(int amount)
        {
            playerData.coins += amount;
            SavePlayerData();
        }

        public static bool CanSpendCoins(int amount)
        {
            return (playerData.coins >= amount);
        }

        public static void SpendCoins(int amount)
        {
            playerData.coins -= amount;
            SavePlayerData();
        }

        static void SavePlayerData()
        {
            string playerDataString = JsonUtility.ToJson(playerData);

            try
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerDataString);
                Debug.Log("<color=green>[PlayerData] Saved.</color>");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        static void LoadPlayerData()
        {
            try
            {
                Debug.Log(Application.persistentDataPath);
                string playerDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/PlayerData.json");
                UnityEngine.Debug.Log("<color=green>[PlayerData] Loaded.</color>");
                playerData = JsonUtility.FromJson<PlayerData>(playerDataString);
                if (playerData == null)
                {
                    throw new MissingReferenceException("Data is null");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

        }

        // CharacterShop Data Methods -----------------------------------------

        public static void AddPurchasedCharacter(int characterId)
        {
            characterShopData.purchasedCharacterIndexes.Add(characterId);
            SaveCharacterShopData();
        }

        public static List<int> GetAllPurchasedCharacter()
        {
            return characterShopData.purchasedCharacterIndexes;
        }
        public static int GetPurchasedCharacter(int id)
        {
            return characterShopData.purchasedCharacterIndexes[id];
        }

        static void SaveCharacterShopData()
        {
            string characterShopDataString = JsonUtility.ToJson(characterShopData);

            try
            {
                System.IO.File.WriteAllText(Application.persistentDataPath + "/Character-shop-data.json", characterShopDataString);
                Debug.Log("<color=green>[CharacterShopData] Saved.</color>");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
        static void LoadCharacterShopData()
        {
            try
            {
                Debug.Log(Application.persistentDataPath);
                string characterShopDataString = System.IO.File.ReadAllText(Application.persistentDataPath + "/Character-shop-data.json");
                UnityEngine.Debug.Log("<color=green>[PlayerData] Loaded.</color>");
                characterShopData = JsonUtility.FromJson<CharacterShopData>(characterShopDataString);
                if (characterShopData == null)
                {
                    throw new MissingReferenceException("Data is null");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

        }
    }

}