using codexhere.MarchingCubes;
using UnityEngine;

public class CubeNoise {
    public class TwoD {
        public static float[] GenNoise(Vector2Int size, Vector3 offset, float scale) {
            size.x++;
            size.y++;
            float[] noiseMap = new float[size.x * size.y * size.x];

            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    for (int z = 0; z < size.x; z++) {
                        int idx = Utils.GetIndexFromVert(new Vector3(x, y, z), size);

                        noiseMap[idx] = y - size.y * Mathf.PerlinNoise(
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