using System.Threading.Tasks;
using codexhere.MarchingCubes.NoiseGen.Behaviors;
using UnityEngine;

namespace codexhere.MarchingCubes.Naive.Behaviors {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class CubeGridBehavior : MonoBehaviour {
        public Vector2Int GridSize = new(8, 4);

        [SerializeField] private float IsoSurfaceLevel = 0.5f;
        [SerializeField] private bool Smooth;
        [SerializeField] private bool Live;
        [SerializeField] private bool Refresh;
        [SerializeField] private NoiseBuilderBehavior noiseBuilder;
        private Task task_Generate;

        private float[] noiseMap;
        private MarchingCubes marcher;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private void Awake() {
            Debug.Log("Initializing Marching Cubes Grid Behavior");

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private async void Update() {
            if (null != task_Generate && false == task_Generate.IsCompleted) {
                Debug.Log("Running Task, skipping");
                return;
            }

            if (!Live && !Refresh) {
                return;
            }

            System.Diagnostics.Stopwatch timer = new();
            timer.Start();
            Debug.Log("Starting new Task...");

            task_Generate = GenerateNoiseAndMesh();
            await task_Generate;

            Refresh = false;

            timer.Stop();
            Debug.LogFormat("CubeGridBehavior Render Time: {0}", timer.Elapsed.TotalSeconds);
        }

        private async Task GenerateNoiseAndMesh() {
            System.Diagnostics.Stopwatch timer = new();
            timer.Start();

            Debug.Log("Starting Noise Gen:  " + timer.ElapsedMilliseconds);
            noiseMap = await noiseBuilder.BuildNoise(GridSize);
            Debug.Log("Ended Noise Gen: " + timer.ElapsedMilliseconds);

            timer.Restart();
            Debug.Log("Starting Mesh Gen: " + timer.ElapsedMilliseconds);
            await GenerateMesh();
            Debug.Log("Ended Mesh Gen: " + timer.ElapsedMilliseconds);

            timer.Stop();
        }

        private async Task GenerateMesh() {
            marcher = new MarchingCubes(transform.position, GridSize, IsoSurfaceLevel, Smooth);
            marcher.ClearMesh();
            await marcher.MarchNoise(noiseMap);

            await Task.Yield();

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