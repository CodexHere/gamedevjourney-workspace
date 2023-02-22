using codexhere.MarchingCubes.NoiseGen.Behaviors;
using UnityEngine;

namespace codexhere.MarchingCubes.Naive.Behaviors {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class CubeGridBehavior : MonoBehaviour {
        public Vector2Int GridSize = new Vector2Int(8, 4);

        [SerializeField] private float IsoSurfaceLevel = 0.5f;
        [SerializeField] private bool Smooth;
        [SerializeField] private bool Live;
        [SerializeField] private bool Refresh;
        [SerializeField] private NoiseBuilderBehavior noiseBuilder;

        private float[] noiseMap;
        private MarchingCubes marcher;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private void Awake() {
            Debug.Log("Initializing Marching Cubes Grid Behavior");

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void Update() {
            if (!Live && !Refresh) {
                return;
            }

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            GenerateNoise();
            GenerateMesh();

            timer.Stop();
            Debug.Log("CubeGridBehavior Render Time: " + timer.ElapsedMilliseconds);

            Refresh = false;
        }

        private void GenerateNoise() {
            noiseMap = noiseBuilder.BuildNoise(GridSize);
        }

        private void GenerateMesh() {
            marcher = new MarchingCubes(transform.position, GridSize, IsoSurfaceLevel, Smooth);
            marcher.ClearMesh();
            marcher.MarchNoise(noiseMap);

            Mesh mesh = marcher.BuildMesh();

            if (Application.isEditor) {
                meshFilter.sharedMesh = mesh;
            } else {
                meshFilter.mesh = mesh;
            }

            meshCollider.sharedMesh = mesh;
        }

    }
}