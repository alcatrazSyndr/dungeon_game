using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGame_Equipment_", menuName = "Dungeon Game/New Equipment")]
public class DungeonGame_EquipmentSO : DungeonGame_ItemSO 
{
    public List<DungeonGame_Attribute> EquipmentAttributeBonus = new List<DungeonGame_Attribute>();
}
