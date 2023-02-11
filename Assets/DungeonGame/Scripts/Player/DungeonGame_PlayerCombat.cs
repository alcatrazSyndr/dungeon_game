using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject _damageSplashVFX;
    [SerializeField] private Transform _chestTransform;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _debugDamage = 20f;

    private DungeonGame_AnimatorHandler _animatorHandler = null;
    private DungeonGame_EntityCombat_MeleeAttack_Basic _meleeBasicController = null;

    private void OnEnable()
    {
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _meleeBasicController = transform.GetComponent<DungeonGame_EntityCombat_MeleeAttack_Basic>();

        _animatorHandler.OnAttackPeak.AddListener(Attack);
    }

    private void OnDisable()
    {
        _animatorHandler.OnAttackPeak.RemoveListener(Attack);
    }

    private void Attack()
    {
        if (_meleeBasicController != null)
        {
            _meleeBasicController.Attack(_attackRange, _debugDamage, _chestTransform, _damageSplashVFX);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_chestTransform.position, _attackRange);
    }
}
