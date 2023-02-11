using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class DungeonGame_Enemy : MonoBehaviour
{
    [SerializeField] protected DungeonGame_EnemySO _myEnemyType;
    [SerializeField] private float _brainTick = 0.5f;
    [SerializeField] protected int _maxAttackCombo = 1;

    protected DungeonGame_NavMeshMovement _movement = null;
    protected DungeonGame_AnimatorHandler _animatorHandler = null;
    protected DungeonGame_EntityHealth _health = null;
    protected Transform _currentTarget = null;
    protected Animator _animator = null;
    protected float _viewRange;
    protected float _attackRange;
    protected float _attackDamage;
    protected float _distanceToTarget = 100f;
    protected float _attackCooldown;
    protected bool _inAttack = false;
    protected int _currentAttackCombo = 1;

    public UnityEvent OnBrainShutoff = new UnityEvent();

    protected void OnEnable()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _movement = transform.GetComponent<DungeonGame_NavMeshMovement>();
        _health = transform.GetComponent<DungeonGame_EntityHealth>();
        _health.SetInitialHealth(_myEnemyType.BaseHealth);
        _movement.SetMovementSpeed(_movement.GetMoveSpeed() * _myEnemyType.MovementSpeed);
        _viewRange = _myEnemyType.ViewRange;
        _attackRange = _myEnemyType.AttackRange;
        _attackDamage = _myEnemyType.BaseAttackDamage;
        _attackCooldown = 0f;

        _animatorHandler.OnAttackEnd.AddListener(AttackEnd);
        _animatorHandler.OnAttackPeak.AddListener(AttackPeak);

        OnEnemyEnabled();

        StartCoroutine(BrainCRT());
    }

    protected void OnDisable()
    {
        StopAllCoroutines();

        _animatorHandler.OnAttackEnd.RemoveListener(AttackEnd);
        _animatorHandler.OnAttackPeak.RemoveListener(AttackPeak);
    }

    protected void OnDestroy()
    {
        StopAllCoroutines();

        _animatorHandler.OnAttackEnd.RemoveListener(AttackEnd);
        _animatorHandler.OnAttackPeak.RemoveListener(AttackPeak);
    }

    protected IEnumerator BrainCRT()
    {
        while (_health != null && _health.Alive())
        {
            BrainTick();
            yield return new WaitForSeconds(_brainTick);
            yield return null;
        }
        OnBrainShutoff?.Invoke();
        yield break;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    protected void BrainTick() 
    {
        //print("tick");
        if (_currentTarget == null)
        {
            //print("checking for players");
            CheckForPlayerInViewRange();
        }
        else
        {
            //print("has target, checking distance");
            _distanceToTarget = Vector3.Distance(transform.position, _currentTarget.position);
            BehaviourOnHasTarget();
        }
    }

    protected void BehaviourOnHasTarget()
    {
        if (_inAttack)
        {
            //print("rotating in attack");
            _movement.RotateTowards(_currentTarget);
        }
        else
        {
            if (_distanceToTarget <= _attackRange && _attackCooldown <= 0f)
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

    protected void CheckForPlayerInViewRange()
    {
        //print("finding game manager");
        if (DungeonGame_GameManager.Instance == null) return;
        //print("checking player list is empty");
        if (DungeonGame_GameManager.Instance.PlayerControllers.Count <= 0) return;

        //print("has player data, starting distance calculation");
        List<Transform> possibleTargets = new List<Transform>();
        foreach (DungeonGame_PlayerController player in DungeonGame_GameManager.Instance.PlayerControllers)
        {
            //print("checking player health");
            if (!player.transform.GetComponent<DungeonGame_EntityHealth>() || !player.transform.GetComponent<DungeonGame_EntityHealth>().Alive()) continue;
            //print("checking player distance");
            if (Vector3.Distance(player.transform.position, transform.position) >= _viewRange)
            {
                //print("distance to player: " + Vector3.Distance(player.transform.position, transform.position).ToString() + " is greater than max view range: " + _viewRange.ToString());
                continue;
            }
            else
            {
                //print("adding player in range to list");
                possibleTargets.Add(player.transform);
            }
        }
        possibleTargets.Sort((p1, p2) => Vector3.Distance(p1.position, transform.position).CompareTo(Vector3.Distance(p2.position, transform.position)));

        if (possibleTargets.Count > 0)
            _currentTarget = possibleTargets[0];
    }

    protected virtual void BehaviourOnTargetInAttackRange() { }

    protected virtual void BehaviourOnTargetOutsideAttackRange() { }

    protected virtual void OnEnemyEnabled() { }

    protected virtual void AttackPeak() { }

    protected virtual void AttackEnd() 
    {
        _attackCooldown = _myEnemyType.AttackCooldown;
        _inAttack = false;
        StartCoroutine(CooldownCRT());
    }

    private IEnumerator CooldownCRT()
    {
        while (_attackCooldown > 0f && _health.Alive())
        {
            if (_currentTarget != null && _distanceToTarget > _attackRange)
            {
                _movement.RotateTowards(_currentTarget);
            }
            _attackCooldown -= Time.fixedDeltaTime * Time.timeScale;
            yield return null;
        }
        yield break;
    }
}
