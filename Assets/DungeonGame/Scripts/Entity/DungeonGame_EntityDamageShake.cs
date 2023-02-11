using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DungeonGame_EntityHealth))]
public class DungeonGame_EntityDamageShake : MonoBehaviour
{
    [SerializeField] private Transform _shakingObject;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _shakeTime = 0.15f;
    [SerializeField] private float _shakeIntensity = 1f;

    private DungeonGame_EntityMaterialChanger _materialChanger = null;
    private DungeonGame_EntityHealth _health = null;
    private Vector3 _originalShakedObjectLocalPosition = Vector3.zero;

    private void OnEnable()
    {
        if (transform.GetComponent<DungeonGame_EntityMaterialChanger>())
            _materialChanger = transform.GetComponent<DungeonGame_EntityMaterialChanger>();

        _health = transform.GetComponent<DungeonGame_EntityHealth>();
        _originalShakedObjectLocalPosition = _shakingObject.localPosition;
        
        _health.OnHealthChanged.AddListener(DamageShake);
    }

    private void OnDisable()
    {
        _health.OnHealthChanged.RemoveListener(DamageShake);
    }

    private void DamageShake(float offset)
    {
        if (offset > 0f) return;
        //if (!_health.Alive()) return;

        StopAllCoroutines();
        StartCoroutine(DamageShakeCRT());
    }

    private IEnumerator DamageShakeCRT()
    {
        _animator.speed = 0f;
        float timer = 0f;
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        if (_materialChanger != null)
            _materialChanger.Hurt();

        while (timer < _shakeTime)
        {
            transform.position = position;
            transform.rotation = rotation;
            _shakingObject.localPosition = _originalShakedObjectLocalPosition + new Vector3(Random.Range(-_shakeIntensity, _shakeIntensity), Random.Range(-_shakeIntensity, _shakeIntensity), Random.Range(-_shakeIntensity, _shakeIntensity));
            timer += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        if (_materialChanger != null)
            _materialChanger.Restore();

        _shakingObject.localPosition = _originalShakedObjectLocalPosition;
        if (_health.Alive())
            _animator.speed = 1f;
        yield break;
    }
}
