using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_ConstantLocalRotation : MonoBehaviour
{
    [SerializeField] private RotateAxisEnum _rotateOnAxis;
    [SerializeField] private float _rotateAngle = 3f;
    private enum RotateAxisEnum { X, Y, Z }

    private void Update()
    {
        Vector3 localEuler = transform.localEulerAngles;

        if (_rotateOnAxis == RotateAxisEnum.X)
            localEuler.x += _rotateAngle;
        else if (_rotateOnAxis == RotateAxisEnum.Z)
            localEuler.z += _rotateAngle;
        else
            localEuler.y += _rotateAngle;

        transform.localEulerAngles = localEuler;
    }
}
