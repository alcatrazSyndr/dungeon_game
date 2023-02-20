using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerEquipment_PlayerEquipmentView : MonoBehaviour
{
    [SerializeField] private List<DungeonGame_PlayerEquipment_EquipmentSlotView> _equipmentSlots = new List<DungeonGame_PlayerEquipment_EquipmentSlotView>();

    private DungeonGame_PlayerInventoryController _inventory = null;

    private void OnEnable()
    {
        _inventory = transform.parent.parent.GetComponentInParent<DungeonGame_PlayerInventoryController>();
        _inventory.OnInventoryChanged.AddListener(RefreshEquipment);

        RefreshEquipment();
    }

    private void OnDisable()
    {
        _inventory.OnInventoryChanged.RemoveListener(RefreshEquipment);
    }

    private void RefreshEquipment()
    {
        foreach (DungeonGame_PlayerEquipment_EquipmentSlotView equipment in _equipmentSlots)
        {
            equipment.UpdateSlot(_inventory);
        }
    }
}
