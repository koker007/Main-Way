using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowRedactorBiomeLoad : MonoBehaviour
{

    [SerializeField]
    SliderCTRL sliderModName;
    [SerializeField]
    SliderCTRL sliderBiomeName;

    string textNotHaveMod = "None";
    string textNotHaveBiome = "None";

    string pathMod;
    string pathBiome;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void iniSliderMod()
    {
        //Получаем все моды
        string[] modsAll = Directory.GetDirectories(Game.GameData.pathMod);

        List<string> mods = new List<string>();

        //Нужно выяснить существует ли папка биомов
        foreach (string mod in modsAll) {
            string pathBiome = mod + "/" + StrC.biomes;
            if (Directory.Exists(pathBiome)) {
                mods.Add(mod);
            }
        }

        if (mods == null ||
            mods.Count <= 0)
        {
            sliderModName.slider.minValue = 0;
            sliderModName.slider.maxValue = 0;
            sliderModName.slider.value = 0;
            return;
        }
        sliderModName.slider.minValue = 0;
        sliderModName.slider.maxValue = mods.Count - 1;


        pathMod = mods[(int)sliderModName.slider.value];
    }
    void iniSliderBiome()
    {
        if (pathMod == null || pathMod.Length <= 0)
        {
            sliderBiomeName.slider.minValue = 0;
            sliderBiomeName.slider.maxValue = 0;
            sliderBiomeName.slider.value = 0;
            return;
        }

        string pathBiomes = pathMod + "/" + StrC.biomes;
        if (!Directory.Exists(pathBiomes))
        {
            pathBiome = "";
            return;
        }


        string[] biomes = Directory.GetDirectories(pathBiomes);
        sliderBiomeName.slider.minValue = 0;
        sliderBiomeName.slider.maxValue = biomes.Length - 1;

        if (biomes.Length <= 0)
            return;

        pathBiome = biomes[(int)sliderBiomeName.slider.value];
    }
    public void acceptSliderModName()
    {
        //применяем новый мод
        iniSliderMod();

        //Если пути к моду нет, выходим
        if (pathMod == null ||
            pathMod.Length <= 0)
        {
            sliderModName.SetValueText(textNotHaveMod);
            return;
        }

        //Вытаскиваем имя мода
        string[] path = pathMod.Split("\\");
        string modName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderModName.SetValueText(modName);
    }
    public void acceptSliderBiomeName()
    {
        //применяем новый мод
        iniSliderBiome();

        //если пути к моду нет - выходим
        if (pathBiome == null ||
            pathBiome.Length == 0)
        {
            sliderBiomeName.SetValueText(textNotHaveBiome);
            return;
        }

        //Вытаскиваем имя мода
        string[] path = pathBiome.Split("\\");
        string BlockName = path[path.Length - 1];
        //Изменяем имя слайдера
        sliderBiomeName.SetValueText(BlockName);
    }

    public void clickButtonModNext()
    {
        sliderModName.slider.value++;
        if (sliderModName.slider.value > sliderModName.slider.maxValue)
            sliderModName.slider.value = sliderModName.slider.maxValue;

        acceptSliderModName();
    }
    public void clickButtonModBack()
    {
        sliderModName.slider.value--;
        if (sliderModName.slider.value < sliderModName.slider.minValue)
            sliderModName.slider.value = sliderModName.slider.minValue;

        acceptSliderModName();
    }
    public void clickButtonBiomeNext()
    {
        sliderBiomeName.slider.value++;
        if (sliderBiomeName.slider.value > sliderBiomeName.slider.maxValue)
            sliderBiomeName.slider.value = sliderBiomeName.slider.maxValue;

        acceptSliderBiomeName();
    }
    public void clickButtonBiomeBack()
    {
        sliderBiomeName.slider.value--;
        if (sliderBiomeName.slider.value < sliderBiomeName.slider.minValue)
            sliderBiomeName.slider.value = sliderBiomeName.slider.minValue;

        acceptSliderBiomeName();
    }

    public void clickButtonLoad()
    {
        if (pathBiome == null || pathBiome.Length <= 0)
            return;

        //Вытаскиваем путь
        string[] pathPart = pathBiome.Split("\\");
        string pathNew = "";
        foreach (string path in pathPart)
        {
            if (pathNew.Length != 0)
                pathNew += '/';

            pathNew += path;
        }

        BiomeData biomeData = BiomeData.LoadData(pathNew);
        RedactorBiomeCTRL.SetBiome(biomeData);
        clickButtonClose();
    }
    public void clickButtonClose()
    {
        WindowMenuCTRL.CloseALL(true);
        UICTRL.IsOpenMainMenu = false;
    }
}
