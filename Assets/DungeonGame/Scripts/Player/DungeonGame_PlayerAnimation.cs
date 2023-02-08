using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class DungeonGame_PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private DungeonGame_PlayerInput _input;
    private Vector2 _targetMovementInput;
    private Vector2 _movementInput;

    private void OnEnable()
    {
        _animator = transform.GetComponent<Animator>();
        _input = transform.GetComponentInParent<DungeonGame_PlayerInput>();

        // Movement Listeners
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
        // Combat Listeners
        _input.OnAttackInput.AddListener(AttackInput);

        StartCoroutine(MovementInputCRT());
    }

    private void OnDisable()
    {
        // Movement Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
        // Combat Listeners
        _input.OnAttackInput.RemoveListener(AttackInput);
    }

    #region Movement

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
            _movementInput = Vector2.Lerp(_movementInput, _targetMovementInput, Time.fixedDeltaTime * 3f);
            _animator.SetFloat("InputX", _movementInput.x);
            _animator.SetFloat("InputY", _movementInput.y);
            yield return null;
        }
    }

    #endregion

    #region Combat

    private void AttackInput()
    {
        _animator.Play("Attack_" + Random.Range(1, 5));
    }

    #endregion
}
