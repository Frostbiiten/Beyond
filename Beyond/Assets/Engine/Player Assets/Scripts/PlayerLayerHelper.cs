using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LayerHelper
{
    public class PlayerLayerHelper
    {
        [Flags]
        public enum Layers
        {
            //Values as in TagManager
            Everything = -1,
            Nothing = 0,
            Default = 1 << 0,
            TransparentFX = 1 << 1,
            IgnoreRaycast = 1 << 2,
            Water = 1 << 4,
            UI = 1 << 5,
            Homeable = 1 << 9,
            Player = 1 << 10,
            Ring = 1 << 11,
            StaticRing = 1 << 12,
            NoPlayerCollide = 1 << 13,
            NonPlayerLand = 1 << 14,
            CameraTrigger = 1 << 15
        }
    }
}

