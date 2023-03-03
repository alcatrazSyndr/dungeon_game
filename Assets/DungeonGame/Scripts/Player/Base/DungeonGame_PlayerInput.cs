using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DungeonGame_PlayerInput))]
public class DungeonGame_PlayerInputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGame_PlayerInput myScript = (DungeonGame_PlayerInput)target;
        if (GUILayout.Button("RMB Toggle On"))
        {
            myScript.Debug_RightClickToggle(true);
        }
        if (GUILayout.Button("RMB Toggle Off"))
        {
            myScript.Debug_RightClickToggle(false);
        }
    }
}
#endif

public class DungeonGame_PlayerInput : MonoBehaviour
{
    private DungeonGame_PlayerControls _playerControls;
    private InputAction _move;
    private InputAction _look;
    private InputAction _sprint;
    private InputAction _roll;
    private InputAction _attack;
    private InputAction _menu;
    private InputAction _secondaryAttack;

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
    public UnityEvent<bool> OnSecondaryAttackInputChanged = new UnityEvent<bool>();

    // Menu Events
    public UnityEvent OnMenuInput = new UnityEvent();

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
        _secondaryAttack = _playerControls.Player.SecondaryFire;
        _secondaryAttack.Enable();
        _secondaryAttack.started += ((InputAction.CallbackContext context) => OnSecondaryAttackInputChanged?.Invoke(true));
        _secondaryAttack.canceled += ((InputAction.CallbackContext context) => OnSecondaryAttackInputChanged?.Invoke(false));

        // Menu Input
        _menu = _playerControls.Player.ToggleMenu;
        _menu.Enable();
        _menu.performed += ((InputAction.CallbackContext context) => OnMenuInput?.Invoke());
    }

    private void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _sprint.Disable();
        _roll.Disable();
        _attack.Disable();
        _secondaryAttack.Disable();
        _menu.Disable();
    }

    public void Debug_RightClickToggle(bool toggle)
    {
        OnSecondaryAttackInputChanged?.Invoke(toggle);
    }
}
