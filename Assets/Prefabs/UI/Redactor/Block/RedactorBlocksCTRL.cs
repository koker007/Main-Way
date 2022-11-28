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
    BlockData[] blockDatas; //Текуший испозьзуемый буфер
    BlockData[] blockDatasBuffer; //Буфер на случай возврата старых данных

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

        variationRedraw();
    }

    void iniBlockDatas() {
        sliderVariationMaximum.slider.value = 0;
        sliderVariationSelected.slider.value = 0;

        blockDatas = new BlockData[(int)sliderVariationMaximum.slider.value + 1];
        for (int num = 0; num < blockDatas.Length; num++) {
            blockDatas[num] = new BlockData();
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
        //Если в буфере пусто
        if (WindowMenuCTRL.buffer.Count == 0)
        {
            if (shtorka.gameObject.activeSelf)
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
        else
        {
            shtorka.color = new Color(0, 0, 0, 0.5f);
        }
    }

    void updateMainParameters() {
        //если блока нет - выходим
        BlockData blockDataLocal = blockData;
        if (blockDataLocal == null)
            return;

        getModName();
        getBlockName();

        variationRedraw();

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
        //Можем применять только если поле ввода есть
        if (inputModName == null ||
            inputModName.inputField.text == null ||
            inputModName.inputField.text.Length == 0
            ) {
            return;
        }

        //Проверяем сам текст
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

        //Проверяем сам текст
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

    //Количество вариаций блока
    public void acceptVariationMaximum()
    {
        //Создаем новый список
        BlockData[] blockDatasNew = new BlockData[(int)sliderVariationMaximum.slider.value];

        //если количество вариаций разширилось
        if (sliderVariationMaximum.slider.value > blockDatas.Length)
        {
            //Заполняем новый список старыми данными
            for (int num = 0; num < blockDatasNew.Length; num++)
            {
                //сперва дублируем то что есть
                if (num < blockDatas.Length) {
                    blockDatasNew[num] = blockDatas[num];
                }
                else
                {
                    //Если в буфере что-то есть вытаскиваем данные из него
                    if (num < blockDatasBuffer.Length)
                    {
                        blockDatasNew[num] = blockDatasBuffer[num];
                    }
                    else {
                        blockDatasNew[num] = new BlockData();
                    }
                }
            }

            blockDatas = blockDatasNew;
            blockDatasBuffer = blockDatas;
        }
        //Иначе если уменьшилось
        else {
            //Если буфер меньше то сохраняем
            if (blockDatasBuffer.Length < blockDatas.Length)
                blockDatasBuffer = blockDatas;

            //то что есть запоминаем в буфер
            for (int num = 0; num < blockDatas.Length; num++)
            {
                blockDatasBuffer[num] = blockDatas[num];
            }

            //Применяем новые значения
            for (int num = 0; num < blockDatasNew.Length; num++) {
                blockDatasNew[num] = blockDatasBuffer[num];
            }

            //Заменяем станое новым
            blockDatas = blockDatasNew;
        }

        acceptGroupParameters();

        variationRedraw();
    }
    public void acceptVariationSelect()
    {
        sliderVariationSelected.SetValueText();
    }

    public void acceptType() {
        //Принять новый тип блока
        if (sliderBlockType.slider.value == (int)BlockData.Type.voxels)
        {
            blockDatas[(int)sliderVariationSelected.slider.value].type = BlockData.Type.voxels;
        }
        else if (sliderBlockType.slider.value == (int)BlockData.Type.liquid) {
            blockDatas[(int)sliderVariationSelected.slider.value].type = BlockData.Type.liquid;
        }
        //По умолчанию блок
        else //if (sliderBlockType.slider.value == (int)BlockData.Type.block)
        {
            blockDatas[(int)sliderVariationSelected.slider.value].type = BlockData.Type.block;
        }
    }

    //Применить груповые данные на все блоки
    void acceptGroupParameters() {
        for (int num = 1; num < blockDatas.Length; num++) {
            blockDatas[num].name = blockDatas[0].name;
            blockDatas[num].mod = blockDatas[0].mod;
            blockDatas[num].variant = num;
        }
    }

    // перерисовать ползунки вариаций
    void variationRedraw() {
        //Обновляем слайдер максимума
        sliderVariationMaximum.slider.minValue = 1;
        sliderVariationMaximum.slider.maxValue = 10;
        sliderVariationMaximum.slider.value = blockDatas.Length;
        sliderVariationMaximum.SetValueText();

        //Обновляем слайдер выбранного
        sliderVariationSelected.slider.minValue = 0;
        sliderVariationSelected.slider.maxValue = sliderVariationMaximum.slider.value - 1;
        sliderVariationSelected.SetValueText();
    }

    public void clickButtonSave() {

        //проверяем что имя мода есть
        if (blockDatas[0].mod == null || blockDatas[0].mod.Length == 0) {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //проверяем что имя мода больше 3
        if (blockDatas[0].mod.Length < 3) {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //Проверяем что имя блока есть
        if (blockDatas[0].name == null || blockDatas[0].name.Length == 0) {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //Проверняем что имя больше 3х символов
        if (blockDatas[0].name.Length < 3) {
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
        BlockData blockDataLocal = blockData;

        if (blockDataLocal.TBlock.wallFace != null)
        {
            blockDataLocal.TBlock.wallFace.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallFace);
        }
        if (blockDataLocal.TBlock.wallBack != null)
        {
            blockDataLocal.TBlock.wallBack.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallBack);
        }
        if (blockDataLocal.TBlock.wallLeft != null)
        {
            blockDataLocal.TBlock.wallLeft.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallLeft);
        }
        if (blockDataLocal.TBlock.wallRight != null)
        {
            blockDataLocal.TBlock.wallRight.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallRight);
        }
        if (blockDataLocal.TBlock.wallUp != null)
        {
            blockDataLocal.TBlock.wallUp.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallUp);
        }
        if (blockDataLocal.TBlock.wallDown != null)
        {
            blockDataLocal.TBlock.wallDown.calcVertices();
            RedactorBlocksColiders.main.delCollidersWall(blockDataLocal.TBlock.wallDown);
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
