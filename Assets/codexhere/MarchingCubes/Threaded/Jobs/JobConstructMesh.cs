using codexhere.MarchingCubes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct JobConstructMesh : IJobParallelFor {
    [ReadOnly]
    public Vector2Int NoiseSize;
    [ReadOnly]
    public float IsoSurfaceLevel;
    [ReadOnly]
    public NativeArray<CubeConfiguration> n_cubeConfigurations;

    [NativeDisableParallelForRestriction] public NativeList<Vector3> n_vertices;
    [NativeDisableParallelForRestriction] public NativeList<int> n_triangles;

    int _vertsPerGridRow;

    public void Execute(int index) {
        // TODO: Try to make this more optimized by only kicking off the right amount
        // We may have less cubeConfigurations than entire grid space, so we need to skip those
        if (index >= n_cubeConfigurations.Length) {
            return;
        }

        // A Grid row has 4 verts times the grid width, and a max of 5 triangles per cubic space within the grid
        _vertsPerGridRow = 4 * 5 * NoiseSize.x;

        Vector3 cubePosition = Utils.GetVertFromIndex(index, NoiseSize);

        AddCubeToMeshData(cubePosition, index);
    }

    public void AddCubeToMeshData(Vector3 position, int index) {
        CubeConfiguration cubeConfig = n_cubeConfigurations[index];
        int[] triangleConfig = Tables.Triangles[cubeConfig.configIndex];

        // TODO: Determine if this is necessary... Probably, but need to check
        // Reallocate more memory, if needed
        // if (n_vertices.Capacity < n_vertices.Length + GridSize.x * 5) {
        //     n_vertices.Capacity = n_vertices.Length + GridSize.x * _vertsPerGridRow;
        // }

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

            // int vertIndex = n_vertices.IndexOf(vertPos);

            // if (-1 == vertIndex) {
            //     n_vertices.Add(vertPos);
            //     n_triangles.Add(n_vertices.Length - 1);
            // } else {
            //     n_triangles.Add(vertIndex);
            // }

            //FIXME: This is the sloppy approach
            n_vertices.Add(vertPos);
            n_triangles.Add(n_vertices.Length - 1);

            // Debug.Log($"Adding {vertPos} to {n_vertices.Length - 1} for index {index}");
        }
    }
}
