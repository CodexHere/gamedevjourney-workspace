using codexhere.MarchingCubes.NoiseGen;
using UnityEngine;

namespace codexhere.MarchingCubes.Naive.Behaviors {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class CubeGridBehavior : MonoBehaviour {
        public Vector2Int Size = new Vector2Int(8, 4);

        public NoiseGeneratorScriptableObject[] noiseGeneratorList;

        [SerializeField] private float IsoSurfaceLevel = 0.5f;
        [SerializeField] private bool Smooth;
        [SerializeField] private bool Live;
        [SerializeField] private bool Refresh;

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
            if (!Live && Refresh == false) {
                return;
            }

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            Refresh = false;

            noiseMap = new float[(Size.x + 1) * (Size.y + 1) * (Size.x + 1)];

            foreach (var noiseGenerator in noiseGeneratorList) {
                noiseMap = noiseGenerator.GenNoise(noiseMap, Size);
            }

            marcher = new MarchingCubes(transform.position, Size, IsoSurfaceLevel, Smooth);
            marcher.ClearMesh();
            marcher.MarchNoise(noiseMap);

            Mesh mesh = marcher.BuildMesh();

            if (Application.isEditor) {
                meshFilter.sharedMesh = mesh;
            } else {
                meshFilter.mesh = mesh;
            }

            meshCollider.sharedMesh = mesh;

            timer.Stop();
            Debug.Log("CubeGridBehavior Render Time: " + timer.ElapsedMilliseconds);
        }
    }
}