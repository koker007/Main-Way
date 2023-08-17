using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cosmos;

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

    //������ ������� � ������� ���������� ��������� �����
    public PlanetData planetData;

    //������� ������� �� ������� ��������� ����
    public PatternPlanet patternPlanet;
    //������ �����
    public BiomeData biomeData;

    BiomeTypeSurface bufferBiomeSurface; //Type 0
    BiomeTypeUnderground bufferBiomeUnderground; //Type 1
    BiomeTypeDwarf bufferBiomeDwarf; // Type 2
    BiomeTypeRings bufferBiomeRings; //Type 3

    //���� ������ ����� ���� ��������
    static public event Action changeBiome;

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
    private void Start()
    {
        main = this;
        inicialize();

        changeBiome += updateUI;

    }

    // Update is called once per frame
    void Update()
    {
        TestOpenGenerator();

        CreatePlanet();

        ChangePlanet();
    }

    static public void SetBiome(BiomeData biomeData) {
        if (biomeData == null)
            return;

        main.biomeData = biomeData;

        if (main.planetData == null)
            return;

        main.planetData.ClearAllBuffer();
        changeBiome();
    }

    void TestOpenGenerator() {
        if (RedactorBiomeGenerator.MAIN != null && !RedactorBiomeGenerator.MAIN.gameObject.activeSelf)
            RedactorBiomeGenerator.MAIN.gameObject.SetActive(true);
    }

    public void clickButtonSave()
    {

        //��������� ��� ��� ���� ����
        if (biomeData.mod == null || biomeData.mod.Length == 0)
        {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //��������� ��� ��� ���� ������ 3
        if (biomeData.mod.Length < 3)
        {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //��������� ��� ��� ����� ����
        if (biomeData.name == null || biomeData.name.Length == 0)
        {
            Debug.Log("NotSave Need Biome Name");
            return;
        }
        //���������� ��� ��� ������ 3� ��������
        if (biomeData.name.Length < 3)
        {
            Debug.Log("NotSave Need Biome Name Lenght > 3");
            return;
        }

        BiomeData.SaveData(biomeData);
    }

    public void clickButtonLoad()
    {
        WindowMenuCTRL.CloseALL(true);
        WindowMenuCTRL.ClickRedactorBiomeLoad();
    }

    public void acceptBiomeMod() {
        biomeData.mod = Mod.text;
    }
    public void acceptBiomeName()
    {
        biomeData.name = Name.text;
    }

    public void updateUI() {
        testName();
        testSliderType();
        testSliderRules();

        void testName() {
            Mod.text = biomeData.mod;
            Name.text = biomeData.name;
        }
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
        changeBiome();
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
        BlockData select = Game.Blocks.GetData(biomeData.genRules[(int)sliderRules.slider.value].blockID, 0);

        return select;
    }
    public BiomeData.GenRule GetSelectGenRule() {
        BiomeData.GenRule genRule = biomeData.genRules[(int)sliderRules.slider.value];
        return genRule;
    }

    public void SetSelectBlock(string modName, string blockName) {

        int blockID = Game.Blocks.GetBlockID(modName, blockName);
        int ruleNum = (int)sliderRules.slider.value;

        BiomeData.GenRule genRule = biomeData.genRules[ruleNum];
        genRule.blockID = blockID;
        biomeData.genRules[ruleNum] = genRule;

        changeBiome();
    }
    public void SetSelectRuleParameters(float scaleAll, int octaves, int freq, Vector3 scale) {
        int ruleNum = (int)sliderRules.slider.value;

        BiomeData.GenRule genRule = biomeData.genRules[ruleNum];

        genRule.scaleAll = scaleAll;
        genRule.octaves = octaves;
        genRule.scaleX = scale.x;
        genRule.scaleY = scale.y;
        genRule.scaleZ = scale.z;
        genRule.freq = freq;

        biomeData.genRules[ruleNum] = genRule;

        changeBiome();
    }

    public void CreatePlanet() {
        if (planetData != null)
            return;

        CellS cellS = new CellS(new Vector3(UnityEngine.Random.Range(0, 15), UnityEngine.Random.Range(0, 15), UnityEngine.Random.Range(0, 15)), UnityEngine.Random.Range(0.0f, 1.0f), GalaxyCtrl.galaxy);

        StarData starData = new StarData(cellS);
        starData.GenData(null, UnityEngine.Random.Range(0.0f, 1.0f));
        starData.GenChilds(Calc.GetSizeInt(Size.s65536) * 10, UnityEngine.Random.Range(0.0f, 1.0f));

        foreach (PlanetData planetData in starData.childs) {
            if (planetData == null || (int)planetData.size < (int)Size.s1024)
                continue;

            this.planetData = planetData;
            RedactorBiomeVisualizator.MAIN.heightMapAll = null;
            break;
        }

        patternPlanet ??= PatternPlanet.GetTestPattern();

        ReCreatePatternPlanet();
    }

    void ReCreatePatternPlanet()
    {
        planetData.pattern = patternPlanet;

        //���������� �����
        patternPlanet.biomesSurface = new List<BiomeTypeSurface>();
        patternPlanet.biomesUnderground = new List<BiomeTypeUnderground>();

        ReLoadBiomes();
    }
    void ReLoadBiomes() {
        //�������� ��� �����
        patternPlanet.biomesSurface = new List<BiomeTypeSurface>();
        patternPlanet.biomesUnderground = new List<BiomeTypeUnderground>();

        BiomeTypeSurface surface = biomeData as BiomeTypeSurface;
        BiomeTypeUnderground underground = biomeData as BiomeTypeUnderground;
        BiomeTypeDwarf dwarf = biomeData as BiomeTypeDwarf;
        BiomeTypeRings rings = biomeData as BiomeTypeRings;

        //�������� ������������� ���� � ������� ��������
        if (sliderType.slider.value == (int)BiomeData.Type.Surface && surface != null)
            CreateSurface();
        else if (sliderType.slider.value == (int)BiomeData.Type.Underground && underground != null)
            CreateUnderground();
        else if (sliderType.slider.value == (int)BiomeData.Type.Dwarf && dwarf != null)
            CreateDwarf();
        else if (sliderType.slider.value == (int)BiomeData.Type.Rings && rings != null)
            CreateRings();

        void CreateSurface() {
            patternPlanet.biomesSurface.Add(surface);
            for (int num = 0; num < 5; num++)
                patternPlanet.biomesSurface.Add(BiomeTypeSurface.GetTestBiome());
        }
        void CreateUnderground() {
            patternPlanet.biomesUnderground.Add(underground);
            for (int num = 0; num < 5; num++)
                patternPlanet.biomesUnderground.Add(BiomeTypeUnderground.GetTestBiome());
        }
        void CreateDwarf() {
            
        }
        void CreateRings() {
            
        }
    }

    public void ChangePlanet() {
        if (!Input.GetKeyDown(KeyCode.P))
            return;

        planetData = null;

        CreatePlanet();
    }
}
