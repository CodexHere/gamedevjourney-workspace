using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Naive {

    public class NoiseBuilder {
        public int YieldDivisor = 25_000;

        private readonly INoiseGenerator[] noiseGenerators;
        private readonly NoiseBuilderOptions[] noiseBuildOptions;
        private readonly Vector2Int gridSize;

        private Vector2Int NoiseSize => gridSize + Vector2Int.one;

        public NoiseBuilder(INoiseGenerator[] noiseGenerators, NoiseBuilderOptions[] noiseBuildOptions, Vector2Int gridSize)
            => (this.noiseGenerators, this.noiseBuildOptions, this.gridSize) = (noiseGenerators, noiseBuildOptions, gridSize);

        public async Task<float[]> BuildNoise() {
            float[] noiseMap = await GenerateNoise();

            noiseMap = await NormalizeNoise(noiseMap);

            return noiseMap;
        }

        private async Task<float[]> GenerateNoise() {
            float[] noiseMap = new float[NoiseSize.x * NoiseSize.y * NoiseSize.x];

            for (int x = 0; x < NoiseSize.x; x++) {
                for (int y = 0; y < NoiseSize.y; y++) {
                    for (int z = 0; z < NoiseSize.x; z++) {
                        // Build point definitions for where we are in our XYZ Grid Space
                        Vector3 noisePos = new(x, y, z);
                        int noiseIdx = Utils.GetIndexFromVert(noisePos, NoiseSize);

                        // Iterate over INoiseGenerator implementations and generate the final noise value for every point in Grid Space
                        for (int noiseGenIdx = 0; noiseGenIdx < noiseGenerators.Count(); noiseGenIdx++) {
                            INoiseGenerator generator = noiseGenerators[noiseGenIdx];
                            NoiseBuilderOptions options = noiseBuildOptions[noiseGenIdx];

                            noiseMap[noiseIdx] = generator.GenNoise(noiseMap[noiseIdx], noisePos, gridSize, options);
                        }

                        if (0 == (noiseIdx % YieldDivisor)) {
                            await Task.Yield();
                        }
                    }
                }
            }

            return noiseMap;
        }

        private async Task<float[]> NormalizeNoise(float[] noiseMap) {
            await Task.Yield();

            float octaveSums = noiseBuildOptions.Sum(noiseBuildOptions => noiseBuildOptions.Octave);

            for (int noiseIdx = 0; noiseIdx < noiseMap.Length; noiseIdx++) {
                // Normalize for the octaves to keep within 0..1
                noiseMap[noiseIdx] /= octaveSums;
                // Normalize to the Vertex position for IsoSurface evaluation while Marching
                Vector3 vert = Utils.GetVertFromIndex(noiseIdx, NoiseSize);
                noiseMap[noiseIdx] = vert.y - noiseMap[noiseIdx];
            }

            return noiseMap;

        }
    }

}