using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float _combatCooldown = 2f;

    private Animator _animator;
    private DungeonGame_PlayerController _playerController;
    private float _lastAttack = 0f;

    public UnityEvent OnRollFinished = new UnityEvent();
    public UnityEvent OnAttackPeaked = new UnityEvent();
    public UnityEvent OnAttackEnded = new UnityEvent();
    public UnityEvent OnLeftCombatStance = new UnityEvent();

    private void OnEnable()
    {
        _playerController = transform.GetComponentInParent<DungeonGame_PlayerController>();
        _animator = transform.GetComponent<Animator>();

        OnAttackEnded.AddListener(EndAttack);

        _playerController.OnRolled.AddListener(Roll);
        _playerController.OnSprintChanged.AddListener(SprintChanged);
        _playerController.OnAttacked.AddListener(StartAttack);
    }

    private void OnDisable()
    {
        OnAttackEnded.RemoveListener(EndAttack);

        _playerController.OnRolled.RemoveListener(Roll);
        _playerController.OnSprintChanged.RemoveListener(SprintChanged);
        _playerController.OnAttacked.RemoveListener(StartAttack);
    }

    private void SprintChanged(float sprint)
    {
        _animator.SetFloat("Sprint", sprint);
    }

    private void Roll()
    {
        _animator.CrossFadeInFixedTime("Roll", 0.2f);
    }

    public void AttackPeak()
    {
        OnAttackPeaked?.Invoke();
    }

    private void StartAttack(int i)
    {
        StopCoroutine(LastAttackCRT());
        _animator.CrossFadeInFixedTime("Attack_" + i.ToString(), 0.2f);
        _lastAttack = _combatCooldown;
        _animator.SetFloat("LastAttack", _lastAttack);
    }

    private void EndAttack()
    {
        StartCoroutine(LastAttackCRT());
    }

    private IEnumerator LastAttackCRT()
    {
        while (_lastAttack > 0f)
        {
            _animator.SetFloat("LastAttack", _lastAttack);
            _lastAttack -= Time.fixedDeltaTime;
            yield return null;
        }
        _lastAttack = 0f;
        _animator.SetFloat("LastAttack", _lastAttack);
        OnLeftCombatStance?.Invoke();
        yield break;
    }
}
