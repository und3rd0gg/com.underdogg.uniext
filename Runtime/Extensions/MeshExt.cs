﻿using System.Collections.Generic;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions {
    public static class MeshExt {
        public static void RecalculateNormals(this Mesh mesh, float angle) {
            var cosineThreshold = Mathf.Cos(angle * Mathf.Deg2Rad);

            var vertices = mesh.vertices;
            var normals = new Vector3[vertices.Length];

            // Holds the normal of each triangle in each sub mesh.
            var triNormals = new Vector3[mesh.subMeshCount][];

            var dictionary = new Dictionary<VertexKey, List<VertexEntry>>(vertices.Length);

            for (var subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; ++subMeshIndex) {
                var triangles = mesh.GetTriangles(subMeshIndex);

                triNormals[subMeshIndex] = new Vector3[triangles.Length / 3];

                for (var i = 0; i < triangles.Length; i += 3) {
                    var i1 = triangles[i];
                    var i2 = triangles[i + 1];
                    var i3 = triangles[i + 2];

                    // Calculate the normal of the triangle
                    var p1 = vertices[i2] - vertices[i1];
                    var p2 = vertices[i3] - vertices[i1];
                    var normal = Vector3.Cross(p1, p2).normalized;
                    var triIndex = i / 3;
                    triNormals[subMeshIndex][triIndex] = normal;

                    List<VertexEntry> entry;
                    VertexKey key;

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i1]), out entry)) {
                        entry = new List<VertexEntry>(4);
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i1));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i2]), out entry)) {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i2));

                    if (!dictionary.TryGetValue(key = new VertexKey(vertices[i3]), out entry)) {
                        entry = new List<VertexEntry>();
                        dictionary.Add(key, entry);
                    }

                    entry.Add(new VertexEntry(subMeshIndex, triIndex, i3));
                }
            }

            // Each entry in the dictionary represents a unique vertex position.

            foreach (var vertList in dictionary.Values)
                for (var i = 0; i < vertList.Count; ++i) {
                    var sum = new Vector3();
                    var lhsEntry = vertList[i];

                    for (var j = 0; j < vertList.Count; ++j) {
                        var rhsEntry = vertList[j];

                        if (lhsEntry.VertexIndex == rhsEntry.VertexIndex) {
                            sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                        } else {
                            // The dot product is the cosine of the angle between the two triangles.
                            // A larger cosine means a smaller angle.
                            var dot = Vector3.Dot(
                                triNormals[lhsEntry.MeshIndex][lhsEntry.TriangleIndex],
                                triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex]);
                            if (dot >= cosineThreshold) sum += triNormals[rhsEntry.MeshIndex][rhsEntry.TriangleIndex];
                        }
                    }

                    normals[lhsEntry.VertexIndex] = sum.normalized;
                }

            mesh.normals = normals;
        }

        private struct VertexKey {
            private readonly long _x;
            private readonly long _y;
            private readonly long _z;

            // Change this if you require a different precision.
            private const int Tolerance = 100000;

            // Magic FNV values. Do not change these.
            private const long FNV32Init = 0x811c9dc5;
            private const long FNV32Prime = 0x01000193;

            public VertexKey(Vector3 position) {
                _x = (long)Mathf.Round(position.x * Tolerance);
                _y = (long)Mathf.Round(position.y * Tolerance);
                _z = (long)Mathf.Round(position.z * Tolerance);
            }

            public override bool Equals(object obj) {
                var key = (VertexKey)obj;
                return _x == key._x && _y == key._y && _z == key._z;
            }

            public override int GetHashCode() {
                var rv = FNV32Init;
                rv ^= _x;
                rv *= FNV32Prime;
                rv ^= _y;
                rv *= FNV32Prime;
                rv ^= _z;
                rv *= FNV32Prime;

                return rv.GetHashCode();
            }
        }

        private struct VertexEntry {
            public readonly int MeshIndex;
            public readonly int TriangleIndex;
            public readonly int VertexIndex;

            public VertexEntry(int meshIndex, int triIndex, int vertIndex) {
                MeshIndex = meshIndex;
                TriangleIndex = triIndex;
                VertexIndex = vertIndex;
            }
        }
    }
}