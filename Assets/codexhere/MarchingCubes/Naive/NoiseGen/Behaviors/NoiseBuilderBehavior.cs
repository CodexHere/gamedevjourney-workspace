using System;
using System.Threading.Tasks;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Naive.Behaviors {
    public class NoiseBuilderBehavior : MonoBehaviour {
        public async Task<float[]> BuildNoise(Vector2Int gridSize) {
            BaseNoiseGeneratorBehavior[] noiseGeneratorList = GetComponents<BaseNoiseGeneratorBehavior>();

            if (0 == noiseGeneratorList.Length) {
                throw new Exception("There needs to be at least 1 Noise Generator applied to the GameObject: " + name);
            }

            INoiseGenerator[] generators = new INoiseGenerator[0];
            NoiseBuilderOptions[] builderOptions = new NoiseBuilderOptions[0];
            int numFound = 0;

            for (int noiseGenIdx = 0; noiseGenIdx < noiseGeneratorList.Length; noiseGenIdx++) {
                BaseNoiseGeneratorBehavior generator = noiseGeneratorList[noiseGenIdx];

                if (generator.Enabled) {
                    numFound++;
                    Array.Resize(ref generators, numFound);
                    Array.Resize(ref builderOptions, numFound);
                    generators[numFound - 1] = generator.Generator;
                    builderOptions[numFound - 1] = noiseGeneratorList[noiseGenIdx].Options;
                }
            }

            if (0 == numFound) {
                throw new Exception("There needs to be at least 1 Noise Generator Enabled on the GameObject: " + name);
            }

            return await new NoiseBuilder(generators, builderOptions, gridSize).BuildNoise();
        }
    }
}
