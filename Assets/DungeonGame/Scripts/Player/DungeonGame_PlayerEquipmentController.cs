using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerEquipmentController : MonoBehaviour
{
    [SerializeField] private Transform _primaryWeaponHolder;
    [SerializeField] private Transform _secondaryWeaponHolder;

    private DungeonGame_PlayerAnimation _animation = null;
    private DungeonGame_PlayerInventoryController _inventory = null;

    private void OnEnable()
    {
        _inventory = transform.GetComponent<DungeonGame_PlayerInventoryController>();
        _animation = transform.GetComponentInChildren<DungeonGame_PlayerAnimation>();

        _inventory.OnEquipmentEquipped.AddListener(EquipmentChanged);
        _inventory.OnEquipmentUnequipped.AddListener(EquipmentUnequipped);
    }

    private void OnDisable()
    {
        _inventory.OnEquipmentEquipped.RemoveListener(EquipmentChanged);
        _inventory.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void OnDestroy()
    {
        _inventory.OnEquipmentEquipped.RemoveListener(EquipmentChanged);
        _inventory.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void EquipmentChanged(DungeonGame_Item newEquipment)
    {
        if (newEquipment.ItemType == DungeonGame_Item.ItemTypes.PrimaryWeapon)
            WeaponChanged(newEquipment);
    }

    private void WeaponChanged(DungeonGame_Item newWeapon)
    {
        ClearHands();
        DungeonGame_WeaponSO weaponData = newWeapon.ItemData as DungeonGame_WeaponSO;
        GameObject weapon = Instantiate(weaponData.WeaponPrefab, _primaryWeaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        _animation.SetWeaponAnimationValue(weaponData.AnimatorValue);
    }

    private void ClearHands()
    {
        foreach (Transform child in _primaryWeaponHolder)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _secondaryWeaponHolder)
        {
            Destroy(child.gameObject);
        }
        _animation.SetWeaponAnimationValue(0f);
    }

    private void EquipmentUnequipped(DungeonGame_Item equipmentToUnequip)
    {
        if (equipmentToUnequip.ItemType == DungeonGame_Item.ItemTypes.PrimaryWeapon)
            ClearHands();
    }
}
