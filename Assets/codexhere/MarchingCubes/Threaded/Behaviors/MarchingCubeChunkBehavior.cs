using UnityEngine;

[ExecuteInEditMode]
public class MarchingCubeChunkBehavior : MonoBehaviour {
    [SerializeField] private Vector2Int gridSize;

    [SerializeField] private bool refresh;
    private MarchingCubeChunkBuilder builder;

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

        Debug.Log("Running ChunkBuilder");

        refresh = false;

        builder?.Cancel();

        builder = new(gridSize);

        builder.Build();
    }

    private void LateUpdate() {
        if (null == builder) {
            return;
        }

        if (builder.IsCompleted) {
            Debug.Log(builder.n_cubeVerts.ToArray().Length);
        } else {
            Debug.Log("Still working on job!");
        }
    }
}
