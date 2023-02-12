using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Enemy_Ranged_Regular : DungeonGame_Enemy
{
    [SerializeField] private GameObject _attackHitVFX;
    [SerializeField] private Transform _attackSourceTransform;
    [SerializeField] private LayerMask _obstaclesLayer;

    private DungeonGame_EntityCombat_RangedAttack_Basic _rangedBasicAttack = null;

    protected override void OnEnemyEnabled()
    {
        _rangedBasicAttack = transform.GetComponent<DungeonGame_EntityCombat_RangedAttack_Basic>();
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
        _rangedBasicAttack.Attack(_attackDamage, _attackSourceTransform, _currentTarget.position + new Vector3(0f, 0.8f, 0f), _attackHitVFX);
        _attackCooldown = _myEnemyType.AttackCooldown;
    }

    protected override void BehaviourOnHasTarget()
    {
        if (_inAttack)
        {
            //print("rotating in attack");
            _movement.RotateTowards(_currentTarget);
        }
        else
        {
            if (_distanceToTarget <= _attackRange && _attackCooldown <= 0f && PlayerVisible())
            {
                _movement.StopAgent();
                BehaviourOnTargetInAttackRange();
            }
            else
            {
                BehaviourOnTargetOutsideAttackRange();
            }
        }
    }

    private bool PlayerVisible()
    {
        if (Physics.Linecast(_attackSourceTransform.position, _currentTarget.position + new Vector3(0f, 0.8f, 0f), _obstaclesLayer.value))
            return false;
        else
            return true;
    }
}
