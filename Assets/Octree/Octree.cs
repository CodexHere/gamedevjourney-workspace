using UnityEngine;

namespace codexhere.Util {
    public partial class Octree<NodeType> {
        public enum Quadrant {
            LowerFrontLeft = 0,     // 000
            LowerFrontRight = 2,    // 010
            LowerBackLeft = 1,      // 001
            LowerBackRight = 3,     // 011
            UpperFrontLeft = 4,     // 100
            UpperFrontRight = 6,    // 110
            UpperBackLeft = 5,      // 101
            UpperBackRight = 7      // 111
        }

        public Node RootNode { get; }

        public Octree(Vector3 position, int maxDepth = 2) {
            RootNode = new Node(position, maxDepth);
        }

        public void Insert(NodeType data, Vector3 lookPosition) {
            RootNode.Insert(data, lookPosition);
        }
    }
}