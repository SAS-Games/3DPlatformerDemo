using SAS.StateMachineCharacterController;
using SAS.StateMachineGraph;
using SAS.StateMachineGraph.Utilities;
using SAS.Utilities.BlackboardSystem;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class AniamatorJumpCount : IStateAction
{
    [FieldRequiresChild] private Animator _animator;
    [FieldRequiresChild] private FSMCharacterController _characterController;
    private int _parameterHash;
    private Actor _actor;
    private BlackboardKey _remainingJumpCountKey = default;
    private BlackboardKey _maxJumpCountKey = default;
    private string _key;
    private AnimatorParameterConfig _parameterConfig;


    void IStateAction.OnInitialize(Actor actor, Tag tag, string key)
    {
        actor.Initialize(this);
        _actor = actor;
        _parameterHash = Animator.StringToHash(key);
        _key = key;
        _remainingJumpCountKey = _actor.GetOrRegisterKey(FSMCharacterBlackboardKey.RemainingJumpCount);
        _maxJumpCountKey = _actor.GetOrRegisterKey(FSMCharacterBlackboardKey.MaxJumpCount);

        actor.TryGet(out _parameterConfig);
    }

    void IStateAction.Execute(ActionExecuteEvent executeEvent)
    {
        var maxJumpCount = _actor.GetValue<int>(_maxJumpCountKey);
        var remainingJumpCount = _actor.GetValue<int>(_remainingJumpCountKey);
        _animator.SetInteger(_parameterHash, maxJumpCount - remainingJumpCount);
        _parameterConfig.ApplyParameters(_animator, in _key);
    }
}
