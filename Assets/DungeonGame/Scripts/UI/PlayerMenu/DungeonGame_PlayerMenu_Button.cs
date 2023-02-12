using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DungeonGame_PlayerMenu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _tweenTime = 0.1f;

    private Vector2 _originalSize = Vector2.zero;
    private Vector2 _bigSize = Vector2.zero;
    private RectTransform _rectTransform = null;

    private void Start()
    {
        _rectTransform = transform.GetComponent<RectTransform>();
        _originalSize = _rectTransform.sizeDelta;
        _bigSize = _originalSize * 1.1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(SizeTweenCRT(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(SizeTweenCRT(false));
    }

    private IEnumerator SizeTweenCRT(bool big)
    {
        float timer = 0f;
        float interpolation = 0f;
        Vector2 startSize = _rectTransform.sizeDelta;
        Vector2 targetSize = _originalSize;
        if (big)
            targetSize = _bigSize;
        while (timer < _tweenTime)
        {
            interpolation = timer / _tweenTime;
            _rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, interpolation);
            timer += Time.deltaTime * Time.timeScale;
            yield return null;
        }
        interpolation = 1f;
        _rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, interpolation);
        yield break;
    }

    public void ResetSize()
    {
        _rectTransform.sizeDelta = _originalSize;
    }
}
