using System;
using codexhere.Util;
using UnityEngine;

public class OctreeVisualizer : MonoBehaviour {
    public Transform[] NodePositions;
    public float Size = 1;
    public float MinSize = 1;

    private Octree<int> octree;

    private void OnDrawGizmos() {
        octree = new Octree<int>(transform.position, Size, MinSize);

        if (0 >= MinSize) {
            throw new Exception("MinSize must be greater than Zero to avoid infinite recursion!");
        }

        for (int nodeIdx = 0; nodeIdx < NodePositions.Length; nodeIdx++) {
            _ = octree.Insert(nodeIdx + 1, NodePositions[nodeIdx].position);
        }

        DrawNode(octree.RootNode);
    }

    private void DrawNode(Octree<int>.Node node) {
        float percentDepth = node.Depth / (float)octree.MaxDepth;

        if (!node.IsLeaf) {
            foreach (Octree<int>.Node child in node.Children) {
                DrawNode(child);
            }

            Color color = Color.cyan;
            color.a = percentDepth;
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
                color.a = percentDepth;
                Gizmos.color = color;
                Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
            }
        }

    }
}
