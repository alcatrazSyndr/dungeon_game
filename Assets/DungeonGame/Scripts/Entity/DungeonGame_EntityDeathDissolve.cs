using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DungeonGame_EntityHealth))]
public class DungeonGame_EntityDeathDissolve : MonoBehaviour
{
    [SerializeField] private AnimationCurve _dissolveCurve;
    [SerializeField] private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField] private float _dissolveTime = 0.5f;

    private DungeonGame_EntityHealth _health = null;
    private Animator _animator = null;

    private void OnEnable()
    {
        _health = transform.GetComponent<DungeonGame_EntityHealth>();
        _animator = transform.GetComponentInChildren<Animator>();

        _health.OnDeath.AddListener(Death);
    }

    private void OnDisable()
    {
        _health.OnDeath.RemoveListener(Death);
    }

    private void Death()
    {
        StartCoroutine(DissolveCRT());
    }

    private IEnumerator DissolveCRT()
    {
        _animator.speed = 0f;
        yield return new WaitForSeconds(0.15f);
        float interpolation = 0f;
        float timer = 0f;
        while (timer < _dissolveTime)
        {
            interpolation = timer / _dissolveTime;
            interpolation = Mathf.Clamp01(interpolation);
            SetDissolve(_dissolveCurve.Evaluate(interpolation));
            timer += Time.deltaTime * Time.timeScale;
            yield return null;
        }
        interpolation = 1f;
        SetDissolve(_dissolveCurve.Evaluate(interpolation));
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
        yield break;
    }

    private void SetDissolve(float interpolation)
    {
        if (_skinnedMeshRenderers.Count > 0)
        {
            foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderers)
            {
                Material[] mats = renderer.materials;
                foreach (Material mat in mats)
                {
                    mat.SetFloat("_Dissolve", interpolation);
                }
                renderer.materials = mats;
            }
        }

        if (_meshRenderers.Count > 0)
        {
            foreach (MeshRenderer renderer in _meshRenderers)
            {
                Material[] mats = renderer.materials;
                foreach (Material mat in mats)
                {
                    mat.SetFloat("_Dissolve", interpolation);
                }
                renderer.materials = mats;
            }
        }
    }
}
