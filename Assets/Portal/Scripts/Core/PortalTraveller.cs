using System;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    [field: SerializeField] public GameObject GraphicsObject { get; private set; }
    public GameObject GraphicsClone { get; private set; }
    public Material[] OriginalMaterials { get; private set; }
    public Material[] CloneMaterials { get; private set; }

    [NonSerialized] public Vector3 previousOffsetFromPortal;
    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold()
    {
        if (GraphicsClone == null)
        {
            GraphicsClone = Instantiate(GraphicsObject);
            GraphicsClone.transform.parent = GraphicsObject.transform.parent;
            GraphicsClone.transform.localScale = GraphicsObject.transform.localScale;
            OriginalMaterials = GetMaterials(GraphicsObject);
            CloneMaterials = GetMaterials(GraphicsClone);
        }
        else
            GraphicsClone.SetActive(true);
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold()
    {
        GraphicsClone.SetActive(false);
        // Disable slicing
        for (int i = 0; i < OriginalMaterials.Length; i++)
            OriginalMaterials[i].SetVector("sliceNormal", Vector3.zero);
    }

    public void SetSliceOffsetDst(float dst, bool clone)
    {
        for (int i = 0; i < OriginalMaterials.Length; i++)
        {
            if (clone)
                CloneMaterials[i].SetFloat("sliceOffsetDst", dst);
            else
                OriginalMaterials[i].SetFloat("sliceOffsetDst", dst);
        }
    }

    private Material[] GetMaterials(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer>();
        var matList = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
                matList.Add(mat);
        }
        return matList.ToArray();
    }
}