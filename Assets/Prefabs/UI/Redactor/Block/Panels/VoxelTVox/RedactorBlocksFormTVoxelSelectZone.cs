using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlocksFormTVoxelSelectZone : MonoBehaviour
{
    static RedactorBlocksFormTVoxelSelectZone main;

    [SerializeField]
    GameObject selectZoneVisualuze;

    [SerializeField]
    SliderCTRL sliderZonePosX;
    [SerializeField]
    SliderCTRL sliderZonePosY;
    [SerializeField]
    SliderCTRL sliderZonePosZ;

    [SerializeField]
    SliderCTRL sliderZoneSizeX;
    [SerializeField]
    SliderCTRL sliderZoneSizeY;
    [SerializeField]
    SliderCTRL sliderZoneSizeZ;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        main = this;

        TestSelectZone();
        UpdateSliders();
    }

    static public Vector3Int GetZonePos() {
        Vector3Int result = new Vector3Int(
            (int)main.sliderZonePosX.slider.value,
            (int)main.sliderZonePosY.slider.value,
            (int)main.sliderZonePosZ.slider.value
            );
        
        return result;
    }
    static public Vector3Int GetZoneSize() {
        Vector3Int result = new Vector3Int(
            (int)main.sliderZoneSizeX.slider.value,
            (int)main.sliderZoneSizeY.slider.value,
            (int)main.sliderZoneSizeZ.slider.value
            );

        return result;
    }

    private void OnDisable()
    {
        if (selectZoneVisualuze.activeSelf)
            selectZoneVisualuze.SetActive(false);
    }

    //Проверка визуализации зоны
    void TestSelectZone() {
        if (!selectZoneVisualuze.activeSelf)
            selectZoneVisualuze.SetActive(true);

        selectZoneVisualuze.transform.localPosition = new Vector3(sliderZonePosX.slider.value / 16, sliderZonePosY.slider.value / 16, sliderZonePosZ.slider.value / 16);
        selectZoneVisualuze.transform.localScale = new Vector3(sliderZoneSizeX.slider.value / 16, sliderZoneSizeY.slider.value / 16, sliderZoneSizeZ.slider.value / 16);
    }

    void UpdateSliders() {
        //Узнаем насколько изменить позицию максимума
        int posMaxX = 16 - (int)sliderZoneSizeX.slider.value;
        int posMaxY = 16 - (int)sliderZoneSizeY.slider.value;
        int posMaxZ = 16 - (int)sliderZoneSizeZ.slider.value;

        int sizeMaxX = 16 - (int)sliderZonePosX.slider.value;
        int sizeMaxY = 16 - (int)sliderZonePosY.slider.value;
        int sizeMaxZ = 16 - (int)sliderZonePosZ.slider.value;

        sliderZonePosX.slider.maxValue = posMaxX;
        sliderZonePosY.slider.maxValue = posMaxY;
        sliderZonePosZ.slider.maxValue = posMaxZ;

        sliderZoneSizeX.slider.maxValue = sizeMaxX;
        sliderZoneSizeY.slider.maxValue = sizeMaxY;
        sliderZoneSizeZ.slider.maxValue = sizeMaxZ;

        sliderZonePosX.SetValueText();
        sliderZonePosY.SetValueText();
        sliderZonePosZ.SetValueText();

        sliderZoneSizeX.SetValueText();
        sliderZoneSizeY.SetValueText();
        sliderZoneSizeZ.SetValueText();
    }
}
