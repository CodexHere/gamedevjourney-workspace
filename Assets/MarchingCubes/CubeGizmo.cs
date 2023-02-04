using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CubeGizmo : MonoBehaviour {
    [SerializeField]
    [Range(0, 255)]
    private int configIndex = 0;
    private int lastRenderedConfigIndex = 1;
    private MeshFilter meshFilter;
    private MarchingCubes marcher;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Mesh mesh;

    private void Awake() {
        Debug.Log("Initializing Marching Cubes Gizmo");
        meshFilter = gameObject.GetComponent<MeshFilter>();
        marcher = new MarchingCubes(transform.position);

        mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();

        meshFilter.mesh = mesh;
    }

    private void Update() {
        if (lastRenderedConfigIndex != configIndex) {
            Debug.Log("Rendering configIndex: " + configIndex);

            lastRenderedConfigIndex = configIndex;
            configIndex = Mathf.Clamp(configIndex, 0, 255);

            // Clear out the existing mesh data
            mesh.Clear();
            vertices.Clear();
            triangles.Clear();

            // Write new mesh data directly to running vert/triangle lists
            MarchingCubes.AddCubeToMeshData(marcher.Origin, configIndex, ref vertices, ref triangles);

            // Update Mesh
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (transform.localScale / 2), transform.localScale);
    }
}
