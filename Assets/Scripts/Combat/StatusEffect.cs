using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class StatusEffect
{
    public string Name { get => GetType().Name; }
    public int StackCount { get; private set; }
    public virtual int MaxStacks { get => 1; }

    protected Stats Stats { get; private set; }
    protected Entity Entity { get; private set; }

    public void Apply(Stats stats, Entity entity, int stacks = 1)
    {
        Stats = stats;
        Entity = entity;

        Apply(stacks);
    }

    public void Apply(int stacks = 1)
    {
        for (int i = 0; i < stacks && StackCount < MaxStacks; i++)
        {
            StackCount++;
            OnApply();
        }
    }

    public void Remove(int stacks = 1)
    {
        for (int i = 0; i < stacks && StackCount > 0; i++)
        {
            StackCount--;
            OnRemove();
        }
    }

    public void Clear()
    {
        Remove(StackCount);
    }

    protected abstract void OnApply();
    protected abstract void OnRemove();
}
