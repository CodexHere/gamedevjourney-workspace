using System;
using codexhere.MarchingCubes.NoiseGen;
using UnityEngine;

namespace codexhere.MarchingCubes.Naive.Behaviors {

    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class CubeGridBehavior : MonoBehaviour {
        public Vector2Int Size = new Vector2Int(8, 4);

        [SerializeField] private float IsoSurfaceLevel = 0.5f;
        [SerializeField] private bool Smooth;
        [SerializeField] private bool Live;
        [SerializeField] private bool Refresh;

        [SerializeReference]
        private AbstractNoiseGenerator[] noiseGeneratorList = new AbstractNoiseGenerator[] { new TwoD() };
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

            GenerateNoise();
            GenerateMesh();

            timer.Stop();
            Debug.Log("CubeGridBehavior Render Time: " + timer.ElapsedMilliseconds);

            Refresh = false;
        }

        private void GenerateNoise() {
            // noiseGeneratorList = GetComponents<AbstractNoiseGenerator>();

            if (0 == noiseGeneratorList.Length) {
                throw new Exception("There needs to be at least 1 Noise Generator applied to the GameObject: " + name);
            }

            noiseMap = new float[(Size.x + 1) * (Size.y + 1) * (Size.x + 1)];

            for (int noiseIdx = 0; noiseIdx < noiseGeneratorList.Length; noiseIdx++) {
                noiseMap = noiseGeneratorList[noiseIdx].GenNoise(noiseMap, Size, Vector2.one, 13f, 1);
            }
        }

        private void GenerateMesh() {
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
        }

    }
}