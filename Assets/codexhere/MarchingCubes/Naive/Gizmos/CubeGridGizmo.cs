using codexhere.MarchingCubes.NoiseGen;
using UnityEngine;
using UGizmos = UnityEngine.Gizmos;

namespace codexhere.MarchingCubes.Naive.Gizmos {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class CubeGridGizmo : MonoBehaviour {
        public Vector2Int Size = new Vector2Int(8, 4);

        public float Scale = 24f;
        public float IsoSurfaceLevel = 0.5f;
        public Vector3 offset;
        public bool Smooth = false;
        private float settingsCacheVal; // Junk just to gate updating unless user changes a value
        private float settingsVal = -1;

        private Vector2Int NoiseSize => Size + Vector2Int.one;

        private float[] noiseMap;
        private MarchingCubes marcher;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private void Awake() {
            Debug.Log("Initializing Marching Cubes Grid Gizmo");

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void Update() {
            settingsVal = Size.magnitude + IsoSurfaceLevel + Scale + offset.magnitude + (Smooth ? 1 : -1);

            if (settingsCacheVal == settingsVal) {
                return;
            }

            settingsCacheVal = settingsVal;

            float[] startMap = new float[(Size.x + 1) * (Size.y + 1) * (Size.x + 1)];

            noiseMap = new TwoD().GenNoise(startMap, Size, offset, Scale, 1);
            marcher = new MarchingCubes(transform.position, Size, IsoSurfaceLevel, Smooth);
            marcher.ClearMesh();
            marcher.MarchNoise(noiseMap);

            Mesh mesh = marcher.BuildMesh();

            if (Application.isEditor) {
                meshFilter.sharedMesh = mesh;
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            } else {
                meshFilter.mesh = mesh;
                meshCollider.sharedMesh = meshFilter.mesh;
            }
        }

        private void OnDrawGizmos() {
            DrawOutline();
            DrawCornerSpheres();
        }

        private void DrawOutline() {
            UGizmos.color = Color.gray;
            Vector3 up = Vector3.up * Size.y;
            Vector3 fwd = Vector3.forward * Size.x;
            Vector3 right = Vector3.right * Size.x;

            // Bottom Rect
            UGizmos.DrawLine(transform.position, transform.position + fwd);
            UGizmos.DrawLine(transform.position, transform.position + right);
            UGizmos.DrawLine(transform.position + fwd, transform.position + fwd + right);
            UGizmos.DrawLine(transform.position + right, transform.position + fwd + right);

            // Top Rect
            UGizmos.DrawLine(transform.position + up, transform.position + up + fwd);
            UGizmos.DrawLine(transform.position + up, transform.position + up + right);
            UGizmos.DrawLine(transform.position + up + fwd, transform.position + up + fwd + right);
            UGizmos.DrawLine(transform.position + up + right, transform.position + up + fwd + right);

            // Vert Lines
            UGizmos.DrawLine(transform.position, transform.position + up);
            UGizmos.DrawLine(transform.position + fwd, transform.position + up + fwd);
            UGizmos.DrawLine(transform.position + right, transform.position + up + right);
            UGizmos.DrawLine(transform.position + fwd + right, transform.position + up + fwd + right);
        }

        private void DrawCornerSpheres() {
            for (int x = 0; x < (NoiseSize.x * NoiseSize.x * NoiseSize.y); x++) {
                float val = noiseMap[x];

                Color clrVal = (val > IsoSurfaceLevel) ? new Color(1, 1, 1, 0.1f) : Color.Lerp(Color.white, Color.black, IsoSurfaceLevel - val);
                UGizmos.color = clrVal;
                Vector3 vert = Utils.GetVertFromIndex(x, NoiseSize);
                UGizmos.DrawSphere(vert, 0.1f);
            }
        }
    }
}