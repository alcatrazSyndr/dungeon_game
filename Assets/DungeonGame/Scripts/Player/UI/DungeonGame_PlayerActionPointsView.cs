using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonGame_PlayerActionPointsView : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private TextMeshProUGUI _apText;
    [SerializeField] private Image _apCircle;
    [SerializeField] private float _apTweenTime = 0f;

    private DungeonGame_EntityActionPoints _actionPoints = null;

    private void OnEnable()
    {
        _actionPoints = transform.GetComponentInParent<DungeonGame_EntityActionPoints>();

        // AP Listeners
        _actionPoints.OnActionPointsChanged.AddListener(ActionPointsChange);
    }

    private void OnDisable()
    {
        // AP Listeners
        _actionPoints.OnActionPointsChanged.RemoveListener(ActionPointsChange);
    }

    private void ActionPointsChange()
    {
        if (_actionPoints == null) return;

        float targetFillAmount = _actionPoints.ActionPoints() / _actionPoints.MaxAP();
        if (_canvas.gameObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(ActionPointChangeCRT(targetFillAmount));
        }
        else
        {
            _apCircle.fillAmount = targetFillAmount;
            _apText.text = ReturnPercentage(_apCircle.fillAmount).ToString() + "%";
        }
    }

    private IEnumerator ActionPointChangeCRT(float target)
    {
        float interpolation = 1f;
        float currentFillAmount = _apCircle.fillAmount;
        float fillDifference = currentFillAmount - target;
        float timer = _apTweenTime;
        while (timer > 0f)
        {
            interpolation = timer / _apTweenTime;
            _apCircle.fillAmount = target + (fillDifference * interpolation);
            _apText.text = ReturnPercentage(_apCircle.fillAmount).ToString() + "%";
            timer -= Time.deltaTime;
            yield return null;
        }
        _apCircle.fillAmount = target;
        _apText.text = ReturnPercentage(_apCircle.fillAmount).ToString() + "%";
        yield break;
    }

    private int ReturnPercentage(float interpolation)
    {
        float percentage = interpolation * 100f;
        percentage = Mathf.Clamp(percentage, 0f, 100f);
        return Mathf.RoundToInt(percentage);
    }
}
