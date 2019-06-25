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
            CameraTrigger = 1 << 15,
            LightSpeedDash = 1 << 16
        }

        public enum LayerInt
        {
            //Values as in TagManager
            Everything = -1,
            Nothing = 0,
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Water = 4,
            UI = 5,
            Homeable = 9,
            Player = 10,
            Ring = 11,
            StaticRing = 12,
            NoPlayerCollide = 13,
            NonPlayerLand = 14,
            CameraTrigger = 15,
            LightSpeedDash = 16
        }
    }
}

