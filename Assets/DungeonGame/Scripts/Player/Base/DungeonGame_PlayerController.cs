using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _debugUI;

    private DungeonGame_NavMeshMovement _movement;
    private DungeonGame_PlayerInput _input;
    private DungeonGame_EntityHealth _health;

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
        _health = transform.GetComponent<DungeonGame_EntityHealth>();

        // Movement Listeners
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
        // Health Listeners
        _health.OnDeath.AddListener(PlayerDeath);

        StartCoroutine(MovementInputCRT());
    }

    private void OnDisable()
    {
        // Movement Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
        // Health Listeners
        _health.OnDeath.RemoveListener(PlayerDeath);
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

    private void PlayerDeath()
    {
        DungeonGame_GameManager.Instance.OnPlayerControllerDisabled?.Invoke(this);

        _debugUI.SetActive(false);
        Destroy(gameObject);
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
