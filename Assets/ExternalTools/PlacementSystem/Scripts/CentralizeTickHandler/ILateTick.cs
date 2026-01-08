using UnityEngine;

public interface ILateTick : ITickProvider
{
    void LateTick();
}
