using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class DungeonGame_AnimatorHandler : MonoBehaviour
{
    private Animator _animator = null;
    private bool _rootMotion = false;
    private System.Action<Vector3, float> _onPostAnimatorMoveCallback = null;
    private DungeonGame_NavMeshMovement _controller = null;
    private bool _isInitialized = false;

    public UnityEvent OnAttackEnd = new UnityEvent();
    public UnityEvent OnAttackPeak = new UnityEvent();
    public UnityEvent OnFootstep = new UnityEvent();

    #region Initialization
    public void Initialize(DungeonGame_NavMeshMovement controller, System.Action<Vector3, float> onPostAnimatorMoveCallback)
    {
        if (!_isInitialized)
        {
            if (_animator == null)
                _animator = GetComponent<Animator>();
            _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            _isInitialized = true;
            this._controller = controller;
            this._onPostAnimatorMoveCallback = onPostAnimatorMoveCallback;
        }
    }

    public void Deinitialize()
    {
        if (_isInitialized)
        {
            _isInitialized = false;
            this._controller = null;
            this._onPostAnimatorMoveCallback = null;
        }
    }
    #endregion

    #region Update, OnAnimatorMove
    public void UpdateAnimatorParameters(Vector3 localVelocity, Vector3 localDirection, float deltaTime)
    {
        _animator.SetFloat("Speed", localVelocity.magnitude, 0.01f, deltaTime);
    }

    private void OnAnimatorMove()
    {
        if (_isInitialized)
        {
            float deltaTime = Time.deltaTime; // Cache deltaTime
            Vector3 motionVector = _controller.MoveVector;
            if (_rootMotion)
                motionVector = _animator.deltaPosition / deltaTime;
            motionVector.y = 0f;
            _onPostAnimatorMoveCallback.Invoke(motionVector, deltaTime);
        }
    }
    #endregion

    public void AttackPeak()
    {
        OnAttackPeak?.Invoke();
    }

    public void AttackEnd()
    {
        OnAttackEnd?.Invoke();
    }

    public void Footstep()
    {
        OnFootstep?.Invoke();
    }
}
