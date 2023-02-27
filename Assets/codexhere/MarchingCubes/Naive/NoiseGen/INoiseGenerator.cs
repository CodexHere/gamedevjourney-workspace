using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Naive {
    public interface INoiseGenerator {
        float GenNoise(float previousValue, Vector3 noisePos, Vector2Int size, NoiseBuilderOptions options);
    }
}