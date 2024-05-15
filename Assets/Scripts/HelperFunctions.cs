using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static T[] ShuffleArray<T>(T[] array, System.Random randomizer)
    {
        List<T> result = new List<T>();
        List<T> remaining = new List<T>();
        remaining.AddRange(array);

        while (remaining.Count > 0)
        {
            int index = randomizer.Next(0, remaining.Count);
            result.Add(remaining[index]);
            remaining.RemoveAt(index);
        }

        return result.ToArray();
    }
}
