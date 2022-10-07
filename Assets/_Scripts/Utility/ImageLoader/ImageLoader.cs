using Dioinecail.ServiceLocator;
using Project.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Project.Utility.Resources
{
    [DefaultImplementation(typeof(IImageLoader))]
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
            UnityWebRequest downloadRequest = new UnityWebRequest(RANDOM_IMAGES_URL);
            downloadRequest.downloadHandler = new DownloadHandlerTexture();

            List<Sprite> images = new List<Sprite>(count);
            UnityWebRequestAsyncOperation operation = null;

            for (int i = 0; i < count; i++)
            {
                operation = downloadRequest.SendWebRequest();

                while(!operation.isDone)
                {
                    // progress callback logic here
                    yield return null;
                }

                var tex = DownloadHandlerTexture.GetContent(downloadRequest);
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
