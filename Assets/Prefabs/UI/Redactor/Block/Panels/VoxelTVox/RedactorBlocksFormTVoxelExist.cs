using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlocksFormTVoxelExist : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderExist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateSliderExist();
    }

    void updateSliderExist() {
        sliderExist.slider.minValue = 0;
        sliderExist.slider.maxValue = 100;

        sliderExist.SetValueText(sliderExist.slider.value + "%");
    } 

    public void acceptSliderExist() {

        Vector3Int pos = RedactorBlocksFormTVoxelSelectZone.GetZonePos();
        Vector3Int size = RedactorBlocksFormTVoxelSelectZone.GetZoneSize();

        RedactorBlocksFormTVoxel.main.acceptVoxExistArray(pos, size, (int)sliderExist.slider.value);
    }
}
