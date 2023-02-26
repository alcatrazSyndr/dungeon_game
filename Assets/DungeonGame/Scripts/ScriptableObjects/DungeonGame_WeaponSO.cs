using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGame_Weapon_", menuName = "Dungeon Game/New Weapon")]
public class DungeonGame_WeaponSO : DungeonGame_EquipmentSO 
{
    public GameObject WeaponPrefab;
    public float AnimatorValue = 0f;
    public int WeaponQuality = 0;
    public float WeaponBaseDamage = 0f;
    public bool RightHanded = true;
    public bool TwoHanded = true;
}
