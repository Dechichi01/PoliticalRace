using UnityEngine;
using System.Collections;
using System;

public static class ExtensionMethods {

    public static int[] SequentialArray(this int[] array, int from, int to)
    {
        if (from > to) return null;

        array = new int[to - from + 1];
        for (int i = 0; i < array.Length; i++)
            array[i] = from + i;

        return array;
    }

}
