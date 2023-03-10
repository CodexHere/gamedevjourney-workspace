using System.Collections.Generic;
using codexhere.MarchingCubes;
using UnityEngine;

public class MarchingCubes {
    public Vector3 Origin { get; }
    public Vector2Int Size { get; }

    private readonly Mesh mesh = new Mesh();
    private readonly float IsoSurfaceLevel;
    private readonly bool Smooth;
    private List<int> triangles = new List<int>();
    private List<Vector3> vertices = new List<Vector3>();

    private Vector2Int NoiseSize => Size + Vector2Int.one;

    public MarchingCubes(Vector3 origin, Vector2Int size, float isoSurfaceLevel, bool smooth) {
        Origin = origin;
        Size = size;
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

    public void MarchNoise(float[] noiseMap) {
        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++) {
                for (int z = 0; z < Size.x; z++) {
                    Vector3 cubePosition = new Vector3(x, y, z);
                    float[] cubeData = BuildCubeData(cubePosition, noiseMap);
                    int cubeConfigIdx = GetCubeConfigIndex(cubeData);

                    AddCubeToMeshData(cubePosition, cubeConfigIdx, cubeData, IsoSurfaceLevel, Smooth, ref vertices, ref triangles);
                }
            }
        }
    }

    private int GetCubeConfigIndex(float[] cubeData) {
        int configIndex = 0;

        for (int cornerIdx = 0; cornerIdx < cubeData.Length; cornerIdx++) {
            if (IsoSurfaceLevel <= cubeData[cornerIdx]) {
                configIndex |= 1 << cornerIdx;
            }
        }

        return configIndex;
    }

    private float[] BuildCubeData(Vector3 cubePosition, float[] noiseMap) {
        float[] cubeData = new float[8];

        for (int cornerIdx = 0; cornerIdx < Tables.CornerOffsets.Length; cornerIdx++) {
            Vector3 cornerPos = cubePosition + Tables.CornerOffsets[cornerIdx];
            int cornerValIdx = Utils.GetIndexFromVert(cornerPos, NoiseSize);
            cubeData[cornerIdx] = noiseMap[cornerValIdx];
        }

        return cubeData;
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

            vertices.Add(edgeVert + position);
            triangles.Add(vertices.Count - 1);
        }
    }

}
