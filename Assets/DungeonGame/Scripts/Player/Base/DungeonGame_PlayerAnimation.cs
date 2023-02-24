using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class DungeonGame_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private int _maxAttackCombo = 2;

    private Animator _animator = null;
    private DungeonGame_PlayerInput _input = null;
    private DungeonGame_AnimatorHandler _animatorHandler = null;
    private DungeonGame_PlayerMenuViewController _menuView = null;
    private Vector2 _targetMovementInput;
    private Vector2 _movementInput;
    private int _attack = 1;
    private bool _inAttack = false;
    private bool _actionBlock = false;

    private void OnEnable()
    {
        _animator = transform.GetComponent<Animator>();
        _input = transform.GetComponentInParent<DungeonGame_PlayerInput>();
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _menuView = transform.parent.GetComponentInChildren<DungeonGame_PlayerMenuViewController>();

        // Movement Listeners
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
        // Combat Listeners
        _input.OnAttackInput.AddListener(AttackInput);
        // Animator Listeners
        _animatorHandler.OnAttackEnd.AddListener(AttackEnd);
        // Menu Listeners
        _menuView.OnMenuToggled.AddListener(ToggleActionInput);

        StartCoroutine(MovementInputCRT());
    }

    private void OnDisable()
    {
        // Movement Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
        // Combat Listeners
        _input.OnAttackInput.RemoveListener(AttackInput);
        // Animator Listeners
        _animatorHandler.OnAttackEnd.RemoveListener(AttackEnd);
        // Menu Listeners
        _menuView.OnMenuToggled.RemoveListener(ToggleActionInput);
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
        if (_inAttack || _actionBlock) return;

        _inAttack = true;
        _animator.Play("Attack_" + _attack.ToString());
        _attack++;
        if (_attack > _maxAttackCombo)
            _attack = 1;
    }

    private void AttackEnd()
    {
        _inAttack = false;
    }

    public void SetWeaponAnimationValue(float value)
    {
        _animator.SetFloat("WeaponType", value);
    }

    #endregion

    private void ToggleActionInput(bool toggle)
    {
        _actionBlock = toggle;
    }
}
