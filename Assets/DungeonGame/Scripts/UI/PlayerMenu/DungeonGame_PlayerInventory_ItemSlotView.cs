using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DungeonGame_PlayerInventory_ItemSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private float _tweenTime = 0.1f;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _contextMenu;

    private DungeonGame_PlayerInventoryController _inventoryController;
    private DungeonGame_Item _myItem = null;
    private Vector2 _originalSize = Vector2.zero;
    private Vector2 _bigSize = Vector2.zero;

    public void Initialize(DungeonGame_Item newItem, DungeonGame_PlayerInventoryController inventoryController)
    {
        _originalSize = _rectTransform.sizeDelta;
        _bigSize = _originalSize * 1.3f;

        _icon.sprite = newItem.ItemIcon;
        _icon.gameObject.SetActive(true);

        _myItem = newItem;
        _inventoryController = inventoryController;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ToggleContextMenu(false);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ToggleContextMenu(false);
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
        ToggleContextMenu(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ToggleContextMenu(true);
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

    public void ItemUsed()
    {
        if (_myItem == null || _inventoryController == null) return;

        _inventoryController.ItemUsed(_myItem);
    }

    public void ToggleContextMenu(bool toggle)
    {
        _contextMenu.SetActive(toggle);
    }
}
