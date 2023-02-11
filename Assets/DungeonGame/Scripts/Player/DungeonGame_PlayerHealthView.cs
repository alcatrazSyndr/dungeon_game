using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGame_PlayerHealthView : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private Image _healthCircle;
    [SerializeField] private bool _localPlayerDebug = false;

    private DungeonGame_EntityHealth _health = null;

    private void Update()
    {
        if (!_canvas.gameObject.activeSelf) return;
        if (_health == null) return;
        if (Camera.main == null) return;

        _canvas.LookAt(Camera.main.transform);
    }

    private void OnEnable()
    {
        _health = transform.GetComponentInParent<DungeonGame_EntityHealth>();

        if (_localPlayerDebug)
            _canvas.gameObject.SetActive(true);

        // Health Listeners
        _health.OnHealthChanged.AddListener(HealthChange);
        _health.OnDeath.AddListener(Death);
    }

    private void OnDisable()
    {
        // Health Listeners
        _health.OnHealthChanged.RemoveListener(HealthChange);
        _health.OnDeath.RemoveListener(Death);
    }

    private void HealthChange(float offset)
    {
        if (_health == null) return;

        if (_health.Alive() && !_canvas.gameObject.activeSelf && !_localPlayerDebug)
            _canvas.gameObject.SetActive(true);

        _healthCircle.fillAmount = _health.Health() / _health.MaxHealth();
    }

    private void Death()
    {
        _canvas.gameObject.SetActive(false);
    }
}
