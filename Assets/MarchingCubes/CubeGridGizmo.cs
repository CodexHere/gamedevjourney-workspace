using codexhere.MarchingCubes;
using UnityEngine;

[ExecuteInEditMode]
public class CubeGridGizmo : MonoBehaviour {
    public Vector2Int Size = new Vector2Int(8, 4);
    public float Scale = 24f;
    public float IsoSurfaceLevel = 0.5f;
    public Vector2Int offset = new Vector2Int(0, 0);
    public MeshFilter meshFilter;

    private float settingsCacheVal; // Junk just to gate updating unless user changes a value
    private float settingsVal = -1;

    private Vector2Int NoiseSize => Size + Vector2Int.one;

    private float[] noiseMap;
    private MarchingCubes marcher;

    private void Awake() {
        Debug.Log("Initializing Marching Cubes Grid Gizmo");

        meshFilter = gameObject.GetComponent<MeshFilter>();
    }

    private void Update() {
        // Simple value-cache check to see if we should update to avoid over processing
        settingsVal = Size.magnitude + offset.magnitude + IsoSurfaceLevel + Scale;
        if (settingsCacheVal == settingsVal) {
            return;
        }
        settingsCacheVal = settingsVal;

        // Get the noisemap
        noiseMap = CubeNoise.TwoD.GenNoise(Size, offset, Scale);

        marcher = new MarchingCubes(transform.position, Size, IsoSurfaceLevel);
        marcher.MarchNoise(noiseMap);

        Mesh mesh = marcher.BuildMesh();
        meshFilter.mesh = mesh;
    }

    private void OnDrawGizmos() {
        DrawOutline();

        DrawCornerSpheres();
    }

    private void DrawOutline() {
        Gizmos.color = Color.gray;
        Vector3 up = Vector3.up * Size.y;
        Vector3 fwd = Vector3.forward * Size.x;
        Vector3 right = Vector3.right * Size.x;

        // Bottom Rect
        Gizmos.DrawLine(transform.position, transform.position + fwd);
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position + fwd, transform.position + fwd + right);
        Gizmos.DrawLine(transform.position + right, transform.position + fwd + right);

        // Top Rect
        Gizmos.DrawLine(transform.position + up, transform.position + up + fwd);
        Gizmos.DrawLine(transform.position + up, transform.position + up + right);
        Gizmos.DrawLine(transform.position + up + fwd, transform.position + up + fwd + right);
        Gizmos.DrawLine(transform.position + up + right, transform.position + up + fwd + right);

        // Vert Lines
        Gizmos.DrawLine(transform.position, transform.position + up);
        Gizmos.DrawLine(transform.position + fwd, transform.position + up + fwd);
        Gizmos.DrawLine(transform.position + right, transform.position + up + right);
        Gizmos.DrawLine(transform.position + fwd + right, transform.position + up + fwd + right);
    }

    private void DrawCornerSpheres() {
        if (null == noiseMap || 0 == noiseMap.Length) {
            return;
        }

        for (int x = 0; x < (NoiseSize.x * NoiseSize.x * NoiseSize.y); x++) {
            float val = noiseMap[x];

            if (IsoSurfaceLevel <= val) {
                continue;
            }

            Gizmos.color = Color.cyan;

            Vector3 vert = Utils.GetVertFromIndex(x, NoiseSize);
            Gizmos.DrawSphere(vert, 0.1f);
        }
    }
}
