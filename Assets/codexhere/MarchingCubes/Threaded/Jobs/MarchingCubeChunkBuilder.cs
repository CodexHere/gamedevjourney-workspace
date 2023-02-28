using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class MarchingCubeChunkBuilder : JobQueueBuilder {
    private Vector2Int gridSize;

    private Vector2Int NoiseSize => gridSize + Vector2Int.one;
    private int NoiseSizeLength {
        get {
            try {
                checked {
                    return NoiseSize.x * NoiseSize.x * NoiseSize.y;
                }
            } catch {
                Debug.Log("Supplied GridSize is too large, try a smaller size.");
            }

            return -1;
        }
    }

    // todo: make private
    public NativeArray<Vector3> n_cubeVerts;

    public MarchingCubeChunkBuilder(Vector2Int gridSize) {
        this.gridSize = gridSize;

        n_cubeVerts = new(NoiseSizeLength, Allocator.Persistent);

        disposableItems = new() {
            n_cubeVerts
        };
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
