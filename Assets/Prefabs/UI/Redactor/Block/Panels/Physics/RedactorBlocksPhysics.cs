using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Контролинует окно настройки физики блока
public class RedactorBlocksPhysics : MonoBehaviour
{
    [SerializeField]
    RedactorBlocksPhysicsCollider colliders;

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

    //Установить параметры по умолчанию
    public void setBasicAll() {
        BlockData blockData = RedactorBlocksCTRL.blockData;

        blockData.physics.zones = null;
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

    public void ReDrawAll() {
        colliders.clearVisualize();
        colliders.updateColliderMax(RedactorBlocksCTRL.blockData.physics, true);
        colliders.updateColliderSelect();
    }
}
