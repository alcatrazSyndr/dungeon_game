using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityCombat_ConeAttack_Basic : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _coneAngle = 10f;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private int _coneSampleRate = 6;
    [SerializeField] private float _attackTick = 0.35f;

    public float AttackTick { get { return _attackTick; } }

    public void Attack(Transform attackRoot, Vector3 attackHeading, RaycastHit firstHit, float damage)
    {
        List<DungeonGame_EntityHealth> entitiesToAttack = new List<DungeonGame_EntityHealth>();
        entitiesToAttack = ReturnEntitiesInCone(ReturnRaysInCone(attackRoot, attackHeading, firstHit));
        if (entitiesToAttack.Count <= 0) return;
        foreach (DungeonGame_EntityHealth entity in entitiesToAttack)
        {
            entity.ChangeHealth(-damage);
        }
    }

    private List<DungeonGame_EntityHealth> ReturnEntitiesInCone(List<Ray> coneRays)
    {
        List<DungeonGame_EntityHealth> entitiesInCone = new List<DungeonGame_EntityHealth>();
        foreach (Ray ray in coneRays)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray, _attackRange, _collisionLayer.value, QueryTriggerInteraction.Collide);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag(transform.tag)) continue;
                if (!hit.transform.GetComponent<DungeonGame_EntityHealth>()) continue;
                DungeonGame_EntityHealth entity = hit.transform.GetComponent<DungeonGame_EntityHealth>();

                if (!entity.Alive()) continue;

                if (!entitiesInCone.Contains(entity))
                {
                    entitiesInCone.Add(entity);
                }
            }
        }
        return entitiesInCone;
    }

    private List<Ray> ReturnRaysInCone(Transform root, Vector3 heading, RaycastHit hit)
    {
        List<Ray> rays = new List<Ray>();

        RaycastHit firstHit;

        if (!hit.transform.CompareTag(transform.tag))
        {
            firstHit = hit;
            Vector3 startPoint = root.position;
            float sampleAngle = _coneAngle / (float)_coneSampleRate;
            float startAngle = -((float)_coneSampleRate / 2f);
            Vector3 startDir = Quaternion.AngleAxis(startAngle, Vector3.up) * heading;
            for (int i = 0; i < _coneSampleRate; i++)
            {
                Vector3 sampledDir = Quaternion.AngleAxis(sampleAngle * i, Vector3.up) * startDir;
                Ray sampledRay = new Ray(startPoint, sampledDir);
                rays.Add(sampledRay);
                Debug.DrawRay(startPoint, sampledDir, Color.blue, _attackTick);
            }
        }

        return rays;
    }
}
