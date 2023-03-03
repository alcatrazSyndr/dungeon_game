using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DungeonGame_ParticleSystemDestroy : MonoBehaviour
{
    [SerializeField] private float _destroyTimer = 2f;

    public void Initialize(float distance)
    {
        transform.GetComponent<VisualEffect>().Play();
        transform.GetComponent<VisualEffect>().SetFloat("Height", distance);
        StartCoroutine(DestroyAfterCRT());
    }

    private IEnumerator DestroyAfterCRT()
    {
        yield return new WaitForSecondsRealtime(_destroyTimer);
        Destroy(gameObject);
        yield break;
    }
}
