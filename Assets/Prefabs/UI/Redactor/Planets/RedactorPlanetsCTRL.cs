using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cosmos;

public class RedactorPlanetsCTRL : MonoBehaviour
{
    [Header("Other")]
    [SerializeField]
    Image shtorka;

    [Header("Terms Gen")]
    [SerializeField]
    SliderCTRL sliderMaxSize;
    [SerializeField]
    SliderCTRL sliderMinSize;
    [SerializeField]
    SliderCTRL sliderMaxTemperature;
    [SerializeField]
    SliderCTRL sliderMinTemperature;
    [SerializeField]
    SliderCTRL sliderStarSpectre;
    [SerializeField]
    SliderCTRL sliderTidalLocking;

    [Header("Terms Gen")]
    [SerializeField]
    SliderCTRL sliderLiquidMax;
    [SerializeField]
    SliderCTRL sliderLiquidMin;
    [SerializeField]
    SliderCTRL sliderAtmosphereMax;
    [SerializeField]
    SliderCTRL sliderAtmosphereMin;

    //Паттерн который редактируем
    PatternPlanet patternPlanet;

    //Объект на примере которого смотрим паттерн (данные)
    public ObjData planetObjData;


    string strMaxSize = "Max Size";
    string strMinSize = "Min Size";
    string strMaxСelsius = "Max Celsius";
    string strMinСelsius = "Min Celsius";

    bool needSave = false;

    string[] keyStarSpectre = {
        "keyNoMatter",
        "keyBlue",
        "keyBlueWhite",
        "keyWhite",
        "keyWhiteYellow",
        "keyYellow",
        "keyOrange",
        "keyRed",
        "keyBrown",
        "keyBlackHole"
    };

    // Start is called before the first frame update
    void Start()
    {
        iniAll();
    }

    // Update is called once per frame
    void Update()
    {
        updateShtorka();

        updateTextAll();

        UpdatePatternData();
        UpdatePlanetData();
    }

    void iniAll() {
        iniMaxSize();
        iniMinSize();
        iniMaxTemperature();
        iniMinTemperature();
        iniStarSpectre();
        iniTidalLocking();

        iniMaxLiquid();
        iniMinLiquid();
        iniMaxAtmosphere();
        iniMinAtmosphere();

        void iniMaxSize()
        {
            sliderMaxSize.slider.maxValue = (int)Size.s16384;
            sliderMaxSize.slider.minValue = (int)Size.s1024;
            sliderMaxSize.slider.value = (int)Size.s16384;
        }
        void iniMinSize()
        {
            sliderMinSize.slider.maxValue = (int)Size.s16384;
            sliderMinSize.slider.minValue = (int)Size.s1024;
            sliderMinSize.slider.value = (int)Size.s1024;
        }
        void iniMaxTemperature()
        {
            sliderMaxTemperature.slider.maxValue = 30;
            sliderMaxTemperature.slider.minValue = 0;
            sliderMaxTemperature.slider.value = sliderMaxTemperature.slider.maxValue;
        }
        void iniMinTemperature() {
            sliderMinTemperature.slider.maxValue = 30;
            sliderMinTemperature.slider.minValue = 0;
            sliderMinTemperature.slider.value = sliderMinTemperature.slider.minValue;
        }
        void iniStarSpectre() {
            sliderStarSpectre.slider.maxValue = keyStarSpectre.Length;
            sliderStarSpectre.slider.minValue = 0;
            sliderStarSpectre.slider.value = 0;
        }
        void iniTidalLocking() {
            sliderTidalLocking.slider.maxValue = (int)PatternPlanet.TidalLocking.Yes;
            sliderTidalLocking.slider.minValue = 0;
            sliderTidalLocking.slider.value = (int)PatternPlanet.TidalLocking.NoMatter;
        }
        void iniMaxLiquid() {
            sliderLiquidMax.slider.maxValue = 100;
            sliderLiquidMax.slider.minValue = 0;
            sliderLiquidMax.slider.value = 60;
        }
        void iniMinLiquid()
        {
            sliderLiquidMin.slider.maxValue = 100;
            sliderLiquidMin.slider.minValue = 0;
            sliderLiquidMin.slider.value = 20;
        }
        void iniMaxAtmosphere() {
            sliderAtmosphereMax.slider.maxValue = 100;
            sliderAtmosphereMax.slider.minValue = 0;
            sliderAtmosphereMax.slider.value = 60;
        }
        void iniMinAtmosphere() {
            sliderAtmosphereMin.slider.maxValue = 100;
            sliderAtmosphereMin.slider.minValue = 0;
            sliderAtmosphereMin.slider.value = 20;
        }
    }

