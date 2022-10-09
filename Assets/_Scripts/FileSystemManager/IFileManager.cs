using Dioinecail.ServiceLocator;
using UnityEngine;

namespace Project.FileSystem
{
    public interface IFileManager : IService
    {
        bool Exists(string path);
        string ReadAllText(string path);
        bool WriteAllText(string path, string content);
        T LoadResource<T>(string path) where T : Object;
        string Combine(params string[] paths);
        bool WriteTexture(string path, Texture2D data);
        Texture2D ReadTexture(string path);
    }
}