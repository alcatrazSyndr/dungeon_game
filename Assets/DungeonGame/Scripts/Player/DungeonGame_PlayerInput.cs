using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DungeonGame_PlayerInput : MonoBehaviour
{
    private DungeonGame_PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _look;
    private InputAction _sprint;
    private InputAction _roll;
    private InputAction _attack;

    // Movement Events
    public UnityEvent<Vector2> OnMovementInputChanged = new UnityEvent<Vector2>();
    public UnityEvent OnMovementInputEnded = new UnityEvent();

    // Sprint Events
    public UnityEvent<bool> OnSprintInputChanged = new UnityEvent<bool>();

    // Roll Events
    public UnityEvent OnRollInputStarted = new UnityEvent();

    // Look Events
    public UnityEvent<Vector2> OnLookInputChanged = new UnityEvent<Vector2>();

    // Combat Events
    public UnityEvent OnAttackInput = new UnityEvent();

    private void Awake()
    {
        _playerControls = new DungeonGame_PlayerControls();
    }

    private void OnEnable()
    {
        // Movement Input
        _move = _playerControls.Player.Move;
        _move.Enable();
        _move.performed += ((InputAction.CallbackContext context) => OnMovementInputChanged?.Invoke(context.ReadValue<Vector2>()));
        _move.canceled += ((InputAction.CallbackContext context) => OnMovementInputEnded?.Invoke());

        // Sprint Input
        _sprint = _playerControls.Player.Sprint;
        _sprint.Enable();
        _sprint.started += ((InputAction.CallbackContext context) => OnSprintInputChanged?.Invoke(true));
        _sprint.canceled += ((InputAction.CallbackContext context) => OnSprintInputChanged?.Invoke(false));

        // Roll Input
        _roll = _playerControls.Player.Roll;
        _roll.Enable();
        _roll.started += ((InputAction.CallbackContext context) => OnRollInputStarted?.Invoke());

        // Look Input
        _look = _playerControls.Player.Look;
        _look.Enable();
        _look.performed += ((InputAction.CallbackContext context) => OnLookInputChanged?.Invoke(context.ReadValue<Vector2>()));

        // Combat Input
        _attack = _playerControls.Player.Fire;
        _attack.Enable();
        _attack.performed += ((InputAction.CallbackContext context) => OnAttackInput?.Invoke());
    }

    private void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _sprint.Disable();
        _roll.Disable();
        _attack.Disable();
    }
}
