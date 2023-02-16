using codexhere.MarchingCubes.Naive.Behaviors;
using codexhere.Util;
using UnityEngine;

[ExecuteInEditMode]
public class OctreeVertBehavior : MonoBehaviour {
    [Range(0.1f, 2f)]
    public float MinimumSize = 1;

    private CubeGridBehavior cubeGrid;
    private MeshFilter meshFilter;
    private OctreePoint<Vector3> octree;

    private void Awake() {
        Debug.Log("Initializing Octree Vert Behavior");

        cubeGrid = GetComponent<CubeGridBehavior>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmos() {
        Mesh meshToUse = Application.isEditor ? meshFilter.sharedMesh : meshFilter.mesh;

        if (!meshToUse) {
            return;
        }

        Vector3[] verts = meshToUse.vertices;

        Debug.Log("There are " + verts.Length + " vertices in the mesh");

        octree = new OctreePoint<Vector3>(transform.position + (meshToUse.bounds.size / 2), cubeGrid.Size.x, MinimumSize);

        for (int vertIdx = 0; vertIdx < verts.Length; vertIdx++) {
            Vector3 vert = verts[vertIdx];
            octree.Insert(vert, vert);
        }

        DrawNodes(octree.RootNode);
    }

    private void DrawNodes(OctreePoint<Vector3>.Node node) {
        float percentDepth = node.Depth / (float)octree.MaxDepth;

        if (!node.IsLeaf) {
            foreach (OctreePoint<Vector3>.Node child in node.Children) {
                DrawNodes(child);
            }

            Color color = Color.grey;
            color.a = percentDepth;
            Gizmos.color = color;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        } else {
            if (null != node.Data) {
                Color color = Color.red;
                color.a = 0.15f;
                Gizmos.color = color;
                Gizmos.DrawCube(node.Position, Vector3.one * node.Size);
            } else {
                Color color = Color.green;
                color.a = percentDepth;
                Gizmos.color = color;
                Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
            }
        }
    }
}
