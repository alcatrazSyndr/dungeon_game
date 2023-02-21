using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerComparatorViewController : MonoBehaviour
{
    [SerializeField] private DungeonGame_PlayerComparator_ItemInfoView _currentInfoView;
    [SerializeField] private DungeonGame_PlayerComparator_ItemInfoView _hoveredInfoView;

    private DungeonGame_PlayerInventoryController _inventory;

    public UnityEvent<DungeonGame_Item> OnEquippedItemHovered = new UnityEvent<DungeonGame_Item>();
    public UnityEvent<DungeonGame_Item> OnItemHovered = new UnityEvent<DungeonGame_Item>();
    public UnityEvent OnItemUnhovered = new UnityEvent();

    private void OnEnable()
    {
        _inventory = transform.parent.parent.GetComponentInParent<DungeonGame_PlayerInventoryController>();

        OnItemHovered.AddListener(ItemHovered);
        OnItemUnhovered.AddListener(ItemUnhovered);
        OnEquippedItemHovered.AddListener(EquippedItemHovered);
    }

    private void OnDisable()
    {
        OnItemHovered.RemoveListener(ItemHovered);
        OnItemUnhovered.RemoveListener(ItemUnhovered);
        OnEquippedItemHovered.RemoveListener(EquippedItemHovered);

        ItemUnhovered();
    }

    private void OnDestroy()
    {
        OnItemHovered.RemoveListener(ItemHovered);
        OnItemUnhovered.RemoveListener(ItemUnhovered);
        OnEquippedItemHovered.RemoveListener(EquippedItemHovered);
    }

    private void ItemUnhovered()
    {
        _currentInfoView.gameObject.SetActive(false);
        _hoveredInfoView.gameObject.SetActive(false);
    }

    private void ItemHovered(DungeonGame_Item item)
    {
        if (item.ItemData is DungeonGame_EquipmentSO)
        {
            _hoveredInfoView.gameObject.SetActive(true);
            _hoveredInfoView.SetInfo(item);
            if (_inventory.PlayerEquipment[item.ItemType] != null)
            {
                _currentInfoView.gameObject.SetActive(true);
                _currentInfoView.SetInfo(_inventory.PlayerEquipment[item.ItemType]);
            }
        }
    }

    private void EquippedItemHovered(DungeonGame_Item item)
    {
        if (item.ItemData is DungeonGame_EquipmentSO)
        {
            if (_inventory.PlayerEquipment[item.ItemType] != null)
            {
                _currentInfoView.gameObject.SetActive(true);
                _currentInfoView.SetInfo(_inventory.PlayerEquipment[item.ItemType]);
            }
        }
    }
}
