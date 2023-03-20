using System;
using codexhere.MarchingCubes.NoiseGen;
using codexhere.Unity.Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct CubeConfiguration {
    public Vector3 cubePosition; //TODO: is this needed? I'm not sure... might need for creating new verts offset by this position!!!!!!
    public FixedList64Bytes<float> cubeData;
    public int configIndex;
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

        n_vertices = new(5 * 4 * NoiseSizeLength, Allocator.Persistent);
        n_triangles = new(5 * 3 * NoiseSizeLength, Allocator.Persistent);
        n_scalarField = new(NoiseSizeLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        n_cubeConfigurations = new(GridSizeLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        disposableItems = new IDisposable[] {
            n_scalarField,
            n_cubeConfigurations,
            n_vertices,
            n_triangles
        };
    }

    public void Build(float IsoSurfaceLevel, NoiseBuilderOptions noiseOptions) {
        // JobCreateScalarField jobCreateScalarField = new() {
        //     // In
        //     noiseOptions = noiseOptions,
        //     NoiseSize = NoiseSize,
        //     // Out
        //     n_scalarField = n_scalarField
        // };

        // JobDetermineCubeConfigs jobDetermineCubeConfigs = new() {
        //     // In
        //     n_scalarField = n_scalarField,
        //     GridSize = gridSize,
        //     IsoSurfaceLevel = IsoSurfaceLevel,
        //     // Out
        //     n_cubeConfigurations = n_cubeConfigurations
        // };

        // FIXME: Remove this test hackery
        n_cubeConfigurations = new(10, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        var cubeData = new FixedList64Bytes<float> {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1
        };

        // Fake a bunch of configs
        for (var x = 0; x < GridSizeLength; x++) {
            n_cubeConfigurations[x] = new CubeConfiguration() {
                configIndex = x + 1,
                cubePosition = Vector3.zero,
                cubeData = cubeData
            };
        }

        JobConstructMesh jobConstructMesh = new() {
            // In
            NoiseSize = NoiseSize,
            IsoSurfaceLevel = IsoSurfaceLevel,
            n_cubeConfigurations = n_cubeConfigurations,
            // Out
            n_vertices = n_vertices,
            n_triangles = n_triangles
        };

        // jobHandle = jobCreateScalarField.Schedule(
        //     NoiseSizeLength,
        //     (int)InnerloopBatchCount.Count_8,
        //     jobHandle
        // );

        // jobHandle = jobDetermineCubeConfigs.Schedule(
        //     NoiseSizeLength,
        //     (int)InnerloopBatchCount.Count_8,
        //     jobHandle
        // );

        jobHandle = jobConstructMesh.Schedule(
            NoiseSizeLength,
            (int)InnerloopBatchCount.Count_8,
            jobHandle
        );

        JobHandle.ScheduleBatchedJobs();
    }
}
