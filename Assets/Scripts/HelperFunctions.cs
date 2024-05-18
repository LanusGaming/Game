using System;
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

    public static Sprite GenerateWhiteFlashSprite(Sprite sprite)
    {
        Texture2D texture = new Texture2D(sprite.texture.width, sprite.texture.height);
        texture.filterMode = FilterMode.Point;

        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                if (sprite.texture.GetPixel(x, y).a > 0)
                    texture.SetPixel(x, y, new Color(1, 1, 1, 1));
                else
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));

        texture.Apply();

        return Sprite.Create(texture, sprite.rect, new Vector2(0.5f, 0.5f), sprite.pixelsPerUnit, 0, SpriteMeshType.Tight, sprite.border);
    }

    public static Quaternion LookTowards(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float angle = Mathf.Atan2(direction.y, direction.x);
        return Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    public static System.Random GetNewRandomizer(int seed = 0)
    {
        return new System.Random((seed != 0) ? seed : (int)(DateTime.Now.Ticks % int.MaxValue));
    }
}
