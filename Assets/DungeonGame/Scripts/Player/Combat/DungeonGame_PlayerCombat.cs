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
    [SerializeField] private LayerMask _collisionLayer;

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

    public List<Ray> ReturnRaysInCone(float angle, int sampleRate, Transform root, Vector3 heading)
    {
        List<Ray> rays = new List<Ray>();

        RaycastHit firstHit;
        List<RaycastHit> hits = ReturnSortedRaycastHits(root);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.CompareTag("Player")) continue;

            firstHit = hit;
            Vector3 startPoint = root.position;
            float sampleAngle = angle / (float)sampleRate;
            float startAngle = -((float)sampleRate / 2f);
            Vector3 startDir = Quaternion.AngleAxis(startAngle, Vector3.up) * heading;
            for (int i = 0; i < sampleRate; i++)
            {
                Vector3 sampledDir = Quaternion.AngleAxis(sampleAngle * i, Vector3.up) * startDir;
                Ray sampledRay = new Ray(startPoint, sampledDir);
                rays.Add(sampledRay);
            }

            break;
        }

        return rays;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_chestTransform.position, _attackRange);
        Gizmos.color = Color.blue;
        List<Ray> rays = ReturnRaysInCone(30f, 6, _fireballPoint, Camera.main.transform.forward);
        if (rays.Count > 0)
        {
            foreach (Ray ray in rays)
            {
                Gizmos.DrawRay(ray);
            }
        }
    }
}
