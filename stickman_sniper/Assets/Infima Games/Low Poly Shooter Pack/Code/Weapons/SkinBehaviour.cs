using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class SkinBehaviour : MonoBehaviour
{
    public List<SkinEntity> Skins;

    [Button]
    public void SetSkins()
    {
        foreach(var group in Skins.GroupBy(g => g.Renderer))
        {
            Material[] currentMaterials = group.Key.sharedMaterials;
            foreach(var item in group)
            {
                currentMaterials[item.MaterialIndex] = item.NewMaterial;
            }

            group.Key.sharedMaterials = currentMaterials;
        }
    }
}

[Serializable]
public class SkinEntity
{
    public Renderer Renderer;
    public int MaterialIndex;
    public Material NewMaterial;
}