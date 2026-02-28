using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class WeightedRandom
    {
        public static bool TryPickKey(IReadOnlyDictionary<string, int> weights, out string key)
        {
            if (weights == null)
                throw new ArgumentNullException(nameof(weights));

            var totalWeight = 0;
            foreach (var pair in weights)
            {
                if (pair.Value > 0)
                    totalWeight += pair.Value;
            }

            if (totalWeight <= 0)
            {
                key = null;
                return false;
            }

            var roll = UnityEngine.Random.Range(1, totalWeight + 1);
            var cumulativeWeight = 0;
            foreach (var pair in weights)
            {
                if (pair.Value <= 0)
                    continue;

                cumulativeWeight += pair.Value;
                if (roll > cumulativeWeight)
                    continue;

                key = pair.Key;
                return true;
            }

            key = null;
            return false;
        }
    }

    public static class MathTool
    {
        [Obsolete("Use WeightedRandom.TryPickKey(...) and handle missing key explicitly.")]
        public static string GetRandomKeyFromWeightDict(Dictionary<string, int> dict)
        {
            return WeightedRandom.TryPickKey(dict, out var key) ? key : null;
        }

        public static bool TryParseVector3(string str, out Vector3 vector)
        {
            vector = default;
            if (string.IsNullOrWhiteSpace(str))
                return false;

            var normalized = str.Replace("(", string.Empty).Replace(")", string.Empty);
            var parts = normalized.Split(',');
            if (parts.Length != 3)
                return false;

            if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
                return false;

            if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                return false;

            if (!float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                return false;

            vector = new Vector3(x, y, z);
            return true;
        }

        [Obsolete("Use TryParseVector3(...) and handle invalid input.")]
        public static Vector3 GetVec3ByString(string str)
        {
            if (TryParseVector3(str, out var vector))
                return vector;

            throw new FormatException($"Failed to parse Vector3 from '{str}'.");
        }
    }
}
