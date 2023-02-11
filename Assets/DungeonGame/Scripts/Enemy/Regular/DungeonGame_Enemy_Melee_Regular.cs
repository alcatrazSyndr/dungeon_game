using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Enemy_Melee_Regular : DungeonGame_Enemy
{
    [SerializeField] private float _attackSwingRange = 1f;
    [SerializeField] private GameObject _attackHitVFX;
    [SerializeField] private Transform _attackSourceTransform;

    private DungeonGame_EntityCombat_MeleeAttack_Basic _meleeBasicAttack = null;

    protected override void OnEnemyEnabled()
    {
        _meleeBasicAttack = transform.GetComponent<DungeonGame_EntityCombat_MeleeAttack_Basic>();
    }

    protected override void BehaviourOnTargetInAttackRange()
    {
        //print("starting attack");
        _inAttack = true;
        _animator.CrossFadeInFixedTime("Attack_" + _currentAttackCombo.ToString(), 0.1f);
        _currentAttackCombo++;
        if (_currentAttackCombo > _maxAttackCombo)
            _currentAttackCombo = 1;
    }

    protected override void BehaviourOnTargetOutsideAttackRange()
    {
        //print("moving to player");
        _movement.MoveToDestination(_currentTarget.position);
    }

    protected override void AttackPeak()
    {
        //print("attack peak");
        _meleeBasicAttack.Attack(_attackSwingRange, _attackDamage, _attackSourceTransform, _attackHitVFX);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_attackSourceTransform.position, _attackSwingRange);
    }
}
