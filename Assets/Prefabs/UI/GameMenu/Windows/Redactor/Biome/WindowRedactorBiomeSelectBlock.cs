using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class WindowRedactorBiomeSelectBlock : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderModName;
    [SerializeField]
    SliderCTRL sliderBlockName;

    [SerializeField]
    RenderTexture renderBlock;
    [SerializeField]
    RawImage rawImageBlock;

    [SerializeField]
    Button buttonLoad;

    string textNotHaveMod = "None";
    string textNotHaveBlock = "None";

    string pathMod;
    string pathBlock;

    event Action changeBlock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void clickButtonModNext()
    {
        sliderModName.slider.value++;
        if (sliderModName.slider.value > sliderModName.slider.maxValue)
            sliderModName.slider.value = sliderModName.slider.maxValue;

        changeBlock();
    }
    public void clickButtonModBack()
    {
        sliderModName.slider.value--;
        if (sliderModName.slider.value < sliderModName.slider.minValue)
            sliderModName.slider.value = sliderModName.slider.minValue;

        changeBlock();
    }
    public void clickButtonBlockNext()
    {
        sliderBlockName.slider.value++;
        if (sliderBlockName.slider.value > sliderBlockName.slider.maxValue)
            sliderBlockName.slider.value = sliderBlockName.slider.maxValue;

        changeBlock();
    }
    public void clickButtonBlockBack()
    {
        sliderBlockName.slider.value--;
        if (sliderBlockName.slider.value < sliderBlockName.slider.minValue)
            sliderBlockName.slider.value = sliderBlockName.slider.minValue;

        changeBlock();

    }

    public void clickButtonLoad()
    {
        if (pathBlock == null || pathBlock.Length <= 0)
            return;

        //����������� ����
        string[] pathPart = pathBlock.Split("\\");
        string pathNew = "";
        foreach (string path in pathPart)
        {
            if (pathNew.Length != 0)
                pathNew += '/';

            pathNew += path;
        }

        RedactorBlocksCTRL.main.loadBlock(pathNew);
        clickButtonClose();
    }

    public void clickButtonClose()
    {
        WindowMenuCTRL.CloseALL(true);
        UICTRL.IsOpenMainMenu = false;
    }

    public void acceptModName()
    {
        //��������� ����� ���
        updateSliderMod();

        //���� ���� � ���� ���, �������
        if (pathMod == null ||
            pathMod.Length <= 0)
        {
            sliderModName.SetValueText(textNotHaveMod);
            return;
        }

        //����������� ��� ����
        string[] path = pathMod.Split("\\");
        string modName = path[path.Length - 1];
        //�������� ��� ��������
        sliderModName.SetValueText(modName);

        void updateSliderMod()
        {
            //�������� ��� ����
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
    }
    public void acceptBlockName()
    {
        //��������� ����� ���
        updateSliderBlock();

        //���� ���� � ���� ��� - �������
        if (pathBlock == null ||
            pathBlock.Length == 0)
        {
            sliderBlockName.SetValueText(textNotHaveBlock);
            return;
        }

        //����������� ��� ����
        string[] path = pathBlock.Split("\\");
        string BlockName = path[path.Length - 1];
        //�������� ��� ��������
        sliderBlockName.SetValueText(BlockName);

        void updateSliderBlock()
        {
            if (pathMod == null || pathMod.Length <= 0)
            {
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
    }

    void updateButtonLoad() {
    
    }

    //���������������� ������� ��������� ���������� �����
    void iniChangeBlock() {
        changeBlock += acceptModName;
        changeBlock += acceptBlockName;

    }
}