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
    public class ImageLoader : IImageLoader
    {
        private const string RANDOM_IMAGES_URL = "https://picsum.photos/256";
        private const int SPRITE_WIDTH = 256;
        private const int SPRITE_HEIGHT = 256;

        private ICoroutineManager _coroutineManager;



        public void GetImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback)
        {

            _coroutineManager.StartCoroutine(DownloadImages(count, progressCallback, finishCallback));
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
        }

        public void Init()
        {
            _coroutineManager = ServiceLocator.Get<ICoroutineManager>();
        }

        public void Clean() { }
    }
}
