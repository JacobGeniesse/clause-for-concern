using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BehaviorTree : Node
{
    public bool isRepeatable = true;

    // Constructor declaration
    public BehaviorTree()
    {
        name = "Tree";
    }

    //Constructor overload
    public BehaviorTree(string n)
    {
        name = n;
    }

    //Func that handles repeating a cycle of a behavior tree
    public override Status Process()
    {
        if(isRepeatable == true && currentChild > children.Count)
        {
            currentChild = 0;
        }
        return children[currentChild].Process();
    }

    //Struct for holding a node's position on the tree
    struct NodeLevel
    {
        public int level;
        public Node node;
    }

    //Debug func for printing the current layout of the behavior tree
    public void PrintTree()
    {
        string treePrintout = ""; //var for containing the printout of the tree
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>(); //Create a stack for nodelevel structs
        Node currentNode = this; //Reference var for the currentNode
        nodeStack.Push(new NodeLevel { level = 0, node = currentNode }); //Add the current node to the stack with a default 0 level

        //While there are still nodes in the stack
        while(nodeStack.Count != 0)
        {
            //Remove the node from the stack and reference it in a var
            NodeLevel nextNode = nodeStack.Pop();
            //Add the content of the node to the readout of treePrintout
            treePrintout += new string('-', nextNode.level) + nextNode.node.name + "\n";

            //For each child node push them onto the stack with an incremental count for the level they are on.
            for (int i = nextNode.node.children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level + 1, node = nextNode.node.children[i] });
            }
        }

        //After gathering all the nodes names and levels into a string print that string.
        Debug.Log(treePrintout);
    }
}
