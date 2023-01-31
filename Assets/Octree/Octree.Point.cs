
using System;
using UnityEngine;

namespace codexhere.Util {
    public partial class OctreePoint<NodeDataType> {
        public Node RootNode { get; }
        public int MaxDepth { get; protected set; }

        public OctreePoint(Vector3 position, float size, float minSize) {
            RootNode = new Node(position, size, minSize);
        }

        public Node Insert(NodeDataType data, Vector3 lookPosition) {
            Node insertedIntoNode = RootNode.Insert(data, lookPosition);

            if (null != insertedIntoNode) {
                MaxDepth = Math.Max(insertedIntoNode.Depth, MaxDepth);
            }

            return insertedIntoNode;
        }
    }
}