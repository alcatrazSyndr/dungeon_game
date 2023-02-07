using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class DungeonGame_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private float _combatCooldown = 2f;
    [SerializeField] private Rig _torsoLayerRig;

    private Animator _animator;
    private DungeonGame_PlayerController _playerController;
    private DungeonGame_PlayerInput _input;
    private float _lastAttack = 0f;
    private bool _rolling = false;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _targetMovementInput = Vector2.zero;

    public UnityEvent OnRollFinished = new UnityEvent();
    public UnityEvent OnRollFinishedPostFrame = new UnityEvent();
    public UnityEvent OnAttackPeaked = new UnityEvent();
    public UnityEvent OnAttackEnded = new UnityEvent();
    public UnityEvent OnAttackPush = new UnityEvent();
    public UnityEvent OnLeftCombatStance = new UnityEvent();

    private void OnEnable()
    {
        _input = transform.GetComponentInParent<DungeonGame_PlayerInput>();
        _playerController = transform.GetComponentInParent<DungeonGame_PlayerController>();
        _animator = transform.GetComponent<Animator>();

        OnAttackEnded.AddListener(EndAttack);
        OnRollFinished.AddListener(RollEnd);

        _playerController.OnRolled.AddListener(Roll);
        _playerController.OnSprintChanged.AddListener(SprintChanged);
        _playerController.OnAttacked.AddListener(StartAttack);

        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);

        StartCoroutine(MovementInputCRT());
    }

    private void OnDisable()
    {
        OnAttackEnded.RemoveListener(EndAttack);
        OnRollFinished.RemoveListener(RollEnd);

        _playerController.OnRolled.RemoveListener(Roll);
        _playerController.OnSprintChanged.RemoveListener(SprintChanged);
        _playerController.OnAttacked.RemoveListener(StartAttack);

        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);

        StopAllCoroutines();
    }

    private void MovementInputChanged(Vector2 input)
    {
        _targetMovementInput = input;
    }

    private void MovementInputEnded()
    {
        _targetMovementInput = Vector2.zero;
    }

    private IEnumerator MovementInputCRT()
    {
        while (true)
        {
            _movementInput = Vector2.Lerp(_movementInput, _targetMovementInput, Time.fixedDeltaTime * 5f);
            _animator.SetFloat("InputX", _movementInput.x);
            _animator.SetFloat("InputY", _movementInput.y);
            yield return null;
        }
    }

    private void SprintChanged(float sprint)
    {
        if (_rolling) return;

        _animator.SetFloat("Sprint", sprint);
        _torsoLayerRig.weight = 1f - sprint;
        _animator.SetLayerWeight(1, 1f - sprint);
    }

    private void RollEnd()
    {
        StartCoroutine(AfterRollFrameCRT());
    }

    private IEnumerator AfterRollFrameCRT()
    {
        float interpolation = 0f;

        while (interpolation < 1f)
        {
            _torsoLayerRig.weight = interpolation;
            _animator.SetLayerWeight(1, interpolation);
            interpolation += Time.fixedDeltaTime;
            yield return null;
        }
        interpolation = 1f;
        yield return new WaitForEndOfFrame();
        _torsoLayerRig.weight = interpolation;
        _animator.SetLayerWeight(1, interpolation);

        _rolling = false;
        OnRollFinishedPostFrame?.Invoke();

        yield break;
    }

    private void Roll()
    {
        _rolling = true;

        _animator.CrossFadeInFixedTime("Roll", 0.2f);
        _animator.SetFloat("Sprint", 0f);
        _torsoLayerRig.weight = 0f;
        _animator.SetLayerWeight(1, 0f);
    }

    public void AttackPeak()
    {
        OnAttackPeaked?.Invoke();
    }

    public void AttackPush()
    {
        OnAttackPush?.Invoke();
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
