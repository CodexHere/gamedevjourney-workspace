using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace codexhere.Util {
    public partial class OctreeBounds<NodeDataType> {
        public new class Node {
            public Vector3 Position { get; }
            public NodeDataType Data { get; protected set; }
            public float MinSize { get; }
            public float HalfSize => Size / 2;

            public int Depth { get; } = 1;
            public IList<Node> Children { get; protected set; }
            public NodeDataType Value { get; }
            public bool IsLeaf => null == Children || 0 == Children.Count;

            protected Bounds NodeBounds;

            protected float _size;
            public float Size {
                get => _size;
                set {
                    _size = value;
                    NodeBounds = new Bounds(Position, Vector3.one * Size);
                }
            }

            public Node(Vector3 position, float size, float minSize, int depth = 1) {
                Position = position;
                Size = size;
                MinSize = minSize;
                Depth = depth;
            }

            public List<Node> Insert(NodeDataType data, Vector3 addPosition, Bounds bounds) {
                float smallestDimension = new float[] { bounds.size.x, bounds.size.y, bounds.size.z }.Min();
                Bounds minBounds = new Bounds(addPosition, Vector3.one * smallestDimension);

                bool fitsWithinMe = minBounds.size.magnitude > (NodeBounds.size.magnitude / 2);

                // We've reached the minimum size, we can't divide anymore!
                if (Size < MinSize || fitsWithinMe) {
                    Data = data;
                    return new List<Node>(new Node[] { this });
                }

                bool contains = Contains(bounds);

                if (!contains) {
                    return null;
                }

                if (IsLeaf) {
                    SubDivide();
                }

                List<Node> insertedNodes = new List<Node>();

                foreach (Node child in Children) {
                    if (child.Contains(bounds)) {
                        List<Node> _subInsertions = child.Insert(data, addPosition, bounds);

                        if (null != _subInsertions && _subInsertions.Count > 0) {
                            insertedNodes = insertedNodes.Concat(_subInsertions).ToList();
                        }
                    }
                }

                return insertedNodes;
            }

            public bool Contains(Bounds bounds) {
                return NodeBounds.Intersects(bounds);
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