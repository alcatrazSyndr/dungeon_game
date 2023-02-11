using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGame_EntityMaterialChanger : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    [SerializeField] private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _hurtColor;

    public void Hurt()
    {
        SetSpecularColor(_hurtColor);
    }

    public void Restore()
    {
        SetSpecularColor(_normalColor);
    }

    private void SetSpecularColor(Color newColor)
    {
        if (_meshRenderers.Count > 0)
        {
            foreach (MeshRenderer renderer in _meshRenderers)
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i].SetColor("_SpecularColor", newColor);
                }
                renderer.materials = mats;
            }
        }
        if (_skinnedMeshRenderers.Count > 0)
        {
            foreach (SkinnedMeshRenderer renderer in _skinnedMeshRenderers)
            {
                Material[] mats = renderer.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i].SetColor("_SpecularColor", newColor);
                }
                renderer.materials = mats;
            }
        }
    }
}
