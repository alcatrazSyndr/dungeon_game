using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGame_EntityHealthView : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private bool _localPlayerDebug = false;
    [SerializeField] private float _healthTweenTime = 0.1f;

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

        if (!_localPlayerDebug)
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

        float targetFillAmount = _health.Health() / _health.MaxHealth();
        if (_canvas.gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(HealthChangeCRT(targetFillAmount));
        }
        else
        {
            _healthSlider.value = targetFillAmount;
        }
    }

    private IEnumerator HealthChangeCRT(float target)
    {
        float interpolation = 1f;
        float currentFillAmount = _healthSlider.value;
        float fillDifference = currentFillAmount - target;
        float timer = _healthTweenTime;
        while (timer > 0f)
        {
            interpolation = timer / _healthTweenTime;
            _healthSlider.value = target + (fillDifference * interpolation);
            timer -= Time.deltaTime;
            yield return null;
        }
        _healthSlider.value = target;
        yield break;
    }

    private void Death()
    {
        _canvas.gameObject.SetActive(false);
    }
}
