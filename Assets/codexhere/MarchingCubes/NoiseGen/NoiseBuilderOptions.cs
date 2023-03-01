using System;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Naive {
    [Serializable]
    public struct NoiseBuilderOptions {
        public Vector3 Offset;
        [Range(0.1f, 100f)] public float Scale;
        [Range(0.1f, 1f)] public float Octave;

        public NoiseBuilderOptions(Vector3 offset, float scale = 1, float octave = 1) => (Offset, Scale, Octave) = (offset, scale, octave);
    }

}