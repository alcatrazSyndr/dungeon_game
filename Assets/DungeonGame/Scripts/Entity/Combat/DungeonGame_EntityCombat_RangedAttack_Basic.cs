using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityCombat_RangedAttack_Basic : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;

    public void Attack(float damage, Transform attackRootTransform, Vector3 targetPos, GameObject _attackSplashVFX)
    {
        GameObject projectileGO = Instantiate(_projectilePrefab, attackRootTransform.position, attackRootTransform.rotation);
        DungeonGame_Projectile projectile = projectileGO.GetComponent<DungeonGame_Projectile>();

        Vector3 dir = targetPos - attackRootTransform.position;

        projectile.EnableProjectile(transform.tag, dir, damage, _attackSplashVFX);
    }
}
