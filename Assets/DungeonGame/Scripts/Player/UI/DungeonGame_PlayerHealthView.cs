using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonGame_PlayerHealthView : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private Image _healthCircle;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private bool _localPlayerDebug = false;
    [SerializeField] private float _healthTweenTime = 0f;

    private DungeonGame_EntityHealth _health = null;

    private void Update()
    {
        if (!_canvas.gameObject.activeSelf) return;
        if (_health == null) return;
        if (Camera.main == null) return;

        Quaternion targetRot = Quaternion.LookRotation(-(Camera.main.transform.position - _canvas.position), Camera.main.transform.up);
        _canvas.rotation = Quaternion.Lerp(_canvas.rotation, targetRot, Time.deltaTime * 20f);
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

        float targetFillAmount = _health.Health() / _health.MaxHealth();
        if (_canvas.gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(HealthChangeCRT(targetFillAmount));
        }
        else
        {
            _healthCircle.fillAmount = targetFillAmount;
            _healthText.text = ReturnPercentage(_healthCircle.fillAmount).ToString() + "%";
        }
    }

    private IEnumerator HealthChangeCRT(float target)
    {
        float interpolation = 1f;
        float currentFillAmount = _healthCircle.fillAmount;
        float fillDifference = currentFillAmount - target;
        float timer = _healthTweenTime;
        while (timer > 0f)
        {
            interpolation = timer / _healthTweenTime;
            _healthCircle.fillAmount = target + (fillDifference * interpolation);
            _healthText.text = ReturnPercentage(_healthCircle.fillAmount).ToString() + "%";
            timer -= Time.deltaTime;
            yield return null;
        }
        _healthCircle.fillAmount = target;
        _healthText.text = ReturnPercentage(_healthCircle.fillAmount).ToString() + "%";
        yield break;
    }

    private int ReturnPercentage(float interpolation)
    {
        float percentage = interpolation * 100f;
        percentage = Mathf.Clamp(percentage, 0f, 100f);
        return Mathf.RoundToInt(percentage);
    }

    private void Death()
    {
        _canvas.gameObject.SetActive(false);
    }
}
