using UnityEngine;

public class Selector : Node
{
    //Constructor overload/constructor declaration
    public Selector(string n)
    {
        name = n;
    }

    //Create a process for handling child nodes returning a status
    public override Status Process()
    {
        foreach (Node child in children)
        {
            if(child.Process() == Status.SUCCESS)
            {
                return Status.SUCCESS;
            }
            else if (child.Process() == Status.RUNNING)
            {
                return Status.RUNNING;
            }
        }
        return Status.FAILURE;
    }
}
