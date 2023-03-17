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
        builder?.Cancel();
    }

    private void OnDestroy() {
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
            Debug.Log(message: $"Job Completed in: {timer.Elapsed.TotalSeconds:F2}s");

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

        Debug.Log($"Ended Verts {builder.n_vertices.Length} ({(float)builder.n_vertices.Length / 3})");
        Debug.Log($"Ended Tris {builder.n_triangles.Length} ({(float)builder.n_triangles.Length / 3})");

        var tris = builder.n_triangles.ToArray();

        Debug.Log($"Ended Tris  2 {tris.Length} ({(float)tris.Length / 3})");

        mesh.vertices = builder.n_vertices.ToArray();
        mesh.triangles = builder.n_triangles.ToArray();

        meshFilter.mesh = mesh;

        return true;
    }
}
