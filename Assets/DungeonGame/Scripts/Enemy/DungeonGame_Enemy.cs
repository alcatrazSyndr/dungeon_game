using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Enemy : MonoBehaviour
{
    [SerializeField] private Transform _playerDebug;

    private DungeonGame_NavMeshMovement _movement;

    private void Start()
    {
        _movement = transform.GetComponent<DungeonGame_NavMeshMovement>();

        StartCoroutine(BrainCRT());
    }

    private IEnumerator BrainCRT()
    {
        while (true)
        {
            _movement.MoveToDestination(_playerDebug.position - ((_playerDebug.position - transform.position).normalized));
            yield return new WaitForSeconds(0.5f);
            yield return null;
        }
    }
}
