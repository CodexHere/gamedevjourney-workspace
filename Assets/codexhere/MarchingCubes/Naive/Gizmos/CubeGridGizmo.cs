using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using codexhere.MarchingCubes.NoiseGen.Naive;
using codexhere.MarchingCubes.NoiseGen.Naive.Behaviors;
using UnityEngine;
using MSSystem = System;
using UGizmos = UnityEngine.Gizmos;

namespace codexhere.MarchingCubes.Naive.Gizmos {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(TwoDNoiseGeneratorBehavior))]
    public class CubeGridGizmo : MonoBehaviour {

        public Vector2Int GridSize;
        public float IsoSurfaceLevel = 0.5f;
        public bool Smooth = false;
        public bool Live = false;
        public bool Refresh = false;
        private Task<float[]> task;

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

        private async void Update() {
            if (!Live && !Refresh || null != task) {
                return;
            }

            MSSystem.Diagnostics.Stopwatch timer = new();
            timer.Start();
            Debug.Log("Starting new Task...");

            Refresh = false;

            BaseNoiseGeneratorBehavior[] noiseGenerators = GetComponents<BaseNoiseGeneratorBehavior>();

            NoiseBuilder builder = new(
                noiseGenerators.Select(n => n.Generator).ToArray(),
                noiseGenerators.Select(n => n.Options).ToArray(),
                GridSize
            );

            task = builder.BuildNoise();

            noiseMap = await task;
            marcher = new MarchingCubes(transform.position, GridSize, IsoSurfaceLevel, Smooth);
            marcher.ClearMesh();
            await marcher.MarchNoise(new CancellationToken(), noiseMap);

            Mesh mesh = marcher.BuildMesh();

            if (Application.isEditor) {
                meshFilter.sharedMesh = mesh;
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            } else {
                meshFilter.mesh = mesh;
                meshCollider.sharedMesh = meshFilter.mesh;
            }

            timer.Stop();
            Debug.LogFormat("CubeGridGizmo Render Time: {0}", timer.Elapsed.TotalSeconds);

            task = null;
        }

        private void OnDrawGizmos() {
            DrawOutline();
            DrawVertexSpheres();
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

        private void DrawVertexSpheres() {
            if (null == noiseMap) {
                return;
            }

            for (int index = 0; index < (NoiseSize.x * NoiseSize.x * NoiseSize.y); index++) {
                float val = noiseMap[index];

                Color clrVal = Color.Lerp(Color.black, Color.white, val - IsoSurfaceLevel);
                float sphereSize = Mathf.Lerp(0.1f, 0.025f, Mathf.Abs(val - IsoSurfaceLevel));

                UGizmos.color = clrVal;
                Vector3 vert = Utils.GetVertFromIndex(index, NoiseSize);
                UGizmos.DrawSphere(vert, sphereSize);
            }
        }
    }
}