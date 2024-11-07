using SAS.StateMachineGraph;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class CacheFrame : IStateAction
{
    public static int CachedFrame;
    public void Execute(ActionExecuteEvent executeEvent)
    {
        CachedFrame = Time.frameCount;
    }

    public void OnInitialize(Actor actor, Tag tag, string key)
    {
    }
}
