using codexhere.MarchingCubes;
using UnityEngine;

public class CubeNoise2D {
    public static float[] GenNoise(int width, int height, float offsetDivisor = 24f) {
        int depth = width;
        float[] noiseMap = new float[width * height * depth];

        for (int x = 0; x < width + 1; x++) {
            for (int y = 0; y < height + 1; y++) {
                for (int z = 0; z < depth + 1; z++) {
                    int idx = Utils.GetIndexFromVert(x, y, z, width, height, depth);

                    noiseMap[idx] = height * Mathf.PerlinNoise((float)x / offsetDivisor + 0.001f, (float)y / offsetDivisor + 0.001f);
                }
            }
        }

        return noiseMap;
    }
}
