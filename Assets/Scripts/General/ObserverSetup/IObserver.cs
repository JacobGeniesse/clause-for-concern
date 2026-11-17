using UnityEngine;

public interface IObserver
{
    public void Trigger(ISource source);
}
