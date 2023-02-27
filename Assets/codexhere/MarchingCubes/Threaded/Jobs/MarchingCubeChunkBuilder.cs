using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class MarchingCubeChunkBuilder : JobQueueBuilder {
    private Vector2Int gridSize;
    private NativeArray<Vector3> n_cubeVerts;

    // TODO: Consider encapsulating in the constructor
    private Vector2Int NoiseSize => gridSize + Vector2Int.one;
    private int NoiseSizeLength => NoiseSize.x * NoiseSize.x * NoiseSize.y;

    public MarchingCubeChunkBuilder(Vector2Int gridSize) {
        disposableItems = new() {
            n_cubeVerts
        };

        this.gridSize = gridSize;

        n_cubeVerts = new(NoiseSizeLength, Allocator.Persistent);
    }

    public void Build() {
        JobCreateCubeVectors jobCreateVectors = new() {
            n_vertices = n_cubeVerts,
            NoiseSize = NoiseSize
        };

        jobHandle = jobCreateVectors.Schedule(jobHandle);

        JobHandle.ScheduleBatchedJobs();
    }
}
