using codexhere.MarchingCubes;
using UnityEngine;

public class CubeNoise {
    public class TwoD {
        public static float[] GenNoise(int width, int height, Vector3 offset, float scale) {
            width++;
            height++;
            float[] noiseMap = new float[width * height * width];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < width; z++) {
                        int idx = Utils.GetIndexFromVert(x, y, z, width, height);

                        noiseMap[idx] = y - height * Mathf.PerlinNoise(
                            ((x + offset.x) / scale) + 0.001f,
                            ((z + offset.z) / scale) + 0.001f
                        );
                    }
                }
            }

            return noiseMap;
        }
    }
}