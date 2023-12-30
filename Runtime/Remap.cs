using UnityEngine;

namespace UniExt
{
    public static class Remap
    {
        public static float DoRemap(float aMin, float aMax, float bMin, float bMax, float v) => 
            Mathf.Lerp(bMin, bMax, Mathf.InverseLerp(aMin, aMax, v));
    }
}