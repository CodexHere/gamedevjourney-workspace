using System;
using codexhere.MarchingCubes.NoiseGen;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {

    [Serializable]
    public abstract class AbstractNoiseGenerator : INoiseGenerator {
        public Vector2 Offset = Vector2.zero;
        [Range(0.1f, 100f)] public float Scale = 1;
        [Range(0.1f, 1f)] public float Octave = 1;

        public abstract float[] GenNoise(float[] previousValues, Vector2Int size, Vector3 offset, float scale, float octave);
    }
}