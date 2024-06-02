using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slider : MenuItem
{
    public int roundToDecimalPlaces = 2;
    public UnityEngine.UI.Slider slider;
    public TextMeshProUGUI indicator;

    private float value;

    public void Initialize(float minValue, float maxValue, bool wholeNumbers)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = wholeNumbers;
    }

    public override object GetValue()
    {
        return value;
    }

    public override void SetValue(object value)
    {
        this.value = (float)value;
        slider.value = this.value;

        indicator.text = HelperFunctions.RoundToPlaces(this.value, roundToDecimalPlaces).ToString();

        base.SetValue(value);
    }

    public void ChangeValue(float value)
    {
        SetValue(value);
    }
}
