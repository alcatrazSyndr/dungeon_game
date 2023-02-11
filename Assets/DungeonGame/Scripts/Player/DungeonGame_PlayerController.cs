using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerController : MonoBehaviour
{
    private DungeonGame_NavMeshMovement _movement;
    private DungeonGame_PlayerInput _input;

    private Vector2 _movementInput;

    private void Start()
    {
        // Game Manager Listeners
        if (DungeonGame_GameManager.Instance != null)
        {
            DungeonGame_GameManager.Instance.OnPlayerControllerEnabled?.Invoke(this);
        }
    }

    private void OnEnable()
    {
        _movement = transform.GetComponent<DungeonGame_NavMeshMovement>();
        _input = transform.GetComponent<DungeonGame_PlayerInput>();

        // Movement Listeners
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);

        StartCoroutine(MovementInputCRT());
    }

    private void OnDisable()
    {
        // Movement Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);

        // Game Manager Listeners
        if (DungeonGame_GameManager.Instance != null)
        {
            DungeonGame_GameManager.Instance.OnPlayerControllerDisabled?.Invoke(this);
        }

        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        // Movement Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);

        // Game Manager Listeners
        if (DungeonGame_GameManager.Instance != null)
        {
            DungeonGame_GameManager.Instance.OnPlayerControllerDisabled?.Invoke(this);
        }

        StopAllCoroutines();
    }

    #region Movement

    private void MovementInputChanged(Vector2 input)
    {
        _movementInput = input;
    }

    private void MovementInputEnded()
    {
        _movementInput = Vector2.zero;
    }

    private IEnumerator MovementInputCRT()
    {
        while (true)
        {
            _movement.Move(_movementInput, true);
            yield return null;
        }
    }

    #endregion
}
