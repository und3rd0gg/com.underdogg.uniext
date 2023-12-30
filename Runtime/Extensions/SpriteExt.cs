using UnityEngine;
using UnityEngine.UI;

namespace UniExt.Extensions {
    public static class SpriteExt {
        public static void SetAlpha(this SpriteRenderer sr, float alpha) {
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }

        public static void SetAlpha(this Image img, float alpha) {
            var c = img.color;
            img.color = new Color(c.r, c.g, c.b, alpha);
        }

        public static Color SetAlpha(this Color c, float alpha) {
            return new Color(c.r, c.g, c.b, alpha);
        }
    }
}