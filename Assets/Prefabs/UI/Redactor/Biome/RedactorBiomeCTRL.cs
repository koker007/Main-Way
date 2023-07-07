using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RedactorBiomeCTRL : MonoBehaviour
{
    static public RedactorBiomeCTRL main;

    [SerializeField]
    Material materialNormal;
    [SerializeField]
    Material materialTransparent;
    [SerializeField]
    Material materialCutOff;

    [SerializeField]
    InputFieldCTRL Mod;
    [SerializeField]
    InputFieldCTRL Name;

    [SerializeField]
    SliderCTRL sliderType;
    const int typeMax = 4;

    [SerializeField]
    SliderCTRL sliderRules;

    //Данные биома
    public BiomeData biomeData;

    BiomeTypeSurface bufferBiomeSurface; //Type 0
    BiomeTypeUnderground bufferBiomeUnderground; //Type 1
    BiomeTypeDwarf bufferBiomeDwarf; // Type 2
    BiomeTypeRings bufferBiomeRings; //Type 3

    //Если данные биома были изменены
    static public event Action changeBiome;
    static public event Action changeSelectBlock;

    const string keySliderType = "RedactorBiomeSliderType";
    const string keySliderRules = "RedactorBiomeSliderRule";

    const string keyBiomeSurface = "RedactorBiomeSurface";
    const string keyBiomeUnderground = "RedactorBiomeUnderground";
    const string keyBiomeDwarf = "RedactorBiomeDwarf";
    const string keyBiomeRings = "RedactorBiomeRings";

    void inicialize()
    {
        biomeData ??= GetNewBiomeType();

        iniSliderType();
        iniSliderRules();

        void iniSliderType()
        {
            sliderType.slider.minValue = 0;
            sliderType.slider.maxValue = typeMax - 1;
            sliderType.slider.value = 0;

        }
        void iniSliderRules()
        {
            sliderRules.slider.minValue = 0;
            sliderRules.slider.maxValue = biomeData.genRules.Count - 1;
        }

        BiomeData GetNewBiomeType() {

            BiomeData biomeTypeNew;

            if (sliderType.slider.value == 1)
                biomeTypeNew = new BiomeTypeUnderground();
            else if (sliderType.slider.value == 2)
                biomeTypeNew = new BiomeTypeDwarf();
            else if (sliderType.slider.value == 3)
                biomeTypeNew = new BiomeTypeRings();

            else
                biomeTypeNew = new BiomeTypeSurface();

            return biomeTypeNew;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        inicialize();

        changeBiome += updateUI;
    }

    // Update is called once per frame
    void Update()
    {
        TestOpenGenerator();
    }

    void TestOpenGenerator() {
        if (RedactorBiomeGenerator.MAIN != null && !RedactorBiomeGenerator.MAIN.gameObject.activeSelf)
            RedactorBiomeGenerator.MAIN.gameObject.SetActive(true);
    }

    public void clickButtonSave()
    {

        /*
        //проверяем что имя мода есть
        if (blockDatas[0].mod == null || blockDatas[0].mod.Length == 0)
        {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //проверяем что имя мода больше 3
        if (blockDatas[0].mod.Length < 3)
        {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //Проверяем что имя блока есть
        if (blockDatas[0].name == null || blockDatas[0].name.Length == 0)
        {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //Проверняем что имя больше 3х символов
        if (blockDatas[0].name.Length < 3)
        {
            Debug.Log("NotSave Need Block Name Lenght > 3");
            return;
        }

        //Применяем общие данные на все блоки
        acceptGroupParameters();

        //Сохраняем все блоки
        for (int num = 0; num < blockDatas.Length; num++)
        {
            BlockData.SaveData(blockDatas[num]);
        }
        */
    }

    public void clickButtonLoad()
    {
        WindowMenuCTRL.CloseALL(true);
        //WindowMenuCTRL.ClickRedactorBlockLoad();
    }


    public void updateUI() {
        testSliderType();
        testSliderRules();

        void testSliderType()
        {
            sliderType.SetDefaultText(keySliderType, "Type");

            if (sliderType.slider.value == 0)
                sliderType.SetValueText(Language.GetTextFromKey(keyBiomeSurface, "Surface"));
            else if (sliderType.slider.value == 1)
                sliderType.SetValueText(Language.GetTextFromKey(keyBiomeUnderground, "Underground"));
            else if (sliderType.slider.value == 2)
                sliderType.SetValueText(Language.GetTextFromKey(keyBiomeDwarf, "Dwarf"));
            else if (sliderType.slider.value == 3)
                sliderType.SetValueText(Language.GetTextFromKey(keyBiomeRings, "Rings"));
            else
                sliderType.SetValueText("???");
        }

        void testSliderRules() {
            sliderRules.slider.minValue = 0;
            sliderRules.slider.maxValue = biomeData.genRules.Count - 1;

            sliderRules.SetDefaultText(keySliderRules, "Rule");
            sliderRules.SetValueText();
        }

    }
    public void sliderBiomeTypeChange()
    {

        BiomeTypeSurface biomeTypeSurface = biomeData as BiomeTypeSurface;
        BiomeTypeUnderground biomeTypeUnderground = biomeData as BiomeTypeUnderground;
        BiomeTypeDwarf biomeTypeDwarf = biomeData as BiomeTypeDwarf;
        BiomeTypeRings biomeTypeRings = biomeData as BiomeTypeRings;

        //Если тип данных не изменился
        if (!isTypeChanges())
            return;

        //требуемый тип данных изменился
        acceptType();

        changeBiome();

        bool isTypeChanges()
        {
            if (sliderType.slider.value == 0 && biomeTypeSurface != null ||
                sliderType.slider.value == 1 && biomeTypeUnderground != null ||
                sliderType.slider.value == 2 && biomeTypeDwarf != null ||
                sliderType.slider.value == 3 && biomeTypeRings != null)
                return false;

            return true;
        }
        void acceptType()
        {

            //Сохраняем текущий тип
            if (biomeTypeSurface != null)
                bufferBiomeSurface = biomeTypeSurface;
            else if (biomeTypeUnderground != null)
                bufferBiomeUnderground = biomeTypeUnderground;
            else if (biomeTypeDwarf != null)
                bufferBiomeDwarf = biomeTypeDwarf;
            else if (biomeTypeRings != null)
                bufferBiomeRings = biomeTypeRings;

            BiomeData bufferBiome = biomeData;

            //Меняем тип на тот что выбран
            if (sliderType.slider.value == 0) setBiomeSurface();
            else if (sliderType.slider.value == 1) setBiomeUnderground();
            else if (sliderType.slider.value == 2) setBiomeDwarf();
            else if (sliderType.slider.value == 3) setBiomeRings();

            //применяем базовые параметры для всех типов биомов
            biomeData.mod = bufferBiome.mod;
            biomeData.name = bufferBiome.name;
            biomeData.genRules = bufferBiome.genRules;

            void setBiomeSurface()
            {
                if (bufferBiomeSurface == null)
                    bufferBiomeSurface = new BiomeTypeSurface();

                biomeData = bufferBiomeSurface;
            }
            void setBiomeUnderground()
            {
                if (bufferBiomeUnderground == null)
                    bufferBiomeUnderground = new BiomeTypeUnderground();

                biomeData = bufferBiomeUnderground;
            }
            void setBiomeDwarf()
            {
                if (bufferBiomeDwarf == null)
                    bufferBiomeDwarf = new BiomeTypeDwarf();

                biomeData = bufferBiomeDwarf;
            }
            void setBiomeRings()
            {
                if (bufferBiomeRings == null)
                    bufferBiomeRings = new BiomeTypeRings();

                biomeData = bufferBiomeRings;
            }

        }
    }
    public void sliderSelectRuleUpdate()
    {
        //Обновляем слайдер правила генерации
        updateUI();
    }
    public void clickButtonAddRule()
    {
        AddRule();

        changeBiome();

        void AddRule() {
            //Добавить новое правило
            BiomeData.GenRule ruleNew = new BiomeData.GenRule();

            //проверяем что слайдер не выходит за пределы правил
            if (sliderRules.slider.value >= biomeData.genRules.Count)
            {
                sliderRules.slider.value = biomeData.genRules.Count - 1;
            }

            biomeData.genRules.Insert((int)sliderRules.slider.value, ruleNew);

        }
    }
    public void clickButtonDeleteRule()
    {
        DelRule();

        changeBiome();

        if(changeSelectBlock != null)
            changeSelectBlock();

        void DelRule() {
            
            biomeData.genRules.RemoveAt((int)sliderRules.slider.value);

            //Должно быть хотябы одно правило генерации
            if (biomeData.genRules.Count == 0)
                biomeData.genRules.Add(new BiomeData.GenRule());
        }
    }

    public void clickButtonPriorityUpper()
    {
        PriorityUP();

        changeBiome();

        void PriorityUP() {
            //Если элемент нулевой в списке, то выходим
            if ((int)sliderRules.slider.value <= 0)
                return;

            //Переключаемся на тоже самое правило
            BiomeData.GenRule temp = biomeData.genRules[(int)sliderRules.slider.value];
            biomeData.genRules[(int)sliderRules.slider.value] = biomeData.genRules[(int)sliderRules.slider.value - 1];
            biomeData.genRules[(int)sliderRules.slider.value - 1] = temp;

            sliderRules.slider.value--;
        }
    }
    public void clickButtonPriorityDowner()
    {
        PriorityDown();

        changeBiome();

        void PriorityDown() {
            //Если элемент последний в списке, то выходим
            if ((int)sliderRules.slider.value >= biomeData.genRules.Count - 1)
                return;

            BiomeData.GenRule temp = biomeData.genRules[(int)sliderRules.slider.value];
            biomeData.genRules[(int)sliderRules.slider.value] = biomeData.genRules[(int)sliderRules.slider.value + 1];
            biomeData.genRules[(int)sliderRules.slider.value + 1] = temp;

            //Переключаемся на тоже самое правило что было активным
            sliderRules.slider.value++;
        }
    }


    public BlockData GetSelectBlock() {

        //Выбираем выбранное правило
        BlockData select = GameData.Blocks.GetData(biomeData.genRules[0].blockID, 0);

        return select;
    }

    public void SetSelectBlock(string modName, string blockName) {

        int blockID = GameData.Blocks.GetBlockID(modName, blockName);
        int ruleNum = (int)sliderRules.slider.value;

        biomeData.genRules[ruleNum].blockID = blockID;

        changeSelectBlock();
    }
}
