using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Enemy_Melee_Regular : DungeonGame_Enemy
{
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
    }
}
