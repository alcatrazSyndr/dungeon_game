using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerController : MonoBehaviour
{
    [SerializeField] private DungeonGame_NavMeshMovement _movement;
    [SerializeField] private DungeonGame_PlayerInput _input;

    private Vector2 _movementInput = Vector2.zero;

    private void OnEnable()
    {
        _input.OnMovementInputStarted.AddListener(MovementInputStarted);
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
    }

    private void OnDisable()
    {
        _input.OnMovementInputStarted.RemoveListener(MovementInputStarted);
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
    }

    private void MovementInputStarted(Vector2 initialInput)
    {
        StopCoroutine(MovementCRT());
        _movementInput = initialInput;
        StartCoroutine(MovementCRT());
    }

    private void MovementInputChanged(Vector2 input)
    {
        _movementInput = input;
    }

    private void MovementInputEnded()
    {
        _movementInput = Vector2.zero;
        StopCoroutine(MovementCRT());
    }

    private IEnumerator MovementCRT()
    {
        while (true)
        {
            _movement.JoystickMove(_movementInput);
            yield return null;
        }
    }
}
