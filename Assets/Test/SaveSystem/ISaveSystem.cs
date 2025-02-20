using SAS.Utilities.TagSystem;

public interface ISaveSystem : IBindable
{
    void Save<T>(string fileName, T data);
    T Load<T>(string fileName);
}
