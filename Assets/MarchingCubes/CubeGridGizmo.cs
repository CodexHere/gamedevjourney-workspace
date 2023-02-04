using codexhere.MarchingCubes;
using UnityEngine;

[ExecuteInEditMode]
public class CubeGridGizmo : MonoBehaviour {
    public int width = 8;
    public int height = 4;
    public float offsetDivisor = 24f;
    public float isoSurface = 0.5f;

    private float settingsCacheVal; // Junk just to gate updating unless user changes a value
    private float settingsVal = -1;

    float[] noiseMap;

    private void Update() {
        settingsCacheVal = width * height * isoSurface;

        if (settingsCacheVal == settingsVal) {
            return;
        }

        settingsCacheVal = settingsVal;

        noiseMap = CubeNoise2D.GenNoise(width, height, offsetDivisor);
    }

    private void OnDrawGizmos() {
        DrawOutline();

        for (int x = 0; x < ((width + 1) * (width + 1) * (height + 1)); x++) {
            float val = noiseMap[x];

            if (val > isoSurface) {
                continue;
            }

            Color clrVal = Color.Lerp(Color.white, Color.black, isoSurface - val);
            // Color clrVal = val < isoSurface ? Color.white : Color.black;
            Gizmos.color = clrVal;

            Vector3 vert = Utils.GetVertFromIndex(x, width + 1, height + 1);
            Gizmos.DrawSphere(vert, 0.1f);
        }
    }

    void DrawOutline() {
        Gizmos.color = Color.gray;
        Vector3 up = Vector3.up * height;
        Vector3 fwd = Vector3.forward * width;
        Vector3 right = Vector3.right * width;

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
}
