using System.IO;
using UnityEngine;

namespace Project.FileSystem
{
    public class StandaloneFileManager : IFileManager
    {
        private object m_Locker = new object();



        public void InitDeps()
        {
            // no deps
        }

        public void Start()
        {
            // no start logic
        }

        public void Clean()
        {

        }

        public string ReadAllText(string path)
        {
            lock (m_Locker)
            {
                if (!File.Exists(path))
                {
                    Debug.LogError($"[{nameof(StandaloneFileManager)}:ReadAllText] File {path} does not exist!");
                    return null;
                }

                return File.ReadAllText(path);
            }
        }

        public T LoadResource<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public bool WriteAllText(string path, string content)
        {
            lock (m_Locker)
            {
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(path))
                    File.Delete(path);

                try
                {
                    File.WriteAllText(path, content);
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    return true;
                }
                catch (System.Exception ex)
                {
#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                    Debug.LogError(ex);
                    return false;
                }
            }
        }

        public bool WriteTexture(string path, Texture2D data)
        {
            var byteData = data.EncodeToPNG();

            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (File.Exists(path))
                File.Delete(path);

            try
            {
                File.WriteAllBytes(path, byteData);

                return true;
            }
            catch(System.Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        public Texture2D ReadTexture(string path)
        {
            if (!File.Exists(path))
                return null;

            var file = File.ReadAllBytes(path);

            var texture = new Texture2D(2, 2);
            texture.LoadImage(file);

            return texture;
        }
    }
}