using codexhere.Util;
using UnityEngine;

public class OctreeVisualizer : MonoBehaviour {
    public Transform[] NodePositions;
    public float MinSize = 1;

    private Octree<int> octree;

    private void OnDrawGizmos() {
        octree = new Octree<int>(transform.position, MinSize);

        for (int nodeIdx = 0; nodeIdx < NodePositions.Length; nodeIdx++) {
            Transform nodePosition = NodePositions[nodeIdx];
            Bounds nodeBounds = nodePosition.GetComponent<Renderer>().bounds;
            bool inserted = octree.Insert(nodeIdx, nodePosition.position, nodeBounds.size.magnitude);

            if (inserted) {
                Debug.Log("Inserted " + nodeIdx + " - " + nodeBounds.size.magnitude);
            }
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
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        }

    }
}
