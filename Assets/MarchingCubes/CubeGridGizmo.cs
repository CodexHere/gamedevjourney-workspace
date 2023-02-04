using UnityEngine;

[ExecuteInEditMode]
public class CubeGridGizmo : MonoBehaviour {
    public int width = 8;
    public int height = 4;
    public float offsetDivisor = 24f;
    public float isoSurface = 0.5f;

    private bool needsRender = true;
    private MeshFilter meshFilter;
    private MarchingCubes marcher;

    private void Awake() {
        Debug.Log("Initializing Marching Cubes Gizmo");
        meshFilter = gameObject.GetComponent<MeshFilter>();
        marcher = new MarchingCubes(transform.position);

        float[] noiseMap = CubeNoise2D.GenNoise(width, height, offsetDivisor);

        marcher.MarchNoise(noiseMap);
    }

    private void Update() {
        if (needsRender) {
            Debug.Log("Rendering configIndex: " + configIndex);

            needsRender = false;
            configIndex = Mathf.Clamp(configIndex, 0, 255);

            marcher.ClearMesh();
            marcher.MarchCube(marcher.Origin, configIndex);
            Mesh mesh = marcher.BuildMesh();

            meshFilter.mesh = mesh;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (transform.localScale / 2), transform.localScale);
    }
}
