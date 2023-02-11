using UnityEngine;
using UnityEngine.Events;

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

    private void OnEnable()
    {
        _maxHealth = _initialHealth;
        _health = _maxHealth;
    }

    public void ChangeHealth(float offset)
    {
        if (!_alive) return;

        _health += offset;
        OnHealthChanged?.Invoke(offset);

        if (_health <= 0f)
        {
            _health = 0f;
            _alive = false;

            OnDeath?.Invoke();
        }
    }

    public void SetInitialHealth(float newHealth)
    {
        _maxHealth = newHealth;
        _health = _maxHealth;
        OnHealthChanged?.Invoke(1f);
    }
}
