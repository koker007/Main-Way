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
        BlockData blockData = RedactorBlocksCTRL.blockData;
        float HStart;
        float SStart;
        float VStart;
        Color.RGBToHSV(colorStart, out HStart, out SStart, out VStart);

        float HEnd;
        float SEnd;
        float VEnd;
        Color.RGBToHSV(colorEnd, out HEnd, out SEnd, out VEnd);

        blockData.TLiquid.data.colorHue = HStart;
        blockData.TLiquid.data.colorSaturation = SStart;
        blockData.TLiquid.data.colorValue = VStart;
        blockData.TLiquid.data.colorHueEnd = HEnd;
        blockData.TLiquid.data.colorSaturationEnd = SEnd;
        blockData.TLiquid.data.colorValueEnd = VEnd;

        blockData.TLiquid.data.perlinOctaves = perlinOctaves;
        blockData.TLiquid.data.perlinScale = scaleALL;
        blockData.TLiquid.data.perlinScaleX = scaleXYZ.x;
        blockData.TLiquid.data.perlinScaleY = scaleXYZ.y;
        blockData.TLiquid.data.perlinScaleZ = scaleXYZ.z;
        blockData.TLiquid.data.texturesMax = animLenght;
        blockData.TLiquid.data.animSpeed = animSpeedALL/8;
        blockData.TLiquid.data.animSpeedX = animSpeedXYZ.x/8;
        blockData.TLiquid.data.animSpeedY = animSpeedXYZ.y/8;
        blockData.TLiquid.data.animSpeedZ = animSpeedXYZ.z/8;
        blockData.TLiquid.data.animSizeX = animSizeXYZ.x;
        blockData.TLiquid.data.animSizeY = animSizeXYZ.y;
        blockData.TLiquid.data.animSizeZ = animSizeXYZ.z;

        blockData.TLiquid.ClearTextures();

    }
}
