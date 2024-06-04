using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MenuItem<object>
{
    public TextMeshProUGUI valueTextMesh;
    public Button moveLeftButton;
    public Button moveRightButton;

    private int currentIndex;
    private object[] values;
    private Func<object, string> toString;

    public void Initialize<T>(T[] values, Func<object, string> toString)
    {
        this.values = MakeGenericArray(values);
        this.toString = toString;
    }

    public void ChangeValue(int delta)
    {
        SetValue(values.ToList().IndexOf(value) + delta);
    }

    public override void SetValue(object value)
    {
        SetValue(values.ToList().IndexOf(value));
    }

    public void SetValue(int index)
    {
        object value = values[index];

        base.SetValue(value);

        valueTextMesh.text = toString.Invoke(value);

        if (index == 0)
            moveLeftButton.interactable = false;
        else
            moveLeftButton.interactable = true;

        if (index == values.Length - 1)
            moveRightButton.interactable = false;
        else
            moveRightButton.interactable = true;
    }

    private static object[] MakeGenericArray<T>(T[] array)
    {
        List<object> newArray = new List<object>();

        foreach (T obj in array)
            newArray.Add(obj);

        return newArray.ToArray();
    }
}
