using UnityEngine;
using UnityEngine.UI;

namespace com.underdogg.uniext.Runtime.Extensions {
    public static class SpriteExt {
        public static void SetAlpha(this SpriteRenderer sr, float alpha) {
            if (sr == null)
                throw new System.ArgumentNullException(nameof(sr));

            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }

        public static void SetAlpha(this Image img, float alpha) {
            if (img == null)
                throw new System.ArgumentNullException(nameof(img));

            var c = img.color;
            img.color = new Color(c.r, c.g, c.b, alpha);
        }

        public static Color SetAlpha(this Color c, float alpha) {
            return new Color(c.r, c.g, c.b, alpha);
        }
    }
}
