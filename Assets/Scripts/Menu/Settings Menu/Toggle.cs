using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toggle : MenuItem
{
    public TextMeshProUGUI toggleButtonText;

    private bool value;

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = (bool) value;

        if (this.value)
            toggleButtonText.text = "X";
        else
            toggleButtonText.text = "";

        base.SetValue(value);
    }

    public void ChangeValue()
    {
        SetValue(!value);
    }
}
