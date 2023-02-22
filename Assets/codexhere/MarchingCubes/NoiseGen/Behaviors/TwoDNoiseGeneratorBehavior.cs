namespace codexhere.MarchingCubes.NoiseGen.Behaviors {
    public class TwoDNoiseGeneratorBehavior : BaseNoiseGeneratorBehavior {
        public TwoDNoiseGeneratorBehavior() {
            _generator = new TwoD();
        }
    }
}