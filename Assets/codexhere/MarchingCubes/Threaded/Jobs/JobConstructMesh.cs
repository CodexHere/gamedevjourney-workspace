using codexhere.MarchingCubes;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobConstructMesh : IJobParallelFor {
    [ReadOnly]
    public Vector2Int GridSize;
    [ReadOnly]
    public float IsoSurfaceLevel;
    [ReadOnly]
    public NativeArray<CubeConfiguration> n_cubeConfigurations;

    public NativeList<Vector3>.ParallelWriter n_vertices;
    public NativeList<int>.ParallelWriter n_triangles;

    public void Execute(int index) {
        // TODO: Try to make this more optimized by only kicking off the right amount
        // We may have less cubeConfigurations than entire grid space, so we need to skip those
        if (index >= n_cubeConfigurations.Length) {
            return;
        }

        Vector3 cubePosition = Utils.GetVertFromIndex(index, GridSize);

        AddCubeToMeshData(cubePosition, index);
    }

    public void AddCubeToMeshData(Vector3 position, int index) {
        CubeConfiguration cubeConfig = n_cubeConfigurations[index];
        int[] triangleConfig = Tables.Triangles[cubeConfig.configIndex];

        for (int edgeCheckIndex = 0; edgeCheckIndex < triangleConfig.Length; edgeCheckIndex++) {
            int edgeIndex = triangleConfig[edgeCheckIndex];

            if (-1 == edgeIndex) {
                return;
            }

            int[] edgePair = Tables.EdgePairs[edgeIndex];

            Vector3 vert1 = Tables.CornerOffsets[edgePair[0]];
            Vector3 vert2 = Tables.CornerOffsets[edgePair[1]];

            Vector3 edgeVert;

            float vert1Val = cubeConfig.cubeData[edgePair[0]];
            float vert2Val = cubeConfig.cubeData[edgePair[1]];

            float diff = vert2Val - vert1Val;
            float offset = diff != 0 ? (IsoSurfaceLevel - vert1Val) / diff : 0.5f;

            edgeVert = vert1 + ((vert2 - vert1) * offset);

            Vector3 vertPos = edgeVert + position;

            int vertIndex = -1;
            // int vertIndex = n_vertices.IndexOf(vertPos);

            if (-1 == vertIndex) {
                n_vertices.AddNoResize(vertPos);
                n_triangles.AddNoResize(n_vertices.Length - 1);
            } else {
                n_triangles.Add(vertIndex);
            }
        }
    }
}
