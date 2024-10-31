using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "ItemShopDatabase", menuName = "Shop System/Items Shop Database")]
public class ItemShopDatabase : ScriptableObject
{
    public ItemHelp[] listItems;
    public int ItemCount
    {
        get { return listItems.Length; } 
    }
    public ItemHelp GetItemShop(int index)
    {
        return listItems[index];
    }

}
