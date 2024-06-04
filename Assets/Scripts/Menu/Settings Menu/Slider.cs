using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slider : MenuItem<float>
{
    public int roundToDecimalPlaces = 2;
    public UnityEngine.UI.Slider slider;
    public TextMeshProUGUI indicator;

    public void Initialize(float minValue, float maxValue, bool wholeNumbers)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = wholeNumbers;
    }

    public override void SetValue(float value)
    {
        base.SetValue(value);

        slider.value = value;
        indicator.text = HelperFunctions.RoundToPlaces(value, roundToDecimalPlaces).ToString();
    }

    public void ChangeValue(float value)
    {
        SetValue(value);
    }
}
