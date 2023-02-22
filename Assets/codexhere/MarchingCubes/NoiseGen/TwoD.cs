using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {

    public class TwoD : INoiseGenerator {
        public float GenNoise(float previousValue, Vector3 noisePos, Vector2Int gridSize, NoiseBuilderOptions options) {
            float noiseValue = options.Octave * Mathf.PerlinNoise(
                ((noisePos.x + options.Offset.x) / options.Scale) + 0.001f,
                ((noisePos.z + options.Offset.z) / options.Scale) + 0.001f
            );

            previousValue += noisePos.y - (gridSize.y * noiseValue);

            return previousValue;
        }
    }
}
