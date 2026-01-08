using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    private List<ITickProvider> ticks;
    public static TickManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Add(ITickProvider tick)
    {
        if (ticks == null) ticks = new List<ITickProvider>();
        if(!ticks.Contains(tick))
            ticks.Add(tick);
    }

    public void Remove(ITickProvider tick)
    {
        if (ticks == null) ticks = new List<ITickProvider>();
        if(ticks.Contains(tick))
            ticks.Remove(tick);
    }
    
    private void Update()
    {
        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i] is ITick tick)
            {
                tick.Tick();
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i] is IFixedTick tick)
            {
                tick.FixedTick();
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < ticks.Count; i++)
        {
            if (ticks[i] is ILateTick tick)
            {
                tick.LateTick();
            }
        } 
    }
}
