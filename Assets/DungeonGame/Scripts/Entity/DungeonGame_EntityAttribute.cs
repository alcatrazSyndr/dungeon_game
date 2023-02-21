using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityAttribute : MonoBehaviour
{
    [SerializeField] private int _damagePerStrengthBonus = 1;

    public enum Attribute { Null, Strength, Vitality, Agility, Intellect }

    private DungeonGame_PlayerInventoryController _inventoryController = null;
    private List<DungeonGame_Attribute> _gearAttributes = new List<DungeonGame_Attribute>();
    public List<DungeonGame_Attribute> GearAttributes { get { return _gearAttributes; } }

    private void OnEnable()
    {
        _inventoryController = transform.GetComponent<DungeonGame_PlayerInventoryController>();

        _inventoryController.OnEquipmentEquipped.AddListener(EquipmentEquipped);
        _inventoryController.OnEquipmentUnequipped.AddListener(EquipmentUnequipped);
    }

    private void OnDestroy()
    {
        _inventoryController.OnEquipmentEquipped.RemoveListener(EquipmentEquipped);
        _inventoryController.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void OnDisable()
    {
        _inventoryController.OnEquipmentEquipped.RemoveListener(EquipmentEquipped);
        _inventoryController.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void EquipmentEquipped(DungeonGame_Item equipment)
    {
        DungeonGame_EquipmentSO equipmentData = equipment.ItemData as DungeonGame_EquipmentSO;
        foreach (DungeonGame_Attribute attribute in equipmentData.EquipmentAttributeBonus)
        {
            _gearAttributes.Add(attribute);
        }
    }

    private void EquipmentUnequipped(DungeonGame_Item equipment)
    {
        DungeonGame_EquipmentSO equipmentData = equipment.ItemData as DungeonGame_EquipmentSO;
        foreach (DungeonGame_Attribute attribute in equipmentData.EquipmentAttributeBonus)
        {
            if (_gearAttributes.Contains(attribute))
            {
                _gearAttributes.Remove(attribute);
            }
        }
    }

    public float ReturnTotalStrengthDamage(float baseDamage)
    {
        float damage = baseDamage;
        foreach (DungeonGame_Attribute attribute in _gearAttributes)
        {
            if (attribute.Attribute == Attribute.Strength)
            {
                damage += ((float)attribute.AttributeValue * (float)_damagePerStrengthBonus);
            }
        }
        return damage;
    }
}

[System.Serializable]
public class DungeonGame_Attribute
{
    public DungeonGame_EntityAttribute.Attribute Attribute = DungeonGame_EntityAttribute.Attribute.Null;
    public int AttributeValue = 0;

    public DungeonGame_Attribute(DungeonGame_EntityAttribute.Attribute attribute, int attributeValue)
    {
        Attribute = attribute;
        AttributeValue = attributeValue;
    }
}
