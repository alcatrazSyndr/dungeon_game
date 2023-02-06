using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private DungeonGame_PlayerController _playerController;

    private void OnEnable()
    {
        _playerController.OnSprintChanged.AddListener(SprintChanged);
    }

    private void OnDisable()
    {
        _playerController.OnSprintChanged.RemoveListener(SprintChanged);
    }

    private void SprintChanged(float sprint)
    {
        _animator.SetFloat("Sprint", sprint);
    }
}
