using codexhere.Util;
using UnityEngine;

public class OctreeVisualizer : MonoBehaviour {
    public Transform[] NodePositions;
    public float Size = 1;
    public float MinSize = 1;
    public float DepthAlphaRatio = 1;

    private Octree<int> octree;

    private void OnDrawGizmos() {
        octree = new Octree<int>(transform.position, Size, MinSize);

        for (int nodeIdx = 0; nodeIdx < NodePositions.Length; nodeIdx++) {
            _ = octree.Insert(nodeIdx + 1, NodePositions[nodeIdx].position);
        }

        DrawNode(octree.RootNode, 1);
    }

    private void DrawNode(Octree<int>.Node node, float depth) {
        if (!node.IsLeaf) {
            foreach (Octree<int>.Node child in node.Children) {
                DrawNode(child, depth + DepthAlphaRatio);
            }

            Color color = Color.cyan;
            color.a = 1 - (1 / depth);
            Gizmos.color = color;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        } else {
            if (0 != node.Data) {
                Color color = Color.red;
                Gizmos.color = color;
                color.a = 0.25f;
                Gizmos.DrawCube(node.Position, Vector3.one * node.Size);
            } else {
                Color color = Color.green;
                color.a = 1 - (1 / depth);
                Gizmos.color = color;
                Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
            }
        }

    }
}
