using codexhere.MarchingCubes;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobCreateCubeVectors : IJob {
    [ReadOnly]
    public Vector2Int NoiseSize;

    public NativeArray<Vector3> n_vertices;

    public void Execute() {
        for (int x = 0; x < NoiseSize.x; x++) {
            for (int y = 0; y < NoiseSize.y; y++) {
                for (int z = 0; z < NoiseSize.x; z++) {
                    Vector3 newVert = new(x, y, z);
                    int vertIdx = Utils.GetIndexFromVert(newVert, NoiseSize);
                    n_vertices[vertIdx] = newVert;
                }
            }
        }
    }
}