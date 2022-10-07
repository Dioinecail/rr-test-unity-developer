using Dioinecail.ServiceLocator;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utility.Resources
{
    public interface IImageLoader : IService
    {
        void GetImages(int count, Action<float> progressCallback, Action<List<Sprite>> finishCallback);
    }
}