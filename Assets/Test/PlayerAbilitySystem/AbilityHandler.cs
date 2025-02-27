using SAS.StateMachineCharacterController;
using SAS.StateMachineGraph;
using SAS.Utilities.BlackboardSystem;
using SAS.Utilities.TagSystem;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandler : MonoBase
{
    private const string FileName = "AbilityData";
    private const string DirName = "AbilityDataDir";

    [FieldRequiresSelf] private Actor _actor;
    [Inject] private ISaveSystem _saveSystem;
    [Inject] IUserModel _userModel;
    private AbilityData _abilityData;

    private void Awake()
    {
        this.Initialize();
    }

    private async void Start()
    {
        _abilityData = await _saveSystem.Load<AbilityData>(GetUserID(), DirName, FileName);
        if (_abilityData == null )
            _abilityData = new AbilityData();
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
        _saveSystem.Save(GetUserID(), DirName, FileName, _abilityData);
    }

#if UNITY_PS5
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            _saveSystem.Save(GetUserID(), DirName, FileName, _abilityData);
    }
#endif

    private int GetUserID()
    {
        return _userModel.GetActiveUserId();
    }
} 
