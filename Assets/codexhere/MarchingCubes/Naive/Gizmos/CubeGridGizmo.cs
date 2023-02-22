using System.Linq;
using codexhere.MarchingCubes.NoiseGen;
using codexhere.MarchingCubes.NoiseGen.Behaviors;
using UnityEngine;
using UGizmos = UnityEngine.Gizmos;

namespace codexhere.MarchingCubes.Naive.Gizmos {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(TwoDNoiseGeneratorBehavior))]
    [RequireComponent(typeof(NormalizeHeightNoiseGeneratorBehavior))]
    public class CubeGridGizmo : MonoBehaviour {

        public Vector2Int GridSize;
        public float IsoSurfaceLevel = 0.5f;
        public bool Smooth = false;
        public bool Live = false;
        public bool Refresh = false;

        private Vector2Int NoiseSize => GridSize + Vector2Int.one;

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
            if (!Live && !Refresh) {
                return;
            }

            Refresh = false;

            BaseNoiseGeneratorBehavior[] noiseGenerators = GetComponents<BaseNoiseGeneratorBehavior>();

            NoiseBuilder builder = new(
                noiseGenerators.Select(n => n.Generator).ToArray(),
                noiseGenerators.Select(n => n.Options).ToArray(),
                GridSize
            );

            noiseMap = builder.BuildNoise();
            marcher = new MarchingCubes(transform.position, GridSize, IsoSurfaceLevel, Smooth);
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
            Vector3 up = Vector3.up * GridSize.y;
            Vector3 fwd = Vector3.forward * GridSize.x;
            Vector3 right = Vector3.right * GridSize.x;

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