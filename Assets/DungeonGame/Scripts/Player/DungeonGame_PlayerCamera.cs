using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerCamera : MonoBehaviour
{
    [SerializeField] private DungeonGame_PlayerInput _input;
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Transform _cameraOffset;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraMaxAngle = 80f;
    [SerializeField] private float _cameraMinAngle = 320f;

    private Vector3 _originalOffset = Vector3.zero;

    private void Start()
    {
        _originalOffset = _cameraRoot.localPosition;
        _cameraRoot.SetParent(null);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _cameraRoot.position = transform.position + _originalOffset;
    }

    private void LookInput(Vector2 input)
    {
        // Vertical Axis Rotation
        Vector3 rootRotation = _cameraRoot.eulerAngles;
        rootRotation.y += (input.x * 0.1f);
        _cameraRoot.eulerAngles = rootRotation;

        // Horizontal Axis Rotation
        Vector3 offsetRotation = _cameraOffset.localEulerAngles;
        offsetRotation.x += (-input.y * 0.1f);
        offsetRotation.x = ClampCameraAngle(offsetRotation.x);
        _cameraOffset.localEulerAngles = offsetRotation;
        //LOCK TO 80, 320
        print(_cameraOffset.localEulerAngles);
    }

    private float ClampCameraAngle(float wantedCameraRotation)
    {
        float angle = 0f;
        if (wantedCameraRotation < _cameraMaxAngle + 10f)
        {
            if (wantedCameraRotation > _cameraMaxAngle)
            {
                angle = _cameraMaxAngle;
            }
            else
            {
                angle = wantedCameraRotation;
            }
        }
        else if (wantedCameraRotation > _cameraMinAngle - 10f)
        {
            if (wantedCameraRotation < _cameraMinAngle)
            {
                angle = _cameraMinAngle;
            }
            else
            {
                angle = wantedCameraRotation;
            }
        }

        return angle;
    }

    private void OnEnable()
    {
        _input.OnLookInputChanged.AddListener(LookInput);
    }

    private void OnDisable()
    {
        _input.OnLookInputChanged.RemoveListener(LookInput);
    }
}
