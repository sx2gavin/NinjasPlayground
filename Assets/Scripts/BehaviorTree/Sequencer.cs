using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Sequencor : Node
    {
        public List<Node> Children = new List<Node>();

        public override bool Perform()
        {
            bool result = false;
            foreach (var childNode in Children)
            {
                result |= childNode.Perform();
            }
            return result;
        }
        
        public override void AddChild(Node child)
        {
            Children.Add(child);
        }
    }
}
