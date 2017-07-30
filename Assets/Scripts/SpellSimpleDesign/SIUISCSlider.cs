using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIUISCSlider : SIBasicVarSlotUI
{
    public Slider slider;
    public Text valueText;
    public Text leftSliderText;
    public Text rightSliderText;
    public float minValue;
    public float maxValue;
    public float startValue;
    public bool intOnly = false;
    public bool showAsPercent = false;

    public override void Initialize()
    {
        base.Initialize();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        if (intOnly)
            slider.wholeNumbers = intOnly;
        slider.value = startValue;
        SliderValueChanged();
        SetSliderEdge(leftSliderText);
        SetSliderEdge(rightSliderText);
        
    }

    public virtual void SliderValueChanged()
    {
        SetText(valueText, slider.value);
    }

    private void SetSliderEdge(Text toSet)
    {
        if (toSet.text == "-max")
            SetText(toSet, slider.maxValue);
        if (toSet.text == "-min")
            SetText(toSet, slider.minValue);
    }

    private void SetText(Text toSet, float _value)
    {
        if (showAsPercent)
            valueText.text = string.Format("{0:0.00}%", _value.ToString());
        else
            valueText.text = string.Format("{0:0.##}", _value.ToString());
    }

    public override string GetValue()
    {
        return slider.value.ToString();
    }

}
