using System;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class DrawExt
    {
        public static void DrawCircle(
            this LineRenderer line,
            ref Vector3[] points,
            float radius,
            float lineWidth,
            float height = 0f,
            int segments = 360)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line));

            if (segments < 3)
                throw new ArgumentOutOfRangeException(nameof(segments), "Circle requires at least 3 segments.");

            if (points == null || points.Length != segments)
                points = new Vector3[segments];

            line.useWorldSpace = false;
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.positionCount = segments;

            var radiansPerSegment = Mathf.PI * 2f / segments;
            for (var i = 0; i < segments; i++)
            {
                var rad = radiansPerSegment * i;
                points[i] = new Vector3(Mathf.Sin(rad) * radius, height, Mathf.Cos(rad) * radius);
            }

            line.SetPositions(points);
        }
    }
}
