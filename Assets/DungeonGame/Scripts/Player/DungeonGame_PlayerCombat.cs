using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject _damageSplashVFX;
    [SerializeField] private Transform _chestTransform;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private float _debugDamage = 20f;
    [SerializeField] private float _debugAttackLockTime = 0.15f;
    [SerializeField] private LayerMask _entityLayer;

    private DungeonGame_AnimatorHandler _animatorHandler = null;
    private DungeonGame_EntityHealth _myHealth = null;
    private Animator _animator = null;

    private void OnEnable()
    {
        _animatorHandler = transform.GetComponentInChildren<DungeonGame_AnimatorHandler>();
        _myHealth = transform.GetComponent<DungeonGame_EntityHealth>();
        _animator = transform.GetComponentInChildren<Animator>();

        _animatorHandler.OnAttackPeak.AddListener(Attack);
    }

    private void OnDisable()
    {
        _animatorHandler.OnAttackPeak.RemoveListener(Attack);
    }

    private void Attack()
    {
        if (_myHealth == null && !_myHealth.Alive()) return;

        //bool didHit = false;
        Collider[] inAttackCollider = Physics.OverlapSphere(_chestTransform.position + (_chestTransform.up * 0.8f), _attackRange, _entityLayer.value, QueryTriggerInteraction.Collide);
        if (inAttackCollider.Length > 0)
        {
            foreach (Collider entityHit in inAttackCollider)
            {
                if (!entityHit.transform.GetComponent<DungeonGame_EntityHealth>()) continue;
                DungeonGame_EntityHealth entityHealth = entityHit.transform.GetComponent<DungeonGame_EntityHealth>();
                if (entityHealth == _myHealth) continue;
                if (!entityHealth.Alive()) continue;

                //didHit = true;
                entityHealth.ChangeHealth(-_debugDamage);
                Instantiate(_damageSplashVFX, entityHealth.transform.position + new Vector3(0f, 0.9f, 0f), Quaternion.identity);
            }
        }

        //if (didHit)
            //StartCoroutine(AttackLockCRT());
    }

    private IEnumerator AttackLockCRT()
    {
        float timer = 0f;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        _animator.speed = 0f;
        while (timer < _debugAttackLockTime)
        {
            transform.position = pos;
            transform.rotation = rot;
            timer += Time.deltaTime * Time.timeScale;
            yield return null;
        }
        _animator.speed = 1f;
        yield break;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_chestTransform.position + (_chestTransform.up * 0.8f), _attackRange);
    }
}
