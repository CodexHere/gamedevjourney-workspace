using System.Collections.Generic;
using codexhere.MarchingCubes;
using Unity.Mathematics;
using UnityEngine;

public class MarchingCubes {
    private readonly Mesh mesh = new Mesh();
    private readonly List<int> triangles = new List<int>();
    private readonly List<Vector3> vertices = new List<Vector3>();

    public Vector3 Origin { get; protected set; }

    public MarchingCubes(Vector3 origin) {
        Origin = origin;
    }

    public void ClearMesh() {
        vertices.Clear();
        triangles.Clear();
    }

    public Mesh BuildMesh() {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    public void MarchNoise(float[] noiseMap) {
        // for (int x = 0; x < width + 1; x++) {
        //     for (int y = 0; y < height + 1; y++) {
        //         for (int z = 0; z < depth + 1; z++) {
        //             int idx = Utils.GetIndexFromVert(x, y, z, width, height, depth);

        //             noiseMap[idx] = height * Mathf.PerlinNoise((float)x / offsetDivisor + 0.001f, (float)y / offsetDivisor + 0.001f);
        //         }
        //     }
        // }
    }

    public static void AddCubeToMeshData(Vector3 position, int configIndex, ref List<Vector3> vertices, ref List<int> triangles) {
        int[] triangleConfig = Tables.Triangles[configIndex];

        for (int edgeCheckIndex = 0; edgeCheckIndex < triangleConfig.Length; edgeCheckIndex++) {
            int edgeIndex = triangleConfig[edgeCheckIndex];

            if (-1 == edgeIndex) {
                return;
            }

            int[] edgePair = Tables.EdgePairs[edgeIndex];

            Vector3 vert1 = Tables.Corners[edgePair[0]];
            Vector3 vert2 = Tables.Corners[edgePair[1]];

            Vector3 edgeVert = (vert1 + vert2) / 2f;

            vertices.Add(edgeVert - position);
            triangles.Add(vertices.Count - 1);
        }
    }


}
