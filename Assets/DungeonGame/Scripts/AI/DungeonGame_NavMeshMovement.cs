using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DungeonGame_NavMeshMovement : MonoBehaviour
{
    [SerializeField] private bool _lookMove = true;
    [SerializeField] private Transform _lookAtPoint;
    [SerializeField] private Transform _cameraTransform;

    private Transform _myTransform = null;
    private NavMeshAgent _agent = null;
    private NavMeshPath _path = null;
    private bool _isPathing = false;
    private DungeonGame_AnimatorHandler _animatorHandler = null;
    public bool IsPathing { get => _isPathing; }
    public NavMeshAgent Agent { get => _agent; }

    private Vector3 _inputVector = Vector3.zero;
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _moveVectorAccelerationTracker = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;
    private Vector3 _velocity = Vector3.zero;
    public Vector3 MoveVector { get => _moveVector; }
    public Vector3 Velocity { get => _velocity; }

    [Header("Movement")]
    [SerializeField, Range(0f, 0.2f)] private float _accelerationDamp = 0.1f;
    [SerializeField, Min(0f)] private float _moveSpeed = 7f;
    [SerializeField, Min(0f)] private float _defaultAgentStoppingDistance = 0.025f;
    [Header("Turning")]
    [SerializeField, Min(0f)] private float _turnSpeed = 9f;
    [SerializeField, Range(0f, 180f)] private float _maxForwardAngle = 80f;
    [SerializeField, Range(0f, 1f)] private float _wideTurnAppliedSpeed = 0.1f;
#if UNITY_EDITOR
    [Header("Debug (EditorOnly)")]
    public bool DebugGizmos = false;
#endif

    // Callbacks:
    public delegate void OnPathingStartedDelegate(Vector3 destination);
    public event OnPathingStartedDelegate OnPathingStarted = null;
    public delegate void OnPathingStoppedDelegate(bool hasReachedDestination);
    public event OnPathingStoppedDelegate OnPathingStopped = null;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _myTransform = transform;
        _targetRotation = _myTransform.rotation;
        _path = new NavMeshPath();
        _animatorHandler = GetComponentInChildren<DungeonGame_AnimatorHandler>(false);
        _animatorHandler.Initialize(this, OnPostAnimatorMoveEventHandler);
        // Setup agent:
        _agent.updatePosition = true;
        _agent.updateRotation = false;
        //_agent.agentTypeID = 
        _agent.baseOffset = 0f;
        _agent.speed = 0f;
        _agent.angularSpeed = 0f;
        _agent.acceleration = _accelerationDamp == 0f ? 9999999f : RemapValue(_accelerationDamp, 0f, 0.2f, 128f, 32f); // UNDONE: Agent acceleration values may need fine-tunning
        _agent.stoppingDistance = _defaultAgentStoppingDistance;
        _agent.autoBraking = true;
        //_agent.radius = character controller collider radius + 0.035f;
        //_agent.height = character controller collider height;
        //_agent.obstacleAvoidanceType =
        //_agent.avoidancePriority = 50;
        _agent.autoTraverseOffMeshLink = true;
        _agent.autoRepath = true;
    }

    public void SetMovementSpeed(float speed)
    {
        _moveSpeed = speed;
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }

    public bool CanReachPosition(Vector3 position)
    {
        NavMeshPath checkPath = new NavMeshPath();
        if (_agent.enabled && _agent.CalculatePath(position, checkPath) && checkPath.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Input
    public bool MoveToDestination(Vector3 destination) => MoveToDestination(destination, _defaultAgentStoppingDistance);
    public bool MoveToDestination(Vector3 destination, float stoppingDistance)
    {
        if (Vector3.Distance(_myTransform.position, destination) >= stoppingDistance)
        {
            if (_inputVector == Vector3.zero && _agent.enabled && _agent.CalculatePath(destination, _path))
                if (_agent.SetPath(_path))
                {
                    _isPathing = true;
                    _agent.stoppingDistance = stoppingDistance;
                    OnPathingStarted?.Invoke(destination);
                    return true;
                }
            StopAgent_Do(false);
        }
        return false;
    }

    public void JoystickMove(Vector2 joystickDeltaOffset, bool lookMove) => Move(joystickDeltaOffset, lookMove);

    public void Move(Vector2 inputVector, bool lookMove)
    {
        _lookMove = lookMove;
        inputVector.Normalize();
        this._inputVector.x = inputVector.x;
        this._inputVector.y = 0f;
        this._inputVector.z = inputVector.y;
        if (this._inputVector != Vector3.zero && _isPathing)
            StopAgent_Do(false);
    }

    public void RotateTowards(Transform target)
    {
        Vector3 dir = new Vector3(target.position.x, _myTransform.position.y, target.position.z) - transform.position;
        _targetRotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    public void StopAgent() => StopAgent_Do(false);
    private void StopAgent_Do(bool hasReachedDestination)
    {
        if (_agent.enabled)
            _agent.ResetPath();

        if (_isPathing)
        {
            _isPathing = false;
            OnPathingStopped?.Invoke(hasReachedDestination);
        }
    }

    private Vector3 DetermineDirectionVectorFromInputVector()
    {
        if (_cameraTransform != null)
            return (_inputVector.x * _cameraTransform.right) + (_inputVector.z * Vector3.Scale(_cameraTransform.forward, new Vector3(1f, 0f, 1f)).normalized);
        return _inputVector;
    }

    private Vector3 DetermineDirectionVectorFromAgentPath()
    {
        Vector3 directionVector = (_agent.steeringTarget - _myTransform.position).normalized;
        directionVector.y = 0f;
        return directionVector;
    }
    #endregion

    #region Phase 1: Update
    private void Update()
    {
        float deltaTime = Time.deltaTime;

        Vector3 directionVector = _isPathing ? DetermineDirectionVectorFromAgentPath() : DetermineDirectionVectorFromInputVector();
        if (_lookMove)
            _targetRotation = TargetRotationFromDirectionHelper(_targetRotation, (_lookAtPoint.position - _myTransform.position).normalized);
        else
            _targetRotation = TargetRotationFromDirectionHelper(_targetRotation, directionVector);

#if UNITY_EDITOR 
        if (DebugGizmos)
        {
            Debug.DrawRay(_myTransform.position, _myTransform.forward, Color.blue); // Visualize current transform rotation
            Debug.DrawRay(_myTransform.position, Vector3.Scale(_velocity, new Vector3(1f, 0f, 1f)), Color.red); // Visualize *horizontal* velocity
            Debug.DrawRay(_myTransform.position, directionVector, Color.magenta); // Visualize directionVector
            if (_isPathing) // Visualize agent path
            {
                Color pathColor = _agent.pathStatus == NavMeshPathStatus.PathComplete ? Color.green : Color.yellow;
                pathColor.a = 0.2f;
                for (int i = 0, cnt = _agent.path.corners.Length; i < cnt; i++)
                {
                    if (i + 1 < cnt)
                        Debug.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1], pathColor);
                    Debug.DrawRay(_agent.path.corners[i], Vector3.up * 0.5f, pathColor);
                }
            }
        }
#endif

        _moveVector = CalculateMoveVector(_moveVector, directionVector, deltaTime);
        _animatorHandler.UpdateAnimatorParameters(_myTransform.InverseTransformDirection(_velocity), _myTransform.InverseTransformDirection(_targetRotation * Vector3.forward), deltaTime);
    }

    private Vector3 CalculateMoveVector(Vector3 moveVector, Vector3 directionVector, float deltaTime)
    {
        // Scale targetMoveVector in local space:
        Vector3 targetMoveVector = InverseTargetRotationDirection(directionVector);
        targetMoveVector.Scale(new Vector3(_moveSpeed, 0f, _moveSpeed));
        targetMoveVector = TargetRotationDirection(targetMoveVector);

        // Turn angle movement limiter:
        if (Quaternion.Angle(_myTransform.rotation, _targetRotation) > _maxForwardAngle)
            targetMoveVector *= _wideTurnAppliedSpeed;

        // (De)Accelerate to targetMoveVector and return:
        float y = moveVector.y;
        moveVector = Vector3.SmoothDamp(moveVector, targetMoveVector, ref _moveVectorAccelerationTracker, _accelerationDamp);
        moveVector.y = y;
        return moveVector;
    }
    #endregion

    #region Phase 2: OnPostAnimatorMoveEventHandler
    private void OnPostAnimatorMoveEventHandler(Vector3 motionVector, float deltaTime)
    {
#if UNITY_EDITOR
        if (DebugGizmos)
        {
            Debug.DrawRay(_myTransform.position, _targetRotation * Vector3.forward, new Color(0f, 0f, 1f, 0.2f)); // Visualize targetRotation (blue transparent)
            Debug.DrawRay(_myTransform.position, Vector3.Scale(motionVector, new Vector3(1f, 0f, 1f)), new Color(1f, 0f, 0f, 0.2f)); // Visualize *horizontal* motionVector (red transparent)
        }
#endif
        _myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, _targetRotation, _turnSpeed * deltaTime); // Update rotation
        if (_isPathing)
        {
            _agent.speed = motionVector.magnitude;
            if (_agent.enabled && _agent.remainingDistance <= _agent.stoppingDistance)
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    StopAgent_Do(true); // Agent has reached destination!
                    motionVector = new Vector3(0f, motionVector.y, 0f); // Fix for overshooting destination due to acceleration
                }
        }
        else
            _agent.velocity = motionVector; // Update position via agent
        _velocity = _agent.velocity; // Store velocity
    }
    #endregion

    #region Warp
    /// <summary>Instantly warps the character to a new position. Returns true if successful.</summary>
    public bool Warp(Vector3 position) => Warp(position, _targetRotation);

    /// <summary>Instantly warps the character to a new position and rotation. Returns true if successful.</summary>
    public bool Warp(Vector3 position, Quaternion rotation)
    {
        StopAgent_Do(false);
        _moveVector = new Vector3(0f, _moveVector.y, 0f); // Fix for overshooting destination due to acceleration
        if (_agent.Warp(position))
        {
            _myTransform.rotation = _targetRotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
            return true;
        }
        return false;
    }
    #endregion

    #region Helpers
    /// <summary>Transforms direction from local space to world space. Similar to transform.TransformDirection(), but uses the TargetRotation instead of current transform.rotation.</summary>
    public Vector3 TargetRotationDirection(Vector3 direction) => _targetRotation * direction;

    /// <summary>Transforms direction from world space to local space. Similar to transform.InverseTransformDirection(), but uses the TargetRotation instead of current transform.rotation.</summary>
    public Vector3 InverseTargetRotationDirection(Vector3 direction) => Quaternion.Inverse(_targetRotation) * direction;

    private Quaternion TargetRotationFromDirectionHelper(Quaternion currentTargetRotation, Vector3 direction)
    {
        direction.y = 0f;
        if (direction != Vector3.zero)
            currentTargetRotation = Quaternion.LookRotation(direction);
        return currentTargetRotation;
    }

    public float RemapValue(float value, float sourceMin, float sourceMax, float targetMin, float targetMax) => (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
    #endregion
}
