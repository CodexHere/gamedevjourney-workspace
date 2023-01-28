using UnityEngine;

namespace codexhere.Util {
    public partial class Octree<NodeType> {
        public enum Quadrant {
            LowerFrontLeft = 0,     // 000
            LowerBackLeft = 1,      // 001
            LowerFrontRight = 2,    // 010
            LowerBackRight = 3,     // 011
            UpperFrontLeft = 4,     // 100
            UpperBackLeft = 5,      // 101
            UpperFrontRight = 6,    // 110
            UpperBackRight = 7      // 111
        }

        public Node RootNode { get; }

        public Octree(Vector3 position, float minSize = 1) {
            RootNode = new Node(position, minSize);
        }

        public bool Insert(NodeType data, Vector3 lookPosition, float size) {
            return RootNode.Insert(data, lookPosition, size);
        }
    }
}