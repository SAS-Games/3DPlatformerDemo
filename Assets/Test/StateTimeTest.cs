using SAS.StateMachineGraph;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class StateTimeTest : IStateAction
{
    float time;
    void IStateAction.Execute(ActionExecuteEvent executeEvent)
    {
        if (executeEvent == ActionExecuteEvent.OnStateEnter)
            time = 0;
        if (executeEvent == ActionExecuteEvent.OnUpdate)
            time += Time.deltaTime;
        if (executeEvent == ActionExecuteEvent.OnStateExit)
            SAS.Debug.Log($"total time:{time}", "time");
    }

    void IStateAction.OnInitialize(Actor actor, Tag tag, string key)
    {
    }
}
