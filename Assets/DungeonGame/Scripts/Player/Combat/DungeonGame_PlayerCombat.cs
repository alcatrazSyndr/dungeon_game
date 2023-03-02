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
    [SerializeField] private ParticleSystem _fireBreathVFX;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _fistDamage = 5f;
    [SerializeField] private LayerMask _collisionLayer;

    private DungeonGame_AnimatorHandler _animatorHandler = null;
    private DungeonGame_EntityCombat_MeleeAttack_Basic _meleeBasicController = null;
    private DungeonGame_EntityCombat_RangedAttack_Fireball _fireballController = null;
    private DungeonGame_EntityCombat_ConeAttack_FireBreath _fireBreathController = null;
    private DungeonGame_EntityCombat_RangedAttack_Arrow _arrowController = null;
    private DungeonGame_PlayerInventoryController _inventoryController = null;
    private DungeonGame_EntityAttribute _attributes = null;
    private DungeonGame_EntityActionPoints _actionPoints = null;
    private GameObject _chargeVFX = null;

    private IEnumerator _secondaryAttackCRT = null;

    private void OnEnable()
    {
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _meleeBasicController = transform.GetComponent<DungeonGame_EntityCombat_MeleeAttack_Basic>();
        _fireballController = transform.GetComponent<DungeonGame_EntityCombat_RangedAttack_Fireball>();
        _fireBreathController = transform.GetComponent<DungeonGame_EntityCombat_ConeAttack_FireBreath>();
        _arrowController = transform.GetComponent<DungeonGame_EntityCombat_RangedAttack_Arrow>();
        _inventoryController = transform.GetComponent<DungeonGame_PlayerInventoryController>();
        _attributes = transform.GetComponent<DungeonGame_EntityAttribute>();
        _actionPoints = transform.GetComponent<DungeonGame_EntityActionPoints>();

        _animatorHandler.OnAttackPeak.AddListener(Attack);
        _animatorHandler.OnAttackBeginCharge.AddListener(AttackBeginCharge);
        _animatorHandler.OnSecondaryAttackStart.AddListener(SecondaryAttackStart);
        _animatorHandler.OnSecondaryAttackEnd.AddListener(SecondaryAttackEnd);
    }

    private void OnDisable()
    {
        _animatorHandler.OnAttackPeak.RemoveListener(Attack);
        _animatorHandler.OnAttackBeginCharge.RemoveListener(AttackBeginCharge);
        _animatorHandler.OnSecondaryAttackStart.RemoveListener(SecondaryAttackStart);
        _animatorHandler.OnSecondaryAttackEnd.RemoveListener(SecondaryAttackEnd);
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
                List<RaycastHit> hits = ReturnSortedRaycastHits(_fireballPoint);
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
                List<RaycastHit> hits = ReturnSortedRaycastHits(_arrowPoint);
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

    private void SecondaryAttackStart()
    {
        SecondaryAttack(true);
    }

    private void SecondaryAttackEnd()
    {
        SecondaryAttack(false);
    }

    private void SecondaryAttack(bool toggle)
    {
        float damage = _fistDamage;
        DungeonGame_Item.WeaponTypes weaponType = DungeonGame_Item.WeaponTypes.Empty;
        if (_inventoryController.PlayerEquipment[DungeonGame_Item.ItemTypes.PrimaryWeapon] != null)
        {
            DungeonGame_WeaponSO weaponData = _inventoryController.PlayerEquipment[DungeonGame_Item.ItemTypes.PrimaryWeapon].ItemData as DungeonGame_WeaponSO;
            weaponType = weaponData.WeaponType;
            damage = weaponData.WeaponBaseDamage;
        }
        if (weaponType == DungeonGame_Item.WeaponTypes.Greatsword)
        {
            // TODO
        }
        else if (weaponType == DungeonGame_Item.WeaponTypes.Mace)
        {
            // TODO
        }
        else if (weaponType == DungeonGame_Item.WeaponTypes.Staff)
        {
            if (toggle && !_actionPoints.BurntOut)
            {
                if (_secondaryAttackCRT != null)
                {
                    StopCoroutine(_secondaryAttackCRT);
                    _secondaryAttackCRT = null;
                }
                _secondaryAttackCRT = SecondaryAttackCRT_FireBreath(damage);
                StartCoroutine(_secondaryAttackCRT);
                if (!_fireBreathVFX.gameObject.activeSelf)
                    _fireBreathVFX.gameObject.SetActive(true);
                _fireBreathVFX.Play();
            }
            else
            {
                if (_secondaryAttackCRT != null)
                {
                    StopCoroutine(_secondaryAttackCRT);
                    _fireBreathVFX.Stop();
                }
            }
        }
        else if (weaponType == DungeonGame_Item.WeaponTypes.Bow)
        {
            // TODO
        }
    }

    private IEnumerator SecondaryAttackCRT_FireBreath(float damage)
    {
        float tick = _fireBreathController.AttackTick;
        while (true)
        {
            if (_fireBreathController != null)
            {
                List<RaycastHit> hits = ReturnSortedRaycastHits(_fireballPoint);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.CompareTag("Player")) continue;

                    _fireBreathController.Attack(_fireballPoint, Camera.main.transform.forward, hit, damage / 4f);
                    break;
                }
            }
            _actionPoints.ChangeActionPoints(-10f);
            yield return new WaitForSecondsRealtime(tick);
            yield return null;
        }
    }

    private void AttackBeginCharge(string charge)
    {
        if (charge == "Fireball")
            _chargeVFX = Instantiate(_fireballChargeVFX, _fireballPoint);
        else if (charge == "FireBreath")
            _chargeVFX = Instantiate(_fireballChargeVFX, _fireballPoint);
    }

    private List<RaycastHit> ReturnSortedRaycastHits(Transform fromPoint)
    {
        Vector3 startPos = fromPoint.position;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Mathf.Infinity, _collisionLayer.value, QueryTriggerInteraction.Collide);
        List<RaycastHit> hitList = new List<RaycastHit>(hits);
        hitList.Sort((hit1, hit2) => Vector3.Distance(startPos, hit1.point).CompareTo(Vector3.Distance(startPos, hit2.point)));
        return hitList;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_chestTransform.position, _attackRange);
    }
}
