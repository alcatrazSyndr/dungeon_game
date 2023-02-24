using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerEquipmentController : MonoBehaviour
{
    [SerializeField] private Transform _primaryWeaponHolder;
    [SerializeField] private Transform _secondaryWeaponHolder;
    [SerializeField] private GameObject _torso;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _feet;
    [SerializeField] private List<GameObject> _helmets = new List<GameObject>();
    [SerializeField] private List<GameObject> _chestpieces = new List<GameObject>();
    [SerializeField] private List<GameObject> _leggings = new List<GameObject>();
    [SerializeField] private List<GameObject> _boots = new List<GameObject>();

    private DungeonGame_PlayerAnimation _animation = null;
    private DungeonGame_PlayerInventoryController _inventory = null;

    private Dictionary<DungeonGame_Item.ItemTypes, GameObject> _equipmentVisuals = new Dictionary<DungeonGame_Item.ItemTypes, GameObject>()
    {
        { DungeonGame_Item.ItemTypes.Helmet, null },
        { DungeonGame_Item.ItemTypes.Chestpiece, null },
        { DungeonGame_Item.ItemTypes.Leggings, null },
        { DungeonGame_Item.ItemTypes.Boots, null }
    };

    private void OnEnable()
    {
        _inventory = transform.GetComponent<DungeonGame_PlayerInventoryController>();
        _animation = transform.GetComponentInChildren<DungeonGame_PlayerAnimation>();

        _inventory.OnEquipmentEquipped.AddListener(EquipmentEquipped);
        _inventory.OnEquipmentUnequipped.AddListener(EquipmentUnequipped);
    }

    private void OnDisable()
    {
        _inventory.OnEquipmentEquipped.RemoveListener(EquipmentEquipped);
        _inventory.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void OnDestroy()
    {
        _inventory.OnEquipmentEquipped.RemoveListener(EquipmentEquipped);
        _inventory.OnEquipmentUnequipped.RemoveListener(EquipmentUnequipped);
    }

    private void EquipmentEquipped(DungeonGame_Item newEquipment)
    {
        if (newEquipment.ItemType == DungeonGame_Item.ItemTypes.PrimaryWeapon)
        {
            WeaponChanged(newEquipment);
        }
        else if (ItemIsVisualArmor(newEquipment))
        {
            ArmorEquipped(newEquipment);
        }
    }

    private void EquipmentUnequipped(DungeonGame_Item equipmentToUnequip)
    {
        if (equipmentToUnequip.ItemType == DungeonGame_Item.ItemTypes.PrimaryWeapon)
        {
            ClearHands();
        }
        else if (ItemIsVisualArmor(equipmentToUnequip))
        {
            ArmorUnequipped(equipmentToUnequip);
        }
    }

    #region Armor
    private bool ItemIsVisualArmor(DungeonGame_Item item)
    {
        if (item.ItemType == DungeonGame_Item.ItemTypes.Helmet || item.ItemType == DungeonGame_Item.ItemTypes.Chestpiece || item.ItemType == DungeonGame_Item.ItemTypes.Leggings || item.ItemType == DungeonGame_Item.ItemTypes.Boots)
            return true;
        else
            return false;
    }

    private GameObject ReturnArmorPieceByID(string id)
    {
        if (id.Contains("_leggings_"))
        {
            foreach (GameObject piece in _leggings)
            {
                if (piece.name == id)
                {
                    return piece;
                }
            }
        }
        else if (id.Contains("_helmet_"))
        {
            foreach (GameObject piece in _helmets)
            {
                if (piece.name == id)
                {
                    return piece;
                }
            }
        }
        else if (id.Contains("_chestpiece_"))
        {
            foreach (GameObject piece in _chestpieces)
            {
                if (piece.name == id)
                {
                    return piece;
                }
            }
        }
        else if (id.Contains("_boots_"))
        {
            foreach (GameObject piece in _boots)
            {
                if (piece.name == id)
                {
                    return piece;
                }
            }
        }
        return null;
    }

    private void ToggleBodyPart(DungeonGame_Item.ItemTypes itemType, bool toggle)
    {
        if (itemType == DungeonGame_Item.ItemTypes.Helmet)
        {
            // TODO
        }
        else if (itemType == DungeonGame_Item.ItemTypes.Chestpiece)
        {
            _torso.SetActive(toggle);
        }
        else if (itemType == DungeonGame_Item.ItemTypes.Leggings)
        {
            _legs.SetActive(toggle);
        }
        else if (itemType == DungeonGame_Item.ItemTypes.Boots)
        {
            _feet.SetActive(toggle);
        }
    }

    private void ArmorUnequipped(DungeonGame_Item armor)
    {
        if (_equipmentVisuals[armor.ItemType] != null)
        {
            _equipmentVisuals[armor.ItemType].SetActive(false);
            _equipmentVisuals[armor.ItemType] = null;
        }
        ToggleBodyPart(armor.ItemType, true);
    }

    private void ArmorEquipped(DungeonGame_Item armor)
    {
        if (_equipmentVisuals[armor.ItemType] != null)
        {
            _equipmentVisuals[armor.ItemType].SetActive(false);
            _equipmentVisuals[armor.ItemType] = null;
        }
        GameObject armorVisual = ReturnArmorPieceByID(armor.ItemData.ID);
        if (armorVisual != null)
        {
            _equipmentVisuals[armor.ItemType] = armorVisual;
            armorVisual.SetActive(true);
            ToggleBodyPart(armor.ItemType, false);
        }
    }
    #endregion

    #region Weapon
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
    #endregion
}
