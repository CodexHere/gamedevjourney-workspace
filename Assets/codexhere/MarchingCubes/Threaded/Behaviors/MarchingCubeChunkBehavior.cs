using System;
using System.Diagnostics;
using System.Threading.Tasks;
using codexhere.MarchingCubes.NoiseGen;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MarchingCubeChunkBehavior : MonoBehaviour {
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float IsoSurfaceLevel;
    [SerializeField] private bool refresh;

    private MarchingCubeChunkBuilder builder;

    private Stopwatch timer;
    private MeshFilter meshFilter;

    private void OnDisable() {
        Debug.Log("OnDisable Called");
        builder?.Cancel();
    }

    private void OnDestroy() {
        Debug.Log("OnDestroy Called");
        builder?.Cancel();
    }

    private void Update() {
        if (!refresh) {
            return;
        }

        refresh = false;

        timer = new();
        timer.Start();
        Debug.Log("Running ChunkBuilder");

        try {
            builder?.Dispose();
            builder = new(gridSize);
            builder.Build(IsoSurfaceLevel, new NoiseBuilderOptions() {
                Octave = 1,
                Offset = Vector3.zero,
                Scale = 1
            });
        } catch (OverflowException e) {
            Debug.LogException(exception: e, this);
        }
    }

    private async void LateUpdate() {
        if (null == builder) {
            return;
        }

        if (builder.Complete()) {
            timer.Stop();
            Debug.Log($"Job Completed in: {timer.Elapsed.TotalSeconds:F2}s");
            Debug.Log("Completed Job # of Items is: " + builder.n_cubeConfigurations[1]);

            _ = await BuildMesh();

            builder.Dispose();
            builder = null;
        } else {
            // Debug.Log("Still working on job!");
        }
    }

    async Task<bool> BuildMesh() {
        await Task.Yield();

        Mesh mesh = new();

        meshFilter = GetComponent<MeshFilter>();

        mesh.triangles = builder.n_triangles.ToArray();
        mesh.vertices = builder.n_vertices.ToArray();

        meshFilter.mesh = mesh;

        return true;
    }
}
