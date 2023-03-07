using System;
using codexhere.MarchingCubes.NoiseGen;
using codexhere.Unity.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct CubeConfiguration {
    public int configIndex;
    public NativeArray<float> cubeData;
}

public class MarchingCubeChunkBuilder : JobQueueBuilder {
    private Vector2Int gridSize;

    public NativeArray<float> n_scalarField;
    public NativeArray<CubeConfiguration> n_cubeConfigurations;

    public NativeList<Vector3> n_vertices;
    public NativeList<int> n_triangles;

    private int GridSizeLength {
        get {
            try {
                checked {
                    return gridSize.x * gridSize.x * gridSize.y;
                }
            } catch {
                throw new OverflowException("Supplied GridSize is too large, try a smaller size.");
            }
        }
    }

    private Vector2Int NoiseSize => gridSize + Vector2Int.one;
    private int NoiseSizeLength {
        get {
            try {
                checked {
                    return NoiseSize.x * NoiseSize.x * NoiseSize.y;
                }
            } catch {
                throw new OverflowException("Supplied GridSize is too large, try a smaller size.");
            }
        }
    }

    public MarchingCubeChunkBuilder(Vector2Int gridSize) {
        this.gridSize = gridSize;

        n_vertices = new(Allocator.Persistent);
        n_triangles = new(Allocator.Persistent);
        n_scalarField = new(NoiseSizeLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        n_cubeConfigurations = new(NoiseSizeLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        disposableItems = new IDisposable[] {
            n_scalarField,
            n_cubeConfigurations,
            n_vertices,
            n_triangles
        };
    }

    public void Build(float IsoSurfaceLevel, NoiseBuilderOptions noiseOptions) {
        JobCreateScalarField jobCreateScalarField = new() {
            // In
            noiseOptions = noiseOptions,
            NoiseSize = NoiseSize,
            // Out
            n_scalarField = n_scalarField
        };

        JobDetermineCubeConfigs jobDetermineCubeConfigs = new() {
            // In
            n_scalarField = n_scalarField,
            GridSize = gridSize,
            IsoSurfaceLevel = IsoSurfaceLevel,
            // Out
            n_cubeConfigurations = n_cubeConfigurations
        };

        JobConstructMesh jobConstructMesh = new() {
            // In
            GridSize = gridSize,
            IsoSurfaceLevel = IsoSurfaceLevel,
            n_cubeConfigurations = n_cubeConfigurations,
            // Out
            n_vertices = n_vertices,
            n_triangles = n_triangles
        };

        jobHandle = jobCreateScalarField.Schedule(
            NoiseSizeLength,
            (int)InnerloopBatchCount.Count_8,
            jobHandle
        );

        jobHandle = jobDetermineCubeConfigs.Schedule(
            NoiseSizeLength,
            (int)InnerloopBatchCount.Count_8,
            jobHandle
        );

        jobHandle = jobConstructMesh.Schedule(
            GridSizeLength,
            (int)InnerloopBatchCount.Count_8,
            jobHandle
        );

        JobHandle.ScheduleBatchedJobs();
    }
}
