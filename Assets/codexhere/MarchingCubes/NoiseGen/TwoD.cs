using System;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {
    [Serializable]
    public class TwoD : INoiseGenerator {
        public float age;

        public float[] GenNoise(float[] previousValues, Vector2Int size, Vector3 offset, float scale, float octave) {
            Vector2Int noiseSize = size + Vector2Int.one;

            for (int x = 0; x < noiseSize.x; x++) {
                for (int y = 0; y < noiseSize.y; y++) {
                    for (int z = 0; z < noiseSize.x; z++) {
                        int idx = Utils.GetIndexFromVert(new Vector3(x, y, z), noiseSize);

                        float noiseValue = octave * Mathf.PerlinNoise(
                            ((x + offset.x) / scale) + 0.001f,
                            ((z + offset.z) / scale) + 0.001f
                        );

                        previousValues[idx] = previousValues[idx] + y - (noiseSize.y * noiseValue);
                    }
                }
            }

            return previousValues;
        }
    }
}
