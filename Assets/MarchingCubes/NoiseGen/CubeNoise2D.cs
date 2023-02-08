using codexhere.MarchingCubes;
using UnityEngine;

public class CubeNoise {
    public class TwoD {
        public static float[] GenNoise(Vector2Int size, Vector3 offset, float scale) {
            Vector2Int noiseSize = size + Vector2Int.one;

            float[] noiseMap = new float[noiseSize.x * noiseSize.y * noiseSize.x];

            for (int x = 0; x < noiseSize.x; x++) {
                for (int y = 0; y < noiseSize.y; y++) {
                    for (int z = 0; z < noiseSize.x; z++) {
                        int idx = Utils.GetIndexFromVert(new Vector3(x, y, z), noiseSize);

                        noiseMap[idx] = y - (noiseSize.y * Mathf.PerlinNoise(
                            ((x + offset.x) / scale) + 0.001f,
                            ((z + offset.z) / scale) + 0.001f
                        ));
                    }
                }
            }

            return noiseMap;
        }
    }
}
