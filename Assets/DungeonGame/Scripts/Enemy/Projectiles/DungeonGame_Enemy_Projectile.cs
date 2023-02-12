using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Enemy_Projectile : MonoBehaviour
{
    [SerializeField] protected LayerMask _collisionLayer;
    [SerializeField] protected float _projectileSpeed = 1f;
    [SerializeField] protected float _projectileSize = 1f;

    protected string _owner = string.Empty;
    protected float _damage = 0f;
    protected GameObject _hitVFXPrefab;
    protected bool _initialized = false;

    private void FixedUpdate()
    {
        if (!_initialized) return;

        transform.position += transform.forward * Time.fixedDeltaTime * Time.timeScale * _projectileSpeed;
        if (CheckCollision())
        {
            CheckCollisionsWithEntities();
        }
    }

    public void EnableProjectile(string owner, Vector3 dir, float damage, GameObject hitVFXPrefab)
    {
        transform.rotation = Quaternion.LookRotation(dir);
        _owner = owner;
        _damage = damage;
        _hitVFXPrefab = hitVFXPrefab;
        _initialized = true;
    }

    protected bool CheckCollision()
    {
        if (Physics.CheckSphere(transform.position, _projectileSize, _collisionLayer.value, QueryTriggerInteraction.Collide))
            return true;
        else
            return false;
    }

    protected void CheckCollisionsWithEntities()
    {
        Collider[] inCollision = Physics.OverlapSphere(transform.position, _projectileSize, _collisionLayer.value, QueryTriggerInteraction.Collide);
        bool destroyPass = false;

        foreach (Collider collider in inCollision)
        {
            if (collider.transform.CompareTag(_owner)) continue;

            destroyPass = true;
            if (collider.transform.GetComponent<DungeonGame_EntityHealth>())
            {
                DungeonGame_EntityHealth health = collider.transform.GetComponent<DungeonGame_EntityHealth>();
                if (health.Alive())
                {
                    Instantiate(_hitVFXPrefab, collider.bounds.center, Quaternion.identity);
                    health.ChangeHealth(-_damage);
                    Destroy(gameObject);
                }
            }
        }

        if (destroyPass)
            Destroy(gameObject);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _projectileSize);
    }
}
