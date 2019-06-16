using UnityEngine;

namespace BehaviorTree
{
    public abstract class Node
    {
        public abstract bool Perform();
        public abstract void AddChild(Node child);
    }
}
