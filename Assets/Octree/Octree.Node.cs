using System.Collections.Generic;
using UnityEngine;

namespace codexhere.Util {
    public partial class Octree<NodeType> {
        public class Node {
            public Vector3 Position { get; }
            public float Size { get; }

            public IList<Node> Children { get; private set; }
            public NodeType Value { get; }
            public bool IsLeaf => null == Children;

            public Node(Vector3 position, float size) {
                Position = position;
                Size = size;
            }

            public void Insert(NodeType data, Vector3 addPosition) {
                if (IsLeaf) {
                    SubDivide();
                }
                
                GetChildIndex(addPosition);
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