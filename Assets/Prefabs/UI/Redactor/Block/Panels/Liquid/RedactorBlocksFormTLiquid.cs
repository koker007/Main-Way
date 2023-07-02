using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlocksFormTLiquid : MonoBehaviour
{
    static public RedactorBlocksFormTLiquid main;

    [SerializeField]
    public SliderCTRL sliderExampleVolumeMax;
    [SerializeField]
    public SliderCTRL sliderExampleVolumeMin;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        IniExampleVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IniExampleVolume() {
        sliderExampleVolumeMax.slider.minValue = 0;
        sliderExampleVolumeMax.slider.maxValue = 15;
        sliderExampleVolumeMax.slider.value = 15;

        sliderExampleVolumeMin.slider.minValue = 0;
        sliderExampleVolumeMin.slider.maxValue = 15;
        sliderExampleVolumeMin.slider.value = 0;
    }

    public void applyExampleVolumeMax() {
        if (sliderExampleVolumeMax.slider.value <= sliderExampleVolumeMin.slider.value)
            sliderExampleVolumeMin.slider.value = sliderExampleVolumeMax.slider.value;

        sliderExampleVolumeMax.SetValueText();
        sliderExampleVolumeMin.SetValueText();
    }
    public void applyExampleVolumeMin() {
        if (sliderExampleVolumeMin.slider.value >= sliderExampleVolumeMax.slider.value)
            sliderExampleVolumeMax.slider.value = sliderExampleVolumeMin.slider.value;

        sliderExampleVolumeMin.SetValueText();
        sliderExampleVolumeMax.SetValueText();
    }

    public void acceptLiquidColor(Color colorStart, Color colorEnd, int perlinOctaves, Vector3 scaleXYZ, float scaleALL, int animLenght, Vector3 animSpeedXYZ, float animSpeedALL, Vector3 animSizeXYZ) {
        TypeLiquid typeLiquid = RedactorBlocksCTRL.blockData as TypeLiquid;
        float HStart;
        float SStart;
        float VStart;
        Color.RGBToHSV(colorStart, out HStart, out SStart, out VStart);

        float HEnd;
        float SEnd;
        float VEnd;
        Color.RGBToHSV(colorEnd, out HEnd, out SEnd, out VEnd);

        typeLiquid.data.colorHue = HStart;
        typeLiquid.data.colorSaturation = SStart;
        typeLiquid.data.colorValue = VStart;
        typeLiquid.data.colorHueEnd = HEnd;
        typeLiquid.data.colorSaturationEnd = SEnd;
        typeLiquid.data.colorValueEnd = VEnd;

        typeLiquid.data.perlinOctaves = perlinOctaves;
        typeLiquid.data.perlinScale = scaleALL;
        typeLiquid.data.perlinScaleX = 50/scaleXYZ.x;
        typeLiquid.data.perlinScaleY = 50/scaleXYZ.y;
        typeLiquid.data.perlinScaleZ = 50/scaleXYZ.z;
        typeLiquid.data.texturesMax = animLenght;
        typeLiquid.data.animSpeed = animSpeedALL;
        typeLiquid.data.animSpeedX = animSpeedXYZ.x/8;
        typeLiquid.data.animSpeedY = animSpeedXYZ.y/8;
        typeLiquid.data.animSpeedZ = animSpeedXYZ.z/8;
        typeLiquid.data.animSizeX = animSizeXYZ.x;
        typeLiquid.data.animSizeY = animSizeXYZ.y;
        typeLiquid.data.animSizeZ = animSizeXYZ.z;

        typeLiquid.ClearTextures();

    }
}
