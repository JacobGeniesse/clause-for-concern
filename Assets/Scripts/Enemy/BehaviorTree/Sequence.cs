using UnityEngine;

public class Sequence : Node
{
    //Constructor declaration and overload
    public Sequence(string n)
    {
        name = n;
    }

    //Func for handling a sequence of child nodes
    public override Status Process()
    {
        //Find the tatus of the child node
        Status childStatus = children[currentChild].Process();

        //Depending on what the child node's status is return that corresponidng status
        if(childStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }

        if(childStatus == Status.FAILURE)
        {
            return childStatus;
        }

        //Increment the current child
        currentChild++;
        if(currentChild >= children.Count)
        {
            //Reset the current counter and then return the successful status
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }
}
