using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerController : MonoBehaviour
{
    [SerializeField] private int _maxAttackCombo = 3;
    [SerializeField] private float _sprintMoveSpeedModifier = 5f;
    [SerializeField] private float _defaultMoveSpeed = 7f;

    public UnityEvent<float> OnSprintChanged = new UnityEvent<float>();
    public UnityEvent OnRolled = new UnityEvent();
    public UnityEvent<int> OnAttacked = new UnityEvent<int>();

    private DungeonGame_PlayerAnimation _animation;
    private DungeonGame_NavMeshMovement _movement;
    private DungeonGame_PlayerInput _input;
    private Vector2 _movementInput = Vector2.zero;
    private float _sprint = 0f;
    private int _attackNumber = 1;
    private bool _rolling = false;
    private bool _attacking = false;

    private void OnEnable()
    {
        _animation = transform.GetComponentInChildren<DungeonGame_PlayerAnimation>();
        _movement = transform.GetComponent<DungeonGame_NavMeshMovement>();
        _input = transform.GetComponent<DungeonGame_PlayerInput>();

        _movement.SetMovementSpeed(_defaultMoveSpeed);

        OnSprintChanged.AddListener(SprintChanged);

        // Input Listeners
        _input.OnMovementInputChanged.AddListener(MovementInputChanged);
        _input.OnMovementInputEnded.AddListener(MovementInputEnded);
        _input.OnSprintInputChanged.AddListener(SprintInputChanged);
        _input.OnRollInputStarted.AddListener(RollInput);
        _input.OnAttackInput.AddListener(AttackInput);

        // Animation Listeners
        _animation.OnRollFinished.AddListener(RollEnd);
        _animation.OnAttackPeaked.AddListener(AttackPeak);
        _animation.OnAttackEnded.AddListener(AttackEnd);

        StartCoroutine(MovementCRT());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        OnSprintChanged.RemoveListener(SprintChanged);

        // Input Listeners
        _input.OnMovementInputChanged.RemoveListener(MovementInputChanged);
        _input.OnMovementInputEnded.RemoveListener(MovementInputEnded);
        _input.OnSprintInputChanged.RemoveListener(SprintInputChanged);
        _input.OnRollInputStarted.RemoveListener(RollInput);
        _input.OnAttackInput.RemoveListener(AttackInput);

        // Animation Listeners
        _animation.OnRollFinished.RemoveListener(RollEnd);
        _animation.OnAttackPeaked.RemoveListener(AttackPeak);
        _animation.OnAttackEnded.RemoveListener(AttackEnd);
    }

    private bool InAction()
    {
        if (_rolling || _attacking)
            return true;
        else
            return false;
    }

    #region Combat

    private void AttackInput()
    {
        if (InAction()) return;

        _attacking = true;
        OnAttacked?.Invoke(_attackNumber);
        _attackNumber++;
        if (_attackNumber > _maxAttackCombo)
            _attackNumber = 1;
    }

    private void AttackPeak()
    {
        print("peak");
    }

    private void AttackEnd()
    {
        _attacking = false;
    }

    #endregion

    #region Roll

    private void RollInput()
    {
        if (_rolling) return;

        _rolling = true;
        _attacking = false;

        if (_sprint > 0f)
            StartCoroutine(SprintOffCRT());

        StartCoroutine(RollCRT());

        OnRolled?.Invoke();
    }

    private IEnumerator RollCRT()
    {
        while (_rolling)
        {
            if (_movementInput == Vector2.zero)
                _movement.JoystickMove(new Vector2(0f, 1f));
            else
                _movement.JoystickMove(_movementInput);

            yield return null;
        }
        yield break;
    }

    private void RollEnd()
    {
        _rolling = false;
    }

    #endregion

    #region Sprint

    private void SprintInputChanged(bool sprinting)
    {
        if (InAction()) return;
        if (_movementInput == Vector2.zero) return;

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

    private void MovementInputChanged(Vector2 input)
    {
        _movementInput = input;
    }

    private void MovementInputEnded()
    {
        _movementInput = Vector2.zero;
    }

    private IEnumerator MovementCRT()
    {
        while (true)
        {
            if (!InAction())
                _movement.JoystickMove(_movementInput);
            else if (_attacking)
                _movement.JoystickMove(Vector2.zero);
            yield return null;
        }
    }

    #endregion
}
