using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private List<DungeonGame_ItemSO> _inventoryDebug = new List<DungeonGame_ItemSO>();

    private List<DungeonGame_Item> _playerInventory = new List<DungeonGame_Item>();
    public List<DungeonGame_Item> PlayerInventory { get { return _playerInventory; } }

    private Dictionary<DungeonGame_Item.ItemTypes, DungeonGame_Item> _playerEquipment = new Dictionary<DungeonGame_Item.ItemTypes, DungeonGame_Item>()
    {
        { DungeonGame_Item.ItemTypes.PrimaryWeapon, null },
        { DungeonGame_Item.ItemTypes.Helmet, null },
        { DungeonGame_Item.ItemTypes.Chestpiece, null },
        { DungeonGame_Item.ItemTypes.Leggings, null },
        { DungeonGame_Item.ItemTypes.Boots, null }
    };
    public Dictionary<DungeonGame_Item.ItemTypes, DungeonGame_Item> PlayerEquipment { get { return _playerEquipment; } }

    public UnityEvent OnInventoryChanged = new UnityEvent();
    public UnityEvent<DungeonGame_Item> OnEquipmentEquipped = new UnityEvent<DungeonGame_Item>();
    public UnityEvent<DungeonGame_Item> OnEquipmentUnequipped = new UnityEvent<DungeonGame_Item>();

    private void Start()
    {
        foreach (DungeonGame_ItemSO item in _inventoryDebug)
        {
            _playerInventory.Add(new DungeonGame_Item(item));
        }
    }

    private void InventoryChange()
    {
        OnInventoryChanged?.Invoke();
    }

    public void ItemUsed(DungeonGame_Item itemUsed)
    {
        if (!_playerInventory.Contains(itemUsed)) return;

        DungeonGame_Item.ItemTypes usedItemType = itemUsed.ItemType;
        if (usedItemType == DungeonGame_Item.ItemTypes.Null) return;

        if (_playerEquipment.ContainsKey(usedItemType))
            EquipmentUsed(itemUsed);

        InventoryChange();
    }

    private void EquipmentUsed(DungeonGame_Item equipmentUsed)
    {
        DungeonGame_Item cachedItem = _playerEquipment[equipmentUsed.ItemType];
        _playerInventory.Remove(equipmentUsed);
        if (cachedItem != null)
            _playerInventory.Add(cachedItem);
        _playerEquipment[equipmentUsed.ItemType] = equipmentUsed;

        OnEquipmentEquipped?.Invoke(equipmentUsed);
    }

    public void EquipmentUnequiped(DungeonGame_Item.ItemTypes slot)
    {
        if (_playerEquipment[slot] == null) return;

        _playerInventory.Add(_playerEquipment[slot]);
        OnEquipmentUnequipped?.Invoke(_playerEquipment[slot]);
        _playerEquipment[slot] = null;

        OnInventoryChanged?.Invoke();
    }
}
