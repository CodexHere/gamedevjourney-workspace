using System.Collections.Generic;
using codexhere.MarchingCubes;
using Unity.Mathematics;
using UnityEngine;

public class MarchingCubes {

    public Vector3 Origin { get; protected set; }
    public Vector2Int Size { get; protected set; }
    public float IsoSurfaceLevel { get; protected set; }
    public bool Smooth { get; protected set; }

    private readonly Mesh mesh = new Mesh();
    private List<int> triangles = new List<int>();
    private List<Vector3> vertices = new List<Vector3>();

    private Vector2Int NoiseSize => Size + Vector2Int.one;

    public MarchingCubes(Vector3 origin, Vector2Int size, float isoSurfaceLevel, bool smooth = true) {
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
                    Vector3Int cubePosition = new Vector3Int(x, y, z);
                    float[] cubeData = GenerateCubeData(cubePosition, noiseMap);
                    int configIndex = GetConfigIndex(cubeData);

                    AddCubeToMeshData(cubePosition, configIndex, ref vertices, ref triangles, Smooth);
                }
            }
        }
    }

    float[] GenerateCubeData(Vector3Int cubePosition, float[] noiseMap) {
        float[] cube = new float[8];
        Vector3Int cornerPos;

        for (int cornerIdx = 0; cornerIdx < Tables.Corners.Length; cornerIdx++) {
            cornerPos = cubePosition + Tables.Corners[cornerIdx];
            int idx = Utils.GetIndexFromVert(cornerPos, NoiseSize);
            cube[cornerIdx] = noiseMap[idx];
        }

        return cube;
    }

    int GetConfigIndex(float[] cubeData) {
        int configIndex = 0;

        for (int cornerIdx = 0; cornerIdx < cubeData.Length; cornerIdx++) {
            if (IsoSurfaceLevel <= cubeData[cornerIdx]) {
                configIndex |= 1 << cornerIdx;
            }
        }

        return configIndex;
    }

    public static void AddCubeToMeshData(Vector3 position, int configIndex, ref List<Vector3> vertices, ref List<int> triangles, bool smooth) {
        int[] triangleConfig = Tables.Triangles[configIndex];

        for (int edgeCheckIndex = 0; edgeCheckIndex < triangleConfig.Length; edgeCheckIndex++) {
            int edgeIndex = triangleConfig[edgeCheckIndex];

            if (-1 == edgeIndex) {
                return;
            }

            int[] edgePair = Tables.EdgePairs[edgeIndex];

            Vector3 vert1 = position + Tables.Corners[edgePair[0]];
            Vector3 vert2 = position + Tables.Corners[edgePair[1]];

            Vector3 edgeVert = (vert1 + vert2) / 2f;

            if (smooth) {

            }

            vertices.Add(edgeVert);
            triangles.Add(vertices.Count - 1);
        }
    }


}
