namespace codexhere.MarchingCubes.NoiseGen.Behaviors {
    public class NormalizeHeightNoiseGeneratorBehavior : BaseNoiseGeneratorBehavior {
        public NormalizeHeightNoiseGeneratorBehavior() => _generator = new NormalizeHeight();
    }
}