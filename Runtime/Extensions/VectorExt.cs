using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions {
    public static class VectorExt {
        public static float Angle(this Vector2 direction) {
            var rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static float Angle(this Vector3 direction) {
            var rotZ = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static float Direction(this Vector2 start, Vector2 target) {
            var direction = start - target;
            var rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return rotZ;
        }

        public static Vector2 Cross(Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
            var dot = Vector2.zero;

            float n;
            if (b.y - a.y != 0) {
                var q = (b.x - a.x) / (a.y - b.y);
                var sn = c.x - d.x + (c.y - d.y) * q;
                var fn = c.x - a.x + (c.y - a.y) * q;
                n = fn / sn;
            } else {
                n = (c.y - a.y) / (c.y - d.y);
            }

            dot.x = c.x + (d.x - c.x) * n;
            dot.y = c.y + (d.y - c.y) * n;

            return dot;
        }

        public static Vector3 Cross(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            var dot = Vector3.zero;

            float n;
            if (b.z - a.z != 0) {
                var q = (b.x - a.x) / (a.z - b.z);
                var sn = c.x - d.x + (c.z - d.z) * q;
                var fn = c.x - a.x + (c.z - a.z) * q;
                n = fn / sn;
            } else {
                n = (c.z - a.z) / (c.z - d.z);
            }

            dot.x = c.x + (d.x - c.x) * n;
            dot.z = c.z + (d.z - c.z) * n;

            return dot;
        }

        public static bool AreLineSegmentsIntersectingDotProduct(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
            var isIntersecting = IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2);

            return isIntersecting;
        }

        private static bool IsPointsOnDifferentSides(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
            var isOnDifferentSides = false;

            var lineDir = p2 - p1;

            var lineNormal = new Vector3(-lineDir.z, lineDir.y, lineDir.x);

            var dot1 = Vector3.Dot(lineNormal, p3 - p1);
            var dot2 = Vector3.Dot(lineNormal, p4 - p1);

            if (dot1 * dot2 < 0f) isOnDifferentSides = true;

            return isOnDifferentSides;
        }

        public static void CalcDampedSimpleHarmonicMotionFast(ref float position, ref float velocity,
            float equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio) {
            var x = position - equilibriumPosition;
            velocity += -dampingRatio * velocity - angularFrequency * x;
            position += velocity * deltaTime;
        }

        public static void CalcDampedSimpleHarmonicMotionFast(ref Vector2 position, ref Vector2 velocity,
            Vector2 equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio) {
            var x = position - equilibriumPosition;
            velocity += -dampingRatio * velocity - angularFrequency * x;
            position += velocity * deltaTime;
        }

        public static void CalcDampedSimpleHarmonicMotionFast(ref Vector3 position, ref Vector3 velocity,
            Vector3 equilibriumPosition, float deltaTime, float angularFrequency, float dampingRatio) {
            var x = position - equilibriumPosition;
            velocity += -dampingRatio * velocity - angularFrequency * x;
            position += velocity * deltaTime;
        }
    }
}