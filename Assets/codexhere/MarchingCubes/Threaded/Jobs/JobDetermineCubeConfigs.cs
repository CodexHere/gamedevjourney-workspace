using codexhere.MarchingCubes;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public struct JobDetermineCubeConfigs : IJobParallelFor {
    [ReadOnly]
    public NativeArray<float> n_scalarField;
    [ReadOnly]
    public Vector2Int GridSize;
    [ReadOnly]
    public float IsoSurfaceLevel;

    public NativeArray<CubeConfiguration> n_cubeConfigurations;

    public void Execute(int index) {
        Vector3 cubePosition = Utils.GetVertFromIndex(index, GridSize);
        UnsafeList<float> cubeData = BuildCubeData(cubePosition);
        int cubeConfigIndex = GetCubeConfigIndex(cubeData, IsoSurfaceLevel);

        n_cubeConfigurations[index] = new CubeConfiguration() {
            cubeData = cubeData,
            configIndex = cubeConfigIndex,
        };
    }

    public UnsafeList<float> BuildCubeData(Vector3 cubePosition) {
        UnsafeList<float> cubeData = new(8, Allocator.Temp);

        for (int cornerIdx = 0; cornerIdx < Tables.CornerOffsets.Length; cornerIdx++) {
            Vector3 cornerPos = cubePosition + Tables.CornerOffsets[cornerIdx];
            int cornerValIdx = Utils.GetIndexFromVert(cornerPos, GridSize);
            cubeData[cornerIdx] = n_scalarField[cornerValIdx];
        }

        return cubeData;
    }

    public int GetCubeConfigIndex(UnsafeList<float> cubeData, float IsoSurfaceLevel) {
        int configIndex = 0;

        for (int cornerIdx = 0; cornerIdx < cubeData.Length; cornerIdx++) {
            if (IsoSurfaceLevel <= cubeData[cornerIdx]) {
                configIndex |= 1 << cornerIdx;
            }
        }

        return configIndex;
    }
}
