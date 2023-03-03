using codexhere.MarchingCubes;
using codexhere.MarchingCubes.NoiseGen;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobCreateScalarField : IJobParallelFor {
    [ReadOnly]
    public Vector2Int NoiseSize;
    [ReadOnly]
    public NoiseBuilderOptions noiseOptions;

    public NativeArray<float> n_scalarField;

    public void Execute(int index) {
        Vector3 noisePos = Utils.GetVertFromIndex(index, NoiseSize);

        float noiseValue = Mathf.PerlinNoise(
            ((noisePos.x + noiseOptions.Offset.x) / noiseOptions.Scale) + 0.001f,
            ((noisePos.z + noiseOptions.Offset.z) / noiseOptions.Scale) + 0.001f
        );

        n_scalarField[index] = noiseOptions.Octave * NoiseSize.y * noiseValue;
    }
}
