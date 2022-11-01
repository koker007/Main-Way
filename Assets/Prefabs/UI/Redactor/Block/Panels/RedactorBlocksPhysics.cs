using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Контролинует окно настройки физики блока
public class RedactorBlocksPhysics : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderLight;
    [SerializeField]
    SliderCTRL sliderLightRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void acceptLight()
    {
        if (sliderLight.slider.value == 0)
        {
            sliderLight.SetValueText("Off", "keyTextOff");
        }
        else if (sliderLight.slider.value == 1)
        {
            sliderLight.SetValueText("On", "keyTextOn");
        }
        else
        {
            sliderLight.SetValueText();
        }
    }
    public void acceptLightRange()
    {
        sliderLightRange.SetValueText();
    }

}
