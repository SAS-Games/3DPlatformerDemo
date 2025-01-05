using System;
using UnityEngine;

[Serializable]
public class SmartBulletHoleGroup
{
    public string tag;
    public Material material;
    public PhysicsMaterial physicMaterial;
    public BulletHolePool bulletHole;

    public SmartBulletHoleGroup()
    {
        tag = "Everything";
        material = null;
        physicMaterial = null;
        bulletHole = null;
    }
    public SmartBulletHoleGroup(string t, Material m, PhysicsMaterial pm, BulletHolePool bh)
    {
        tag = t;
        material = m;
        physicMaterial = pm;
        bulletHole = bh;
    }
}


