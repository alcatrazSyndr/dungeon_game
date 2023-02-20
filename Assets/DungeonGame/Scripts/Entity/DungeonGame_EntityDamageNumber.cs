using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonGame_EntityDamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private float _existTimerBeforeInvisibility = 1f;
    [SerializeField] private float _floatSpeed = 1f;
    [SerializeField] private float _invisibilityTimer = 1f;
    [SerializeField] private AnimationCurve _disappearCurve;


    public void Initialize(float damage)
    {
        _damageText.text = damage.ToString();
        StartCoroutine(FloatCRT());
        StartCoroutine(DisappearCRT());
    }

    private IEnumerator FloatCRT()
    {
        while (true)
        {
            Vector2 pos = transform.localPosition;
            pos.y += _floatSpeed * Time.deltaTime * Time.timeScale;
            transform.localPosition = pos;
            yield return null;
        }
    }

    private IEnumerator DisappearCRT()
    {
        yield return new WaitForSeconds(_existTimerBeforeInvisibility);
        float timer = 0f;
        float interpolation = 0f;
        while (timer < _invisibilityTimer)
        {
            interpolation = Mathf.Clamp01(timer / _invisibilityTimer);
            Color color = _damageText.color;
            color.a = _disappearCurve.Evaluate(interpolation);
            _damageText.color = color;
            timer += Time.deltaTime * Time.timeScale;
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
