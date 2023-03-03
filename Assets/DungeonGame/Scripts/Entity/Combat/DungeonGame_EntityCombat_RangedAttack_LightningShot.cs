using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityCombat_RangedAttack_LightningShot : DungeonGame_EntityCombat_RangedAttack_HitScan
{
    [SerializeField] private GameObject _collisionVFX;
    [SerializeField] private LayerMask _entityLayer;
    [SerializeField] private float _aoeDamageSize = 1.5f;

    protected override void ProjectileReachedEnd(Vector3 position, float damage)
    {
        Instantiate(_collisionVFX, position, Quaternion.identity);
        Collider[] inCollision = Physics.OverlapSphere(position, _aoeDamageSize, _entityLayer.value, QueryTriggerInteraction.Collide);
        foreach (Collider collider in inCollision)
        {
            if (collider.transform.CompareTag(transform.tag)) continue;
            if (collider.transform.GetComponent<DungeonGame_EntityHealth>())
            {
                DungeonGame_EntityHealth hitHealth = collider.transform.GetComponent<DungeonGame_EntityHealth>();
                if (hitHealth.Alive())
                {
                    hitHealth.ChangeHealth(-damage);
                }
            }
        }
    }
}
