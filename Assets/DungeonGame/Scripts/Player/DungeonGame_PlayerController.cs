using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerController : MonoBehaviour
{
    [SerializeField] private float _sprintMoveSpeedModifier = 5f;
    [SerializeField] private float _defaultMoveSpeed = 7f;
    [SerializeField] private DungeonGame_NavMeshMovement _movement;
    [SerializeField] private DungeonGame_PlayerInput _input;

    public UnityEvent<float> OnSprintChanged = new UnityEvent<float>();

    private Vector2 _movementInput = Vector2.zero;
    private float _sprint = 0f;

    private void OnEnable()
    {
        _movement.SetMovementSpeed(_defaultMoveSpeed);

        OnSprintChanged.AddListener(SprintChanged);

        _input.OnMovementInputStarted.AddListener(MovementInputStarted);
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
        _input.OnSprintInputChanged.AddListener(SprintInputChanged);
    }

    private void OnDisable()
    {
        OnSprintChanged.RemoveListener(SprintChanged);

        _input.OnMovementInputStarted.RemoveListener(MovementInputStarted);
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
        _input.OnSprintInputChanged.RemoveListener(SprintInputChanged);
    }

    #region Sprint

    private void SprintInputChanged(bool sprinting)
    {
        StopCoroutine(SprintOnCRT());
        StopCoroutine(SprintOffCRT());

        if (sprinting)
            StartCoroutine(SprintOnCRT());
        else
            StartCoroutine(SprintOffCRT());
    }

    private IEnumerator SprintOnCRT()
    {
        while (_sprint < 1f)
        {
            _sprint += Time.fixedDeltaTime;
            OnSprintChanged?.Invoke(_sprint);
            yield return null;
        }
        _sprint = 1f;
        OnSprintChanged?.Invoke(_sprint);
        yield break;
    }

    private IEnumerator SprintOffCRT()
    {
        while (_sprint > 0f)
        {
            _sprint -= Time.fixedDeltaTime;
            OnSprintChanged?.Invoke(_sprint);
            yield return null;
        }
        _sprint = 0f;
        OnSprintChanged?.Invoke(_sprint);
        yield break;
    }

    private void SprintChanged(float sprint)
    {
        float newSpeed = _defaultMoveSpeed + (_sprintMoveSpeedModifier * sprint);
        _movement.SetMovementSpeed(newSpeed);
    }

    #endregion

    #region Movement

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

    #endregion
}
