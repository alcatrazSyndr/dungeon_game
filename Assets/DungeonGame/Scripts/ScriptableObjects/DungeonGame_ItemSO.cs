using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGame_Item_", menuName = "Dungeon Game/New Item")]
public class DungeonGame_ItemSO : ScriptableObject
{
    public string ID;
    public string Name;
    public string ItemDescription;
    public Sprite Icon;
    public DungeonGame_Item.ItemTypes ItemType;
}
