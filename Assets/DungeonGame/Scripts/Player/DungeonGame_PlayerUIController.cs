using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject _debugPlayerUI;

    private DungeonGame_PlayerMenuViewController _menu = null;

    private void OnEnable()
    {
        _menu = transform.GetComponentInChildren<DungeonGame_PlayerMenuViewController>();

        _menu.OnMenuToggled.AddListener(MenuInput);
    }

    private void OnDisable()
    {
        _menu.OnMenuToggled.RemoveListener(MenuInput);
    }

    private void MenuInput(bool inMenu)
    {
        _debugPlayerUI.SetActive(!inMenu);
    }
}
