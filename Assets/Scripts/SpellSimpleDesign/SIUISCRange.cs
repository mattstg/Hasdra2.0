using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIUISCRange : SIUISCSlider
{
    public override void SliderValueChanged()
    {
        switch((int)slider.value)
        {
            case -2:
                valueText.text = "Self Cast";
                break;
            case -1:
                valueText.text = "Aoe";
                break;
            default:
                base.SliderValueChanged();
                break;

        }

    }


}
