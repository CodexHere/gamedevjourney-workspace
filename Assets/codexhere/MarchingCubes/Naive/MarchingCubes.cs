using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace codexhere.MarchingCubes.Naive {
    public class MarchingCubes {

        public event EventHandler<int> OnCubeProcessed;
        public event EventHandler OnMarchingCompleted;

        public int YieldDivisor = 100;

        public Vector3 Origin { get; }
        public Vector2Int GridSize { get; }

        private readonly Mesh mesh = new();
        private readonly float IsoSurfaceLevel;
        private readonly bool Smooth;
        private List<int> triangles = new();
        private List<Vector3> vertices = new();

        private Vector2Int NoiseSize => GridSize + Vector2Int.one;

        public MarchingCubes(Vector3 origin, Vector2Int size, float isoSurfaceLevel, bool smooth) {
            Origin = origin;
            GridSize = size;
            IsoSurfaceLevel = isoSurfaceLevel;
            Smooth = smooth;
        }

        public void ClearMesh() {
            vertices.Clear();
            triangles.Clear();
        }

        public Mesh BuildMesh() {
            mesh.Clear();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }

        public async Task MarchNoise(CancellationToken cancellationToken, float[] noiseMap) {
            for (int x = 0; x < GridSize.x; x++) {
                for (int y = 0; y < GridSize.y; y++) {
                    for (int z = 0; z < GridSize.x; z++) {
                        if (cancellationToken.IsCancellationRequested) {
                            return;
                        }

                        Vector3 cubePosition = new(x, y, z);
                        int cubeVertIdx = Utils.GetIndexFromVert(cubePosition, GridSize);

                        float[] cubeData = Utils.BuildCubeData(cubePosition, noiseMap, NoiseSize);
                        int cubeConfigIdx = Utils.GetCubeConfigIndex(cubeData, IsoSurfaceLevel);

                        AddCubeToMeshData(cubePosition, cubeConfigIdx, cubeData, IsoSurfaceLevel, Smooth, ref vertices, ref triangles);

                        OnCubeProcessed?.Invoke(this, cubeVertIdx);

                        if (0 == (cubeVertIdx % YieldDivisor)) {
                            await Task.Yield();
                        }
                    }
                }
            }

            OnMarchingCompleted?.Invoke(this, EventArgs.Empty);
        }

        public static void AddCubeToMeshData(Vector3 position, int configIndex, float[] cubeData, float isoSurfaceLevel, bool smooth, ref List<Vector3> vertices, ref List<int> triangles) {
            int[] triangleConfig = Tables.Triangles[configIndex];

            for (int edgeCheckIndex = 0; edgeCheckIndex < triangleConfig.Length; edgeCheckIndex++) {
                int edgeIndex = triangleConfig[edgeCheckIndex];

                if (-1 == edgeIndex) {
                    return;
                }

                int[] edgePair = Tables.EdgePairs[edgeIndex];

                Vector3 vert1 = Tables.CornerOffsets[edgePair[0]];
                Vector3 vert2 = Tables.CornerOffsets[edgePair[1]];

                Vector3 edgeVert = (vert1 + vert2) / 2f;

                if (smooth) {
                    float vert1Val = cubeData[edgePair[0]];
                    float vert2Val = cubeData[edgePair[1]];

                    float diff = vert2Val - vert1Val;
                    float offset = (isoSurfaceLevel - vert1Val) / diff;

                    edgeVert = vert1 + ((vert2 - vert1) * offset);
                }

                Vector3 vertPos = edgeVert + position;

                int vertIndex = vertices.IndexOf(vertPos);

                if (-1 == vertIndex) {
                    vertices.Add(vertPos);
                    triangles.Add(vertices.Count - 1);
                } else {
                    triangles.Add(vertIndex);
                }
            }
        }

    }

}