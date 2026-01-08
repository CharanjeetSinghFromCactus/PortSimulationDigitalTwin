using UnityEngine;

public interface IFixedTick : ITickProvider
{
    void FixedTick();
}
