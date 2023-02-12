using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonGame_PlayerMenuViewController : MonoBehaviour
{
    [SerializeField] private Transform _menuCanvas;

    private DungeonGame_PlayerInput _input = null;
    private bool _inMenu = false;

    public UnityEvent<bool> OnMenuToggled = new UnityEvent<bool>();

    private void OnEnable()
    {
        _input = transform.GetComponentInParent<DungeonGame_PlayerInput>();

        _input.OnMenuInput.AddListener(MenuInput);
    }

    private void OnDisable()
    {
        _input.OnMenuInput.RemoveListener(MenuInput);
    }

    private void Update()
    {
        if (!_inMenu) return;
        if (Camera.main == null) return;

        _menuCanvas.rotation = Quaternion.LookRotation(-(Camera.main.transform.position - _menuCanvas.position), Camera.main.transform.up);
    }

    public void MenuInput()
    {
        _inMenu = !_inMenu;
        if (_inMenu)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        _menuCanvas.gameObject.SetActive(_inMenu);
        OnMenuToggled?.Invoke(_inMenu);
    }

    public void ClickDebug(string toPrint)
    {
        print(toPrint);
    }
}
