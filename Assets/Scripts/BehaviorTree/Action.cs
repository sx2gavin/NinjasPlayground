using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Action : Node
    {
        private readonly ActionDelegate PerformableAction;

        public Action(ActionDelegate action)
        {
            PerformableAction = action;
        }

        public override bool Perform()
        {
            return PerformableAction();
        }

        public override void AddChild(Node child)
        {
            Debug.LogError("You are not suppose to call this method within ActionNode");
            return;
        }
    }
}

