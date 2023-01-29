using System.Collections.Generic;
using UnityEngine;

namespace codexhere.Util {
    public partial class Octree<NodeType> {
        public class Node {
            public Vector3 Position { get; }
            public NodeType Data { get; internal set; }
            public float Size { get; }
            public float MinSize { get; }
            public float HalfSize => Size / 2;

            public IList<Node> Children { get; private set; }
            public NodeType Value { get; }
            public bool IsLeaf => null == Children || 0 == Children.Count;

            public Node(Vector3 position, float size, float minSize) {
                Position = position;
                Size = size;
                MinSize = minSize;
            }

            public bool Insert(NodeType data, Vector3 addPosition) {
                // We've reached the minimum size, we can't divide anymore!
                if (Size < MinSize) {
                    Data = data;
                    return true;
                }

                bool contains = Contains(addPosition);

                if (!contains) {
                    return false;
                }

                if (IsLeaf) {
                    SubDivide();
                }

                foreach (Node child in Children) {
                    if (child.Contains(addPosition)) {
                        return child.Insert(data, addPosition);
                    }
                }

                return false;
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

                    Children[childIdx] = new Node(childPos, Size * 0.5f, MinSize);
                }
            }

            public int GetChildIndex(Vector3 lookPosition) {
                int childIndex = 0;

                childIndex |= lookPosition.y > Position.y ? 4 : 0;
                childIndex |= lookPosition.x > Position.x ? 2 : 0;
                childIndex |= lookPosition.z > Position.z ? 1 : 0;

                return childIndex;
            }
        }
    }
}