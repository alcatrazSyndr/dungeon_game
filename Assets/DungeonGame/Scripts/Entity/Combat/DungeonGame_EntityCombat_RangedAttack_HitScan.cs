using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityCombat_RangedAttack_HitScan : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _startVFX = null;
    [SerializeField] private float _travelTime = 0.2f;

    public void Attack(float damage, Vector3 targetPosition, DungeonGame_EntityHealth targetHealth, GameObject hitVFX, Transform rootPosition)
    {
        StartCoroutine(FlightCRT(damage, targetPosition, targetHealth, hitVFX, rootPosition));
    }

    private IEnumerator FlightCRT(float damage, Vector3 targetPosition, DungeonGame_EntityHealth targetHealth, GameObject hitVFX, Transform rootPosition)
    {
        GameObject projectile = Instantiate(_projectilePrefab, rootPosition.position, Quaternion.LookRotation(targetPosition - rootPosition.position, Vector3.up));
        float timer = 0f;
        float intepolation = 0f;
        Vector3 startPos = projectile.transform.position;
        projectile.transform.rotation = Quaternion.LookRotation(targetPosition - projectile.transform.position, Vector3.up);
        if (_startVFX != null)
        {
            GameObject startVFX = Instantiate(_startVFX, rootPosition);
            startVFX.transform.rotation = Quaternion.LookRotation(projectile.transform.position - targetPosition, Vector3.up);
        }
        while (timer < _travelTime)
        {
            intepolation = Mathf.Clamp01(timer / _travelTime);
            projectile.transform.position = Vector3.Lerp(startPos, targetPosition, intepolation);
            timer += Time.deltaTime;
            yield return null;
        }
        if (targetHealth != null && targetHealth.Alive())
        {
            targetHealth.ChangeHealth(-damage);
            Instantiate(hitVFX, projectile.transform.position, Quaternion.identity);
        }
        ProjectileReachedEnd(projectile.transform.position, damage);
        Destroy(projectile);
        yield break;
    }

    protected virtual void ProjectileReachedEnd(Vector3 position, float damage) { }
}
