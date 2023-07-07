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

    //������ �����
    public BiomeData biomeData;

    BiomeTypeSurface bufferBiomeSurface; //Type 0
    BiomeTypeUnderground bufferBiomeUnderground; //Type 1
    BiomeTypeDwarf bufferBiomeDwarf; // Type 2
    BiomeTypeRings bufferBiomeRings; //Type 3

    //���� ������ ����� ���� ��������
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
        //��������� ��� ��� ���� ����
        if (blockDatas[0].mod == null || blockDatas[0].mod.Length == 0)
        {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //��������� ��� ��� ���� ������ 3
        if (blockDatas[0].mod.Length < 3)
        {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //��������� ��� ��� ����� ����
        if (blockDatas[0].name == null || blockDatas[0].name.Length == 0)
        {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //���������� ��� ��� ������ 3� ��������
        if (blockDatas[0].name.Length < 3)
        {
            Debug.Log("NotSave Need Block Name Lenght > 3");
            return;
        }

        //��������� ����� ������ �� ��� �����
        acceptGroupParameters();

        //��������� ��� �����
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

        //���� ��� ������ �� ���������
        if (!isTypeChanges())
            return;

        //��������� ��� ������ ���������
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

            //��������� ������� ���
            if (biomeTypeSurface != null)
                bufferBiomeSurface = biomeTypeSurface;
            else if (biomeTypeUnderground != null)
                bufferBiomeUnderground = biomeTypeUnderground;
            else if (biomeTypeDwarf != null)
                bufferBiomeDwarf = biomeTypeDwarf;
            else if (biomeTypeRings != null)
                bufferBiomeRings = biomeTypeRings;

            BiomeData bufferBiome = biomeData;

            //������ ��� �� ��� ��� ������
            if (sliderType.slider.value == 0) setBiomeSurface();
            else if (sliderType.slider.value == 1) setBiomeUnderground();
            else if (sliderType.slider.value == 2) setBiomeDwarf();
            else if (sliderType.slider.value == 3) setBiomeRings();

            //��������� ������� ��������� ��� ���� ����� ������
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
        //��������� ������� ������� ���������
        updateUI();
    }
    public void clickButtonAddRule()
    {
        AddRule();

        changeBiome();

        void AddRule() {
            //�������� ����� �������
            BiomeData.GenRule ruleNew = new BiomeData.GenRule();

            //��������� ��� ������� �� ������� �� ������� ������
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

            //������ ���� ������ ���� ������� ���������
            if (biomeData.genRules.Count == 0)
                biomeData.genRules.Add(new BiomeData.GenRule());
        }
    }

    public void clickButtonPriorityUpper()
    {
        PriorityUP();

        changeBiome();

        void PriorityUP() {
            //���� ������� ������� � ������, �� �������
            if ((int)sliderRules.slider.value <= 0)
                return;

            //������������� �� ���� ����� �������
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
            //���� ������� ��������� � ������, �� �������
            if ((int)sliderRules.slider.value >= biomeData.genRules.Count - 1)
                return;

            BiomeData.GenRule temp = biomeData.genRules[(int)sliderRules.slider.value];
            biomeData.genRules[(int)sliderRules.slider.value] = biomeData.genRules[(int)sliderRules.slider.value + 1];
            biomeData.genRules[(int)sliderRules.slider.value + 1] = temp;

            //������������� �� ���� ����� ������� ��� ���� ��������
            sliderRules.slider.value++;
        }
    }


    public BlockData GetSelectBlock() {

        //�������� ��������� �������
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
