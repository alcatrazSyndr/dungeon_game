using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_GameManager : MonoBehaviour
{
    public static DungeonGame_GameManager Instance { get; private set; } = null;

    private List<DungeonGame_PlayerController> _playerControllers = new List<DungeonGame_PlayerController>();
    public List<DungeonGame_PlayerController> PlayerControllers { get { return _playerControllers; } }

    public UnityEvent<DungeonGame_PlayerController> OnPlayerControllerEnabled = new UnityEvent<DungeonGame_PlayerController>();
    public UnityEvent<DungeonGame_PlayerController> OnPlayerControllerDisabled = new UnityEvent<DungeonGame_PlayerController>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            Instance = this;
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        OnPlayerControllerEnabled.AddListener(PlayerControllerEnabled);
        OnPlayerControllerDisabled.AddListener(PlayerControllerDisabled);
    }

    private void OnDestroy()
    {
        OnPlayerControllerEnabled.RemoveListener(PlayerControllerEnabled);
        OnPlayerControllerDisabled.RemoveListener(PlayerControllerDisabled);

        Instance = null;
    }

    private void PlayerControllerEnabled(DungeonGame_PlayerController player)
    {
        if (!_playerControllers.Contains(player))
        {
            _playerControllers.Add(player);
        }
    }

    private void PlayerControllerDisabled(DungeonGame_PlayerController player)
    {
        if (_playerControllers.Contains(player))
        {
            _playerControllers.Remove(player);
        }
    }
}
