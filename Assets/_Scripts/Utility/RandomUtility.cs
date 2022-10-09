using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtility
{
    public static T GetRandomObject<T>(T[] objects)
    {
        int rnd = Random.Range(0, objects.Length);

        return objects[rnd];
    }

    public static T GetRandomObject<T>(List<T> objects)
    {
        int rnd = Random.Range(0, objects.Count);

        return objects[rnd];
    }

    public static T GetRandom<T>(this List<T>list, bool remove = false)
    {
        if (list.Count == 0)
            return default(T);

        int rnd = Random.Range(0, list.Count);

        T item = list[rnd];

        if(remove)
            list.RemoveAt(rnd);

        return item;
    }

    public static T GetRandom<T>(this List<T> list, int min, int max)
    {
        int rnd = Random.Range(min, max);

        return list[rnd];
    }

    public static T GetRandom<T>(this T[] array)
    {
        int rnd = Random.Range(0, array.Length);

        return array[rnd];
    }
}
