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

    string textNotHaveMod = "None";
    string textNotHaveBlock = "None";

    string pathMod;
    string pathBlock;

    // Start is called before the first frame update
    void Start()
    {
        iniSliderMod();
        iniSliderBlock();

        Invoke("acceptSliderModName", 0.1f);
        Invoke("acceptSliderBlockName", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void iniSliderMod() {
        //Получаем все моды
        string[] mods = Directory.GetDirectories(GameData.GameData.pathMod);

        if (mods == null ||
            mods.Length <= 0)
        {
            sliderModName.slider.minValue = 0;
            sliderModName.slider.maxValue = 0;
            sliderModName.slider.value = 0;
            return;
        }
        sliderModName.slider.minValue = 0;
        sliderModName.slider.maxValue = mods.Length - 1;


        pathMod = mods[(int)sliderModName.slider.value];
    }
    void iniSliderBlock() {
        if (pathMod == null || pathMod.Length <= 0) {
            sliderBlockName.slider.minValue = 0;
            sliderBlockName.slider.maxValue = 0;
            sliderBlockName.slider.value = 0;
            return;
        }

        string pathblocks = pathMod + "/" + StrC.blocks;
        if (!Directory.Exists(pathblocks))
        {
            pathBlock = "";
            return;
        }


        string[] blocks = Directory.GetDirectories(pathblocks);
        sliderBlockName.slider.minValue = 0;
        sliderBlockName.slider.maxValue = blocks.Length - 1;

        if (blocks.Length <= 0)
            return;

        pathBlock = blocks[(int)sliderBlockName.slider.value];
    }

    public void acceptSliderModName() {
        //применяем новый мод
        iniSliderMod();

        //Если пути к моду нет, выходим
        if (pathMod == null ||
            pathMod.Length <= 0) {
            sliderModName.SetValueText(textNotHaveMod);
            return;
        }

        //Вытаскиваем имя мода
        string[] path = pathMod.Split("\\");
        string modName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderModName.SetValueText(modName);
    }
    public void acceptSliderBlockName() {
        //применяем новый мод
        iniSliderBlock();

        //если пути к моду нет - выходим
        if (pathBlock == null ||
            pathBlock.Length == 0) {
            sliderBlockName.SetValueText(textNotHaveBlock);
            return;
        }

        //Вытаскиваем имя мода
        string[] path = pathBlock.Split("\\");
        string BlockName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderBlockName.SetValueText(BlockName);
    }

    public void clickButtonModNext() {
        sliderModName.slider.value++;
        if (sliderModName.slider.value > sliderModName.slider.maxValue)
            sliderModName.slider.value = sliderModName.slider.maxValue;

        acceptSliderModName();
    }
    public void clickButtonModBack() {
        sliderModName.slider.value--;
        if (sliderModName.slider.value < sliderModName.slider.minValue)
            sliderModName.slider.value = sliderModName.slider.minValue;

        acceptSliderModName();
    }
    public void clickButtonBlockNext() {
        sliderBlockName.slider.value++;
        if (sliderBlockName.slider.value > sliderBlockName.slider.maxValue)
            sliderBlockName.slider.value = sliderBlockName.slider.maxValue;

        acceptSliderBlockName();
    }
    public void clickButtonBlockBack() {
        sliderBlockName.slider.value--;
        if (sliderBlockName.slider.value < sliderBlockName.slider.minValue)
            sliderBlockName.slider.value = sliderBlockName.slider.minValue;

        acceptSliderBlockName();
    }

    public void clickButtonLoad() {
        if (pathBlock == null || pathBlock.Length <= 0)
            return;

        //Вытаскиваем путь
        string[] pathPart = pathBlock.Split("\\");
        string pathNew = "";
        foreach (string path in pathPart) {
            if (pathNew.Length != 0)
                pathNew += '/';

            pathNew += path;
        }

        RedactorBlocksCTRL.main.loadBlock(pathNew);
        clickButtonClose();
    }
    public void clickButtonClose() {
        WindowMenuCTRL.CloseALL(true);
        UICTRL.IsOpenMainMenu = false;
    }
}
