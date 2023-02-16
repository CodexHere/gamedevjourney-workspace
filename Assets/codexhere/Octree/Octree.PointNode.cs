using System.Collections.Generic;
using UnityEngine;

namespace codexhere.Util {
    public partial class OctreePoint<NodeDataType> {
        public class Node {
            public Vector3 Position { get; }
            public NodeDataType Data { get; protected set; }
            public float Size { get; }
            public float MinSize { get; }
            public float HalfSize => Size / 2;

            public int Depth { get; } = 1;
            public IList<Node> Children { get; protected set; }
            public NodeDataType Value { get; }
            public bool IsLeaf => null == Children || 0 == Children.Count;

            public Node(Vector3 position, float size, float minSize, int depth = 1) {
                Position = position;
                Size = size;
                MinSize = minSize;
                Depth = depth;
            }

            public Node Insert(NodeDataType data, Vector3 addPosition) {
                // We've reached the minimum size, we can't divide anymore!
                if (Size < MinSize) {
                    Data = data;
                    return this;
                }

                bool contains = Contains(addPosition);

                if (!contains) {
                    return null;
                }

                if (IsLeaf) {
                    SubDivide();
                }

                foreach (Node child in Children) {
                    if (child.Contains(addPosition)) {
                        return child.Insert(data, addPosition);
                    }
                }

                return null;
            }

            public bool Contains(Vector3 lookPosition) {
                bool isInX = lookPosition.x >= (Position.x - HalfSize) && lookPosition.x <= (Position.x + HalfSize);
                bool isInY = lookPosition.y >= (Position.y - HalfSize) && lookPosition.y <= (Position.y + HalfSize);
                bool isInZ = lookPosition.z >= (Position.z - HalfSize) && lookPosition.z <= (Position.z + HalfSize);

                return isInX && isInY && isInZ;
            }

            public void SubDivide() {
                Children = new Node[8];

                for (int childIdx = 0; childIdx < Children.Count; childIdx++) {
                    Vector3 childPos = Position;
                    float childSize = Size * 0.25f;

                    childPos.y += ((childIdx & 4) == 4) ? -childSize : childSize;
                    childPos.x += ((childIdx & 2) == 2) ? -childSize : childSize;
                    childPos.z += ((childIdx & 1) == 1) ? -childSize : childSize;

                    Children[childIdx] = new Node(childPos, Size * 0.5f, MinSize, Depth + 1);
                }
            }
        }
    }
}