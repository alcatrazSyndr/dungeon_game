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

    // Movement Events
    public UnityEvent<Vector2> OnMovementInputStarted = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnMovementInputChanged = new UnityEvent<Vector2>();
    public UnityEvent OnMovementInputEnded = new UnityEvent();

    // Look Events
    public UnityEvent<Vector2> OnLookInputChanged = new UnityEvent<Vector2>();

    private void Awake()
    {
        _playerControls = new DungeonGame_PlayerControls();
    }

    private void OnEnable()
    {
        // Movement Input
        _move = _playerControls.Player.Move;
        _move.Enable();
        _move.started += ((InputAction.CallbackContext context) => OnMovementInputStarted?.Invoke(context.ReadValue<Vector2>()));
        _move.performed += ((InputAction.CallbackContext context) => OnMovementInputChanged?.Invoke(context.ReadValue<Vector2>()));
        _move.canceled += ((InputAction.CallbackContext context) => OnMovementInputEnded?.Invoke());

        // Look Input
        _look = _playerControls.Player.Look;
        _look.Enable();
        _look.performed += ((InputAction.CallbackContext context) => OnLookInputChanged?.Invoke(context.ReadValue<Vector2>()));
    }

    private void OnDisable()
    {
        _move.Disable();
        _look.Disable();
    }
}