    //Проверка того что данные планеты готовы и соответствуют настройкам редактора
    void UpdatePlanetData()
    {
        //Если данные есть - выходим
        if (planetObjData != null)
            return;

        //Если данных планеты нет, создаем 

        CellS cellS = GalaxyCtrl.galaxy.cells[0, 0, 0];
        //Создаем в нулевой ячейке указанную звезду

        cellS.pos = new Vector3(0.5f, 0.5f, 0.5f);
        cellS.mainObjs = new PlanetData(cellS);
        cellS.mainObjs.size = Size.s65536;
        cellS.mainObjs.radiusChildZone = 500000;

        //Создаем планету
        cellS.mainObjs.childs = new List<ObjData>();

        cellS.mainObjs.childs.Add(new PlanetData(cellS));
        planetObjData = cellS.mainObjs.childs[0];

        float seed = Random.Range(0.0f, 100);

        //Берем рандомный размер
        planetObjData.size = (Size)Random.Range(sliderMinSize.slider.value, sliderMaxSize.slider.value);

        //Создаем объект по паттерну
        planetObjData.GenData(cellS.mainObjs, seed);

        planetObjData.GenPerlinLoc(seed);
    }
    void UpdatePatternData() {
        if (patternPlanet != null)
            return;

        PatternPlanet.Terms terms = new PatternPlanet.Terms();
        terms.sizeMax = (Size)sliderMaxSize.slider.value;
        terms.sizeMin = (Size)sliderMinSize.slider.value;
        terms.temperatureMax = sliderMaxTemperature.slider.value;
        terms.temperatureMax = sliderMaxTemperature.slider.value;
        terms.starSpectre = (PatternPlanet.StarSpectre)sliderStarSpectre.slider.value;
        terms.tidalLocking = (PatternPlanet.TidalLocking)sliderTidalLocking.slider.value;

        PatternPlanet.Parameters parameters = new PatternPlanet.Parameters();
        parameters.LiquidLevelMax = (int)sliderLiquidMax.slider.value;
        parameters.LiquidLevelMin = (int)sliderLiquidMin.slider.value;
        parameters.AtmosphereMax = (int)sliderAtmosphereMax.slider.value;
        parameters.AtmosphereMin = (int)sliderAtmosphereMin.slider.value;


        //создаем новый паттерн
        patternPlanet = new PatternPlanet("", terms, parameters, new List<NoisePlanetData>());
    }

    void updateTextAll() {

        UpdateTextMaxSize();
        UpdateTextMinSize();
        UpdateMaxTemperature();
        UpdateMinTemperature();

        UpdateStarSpectre();
        UpdateTidalLocking();

        UpdateMaxLiquid();
        UpdateMinLiquid();
        UpdateMaxAtmosphere();
        UpdateMinAtmosphere();

        void UpdateTextMaxSize() {
            //sliderMaxSize.SetDefaultText(strMaxSize);
            sliderMaxSize.SetValueText(Calc.GetSizeInt((Size)sliderMaxSize.slider.value).ToString());
        }
        void UpdateTextMinSize() {
            //sliderMinSize.SetDefaultText(strMinSize);
            sliderMinSize.SetValueText(Calc.GetSizeInt((Size)sliderMinSize.slider.value).ToString());
        }
        void UpdateMaxTemperature() {
            //sliderMaxTemperature.SetDefaultText(strMaxСelsius);
            sliderMaxTemperature.SetValueText(((int)Mathf.Pow(2, sliderMaxTemperature.slider.value * 0.5f)).ToString());

        }
        void UpdateMinTemperature() {
            //sliderMinTemperature.SetDefaultText(strMinСelsius);
            sliderMinTemperature.SetValueText(((int)Mathf.Pow(2, sliderMinTemperature.slider.value * 0.5f)).ToString());
        }

        void UpdateStarSpectre() {
            if ((int)sliderStarSpectre.slider.value >= keyStarSpectre.Length)
                return;

            sliderStarSpectre.SetValueText(keyStarSpectre[(int)sliderStarSpectre.slider.value], keyStarSpectre[(int)sliderStarSpectre.slider.value]);
        }

        void UpdateTidalLocking() {
            if(sliderTidalLocking.slider.value == (int)PatternPlanet.TidalLocking.No)
                sliderTidalLocking.SetValueText("No");
            if (sliderTidalLocking.slider.value == (int)PatternPlanet.TidalLocking.NoMatter)
                sliderTidalLocking.SetValueText("No Matter");
            if (sliderTidalLocking.slider.value == (int)PatternPlanet.TidalLocking.Yes)
                sliderTidalLocking.SetValueText("Yes");
        }

        void UpdateMaxLiquid() {
            sliderLiquidMax.SetValueText();
        }
        void UpdateMinLiquid() {
            sliderLiquidMin.SetValueText();
        }

        void UpdateMaxAtmosphere() {
            sliderAtmosphereMax.SetValueText();
        }
        void UpdateMinAtmosphere() {
            sliderAtmosphereMin.SetValueText();
        }
    }
    void updateShtorka() {
        //Если в буфере пусто
        if (WindowMenuCTRL.buffer.Count == 0) {
            if(shtorka.gameObject.activeSelf)
                shtorka.gameObject.SetActive(false);

            return;
        }

        //Если в буфере что-то есть
        if (!shtorka.gameObject.activeSelf)
            shtorka.gameObject.SetActive(true);

        if (needSave)
        {
            shtorka.color = new Color(1, 0, 0, 0.5f);
        }
        else {
            shtorka.color = new Color(0, 0, 0, 0.5f);
        }
    }

