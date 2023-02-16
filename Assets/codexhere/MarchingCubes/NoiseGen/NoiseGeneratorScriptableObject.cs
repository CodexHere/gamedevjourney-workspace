using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {
    public class NoiseGeneratorScriptableObject : ScriptableObject {
        [SerializeField] protected float Scale = 24f;
        [SerializeField] protected Vector3 Offset;
        [SerializeField] protected float Octave = 1;

        public virtual float[] GenNoise(float[] previousValues, Vector2Int size) {
            return new float[(size.x + 1) * (size.y + 1) * (size.x + 1)];
        }
    }
}