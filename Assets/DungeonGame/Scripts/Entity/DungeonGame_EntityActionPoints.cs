using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DungeonGame_EntityActionPoints))]
public class EntityActionPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGame_EntityActionPoints myScript = (DungeonGame_EntityActionPoints)target;
        if (GUILayout.Button("Remove 10"))
        {
            myScript.ChangeActionPoints(-10f);
        }
        if (GUILayout.Button("Remove 20"))
        {
            myScript.ChangeActionPoints(-20f);
        }
    }
}
#endif

public class DungeonGame_EntityActionPoints : MonoBehaviour
{
    [SerializeField] private float _initialAP = 100f;
    [SerializeField] private float _burnOutUntilAP = 15f;
    [SerializeField] private float _apRegainRate = 1f;
    [SerializeField] private float _regainCooldown = 0.5f;

    private float _maxActionPoints = 0f;
    public float MaxAP() { return _maxActionPoints; }
    private float _actionPoints = 0f;
    public float ActionPoints() { return _actionPoints; }
    private bool _burntOut = false;
    public bool BurntOut { get { return _burntOut; } }
    private float _currentRegainCooldown = 0f;

    public UnityEvent OnActionPointsChanged = new UnityEvent();
    public UnityEvent OnBurnOut = new UnityEvent();

    private void OnEnable()
    {
        _maxActionPoints = _initialAP;
        _actionPoints = _maxActionPoints;
        StartCoroutine(ActionPointsRegainCRT());
    }

    public void ChangeActionPoints(float offset, bool regain = false)
    {
        if (!regain)
        {
            _currentRegainCooldown = _regainCooldown;
        }

        _actionPoints += offset;
        _actionPoints = Mathf.Clamp(_actionPoints, 0f, _maxActionPoints + 1f);
        OnActionPointsChanged?.Invoke();

        if (_actionPoints <= 0f)
        {
            _actionPoints = 0f;
            _burntOut = true;

            OnBurnOut?.Invoke();
            StopCoroutine(BurnOutCRT());
            StartCoroutine(BurnOutCRT());
        }
    }

    private IEnumerator BurnOutCRT()
    {
        while (_actionPoints < _burnOutUntilAP)
        {
            yield return null;
        }
        _burntOut = false;
        yield break;
    }

    private IEnumerator ActionPointsRegainCRT()
    {
        while (true)
        {
            if (_currentRegainCooldown <= 0f)
            {
                if (_actionPoints < _maxActionPoints)
                {
                    ChangeActionPoints(_apRegainRate, true);
                }
                else if (_actionPoints != _maxActionPoints)
                {
                    _actionPoints = _maxActionPoints;
                    OnActionPointsChanged?.Invoke();
                }
            }
            else
            {
                _currentRegainCooldown -= Time.fixedUnscaledDeltaTime;
            }
            yield return null;
        }
    }
}
