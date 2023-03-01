using codexhere.MarchingCubes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct JobConstructMesh : IJobParallelFor {
    [ReadOnly]
    public Vector2Int GridSize;
    [ReadOnly]
    public float IsoSurfaceLevel;
    [ReadOnly]
    public NativeArray<int> n_cubeConfigurations;
    [ReadOnly]
    public NativeArray<NativeArray<float>> n_cubeDatas;

    public NativeList<Vector3> n_vertices;
    public NativeList<int> n_triangles;

    public void Execute(int index) {
        Vector3 cubePosition = Utils.GetVertFromIndex(index, size: GridSize);

        AddCubeToMeshData(cubePosition, index);
    }

    public void AddCubeToMeshData(Vector3 position, int index) {
        int[] triangleConfig = Tables.Triangles[n_cubeConfigurations[index]];

        for (int edgeCheckIndex = 0; edgeCheckIndex < triangleConfig.Length; edgeCheckIndex++) {
            int edgeIndex = triangleConfig[edgeCheckIndex];

            if (-1 == edgeIndex) {
                return;
            }

            int[] edgePair = Tables.EdgePairs[edgeIndex];

            Vector3 vert1 = Tables.CornerOffsets[edgePair[0]];
            Vector3 vert2 = Tables.CornerOffsets[edgePair[1]];

            Vector3 edgeVert;

            float vert1Val = n_cubeDatas[index][edgePair[0]];
            float vert2Val = n_cubeDatas[index][edgePair[1]];

            float diff = vert2Val - vert1Val;
            float offset = (IsoSurfaceLevel - vert1Val) / diff;

            edgeVert = vert1 + ((vert2 - vert1) * offset);

            Vector3 vertPos = edgeVert + position;

            int vertIndex = n_vertices.IndexOf(vertPos);

            if (-1 == vertIndex) {
                n_vertices.Add(vertPos);
                n_triangles.Add(n_vertices.Length - 1);
            } else {
                n_triangles.Add(vertIndex);
            }
        }
    }
}
