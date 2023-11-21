using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Naive {

    public class TwoD : INoiseGenerator {
        public float GenNoise(float previousValue, Vector3 noisePos, Vector2Int gridSize, NoiseBuilderOptions options) {
            float noiseValue = Mathf.PerlinNoise(
                ((noisePos.x + options.Offset.x) / options.Scale) + 0.001f,
                ((noisePos.z + options.Offset.z) / options.Scale) + 0.001f
            );

            previousValue += options.Octave * gridSize.y * noiseValue;

            return previousValue;
        }
    }
}
