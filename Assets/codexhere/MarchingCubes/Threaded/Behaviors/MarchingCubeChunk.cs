using UnityEngine;

[ExecuteInEditMode]
public class MarchingCubeChunk : MonoBehaviour {
    [SerializeField] private Vector2Int gridSize;

    [SerializeField] private bool refresh;

    MarchingCubeChunkBuilder builder;

    private void OnDestroy() {
        builder?.Cancel();
    }

    private void Update() {
        if (!refresh) {
            return;
        }

        Debug.Log("Running ChunkBuilder");

        refresh = false;

        builder = new(gridSize);

        builder.Build();
    }

    private void LateUpdate() {
        builder?.Complete();
    }
}
