using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(DungeonGame_EntityHealth))]
public class EntityHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonGame_EntityHealth myScript = (DungeonGame_EntityHealth)target;
        if (GUILayout.Button("Heal 10"))
        {
            myScript.HealthDebug(10f);
        }
        if (GUILayout.Button("Heal 20"))
        {
            myScript.HealthDebug(20f);
        }
        if (GUILayout.Button("Heal 1000000"))
        {
            myScript.HealthDebug(1000000f);
        }
        if (GUILayout.Button("Damage 10"))
        {
            myScript.HealthDebug(-10f);
        }
        if (GUILayout.Button("Damage 20"))
        {
            myScript.HealthDebug(-20f);
        }
        if (GUILayout.Button("Damage 1000000"))
        {
            myScript.HealthDebug(-1000000f);
        }
    }
}
#endif

[RequireComponent(typeof(Collider))]
public class DungeonGame_EntityHealth : MonoBehaviour
{
    [SerializeField] private float _initialHealth = 100f;

    private float _maxHealth = 0f;
    public float MaxHealth() { return _maxHealth; }
    private float _health = 0f;
    public float Health() { return _health; }
    private bool _alive = true;
    public bool Alive() { return _alive; }

    public UnityEvent<float> OnHealthChanged = new UnityEvent<float>();
    public UnityEvent OnDeath = new UnityEvent();

    private float _damageNegation = 1f;

    private void OnEnable()
    {
        _maxHealth = _initialHealth;
        _health = _maxHealth;
    }

    public void ChangeHealth(float offset)
    {
        if (!_alive) return;

        if (offset < 0f && _damageNegation < 1f)
        {
            offset *= _damageNegation;
        }

        _health += offset;
        _health = Mathf.Clamp(_health, 0f, _maxHealth);
        OnHealthChanged?.Invoke(offset);

        if (_health <= 0f)
        {
            _health = 0f;
            _alive = false;

            OnDeath?.Invoke();
        }
    }

    public void SetDamageNegation(float negation)
    {
        _damageNegation = negation;
    }

    public void SetInitialHealth(float newHealth)
    {
        _maxHealth = newHealth;
        _health = _maxHealth;
        OnHealthChanged?.Invoke(1f);
    }

    public void HealthDebug(float heal)
    {
        ChangeHealth(heal);
    }
}
