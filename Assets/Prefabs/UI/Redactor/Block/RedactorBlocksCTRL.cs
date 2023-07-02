using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBlocksCTRL : MonoBehaviour
{
    public static RedactorBlocksCTRL main;
    public const float voxSize = 0.0625f;

    [Header("Other")]
    [SerializeField]
    Image shtorka;

    //BlockData blockDataLocal;
    BlockData[] blockDatas; //������� ������������ �����
    BlockData[] blockDatasBuffer; //����� �� ������ �������� ������ ������

    static public BlockData blockData{
        get {
            if (main == null)
                return null;

            if (main.blockDatas == null)
                return null;

            if (main.blockDatas.Length <= main.sliderVariationSelected.slider.value)
                return null;

            return main.blockDatas[(int)main.sliderVariationSelected.slider.value];
        }
    }

    bool needSave = false;

    [SerializeField]
    RedactorBlockRightPanelsCTRL panelsCTRL;

    [SerializeField]
    InputFieldCTRL inputModName;
    [SerializeField]
    InputFieldCTRL inputBlockName;
    [SerializeField]
    SliderCTRL sliderVariationMaximum;
    [SerializeField]
    SliderCTRL sliderVariationSelected;
    [SerializeField]
    SliderCTRL sliderBlockType;

    // Start is called before the first frame update
    void Start()
    {
        main = this;

        iniBlockDatas();

        redrawVariation();
        redrawType();
    }

    void iniBlockDatas() {
        sliderVariationMaximum.slider.value = 0;
        sliderVariationSelected.slider.value = 0;

        blockDatas = new BlockData[(int)sliderVariationMaximum.slider.value + 1];
        for (int num = 0; num < blockDatas.Length; num++) {
            blockDatas[num] = new TypeBlock();
        }

        blockDatasBuffer = new BlockData[1];

        acceptVariationMaximum();
        acceptVariationSelect();
    }

    // Update is called once per frame
    void Update()
    {
        updateShtorka();
    }

    void updateShtorka()
    {
        //���� � ������ �����
        if (WindowMenuCTRL.buffer.Count == 0)
        {
            if (shtorka.gameObject.activeSelf)
                shtorka.gameObject.SetActive(false);

            return;
        }

        //���� � ������ ���-�� ����
        if (!shtorka.gameObject.activeSelf)
            shtorka.gameObject.SetActive(true);

        if (needSave)
        {
            shtorka.color = new Color(1, 0, 0, 0.5f);
        }
        else
        {
            shtorka.color = new Color(0, 0, 0, 0.5f);
        }
    }

    void updateMainParameters() {
        //���� ����� ��� - �������
        BlockData blockDataLocal = blockData;
        if (blockDataLocal == null)
            return;

        getModName();
        getBlockName();

        redrawVariation();
        redrawType();

        void getModName() {
            if (blockDatas[0].mod == null ||
                blockDatas[0].mod.Length <= 0) {

                inputModName.inputField.text = "";
                return;
            }

            inputModName.inputField.text = blockDatas[0].mod;
        }
        void getBlockName() {
            if (blockDatas[0].name == null ||
                blockDatas[0].name.Length <= 0)
            {

                inputBlockName.inputField.text = "";
                return;
            }

            inputBlockName.inputField.text = blockDatas[0].name;
        }
    }
    public void acceptModName() {
        //����� ��������� ������ ���� ���� ����� ����
        if (inputModName == null ||
            inputModName.inputField.text == null ||
            inputModName.inputField.text.Length == 0
            ) {
            return;
        }

        //��������� ��� �����
        if (inputModName.inputField.text[0] == ' ') {
            string textNew = "";
            bool symbolOld = false;
            for (int x = 0; x < inputModName.inputField.text.Length; x++) {
                if (inputModName.inputField.text[x] == ' ')
                {
                    if (!symbolOld)
                        continue;
                }
                else {
                    symbolOld = true;
                }
                textNew += inputModName.inputField.text;
            }

            inputModName.inputField.text = textNew;
        }

        blockDatas[0].mod = inputModName.inputField.text;

    }
    public void acceptBlockName() {
        if (inputBlockName == null ||
           inputBlockName.inputField.text == null ||
           inputBlockName.inputField.text.Length == 0) {
            return;
        }

        //��������� ��� �����
        if (inputBlockName.inputField.text[0] == ' ')
        {
            string textNew = "";
            bool symbolOld = false;
            for (int x = 0; x < inputBlockName.inputField.text.Length; x++)
            {
                if (inputBlockName.inputField.text[x] == ' ')
                {
                    if (!symbolOld)
                        continue;
                }
                else
                {
                    symbolOld = true;
                }
                textNew += inputBlockName.inputField.text;
            }

            inputBlockName.inputField.text = textNew;
        }

        blockDatas[0].name = inputBlockName.inputField.text;
    }

    //���������� �������� �����
    public void acceptVariationMaximum()
    {
        //������� ����� ������
        BlockData[] blockDatasNew = new BlockData[(int)sliderVariationMaximum.slider.value];

        //���� ���������� �������� �����������
        if (sliderVariationMaximum.slider.value > blockDatas.Length)
        {
            //��������� ����� ������ ������� �������
            for (int num = 0; num < blockDatasNew.Length; num++)
            {
                //������ ��������� �� ��� ����
                if (num < blockDatas.Length) {
                    blockDatasNew[num] = blockDatas[num];
                }
                else
                {
                    //���� � ������ ���-�� ���� ����������� ������ �� ����
                    if (num < blockDatasBuffer.Length)
                    {
                        blockDatasNew[num] = blockDatasBuffer[num];
                    }
                    else {
                        blockDatasNew[num] = new TypeBlock();
                    }
                }
            }

            blockDatas = blockDatasNew;
            blockDatasBuffer = blockDatas;
        }
        //����� ���� �����������
        else {
            //���� ����� ������ �� ���������
            if (blockDatasBuffer.Length < blockDatas.Length)
                blockDatasBuffer = blockDatas;

            //�� ��� ���� ���������� � �����
            for (int num = 0; num < blockDatas.Length; num++)
            {
                blockDatasBuffer[num] = blockDatas[num];
            }

            //��������� ����� ��������
            for (int num = 0; num < blockDatasNew.Length; num++) {
                blockDatasNew[num] = blockDatasBuffer[num];
            }

            //�������� ������ �����
            blockDatas = blockDatasNew;
        }

        acceptGroupParameters();

        redrawVariation();
    }
    public void acceptVariationSelect()
    {
        sliderVariationSelected.SetValueText();
    }

    public void acceptType() {
        //������� ����� ��� �����
        if (sliderBlockType.slider.value == (int)BlockData.Type.voxels)
        {
            TypeVoxel typeVoxel = blockDatas[(int)sliderVariationSelected.slider.value] as TypeVoxel;
            typeVoxel ??= new TypeVoxel(blockDatas[(int)sliderVariationSelected.slider.value]);

            blockDatas[(int)sliderVariationSelected.slider.value] = typeVoxel;
        }
        else if (sliderBlockType.slider.value == (int)BlockData.Type.liquid) {
            TypeLiquid typeLiquid = blockDatas[(int)sliderVariationSelected.slider.value] as TypeLiquid;
            typeLiquid ??= new TypeLiquid(blockDatas[(int)sliderVariationSelected.slider.value]);

            blockDatas[(int)sliderVariationSelected.slider.value] = typeLiquid;
        }
        //�� ��������� ����
        else //if (sliderBlockType.slider.value == (int)BlockData.Type.block)
        {
            TypeBlock typeBlock = blockDatas[(int)sliderVariationSelected.slider.value] as TypeBlock;
            typeBlock ??= new TypeBlock(blockDatas[(int)sliderVariationSelected.slider.value]);

            blockDatas[(int)sliderVariationSelected.slider.value] = typeBlock;
        }

        redrawType();

        if(RedactorBlockRightPanelsCTRL.main)
            RedactorBlockRightPanelsCTRL.main.ClickSliderPanels();


    }

    //��������� �������� ������ �� ��� �����
    void acceptGroupParameters() {
        for (int num = 1; num < blockDatas.Length; num++) {
            blockDatas[num].name = blockDatas[0].name;
            blockDatas[num].mod = blockDatas[0].mod;
            blockDatas[num].variant = num;
        }
    }

    // ������������ ��������
    void redrawVariation() {
        //��������� ������� ���������
        sliderVariationMaximum.slider.minValue = 1;
        sliderVariationMaximum.slider.maxValue = 10;
        sliderVariationMaximum.slider.value = blockDatas.Length;
        sliderVariationMaximum.SetValueText();

        //��������� ������� ����������
        sliderVariationSelected.slider.minValue = 0;
        sliderVariationSelected.slider.maxValue = sliderVariationMaximum.slider.value - 1;
        sliderVariationSelected.SetValueText();
    }
    void redrawType() {
        BlockData data = blockData;
        TypeBlock typeBlock = data as TypeBlock;
        TypeVoxel typeVoxel = data as TypeVoxel;
        TypeLiquid typeLiquid = data as TypeLiquid;

        int maximum = (int)BlockData.Type.block;
        if (maximum < (int)BlockData.Type.liquid)
            maximum = (int)BlockData.Type.liquid;
        else if (maximum < (int)BlockData.Type.voxels)
            maximum = (int)BlockData.Type.voxels;

        sliderBlockType.slider.minValue = 0;
        sliderBlockType.slider.maxValue = maximum;


        //������ �������� ��������
        if (typeBlock != null)
        {
            sliderBlockType.slider.value = (int)BlockData.Type.block;
            sliderBlockType.SetValueText(StrC.blocks);
        }
        else if (typeVoxel != null)
        {
            sliderBlockType.slider.value = (int)BlockData.Type.voxels;
            sliderBlockType.SetValueText(StrC.voxels);
        }
        else if (typeLiquid != null)
        {
            sliderBlockType.slider.value = (int)BlockData.Type.liquid;
            sliderBlockType.SetValueText(StrC.liquid);
        }
    }

    public void clickButtonSave() {

        //��������� ��� ��� ���� ����
        if (blockDatas[0].mod == null || blockDatas[0].mod.Length == 0) {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //��������� ��� ��� ���� ������ 3
        if (blockDatas[0].mod.Length < 3) {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //��������� ��� ��� ����� ����
        if (blockDatas[0].name == null || blockDatas[0].name.Length == 0) {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //���������� ��� ��� ������ 3� ��������
        if (blockDatas[0].name.Length < 3) {
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
    }
    public void clickButtonLoad() {
        WindowMenuCTRL.CloseALL(true);
        WindowMenuCTRL.ClickRedactorBlockLoad();
    }

    public void loadBlock(string pathBlock) {

        blockDatas = BlockData.LoadDatas(pathBlock);
        reDrawBlock();

        panelsCTRL.ReDrawingAll();

        updateMainParameters();
    }

    public void reDrawBlock() {
        TypeBlock typeBlock = blockData as TypeBlock;

        if (typeBlock == null)
            return;

        if (typeBlock.wallFace != null)
        {
            typeBlock.wallFace.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallFace);
        }
        if (typeBlock.wallBack != null)
        {
            typeBlock.wallBack.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallBack);
        }
        if (typeBlock.wallLeft != null)
        {
            typeBlock.wallLeft.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallLeft);
        }
        if (typeBlock.wallRight != null)
        {
            typeBlock.wallRight.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallRight);
        }
        if (typeBlock.wallUp != null)
        {
            typeBlock.wallUp.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallUp);
        }
        if (typeBlock.wallDown != null)
        {
            typeBlock.wallDown.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(typeBlock.wallDown);
        }
    }
}

public enum TypeBlockTransparent {
    NoTransparent = 0,
    CutOff = 1,
    Alpha = 2
}
public struct KeyBlockTransparent {
    public const string keyOff = "keyOff";
    public const string keyCutOff = "keyCutOff";
    public const string keyAlpha = "keyAlpha";
}
