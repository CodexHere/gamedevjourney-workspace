using System;
using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Behaviors {
    public class NoiseBuilderBehavior : MonoBehaviour {
        public float[] BuildNoise(Vector2Int gridSize) {
            BaseNoiseGeneratorBehavior[] noiseGeneratorList = GetComponents<BaseNoiseGeneratorBehavior>();

            if (0 == noiseGeneratorList.Length) {
                throw new Exception("There needs to be at least 1 Noise Generator applied to the GameObject: " + name);
            }

            INoiseGenerator[] generators = new INoiseGenerator[noiseGeneratorList.Length];
            NoiseBuilderOptions[] builderOptions = new NoiseBuilderOptions[noiseGeneratorList.Length]; ;

            for (int noiseGenIdx = 0; noiseGenIdx < noiseGeneratorList.Length; noiseGenIdx++) {
                generators[noiseGenIdx] = noiseGeneratorList[noiseGenIdx].Generator;
                builderOptions[noiseGenIdx] = noiseGeneratorList[noiseGenIdx].Options;
            }

            return new NoiseBuilder(generators, builderOptions, gridSize).BuildNoise();
        }
    }
}
