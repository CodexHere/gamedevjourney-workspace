using UnityEngine;

[ExecuteInEditMode]
public class CubeGizmo : MonoBehaviour {
    [SerializeField]
    [Range(0, 255)]
    private int configIndex = 0;
    private int lastRenderedConfigIndex = 1;
    private MeshFilter meshFilter;
    private MarchingCubes marcher;

    private void Awake() {
        Debug.Log("Initializing Marching Cubes Gizmo");
        meshFilter = gameObject.GetComponent<MeshFilter>();
        marcher = new MarchingCubes(transform.position);
    }

    private void Update() {
        if (lastRenderedConfigIndex != configIndex) {
            Debug.Log("Rendering configIndex: " + configIndex);

            lastRenderedConfigIndex = configIndex;
            configIndex = Mathf.Clamp(configIndex, 0, 255);

            marcher.ClearMesh();
            marcher.MarchCube(marcher.Origin, configIndex);
            Mesh mesh = marcher.BuildMesh();

            meshFilter.mesh = mesh;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (transform.localScale / 2), transform.localScale);
    }

}
