using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class LayerMaskExt
    {
        public static int Inverse(this int mask) => ~mask;

        public static int Combined(this int mask, int other) => mask | other;

        public static bool Includes(this int mask, int layer)
        {
            if (layer < 0 || layer > 31)
                return false;

            return ((1 << layer) & mask) != 0;
        }

        public static bool Includes(this int mask, string layerName)
        {
            if (string.IsNullOrWhiteSpace(layerName))
                return false;

            var layer = LayerMask.NameToLayer(layerName);
            if (layer < 0)
                return false;

            return mask.Includes(layer);
        }

        public static bool IncludesAll(this int mask, IEnumerable<int> layers)
        {
            if (layers == null)
                throw new ArgumentNullException(nameof(layers));

            foreach (var layer in layers)
            {
                if (!mask.Includes(layer))
                    return false;
            }

            return true;
        }

        public static bool IncludesAny(this int mask, IEnumerable<int> layers)
        {
            if (layers == null)
                throw new ArgumentNullException(nameof(layers));

            foreach (var layer in layers)
            {
                if (mask.Includes(layer))
                    return true;
            }

            return false;
        }

        public static LayerMask Inverse(this LayerMask layerMask) => ~layerMask;

        public static LayerMask Combined(this LayerMask layerMask, LayerMask other) => layerMask | other;

        public static bool Includes(this LayerMask layerMask, int layer) => layerMask.value.Includes(layer);

        public static bool Includes(this LayerMask layerMask, string layerName) => layerMask.value.Includes(layerName);

        public static bool IncludesAll(this LayerMask layerMask, IEnumerable<int> layers) => layerMask.value.IncludesAll(layers);

        public static bool IncludesAny(this LayerMask layerMask, IEnumerable<int> layers) => layerMask.value.IncludesAny(layers);
    }
}
