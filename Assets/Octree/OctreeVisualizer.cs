using System;
using codexhere.Util;
using UnityEngine;

public class OctreeVisualizer : MonoBehaviour {
    public Transform[] NodePositions;
    public float Size = 1;
    public float MinSize = 1;
    public bool UseBounds = false;

    private OctreePoint<int> octreePoint;
    private OctreeBounds<int> octreeBounds;

    private void OnDrawGizmos() {
        octreePoint = new OctreePoint<int>(transform.position, Size, MinSize);
        octreeBounds = new OctreeBounds<int>(transform.position, Size, MinSize);

        if (0 >= MinSize) {
            throw new Exception("MinSize must be greater than Zero to avoid infinite recursion!");
        }

        for (int nodeIdx = 0; nodeIdx < NodePositions.Length; nodeIdx++) {
            Renderer renderer = NodePositions[nodeIdx].GetComponent<Renderer>();
            _ = octreePoint.Insert(nodeIdx + 1, NodePositions[nodeIdx].position);

            if (renderer) {
                octreeBounds.Insert(nodeIdx + 1, NodePositions[nodeIdx].position, NodePositions[nodeIdx].GetComponent<Renderer>().bounds);
            } else {
                throw new Exception(NodePositions[nodeIdx].name + " doesn't have a Renderer, please add one to calculate");
            }
        }

        if (UseBounds) {
            DrawBoundsNode(octreeBounds.RootNode);
        } else {
            DrawPointNode(octreePoint.RootNode);
        }
    }

    private void DrawPointNode(OctreePoint<int>.Node node) {
        float percentDepth = node.Depth / (float)octreePoint.MaxDepth;

        if (!node.IsLeaf) {
            foreach (OctreePoint<int>.Node child in node.Children) {
                DrawPointNode(child);
            }

            Color color = Color.grey;
            color.a = percentDepth;
            Gizmos.color = color;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        } else {
            if (0 != node.Data) {
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

    private void DrawBoundsNode(OctreeBounds<int>.Node node) {
        float percentDepth = node.Depth / (float)octreeBounds.MaxDepth;

        if (!node.IsLeaf) {
            foreach (OctreeBounds<int>.Node child in node.Children) {
                DrawBoundsNode(child);
            }

            Color color = Color.yellow;
            color.a = percentDepth;
            Gizmos.color = color;
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        } else {
            if (0 != node.Data) {
                Color color = Color.red;
                color.a = 0.15f;
                Gizmos.color = color;
                Gizmos.DrawCube(node.Position, Vector3.one * node.Size);
            } else {
                Color color = Color.grey;
                color.a = percentDepth;
                Gizmos.color = color;
                Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
            }
        }
    }

}
