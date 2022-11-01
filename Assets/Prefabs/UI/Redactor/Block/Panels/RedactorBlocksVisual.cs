using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlocksVisual : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderTransparent;
    [SerializeField]
    SliderCTRL sliderTransparentPower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void acceptTransparent()
    {
        if (sliderTransparent.slider.value == (int)TypeBlockTransparent.NoTransparent)
        {
            sliderTransparent.SetValueText("Off", KeyBlockTransparent.keyOff);
        }
        else if (sliderTransparent.slider.value == (int)TypeBlockTransparent.CutOff)
        {
            sliderTransparent.SetValueText("CutOff", KeyBlockTransparent.keyCutOff);
        }
        else if (sliderTransparent.slider.value == (int)TypeBlockTransparent.Alpha)
        {
            sliderTransparent.SetValueText("Alpha", KeyBlockTransparent.keyAlpha);
        }
        else
        {
            sliderTransparent.SetValueText();
        }
    }
    public void acceptTransparentPower()
    {
        sliderTransparentPower.SetValueText((sliderTransparentPower.slider.value / 100.0f) + "");
    }
}
