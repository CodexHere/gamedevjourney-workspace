using codexhere.MarchingCubes;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobDetermineCubeConfigs : IJobParallelFor {
    [ReadOnly]
    public NativeArray<float> n_scalarField;
    [ReadOnly]
    public Vector2Int GridSize;
    [ReadOnly]
    public Vector2Int NoiseSize;
    [ReadOnly]
    public float IsoSurfaceLevel;

    public NativeArray<CubeConfiguration> n_cubeConfigurations;

    public void Execute(int index) {
        Vector3 cubePosition = Utils.GetVertFromIndex(index, GridSize);
        FixedList64Bytes<float> cubeData = BuildCubeData(cubePosition);
        int cubeConfigIndex = GetCubeConfigIndex(cubeData, IsoSurfaceLevel);

        n_cubeConfigurations[index] = new CubeConfiguration() {
            cubePosition = cubePosition,
            cubeData = cubeData,
            configIndex = cubeConfigIndex
        };
    }

    public FixedList64Bytes<float> BuildCubeData(Vector3 cubePosition) {
        FixedList64Bytes<float> cubeData = new();

        for (int cornerIdx = 0; cornerIdx < Tables.CornerOffsets.Length; cornerIdx++) {
            Vector3 cornerPos = cubePosition + Tables.CornerOffsets[cornerIdx];
            int cornerValIdx = Utils.GetIndexFromVert(cornerPos, NoiseSize);
            cubeData.Add(n_scalarField[cornerValIdx]);
        }

        return cubeData;
    }

    public int GetCubeConfigIndex(FixedList64Bytes<float> cubeData, float IsoSurfaceLevel) {
        int configIndex = 0;

        for (int cornerIdx = 0; cornerIdx < cubeData.Length; cornerIdx++) {
            if (IsoSurfaceLevel <= cubeData[cornerIdx]) {
                configIndex |= 1 << cornerIdx;
            }
        }

        return configIndex;
    }
}
