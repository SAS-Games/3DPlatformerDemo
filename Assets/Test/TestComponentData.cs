using SAS.WeaponSystem.Components;
using UnityEngine;

[System.Serializable]
public class TestAttackData : AttackData
{
    [SerializeField] private bool m_Debug;
    [SerializeField] private int m_AttackIndexTest = 0;
}

public class TestComponentData : ComponentData<TestAttackData>
{
    [SerializeField] private int m_AttackIndex = 0;
    [SerializeField] private TestAttackData m_TestAttackData;
    protected override void SetComponentDependency()
    {
    }
}
