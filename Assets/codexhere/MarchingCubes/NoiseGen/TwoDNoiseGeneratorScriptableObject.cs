using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {
    [CreateAssetMenu(fileName = "TwoDNoiseGenerator", menuName = "CodexHere/MarchingCubes/TwoDNoiseGenerator", order = 1)]
    public class TwoDNoiseGeneratorScriptableObject : NoiseGeneratorScriptableObject {
        override public float[] GenNoise(float[] previousValues, Vector2Int size) {
            return new TwoD().GenNoise(previousValues, size, Offset, Scale, Octave);
        }
    }
}