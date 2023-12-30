using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UniExt.Extensions {
    public class MathTool {
        public static string GetRandomKeyFromWeightDict(Dictionary<string, int> dict) {
            var num = 0;
            foreach (var item in dict) num += item.Value;

            var num2 = Random.Range(1, num + 1);
            var list = new List<string>(dict.Keys);
            var num3 = 0;
            foreach (var item2 in list) {
                num3 += dict[item2];
                if (num2 <= num3) return item2;
            }

            return "None";
        }

        public static Vector3 GetVec3ByString(string str) {
            str = str.Replace("(", "").Replace(")", "");
            var array = str.Split(',');
            return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture),
                float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
        }
    }
}