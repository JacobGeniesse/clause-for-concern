using UnityEngine;

public interface ISource
{
    void StartListening(IObserver observer);

    void StopListening(IObserver observer);

    void Notify();

}
