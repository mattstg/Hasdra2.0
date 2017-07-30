using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIUISCRps : SIUISCSlider
{

    public override void Initialize()
    {
        base.Initialize();
        leftSliderText.text = "Instanteous";
    }

    public override void SliderValueChanged()
    {
        switch ((int)slider.value)
        {
            case 0:
                valueText.text = "Instant";
                break;
            default:
                base.SliderValueChanged();
                break;

        }

    }


}
