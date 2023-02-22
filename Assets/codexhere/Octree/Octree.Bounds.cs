using System;
using System.Collections.Generic;
using UnityEngine;

namespace codexhere.Util {
    public partial class OctreeBounds<NodeDataType> : OctreePoint<NodeDataType> {
        public new Node RootNode { get; }
        public new int MaxDepth { get; protected set; }

        public OctreeBounds(Vector3 position, float size, float minSize) : base(position, size, minSize)
            => RootNode = new Node(position, size, minSize);

        public List<Node> Insert(NodeDataType data, Vector3 lookPosition, Bounds bounds) {
            List<Node> insertedIntoNode = RootNode.Insert(data, lookPosition, bounds);

            if (null != insertedIntoNode) {
                foreach (Node insertion in insertedIntoNode) {
                    MaxDepth = Math.Max(insertion.Depth, MaxDepth);
                }
            }

            return insertedIntoNode;
        }
    }
}