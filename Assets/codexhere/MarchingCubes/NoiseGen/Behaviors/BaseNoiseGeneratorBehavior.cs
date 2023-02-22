using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Behaviors {

    public class BaseNoiseGeneratorBehavior : MonoBehaviour {
        public bool Enabled;
        public NoiseBuilderOptions Options;
        public INoiseGenerator Generator => _generator;

        protected INoiseGenerator _generator;
    }
}