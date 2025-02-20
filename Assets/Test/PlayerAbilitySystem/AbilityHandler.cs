using SAS.StateMachineCharacterController;
using SAS.StateMachineGraph;
using SAS.Utilities.BlackboardSystem;
using SAS.Utilities.TagSystem;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBase
{
    private  const string AbilityDataFileName = "AbilityData";

    [FieldRequiresSelf, HideInInspector] private Actor _actor;
    [Inject] private ISaveSystem _saveSystem;
    private AbilityData _abilityData;

    private void Awake()
    {
        this.InjectFieldBindings();
    }

    private async void Start()
    {
        _abilityData = await _saveSystem.Load<AbilityData>(AbilityDataFileName);
        InitializeAbilities();
    }

    private void InitializeAbilities()
    {
        var abilityMap = new Dictionary<AbilityType, bool>
        {
            { AbilityType.Dash, _abilityData.DashUnlocked },
            { AbilityType.DoubleJump, _abilityData.DoubleJumpUnlocked },
            { AbilityType.WallClimb, _abilityData.WallClimbUnlocked }
        };

        foreach (var ability in abilityMap)
        {
            UnlockAbility(ability.Key, ability.Value);
        }
    }

    public bool IsAbilityUnlocked(AbilityType ability)
    {
        return ability switch
        {
            AbilityType.Dash => _abilityData.DashUnlocked,
            AbilityType.DoubleJump => _abilityData.DoubleJumpUnlocked,
            AbilityType.WallClimb => _abilityData.WallClimbUnlocked,
            _ => false
        };
    }

    public void UnlockAbility(AbilityType ability, bool status)
    {
        switch (ability)
        {
            case AbilityType.Dash:
                _abilityData.DashUnlocked = status;
                break;
            case AbilityType.DoubleJump:
                _abilityData.DoubleJumpUnlocked = status;
                _actor.SetValue(new BlackboardKey(FSMCharacterBlackboardKey.MaxJumpCount), status ? 2 : 1);
                _actor.SetValue(new BlackboardKey(FSMCharacterBlackboardKey.RemainingJumpCount), status ? 2 : 1);

                break;
            case AbilityType.WallClimb:
                _abilityData.WallClimbUnlocked = status;
                break;
        }
    }

    private void OnApplicationQuit()
    {
        _saveSystem.Save(AbilityDataFileName, _abilityData);
    }
}
