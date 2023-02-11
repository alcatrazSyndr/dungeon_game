using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityCombat_MeleeAttack_Basic : MonoBehaviour
{
    private DungeonGame_EntityHealth _myHealth = null;

    private void OnEnable()
    {
        _myHealth = transform.GetComponent<DungeonGame_EntityHealth>();
    }

    public void Attack(float range, float damage, Transform attackRootTransform, GameObject damageSplashVFX)
    {
        if (_myHealth == null && !_myHealth.Alive()) return;

        Collider[] inAttackCollider = Physics.OverlapSphere(attackRootTransform.position, range, LayerMask.NameToLayer("Entity"), QueryTriggerInteraction.Collide);
        if (inAttackCollider.Length > 0)
        {
            foreach (Collider entityHit in inAttackCollider)
            {
                if (entityHit.transform.CompareTag(transform.tag)) continue;
                if (!entityHit.transform.GetComponent<DungeonGame_EntityHealth>()) continue;
                DungeonGame_EntityHealth entityHealth = entityHit.transform.GetComponent<DungeonGame_EntityHealth>();
                if (entityHealth == _myHealth) continue;
                if (!entityHealth.Alive()) continue;

                //didHit = true;
                entityHealth.ChangeHealth(-damage);
                Instantiate(damageSplashVFX, entityHealth.transform.position + new Vector3(0f, 0.9f, 0f), Quaternion.identity);
            }
        }
    }
}
