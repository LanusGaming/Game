using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toggle : MenuItem<bool>
{
    public TextMeshProUGUI toggleButtonText;

    public override void SetValue(bool value)
    {
        base.SetValue(value);

        if (value)
            toggleButtonText.text = "X";
        else
            toggleButtonText.text = "";
    }

    public void ChangeValue()
    {
        SetValue(!value);
    }
}
