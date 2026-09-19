using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node
{

    //Definition for the status of the nde
    public enum Status
    {
        SUCCESS, //Node has successfully finished its task and can proceed to the next node
        RUNNING, //Node is still working on its task and will remain on the current node
        FAILURE //Node has failed its task and should start from the first node again
    };

    //Reference to the status of the node
    public Status status;
    
    //Children of the node
    public List<Node> children = new List<Node>();

    //Position in the list of children
    public int currentChild = 0;

    //name of the Node
    public string name;

    //Constructor definition
    public Node() { }

    //Constructor overload definition
    public Node(string n)
    {
        name = n;
    }

    //Virtual func for referencing the status of the child node of this node
    public virtual Status Process()
    {
        return children[currentChild].Process();
    }

    //Func for adding children to the node
    public void AddChild(Node n)
    {
        children.Add(n);
    }
}
