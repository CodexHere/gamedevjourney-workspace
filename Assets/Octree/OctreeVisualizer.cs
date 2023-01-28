using System.Linq;
using codexhere.Util;
using UnityEngine;

public class OctreeVisualizer : MonoBehaviour {
    public Transform[] NodePositions;

    private Octree<int> octree;

    private void OnDrawGizmos() {
        octree = new Octree<int>(transform.position, 5);

        if (NodePositions.Count() > 0) {
            octree.Insert(0, NodePositions[0].position);
        }

        DrawNode(octree.RootNode);
    }

    private void DrawNode(Octree<int>.Node node) {
        if (!node.IsLeaf) {
            foreach (Octree<int>.Node child in node.Children) {
                DrawNode(child);
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        } else {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(node.Position, Vector3.one * node.Size);
        }

    }
}
