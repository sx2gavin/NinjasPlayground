﻿using System.Collections.Generic;

namespace BehaviorTree
{
    public class Selector : Node
    {
        public List<Node> Children = new List<Node>();

        public override bool Perform()
        {
            foreach (var childNode in Children)
            {
                if (childNode.Perform())
                {
                    return true;
                }
            }
            return false;
        }

        public override void AddChild(Node child)
        {
            Children.Add(child);
        }
    }
}