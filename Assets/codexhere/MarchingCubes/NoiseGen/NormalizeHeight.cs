using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen {
    /**
     * Normalizes Noise values relative to the Height position within the grid it's caluclated at.
     * This allows for actual cubical data to be calculated, and potentially allow for gaps in space.
     * Unlike a standard Heightmap, calculations for an IsoSurface value must be relative to the Y-component.
     *
     * As this only offsets based on NoisePos.y, all other injected parameters are ignored.
     **/
    public class NormalizeHeight : INoiseGenerator {
        public float GenNoise(float previousValue, Vector3 noisePos, Vector2Int gridSize, NoiseBuilderOptions options) {
            return noisePos.y - previousValue;
        }
    }
}
