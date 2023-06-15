//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VIVE
{
    namespace FacialTracking.Sample
    {
        public enum EyeShape
        {
            None = -1,
            Eye_Left_Blink = 0,
            Eye_Left_Wide,
            Eye_Left_Right,
            Eye_Left_Left,
            Eye_Left_Up,
            Eye_Left_Down,
            Eye_Right_Blink = 6,
            Eye_Right_Wide,
            Eye_Right_Right,
            Eye_Right_Left,
            Eye_Right_Up,
            Eye_Right_Down,
            Eye_Frown = 12,
            Eye_Left_Squeeze,
            Eye_Right_Squeeze,
            Max = 15,
        }

    }
}