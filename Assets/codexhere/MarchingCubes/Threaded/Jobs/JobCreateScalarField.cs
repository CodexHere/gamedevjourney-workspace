using codexhere.MarchingCubes;
using codexhere.MarchingCubes.NoiseGen;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct JobCreateScalarField : IJobParallelFor {
    [ReadOnly]
    public Vector2Int GridSize;
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

        // Normalize with noisePos.y
        //TODO: Move normalization to after all additive noise concepts. Take a look at the builder for the naive implementation.
        n_scalarField[index] = noisePos.y - (noiseOptions.Octave * GridSize.y * noiseValue);
    }
}
