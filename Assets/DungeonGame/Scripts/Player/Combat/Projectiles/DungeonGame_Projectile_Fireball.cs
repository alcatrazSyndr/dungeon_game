using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_Projectile_Fireball : DungeonGame_Projectile 
{
    [SerializeField] private float _aoeDamageSize = 3f;

    protected override void CheckCollisionsWithEntities()
    {
        Collider[] inCollision = Physics.OverlapSphere(transform.position, _aoeDamageSize, _collisionLayer.value, QueryTriggerInteraction.Collide);
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
        {
            if (_collisionVFXPrefab != null)
            {
                Instantiate(_collisionVFXPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
