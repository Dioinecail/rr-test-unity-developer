using Dioinecail.ServiceLocator;
using Newtonsoft.Json;
using Project.Core;
using Project.FileSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Project.Utility.Resources
{
    [Serializable]
    public class CacheManifestData
    {
        public List<string> ImagePaths;
    }

    public class CachedImageLoader : IImageLoader
    {
        private static string _cacheManifestPath = Application.persistentDataPath + "/ImageCache/Cache.json";
        private static string _cachedLocation = Application.persistentDataPath + "/ImageCache/";
        private const string RANDOM_IMAGES_URL = "https://picsum.photos/256";
        private const int SPRITE_WIDTH = 256;
        private const int SPRITE_HEIGHT = 256;

        private ICoroutineManager _coroutineManager;
        private IFileManager _fileManager;



        public void GetImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {
            if (!CheckCache())
                _coroutineManager.StartCoroutine(DownloadImages(count, progressCallback, finishCallback));
            else
                _coroutineManager.StartCoroutine(LoadImagesFromCache(count, progressCallback, finishCallback));
        }

        private bool CheckCache()
        {
            return _fileManager.Exists(_cacheManifestPath);
        }

        private IEnumerator LoadImagesFromCache(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {
            var manifestData = JsonConvert.DeserializeObject<CacheManifestData>(_fileManager.ReadAllText(_cacheManifestPath));
            var manifestUpdated = false;

            var sprites = new List<Sprite>();
            List<UnityWebRequestAsyncOperation> downloadOperations = new List<UnityWebRequestAsyncOperation>();

            for (int i = 0; i < count; i++)
            {
                if(i < manifestData.ImagePaths.Count)
                {
                    var path = manifestData.ImagePaths[i];
                    var texture = _fileManager.ReadTexture(path);

                    var sprite = Sprite.Create(texture, new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT), new Vector2(0.5f, 0.5f));

                    sprites.Add(sprite);
                }
                else
                {
                    UnityWebRequest downloadRequest = new UnityWebRequest(RANDOM_IMAGES_URL);
                    downloadRequest.downloadHandler = new DownloadHandlerTexture();

                    downloadOperations.Add(downloadRequest.SendWebRequest());
                }
            }

            while (downloadOperations.Any(o => !o.isDone))
                yield return null;

            foreach (var doperation in downloadOperations)
            {
                var tex = DownloadHandlerTexture.GetContent(doperation.webRequest);
                var guid = Guid.NewGuid().ToString();
                var path = _fileManager.Combine(_cachedLocation, guid, ".png");

                _fileManager.WriteTexture(path, tex);
                manifestData.ImagePaths.Add(path);
                manifestUpdated = true;

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT), new Vector2(0.5f, 0.5f));
                sprites.Add(sprite);
            }

            if(manifestUpdated)
            {
                var updatedManifest = JsonConvert.SerializeObject(manifestData);
                _fileManager.WriteAllText(_cacheManifestPath, updatedManifest);
            }

            finishCallback?.Invoke(sprites);
        }

        private IEnumerator DownloadImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {
            List<Sprite> images = new List<Sprite>(count);

            List<UnityWebRequestAsyncOperation> downloadOperations = new List<UnityWebRequestAsyncOperation>();

            for (int i = 0; i < count; i++)
            {
                UnityWebRequest downloadRequest = new UnityWebRequest(RANDOM_IMAGES_URL);
                downloadRequest.downloadHandler = new DownloadHandlerTexture();

                downloadOperations.Add(downloadRequest.SendWebRequest());
            }

            while (downloadOperations.Any(o => !o.isDone))
                yield return null;

            foreach (var doperation in downloadOperations)
            {
                var tex = DownloadHandlerTexture.GetContent(doperation.webRequest);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT), new Vector2(0.5f, 0.5f));
                images.Add(sprite);
            }

            finishCallback?.Invoke(images);

            CacheImages(images);
        }

        public void Init()
        {
            _coroutineManager = ServiceLocator.Get<ICoroutineManager>();
            _fileManager = ServiceLocator.Get<IFileManager>();
        }

        public void Clean()
        {

        }

        private void CacheImages(List<Sprite> sprites)
        {
            CacheManifestData manifestData;

            if (_fileManager.Exists(_cacheManifestPath))
            {
                manifestData = JsonConvert.DeserializeObject<CacheManifestData>(_fileManager.ReadAllText(_cacheManifestPath));
            }
            else
            {
                manifestData = new CacheManifestData()
                {
                    ImagePaths = new List<string>()
                };
            }
            foreach (var sprite in sprites)
            {
                var guid = Guid.NewGuid().ToString();
                var path = _fileManager.Combine(_cachedLocation, guid) + ".png";

                _fileManager.WriteTexture(path, sprite.texture);
                manifestData.ImagePaths.Add(path);
            }

            var updatedManifest = JsonConvert.SerializeObject(manifestData, Formatting.Indented);
            _fileManager.WriteAllText(_cacheManifestPath, updatedManifest);
        }
    }
}