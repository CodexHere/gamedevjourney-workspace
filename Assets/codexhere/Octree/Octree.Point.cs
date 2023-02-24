using System;
using UnityEngine;

namespace codexhere.Util {
    public partial class OctreePoint<NodeDataType> {
        public Node RootNode { get; protected set; }
        public int MaxDepth { get; protected set; }

        private readonly Vector3 position;
        private readonly float size;
        private readonly float minSize;

        public OctreePoint(Vector3 position, float size, float minSize) {
            this.position = position;
            this.size = size;
            this.minSize = minSize;

            Clear();
        }

        public void Clear() {
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