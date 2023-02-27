using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject _damageSplashVFX;
    [SerializeField] private Transform _chestTransform;
    [SerializeField] private Transform _fireballPoint;
    [SerializeField] private Transform _arrowPoint;
    [SerializeField] private GameObject _fireballChargeVFX;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _fistDamage = 5f;

    private DungeonGame_AnimatorHandler _animatorHandler = null;
    private DungeonGame_EntityCombat_MeleeAttack_Basic _meleeBasicController = null;
    private DungeonGame_EntityCombat_RangedAttack_Fireball _fireballController = null;
    private DungeonGame_EntityCombat_RangedAttack_Arrow _arrowController = null;
    private DungeonGame_PlayerInventoryController _inventoryController = null;
    private DungeonGame_EntityAttribute _attributes = null;

    private GameObject _chargeVFX = null;

    private void OnEnable()
    {
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _meleeBasicController = transform.GetComponent<DungeonGame_EntityCombat_MeleeAttack_Basic>();
        _fireballController = transform.GetComponent<DungeonGame_EntityCombat_RangedAttack_Fireball>();
        _arrowController = transform.GetComponent<DungeonGame_EntityCombat_RangedAttack_Arrow>();
        _inventoryController = transform.GetComponent<DungeonGame_PlayerInventoryController>();
        _attributes = transform.GetComponent<DungeonGame_EntityAttribute>();

        _animatorHandler.OnAttackPeak.AddListener(Attack);
        _animatorHandler.OnAttackBeginCharge.AddListener(AttackBeginCharge);
    }

    private void OnDisable()
    {
        _animatorHandler.OnAttackPeak.RemoveListener(Attack);
        _animatorHandler.OnAttackBeginCharge.RemoveListener(AttackBeginCharge);
    }

    private void Attack()
    {
        float damage = _fistDamage;
        DungeonGame_Item.WeaponTypes weaponType = DungeonGame_Item.WeaponTypes.Empty;
        if (_inventoryController.PlayerEquipment[DungeonGame_Item.ItemTypes.PrimaryWeapon] != null)
        {
            DungeonGame_WeaponSO weaponData = _inventoryController.PlayerEquipment[DungeonGame_Item.ItemTypes.PrimaryWeapon].ItemData as DungeonGame_WeaponSO;
            weaponType = weaponData.WeaponType;
            damage = weaponData.WeaponBaseDamage;
        }
        if (weaponType == DungeonGame_Item.WeaponTypes.Greatsword || weaponType == DungeonGame_Item.WeaponTypes.Mace || weaponType == DungeonGame_Item.WeaponTypes.Empty)
        {
            if (_meleeBasicController != null)
            {
                _meleeBasicController.Attack(_attackRange, _attributes.ReturnTotalStrengthDamage(damage), _chestTransform, _damageSplashVFX);
            }
        }
        else if (weaponType == DungeonGame_Item.WeaponTypes.Staff)
        {
            if (_fireballController != null)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.CompareTag("Player")) continue;

                    _fireballController.Attack(damage, _fireballPoint, hit.point, _damageSplashVFX);
                    break;
                }
            }
        }
        else if (weaponType == DungeonGame_Item.WeaponTypes.Bow)
        {
            if (_arrowController != null)
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.CompareTag("Player")) continue;

                    DungeonGame_EntityHealth health = null;
                    if (hit.transform.GetComponent<DungeonGame_EntityHealth>())
                    {
                        health = hit.transform.GetComponent<DungeonGame_EntityHealth>();
                    }
                    _arrowController.Attack(damage, hit.point, health, _damageSplashVFX, _arrowPoint);
                    break;
                }
            }
        }

        if (_chargeVFX != null)
            Destroy(_chargeVFX);
    }

    private void AttackBeginCharge(string charge)
    {
        if (charge == "Fireball")
            _chargeVFX = Instantiate(_fireballChargeVFX, _fireballPoint);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_chestTransform.position, _attackRange);
    }
}
