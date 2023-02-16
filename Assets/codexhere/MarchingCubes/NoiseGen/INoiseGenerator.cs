using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {
    public interface INoiseGenerator {
        float[] GenNoise(float[] previousValues, Vector2Int size, Vector3 offset, float scale, float octave);
    }
}