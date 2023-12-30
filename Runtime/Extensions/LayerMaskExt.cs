using System.Collections.Generic;
using UnityEngine;

namespace UniExt.Extensions {
    public static class LayerMaskExt {
        public static int Inverse(this int mask) {
            return ~mask;
        }

        public static int Combined(this int mask, int other) {
            return mask | other;
        }

        public static bool Includes(this int mask, int value) {
            return ((1 << value) & mask) != 0;
        }

        public static bool Includes(this int mask, string layerName) {
            return mask.Includes(LayerMask.NameToLayer(layerName));
        }

        public static bool IncludesAll(this int mask, IEnumerable<int> values) {
            foreach (var layer in values)
                if (!mask.Includes(layer))
                    return false;

            return true;
        }

        public static bool IncludesAny(this int mask, IEnumerable<int> values) {
            foreach (var layer in values)
                if (mask.Includes(layer))
                    return true;

            return false;
        }


        public static LayerMask Inverse(this LayerMask layerMask) {
            return ~layerMask;
        }

        public static LayerMask Combined(this LayerMask layerMask, LayerMask other) {
            return layerMask | other;
        }

        public static bool Includes(this LayerMask layerMask, int value) {
            return layerMask.value.Includes(value);
        }

        public static bool Includes(this LayerMask layerMask, string layerName) {
            return layerMask.value.Includes(layerName);
        }

        public static bool IncludesAll(this LayerMask layerMask, IEnumerable<int> values) {
            return layerMask.value.IncludesAll(values);
        }

        public static bool IncludesAny(this LayerMask layerMask, IEnumerable<int> values) {
            return layerMask.value.IncludesAny(values);
        }
    }
}