    void acceptPattern() {
        if (patternPlanet == null)
            return;

        patternPlanet.termsGenerate.sizeMax = (Size)sliderMaxSize.slider.value;
        patternPlanet.termsGenerate.sizeMin = (Size)sliderMaxSize.slider.value;
        patternPlanet.termsGenerate.temperatureMax = sliderMaxTemperature.slider.value;
        patternPlanet.termsGenerate.temperatuteMin = sliderMinTemperature.slider.value;
        patternPlanet.termsGenerate.starSpectre = (PatternPlanet.StarSpectre)sliderStarSpectre.slider.value;
        patternPlanet.termsGenerate.tidalLocking = (PatternPlanet.TidalLocking)sliderTidalLocking.slider.value;
    }
    public void acceptMaxSize() {
        //Проверяем значение максимальное
        //Если максимальное значение меньше минимального
        if (sliderMaxSize.slider.value < sliderMinSize.slider.value) {
            sliderMinSize.slider.value = sliderMaxSize.slider.value;
        }
        acceptPattern();
    }
    public void acceptMinSize() {
        //Если максимальное значение меньше минимального
        if (sliderMinSize.slider.value > sliderMaxSize.slider.value)
        {
            sliderMaxSize.slider.value = sliderMinSize.slider.value;
        }
        acceptPattern();
    }
    public void acceptMaxTemperature() {
        //Если максимальное значение меньше минимального
        if (sliderMaxTemperature.slider.value < sliderMinTemperature.slider.value)
        {
            sliderMinTemperature.slider.value = sliderMaxTemperature.slider.value;
        }
        acceptPattern();
    }
    public void acceptMinTemperature() {
        if (sliderMinTemperature.slider.value > sliderMaxTemperature.slider.value)
        {
            sliderMaxTemperature.slider.value = sliderMinTemperature.slider.value;
        }
        acceptPattern();
    }
    public void acceptStarSpectre() {

        acceptPattern();
    }
    public void acceptTidalLocking() {

        acceptPattern();
    }

    public void acceptMaxLiquid() {
        if (sliderLiquidMax.slider.value < sliderLiquidMin.slider.value)
        {
            sliderLiquidMin.slider.value = sliderLiquidMax.slider.value;
        }
        acceptPattern();
    }
    public void acceptMinLiquid() {
        if (sliderLiquidMin.slider.value > sliderLiquidMax.slider.value)
        {
            sliderLiquidMax.slider.value = sliderLiquidMin.slider.value;
        }
        acceptPattern();
    }

    public void acceptMaxAtmosphere() {
        if (sliderAtmosphereMax.slider.value < sliderAtmosphereMin.slider.value)
        {
            sliderAtmosphereMin.slider.value = sliderAtmosphereMax.slider.value;
        }
        acceptPattern();
    }
    public void acceptMinAtmosphere() {
        if (sliderAtmosphereMin.slider.value > sliderAtmosphereMax.slider.value)
        {
            sliderAtmosphereMax.slider.value = sliderAtmosphereMin.slider.value;
        }
        acceptPattern();
    }

}
