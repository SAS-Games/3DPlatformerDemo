using SAS.StateMachineGraph;
using SAS.Utilities.TagSystem;
using SAS.SceneManagement;

public class LoadSceneGroup : IAwaitableStateAction
{
    [FieldRequiresInScene] private SceneLoader _sceneLoader;
    public bool IsCompleted { get; private set; }

    private string _sceneGroupName;

    void IStateAction.OnInitialize(Actor actor, Tag tag, string key)
    {
        actor.Initialize(this);
        _sceneGroupName = key;
    }

    async void IStateAction.Execute(ActionExecuteEvent executeEvent)
    {
        IsCompleted = false;
        await _sceneLoader.LoadSceneGroup(_sceneGroupName);
        IsCompleted = true;
    }
}
