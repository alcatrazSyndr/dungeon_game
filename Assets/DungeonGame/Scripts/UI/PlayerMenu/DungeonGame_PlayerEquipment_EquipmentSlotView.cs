using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DungeonGame_PlayerEquipment_EquipmentSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject _placeholderObject;
    [SerializeField] private Image _equipmentIcon;
    [SerializeField] private DungeonGame_Item.ItemTypes _slotType = DungeonGame_Item.ItemTypes.Null;
    [SerializeField] private float _tweenTime = 0.1f;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _contextMenu;
    [SerializeField] private DungeonGame_PlayerInventoryController _inventoryController;

    private Vector2 _originalSize = Vector2.zero;
    private Vector2 _bigSize = Vector2.zero;

    private void Start()
    {
        _originalSize = _rectTransform.sizeDelta;
        _bigSize = _originalSize * 1.3f;
    }

    private void OnEnable()
    {
        if (_originalSize != Vector2.zero)
            _rectTransform.sizeDelta = _originalSize;
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

    public void UpdateSlot(DungeonGame_PlayerInventoryController inventory)
    {
        if (inventory.PlayerEquipment.ContainsKey(_slotType) && inventory.PlayerEquipment[_slotType] != null)
        {
            _equipmentIcon.gameObject.SetActive(true);
            _placeholderObject.gameObject.SetActive(false);
            _equipmentIcon.sprite = inventory.PlayerEquipment[_slotType].ItemIcon;
        }
        else
        {
            _equipmentIcon.gameObject.SetActive(false);
            _placeholderObject.gameObject.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_placeholderObject.activeSelf) return;

        StopAllCoroutines();
        StartCoroutine(SizeTweenCRT(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_placeholderObject.activeSelf) return;

        StopAllCoroutines();
        StartCoroutine(SizeTweenCRT(false));
        ToggleContextMenu(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_placeholderObject.activeSelf) return;

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

    public void ToggleContextMenu(bool toggle)
    {
        _contextMenu.SetActive(toggle);
    }

    public void UnequipItem()
    {
        _inventoryController.EquipmentUnequiped(_slotType);
        ToggleContextMenu(false);
    }
}
