using codexhere.MarchingCubes;
using UnityEngine;

[ExecuteInEditMode]
public class CubeGridGizmo : MonoBehaviour {
    public Vector2Int Size = new Vector2Int(8, 4);

    public float Scale = 24f;
    public float IsoSurfaceLevel = 0.5f;
    public Vector3 offset;

    float settingsCacheVal; // Junk just to gate updating unless user changes a value
    float settingsVal = -1;
    Vector2Int NoiseSize => Size + Vector2Int.one;
    float[] noiseMap;
    MarchingCubes marcher;
    MeshFilter meshFilter;
    private MeshCollider meshCollider;

    void Awake() {
        Debug.Log("Initializing Marching Cubes Grid Gizmo");

        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update() {
        settingsVal = Size.magnitude + IsoSurfaceLevel + Scale + offset.magnitude;

        if (settingsCacheVal == settingsVal) {
            return;
        }

        settingsCacheVal = settingsVal;

        noiseMap = CubeNoise.TwoD.GenNoise(Size, offset, Scale);
        marcher = new MarchingCubes(transform.position, Size, IsoSurfaceLevel);
        marcher.ClearMesh();
        marcher.MarchNoise(noiseMap);
        meshFilter.mesh = marcher.BuildMesh();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    void OnDrawGizmos() {
        DrawOutline();
        DrawCornerSpheres();
    }

    void DrawOutline() {
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

    void DrawCornerSpheres() {
        for (int x = 0; x < (NoiseSize.x * NoiseSize.x * NoiseSize.y); x++) {
            float val = noiseMap[x];

            Color clrVal = (val > IsoSurfaceLevel) ? new Color(1, 1, 1, 0.1f) : Color.Lerp(Color.white, Color.black, IsoSurfaceLevel - val);
            Gizmos.color = clrVal;
            Vector3 vert = Utils.GetVertFromIndex(x, NoiseSize);
            Gizmos.DrawSphere(vert, 0.1f);
        }
    }
}
