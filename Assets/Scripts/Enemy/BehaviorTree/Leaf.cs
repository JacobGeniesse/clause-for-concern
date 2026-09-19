using UnityEngine;

public class Leaf : Node
{
    //Definition for a method determining status on a given frame
    public delegate Status Tick();
    
    //reference to a Tick()
    public Tick processMethod;

    //Constructor definition
    public Leaf() { }

    //Constructor overload
    public Leaf(string n, Tick pm)
    {
        name = n;
        processMethod = pm;
    }

    /*
     * Override for the Node's process function determining that the processMethod will
     * determine the Status of the node
     */
    public override Status Process()
    {
        if(processMethod != null)
        {
            return processMethod();
        }
        return Status.FAILURE;
    }
}
