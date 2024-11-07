using SAS.StateMachineGraph;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class LogFrameDiff : IStateAction
{
    public void Execute(ActionExecuteEvent executeEvent)
    {
        Debug.Log($"LogFrameDiff:  {(Time.frameCount - CacheFrame.CachedFrame)}");
    }

    public void OnInitialize(Actor actor, Tag tag, string key)
    {
    }
}
