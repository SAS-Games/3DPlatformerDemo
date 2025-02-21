using SAS.Utilities.TagSystem;
using System.Threading.Tasks;

public interface ISaveSystem : IBindable
{
    Task<T> Load<T>(int userId, string fileName) where T : new();
    Task<T> Load<T>(int userId, string dirName, string fileName) where T : new();
    Task Save<T>(int userId, string fileName, T data);
    Task Save<T>(int userId, string dirName, string fileName, T data);
}
