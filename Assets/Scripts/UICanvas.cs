using System;
using UnityEngine;
using Assets.Utils;

namespace Assets.Scripts
{
    public class UICanvas : UnitySingleton<UICanvas>
    {
        public Canvas WorldCanvas { get; private set; }
        void Awake()
        {
            WorldCanvas = Instantiate(Resources.Load<Canvas>("UI/Canvas - World")) as Canvas;
            WorldCanvas.name = "Canvas - World";
        }
    }
}