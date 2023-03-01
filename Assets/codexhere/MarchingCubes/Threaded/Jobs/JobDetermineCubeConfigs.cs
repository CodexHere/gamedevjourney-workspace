using codexhere.MarchingCubes;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct JobDetermineCubeConfigs : IJobParallelFor {
    [ReadOnly]
    public NativeArray<float> n_scalarField;
    [ReadOnly]
    public Vector2Int GridSize;
    [ReadOnly]
    public float IsoSurfaceLevel;

    public NativeArray<int> n_cubeConfigurations;
    public NativeArray<NativeArray<float>> n_cubeDatas;

    public void Execute(int index) {
        Vector3 cubePosition = Utils.GetVertFromIndex(index, GridSize);
        NativeArray<float> cubeData = BuildCubeData(cubePosition);
        int cubeConfig = GetCubeConfigIndex(cubeData, IsoSurfaceLevel);

        n_cubeConfigurations[index] = cubeConfig;
        n_cubeDatas[index] = cubeData;
        
    }

    public NativeArray<float> BuildCubeData(Vector3 cubePosition) {
        NativeArray<float> cubeData = new(length: 8, allocator: Allocator.Temp);

        for (int cornerIdx = 0; cornerIdx < Tables.CornerOffsets.Length; cornerIdx++) {
            Vector3 cornerPos = cubePosition + Tables.CornerOffsets[cornerIdx];
            int cornerValIdx = Utils.GetIndexFromVert(cornerPos, GridSize);
            cubeData[cornerIdx] = n_scalarField[cornerValIdx];
        }

        return cubeData;
    }

    public int GetCubeConfigIndex(NativeArray<float> cubeData, float IsoSurfaceLevel) {
        int configIndex = 0;

        for (int cornerIdx = 0; cornerIdx < cubeData.Length; cornerIdx++) {
            if (IsoSurfaceLevel <= cubeData[cornerIdx]) {
                configIndex |= 1 << cornerIdx;
            }
        }

        return configIndex;
    }
}
