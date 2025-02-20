using SAS.Utilities.TagSystem;
using System.Threading.Tasks;

public interface ISaveSystem : IBindable
{
    void Save<T>(string fileName, T data);
    Task<T> Load<T>(string fileName) where T : new();
}
