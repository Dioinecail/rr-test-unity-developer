using Dioinecail.ServiceLocator;
using Project.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Project.Utility.Resources
{
    [Serializable]
    public class JsonPicturesDataWrapper
    {
        public List<JsonPicturesData> data;
    }

    [Serializable]
    public class JsonPicturesData
    {
        public string image_url;
        public string description;
        public string url;
    }

    public class ImageLoader : IImageLoader
    {
        private const string IMAGES_JSON_REQUEST_URL = "https://randomwordgenerator.com/json/pictures.php?category=all";
        private const string RANDOM_IMAGES_URL = "https://randompicturegenerator.com";
        private const int SPRITE_WIDTH = 256;
        private const int SPRITE_HEIGHT = 256;

        private ICoroutineManager _coroutineManager;



        public void GetImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {
            _coroutineManager.StartCoroutine(DownloadImages(count, progressCallback, finishCallback));
        }

        private IEnumerator DownloadImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {
            UnityWebRequest jsonRequest = UnityWebRequest.Get(IMAGES_JSON_REQUEST_URL);

            yield return jsonRequest.SendWebRequest();

            var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonPicturesDataWrapper>(jsonRequest.downloadHandler.text);

            List<Sprite> images = new List<Sprite>(count);
            List<UnityWebRequestAsyncOperation> downloadOperations = new List<UnityWebRequestAsyncOperation>();

            for (int i = 0; i < count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, jsonData.data.Count);

                string imageRequestUrl = string.Concat(RANDOM_IMAGES_URL, jsonData.data[randomIndex].image_url);

                UnityWebRequest downloadRequest = new UnityWebRequest(imageRequestUrl);
                downloadRequest.downloadHandler = new DownloadHandlerTexture();

                downloadOperations.Add(downloadRequest.SendWebRequest());
            }

            while (downloadOperations.Any(o => !o.isDone))
                yield return null;

            foreach (var doperation in downloadOperations)
            {
                Texture2D tex = null;
                bool isError = false;

                try
                {
                    tex = DownloadHandlerTexture.GetContent(doperation.webRequest);
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, SPRITE_WIDTH, SPRITE_HEIGHT), new Vector2(0.5f, 0.5f));
                    images.Add(sprite);
                }
                catch (InvalidOperationException ex)
                {
                    isError = true;
                }

                if (isError)
                {
                    yield return DownloadImages(count, progressCallback, finishCallback);
                    yield break;
                }
            }

            finishCallback?.Invoke(images);
        }

        public void Init()
        {
            _coroutineManager = ServiceLocator.Get<ICoroutineManager>();
        }

        public void Clean() { }
    }
}
