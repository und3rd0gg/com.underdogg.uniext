using UnityEngine;

namespace UniExt.Extensions {
    public static class DrawExt {
        public static void DrawCircle(this LineRenderer line, ref Vector3[] points, float radius, float lineWidth,
            float height = 0f) {
            line.useWorldSpace = false;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = 360;

            var pointCount = 360;

            for (var i = 0; i < pointCount; i++) {
                var rad = Mathf.Deg2Rad * i;
                points[i] = new Vector3(Mathf.Sin(rad) * radius, height, Mathf.Cos(rad) * radius);
            }

            line.SetPositions(points);
        }
    }
}