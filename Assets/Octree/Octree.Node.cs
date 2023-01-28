using System.Collections.Generic;
using UnityEngine;

namespace codexhere.Util {
    public partial class Octree<NodeType> {
        public class Node {
            public Vector3 Position { get; }
            public float Size { get; }
            public float HalfSize => Size / 2;

            public IList<Node> Children { get; private set; }
            public NodeType Value { get; }
            public bool IsLeaf => null == Children;

            public Node(Vector3 position, float size) {
                Position = position;
                Size = size;
            }

            public bool Insert(NodeType data, Vector3 addPosition, float size) {
                bool contains = Contains(addPosition, size);

                if (!contains) {
                    return false;
                }

                if (IsLeaf) {
                    SubDivide();
                }

                int idx = GetChildIndex(addPosition);
                Debug.Log(idx + " contains? " + contains);

                return Children[idx].Insert(data, addPosition, size);
            }

            public bool Contains(Vector3 lookPosition, float size) {
                float halfLookSize = size / 2;
                bool isInX = (lookPosition.x - halfLookSize) > (Position.x - HalfSize) && (lookPosition.x + halfLookSize) < (Position.x + HalfSize);
                bool isInY = (lookPosition.y - halfLookSize) > (Position.y - HalfSize) && (lookPosition.y + halfLookSize) < (Position.y + HalfSize);
                bool isInZ = (lookPosition.z - halfLookSize) > (Position.z - HalfSize) && (lookPosition.z + halfLookSize) < (Position.z + HalfSize);

                return isInX && isInY && isInZ;
            }

            public void SubDivide() {
                Children = new Node[8];

                for (int childIdx = 0; childIdx < Children.Count; childIdx++) {
                    Vector3 childPos = Position * 1; // clone vector cheat
                    float childSize = Size * 0.25f;

                    childPos.y += ((childIdx & 4) == 4) ? -childSize : childSize;
                    childPos.x += ((childIdx & 2) == 2) ? -childSize : childSize;
                    childPos.z += ((childIdx & 1) == 1) ? -childSize : childSize;

                    Children[childIdx] = new Node(childPos, Size * 0.5f);
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