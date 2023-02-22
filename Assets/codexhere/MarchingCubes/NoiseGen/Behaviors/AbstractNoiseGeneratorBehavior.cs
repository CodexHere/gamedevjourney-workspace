using UnityEngine;

namespace codexhere.MarchingCubes.NoiseGen.Behaviors {

    public class BaseNoiseGeneratorBehavior : MonoBehaviour {
        public NoiseBuilderOptions Options;
        public INoiseGenerator Generator { get => _generator; }

        protected INoiseGenerator _generator;
    }
}