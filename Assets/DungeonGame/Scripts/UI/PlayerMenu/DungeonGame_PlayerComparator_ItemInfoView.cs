using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonGame_PlayerComparator_ItemInfoView : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private Transform _itemAttributeRoot;
    [SerializeField] private GameObject _itemAttributeTextPrefab;
    [SerializeField] private GameObject _itemDescriptionTextPrefab;

    public void SetInfo(DungeonGame_Item item)
    {
        foreach (Transform child in _itemAttributeRoot)
        {
            Destroy(child.gameObject);
        }
        if (item.ItemData is DungeonGame_EquipmentSO)
        {
            DungeonGame_EquipmentSO equipmentData = item.ItemData as DungeonGame_EquipmentSO;
            foreach (DungeonGame_Attribute attribute in equipmentData.EquipmentAttributeBonus)
            {
                GameObject attributeTextGO = Instantiate(_itemAttributeTextPrefab, _itemAttributeRoot);
                TextMeshProUGUI attributeText = attributeTextGO.GetComponent<TextMeshProUGUI>();
                attributeText.text = attribute.Attribute.ToString() + ": " + attribute.AttributeValue.ToString();
            }
            _itemIcon.sprite = equipmentData.Icon;
            _itemName.text = equipmentData.Name;
        }
        GameObject descriptionGO = Instantiate(_itemDescriptionTextPrefab, _itemAttributeRoot);
        descriptionGO.GetComponent<TextMeshProUGUI>().text = item.ItemData.ItemDescription;
    }
}
