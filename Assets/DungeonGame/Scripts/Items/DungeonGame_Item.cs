using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_Item
{
    public enum ItemTypes { Null, Resource, PrimaryWeapon, Helmet, Chestpiece, Leggings, Boots, SecondaryWeapon }

    public ItemTypes ItemType = ItemTypes.Null;
    public Sprite ItemIcon = null;
    public string ItemName = string.Empty;
    public DungeonGame_ItemSO ItemData = null;

    public DungeonGame_Item(DungeonGame_ItemSO item)
    {
        ItemType = item.ItemType;
        ItemIcon = item.Icon;
        ItemName = item.Name;
        ItemData = item;
    }
}
