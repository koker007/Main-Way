using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WindowRedactorBlockLoad : MonoBehaviour
{

    [SerializeField]
    SliderCTRL sliderModName;
    [SerializeField]
    SliderCTRL sliderBlockName;

    string pathMod;
    string pathBlock;

    // Start is called before the first frame update
    void Start()
    {
        iniSliderMod();
        iniSliderBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void iniSliderMod() {
        //Получаем все моды
        string[] mods = Directory.GetDirectories(GameData.pathMod);
        sliderModName.slider.minValue = 0;
        sliderModName.slider.maxValue = mods.Length - 1;

        pathMod = mods[(int)sliderModName.slider.value];
    }
    void iniSliderBlock() {

        string[] blocks = Directory.GetDirectories(pathBlock);
        sliderBlockName.slider.minValue = 0;
        sliderBlockName.slider.maxValue = blocks.Length - 1;

        pathBlock = blocks[(int)sliderBlockName.slider.value];
    }

    public void acceptSliderModName() {
        //применяем новый мод
        iniSliderMod();

        //Вытаскиваем имя мода
        string[] path = pathMod.Split('/');
        string modName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderModName.SetValueText(modName);
    }
    public void acceptSliderBlockName() {
        //применяем новый мод
        iniSliderBlock();

        //Вытаскиваем имя мода
        string[] path = pathMod.Split('/');
        string BlockName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderBlockName.SetValueText(BlockName);
    }

    public void clickButtonModNext() {
        sliderModName.slider.value++;
        if (sliderModName.slider.value > sliderModName.slider.maxValue)
            sliderModName.slider.value = sliderModName.slider.maxValue;
    }
    public void clickButtonModBack() {
        sliderModName.slider.value--;
        if (sliderModName.slider.value < sliderModName.slider.minValue)
            sliderModName.slider.value = sliderModName.slider.minValue;
    }
    public void clickButtonBlockNext() {
        sliderBlockName.slider.value++;
        if (sliderBlockName.slider.value > sliderBlockName.slider.maxValue)
            sliderBlockName.slider.value = sliderBlockName.slider.maxValue;
    }
    public void clickButtonBlockBack() {
        sliderBlockName.slider.value--;
        if (sliderBlockName.slider.value < sliderBlockName.slider.minValue)
            sliderBlockName.slider.value = sliderBlockName.slider.minValue;
    }
}
