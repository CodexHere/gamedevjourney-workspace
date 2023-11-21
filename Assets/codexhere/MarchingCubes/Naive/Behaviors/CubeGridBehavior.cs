using System;
using System.Threading;
using System.Threading.Tasks;
using codexhere.MarchingCubes.NoiseGen.Naive.Behaviors;
using codexhere.UI;
using UnityEngine;
using MSSystem = System;

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
        [SerializeField] private LoadingBarBehavior progressBar;
        [SerializeField] private Rigidbody characterController;

        private Task task_Generate;
        private float[] noiseMap;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        readonly CancellationTokenSource cancellationTokenSrc = new();

        private void Awake() {
            Debug.Log("Initializing Marching Cubes Grid Behavior");

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        private void OnDestroy() {
            cancellationTokenSrc.Cancel();
        }

        private async void Update() {
            bool firstTimeRunMode = Application.isPlaying && null == task_Generate;
            bool taskProcessing = null != task_Generate && !task_Generate.IsCompleted;
            bool forcedProcess = Live || Refresh;

            if (!firstTimeRunMode && (taskProcessing || !forcedProcess)) {
                return;
            }

            Refresh = false;

            progressBar.FadeTo(1);

            MSSystem.Diagnostics.Stopwatch timer = new();
            timer.Start();
            Debug.Log("Starting new Task...");

            task_Generate = GenerateNoiseAndMesh();
            await task_Generate;

            timer.Stop();
            Debug.LogFormat("CubeGridBehavior Render Time: {0}", timer.Elapsed.TotalSeconds);
        }

        private async Task GenerateNoiseAndMesh() {
            MSSystem.Diagnostics.Stopwatch timer = new();
            timer.Start();

            noiseMap = await noiseBuilder.BuildNoise(GridSize);
            Debug.Log("Noise Gen: " + timer.ElapsedMilliseconds);

            timer.Restart();
            await GenerateMesh();

            if (cancellationTokenSrc.IsCancellationRequested) {
                Debug.Log("Mesh Gen Cancelled");
            } else {
                Debug.Log($"Mesh Gen: {timer.ElapsedMilliseconds}");
            }

            timer.Stop();
        }

        private async Task GenerateMesh() {
            int totalVerts = GridSize.x * GridSize.x * GridSize.y - 1;
            MarchingCubes marcher = new(transform.position, GridSize, IsoSurfaceLevel, Smooth);

            // Events from the Marcher
            marcher.OnCubeProcessed += (object sender, int cubeNum) => progressBar.Percentage = cubeNum / (float)totalVerts * 100;
            marcher.OnMarchingCompleted += (object sender, EventArgs e) => {
                progressBar.FadeTo(0, 0.5f);
                characterController.useGravity = true;
            };

            marcher.ClearMesh();
            await marcher.MarchNoise(cancellationTokenSrc.Token, noiseMap);

            if (cancellationTokenSrc.IsCancellationRequested) {
                return;
            }

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