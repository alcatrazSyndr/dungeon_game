using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerInventory_PlayerInventoryView : MonoBehaviour
{
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Transform _inventoryContentRoot;
    [SerializeField] private DungeonGame_PlayerComparatorViewController _comparator;

    private DungeonGame_PlayerInventoryController _inventory = null;

    private void OnEnable()
    {
        _inventory = transform.parent.parent.GetComponentInParent<DungeonGame_PlayerInventoryController>();
        _inventory.OnInventoryChanged.AddListener(RefreshInventory);

        RefreshInventory();
    }

    private void OnDisable()
    {
        _inventory.OnInventoryChanged.RemoveListener(RefreshInventory);
    }

    private void RefreshInventory()
    {
        foreach (Transform child in _inventoryContentRoot)
        {
            Destroy(child.gameObject);
        }
        int count = _inventory.PlayerInventory.Count + 10;
        foreach (DungeonGame_Item item in _inventory.PlayerInventory)
        {
            GameObject newSlot = Instantiate(_inventorySlotPrefab, _inventoryContentRoot);
            newSlot.GetComponent<DungeonGame_PlayerInventory_ItemSlotView>().Initialize(item, _inventory, _comparator);
            newSlot.GetComponent<Canvas>().sortingOrder = count;
            count--;
        }
    }
}
