using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBlockRightPanelsCTRL : MonoBehaviour
{
    static public RedactorBlockRightPanelsCTRL main;

    [SerializeField]
    SliderCTRL sliderPanels;

    [SerializeField]
    GameObject[] Panels;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniPanels();
    }

    //Надо получить все панели из списка и определить границы сладера
    void iniPanels()
    {
        //Сперва определяемся какое количество панелей следует показывать в зависимости от типа блока
        int maxPanels = 0;

        foreach (GameObject panel in Panels) {
            if (IsCorrectPanelType(panel))
                maxPanels++;
        }

        sliderPanels.slider.minValue = 0;
        sliderPanels.slider.maxValue = maxPanels - 1;
    }

    //Принимает тип панели и в зависимости от типа
    bool IsCorrectPanelType(GameObject panel) {
        bool result = false;

        if (RedactorBlocksCTRL.blockData.type == BlockData.Type.block)
        {
            if (panel.GetComponent<RedactorBlocksVoxel>() != null ||
                panel.GetComponent<RedactorBlocksPhysics>() != null ||
                panel.GetComponent<RedactorBlocksVisual>() != null)
                return true;
        }
        else if (RedactorBlocksCTRL.blockData.type == BlockData.Type.voxels)
        {
            if (panel.GetComponent<RedactorBlocksFormTVoxel>() != null ||
                panel.GetComponent<RedactorBlocksPhysics>() != null)
                return true;
        }
        else if (RedactorBlocksCTRL.blockData.type == BlockData.Type.liquid) {
            if (panel.GetComponent<RedactorBlocksFormTLiquid>() != null ||
                panel.GetComponent<RedactorBlocksFormTLiquidPhysics>() != null)
                return true;
        }

        return result;
    }

    GameObject GetSelectPanel() {
        int SelectNum = 0;
        GameObject panelLastCorrect = null;
        for (int x = 0; x < Panels.Length; x++) {
            if (IsCorrectPanelType(Panels[x])) {
                panelLastCorrect = Panels[x];
                if (sliderPanels.slider.value == SelectNum) {
                    return Panels[x];
                }
                SelectNum++;
            }
        }

        return panelLastCorrect;
    }

    // Update is called once per frame
    void Update()
    {
        iniPanels();
    }

    void CloseAllPanels()
    {
        foreach (GameObject panel in Panels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    public void ClickSliderPanels()
    {
        //Выключаем все панели
        CloseAllPanels();

        //Включаем только выбранную слайдером
        GameObject panelSelect = GetSelectPanel();
        panelSelect.SetActive(true);

        string strVoxel = "Voxel";
        string strPhysics = "Physics";
        string strVisual = "Visual";
        string strFormTVoxel = "Form Voxels";
        string strFormTLiquid = "Liquid color";
        string strFormTLiquidPhysics = "Liquid physics";


        //Узнаем какого рода эта панель
        if (panelSelect.GetComponent<RedactorBlocksVoxel>() != null)
        {
            sliderPanels.SetValueText(strVoxel);
        }
        else if (panelSelect.GetComponent<RedactorBlocksPhysics>() != null)
        {
            sliderPanels.SetValueText(strPhysics);
        }
        else if (panelSelect.GetComponent<RedactorBlocksVisual>() != null)
        {
            sliderPanels.SetValueText(strVisual);
        }
        else if (panelSelect.GetComponent<RedactorBlocksFormTVoxel>() != null)
        {
            sliderPanels.SetValueText(strFormTVoxel);
        }
        else if (panelSelect.GetComponent<RedactorBlocksFormTLiquid>() != null)
        {
            sliderPanels.SetValueText(strFormTLiquid);
        }
        else if (panelSelect.GetComponent<RedactorBlocksFormTLiquidPhysics>() != null) {
            sliderPanels.SetValueText(strFormTLiquidPhysics);
        }

        panelSelect.SetActive(true);
    }

    public void ReDrawingAll() {
        //Перебираем панели
        foreach (GameObject panel in Panels) {
            //Узнаем какого рода эта панель
            if (panel.GetComponent<RedactorBlocksVoxel>() != null)
            {
            }
            else if (panel.GetComponent<RedactorBlocksPhysics>() != null)
            {
                panel.GetComponent<RedactorBlocksPhysics>().ReDrawAll();
            }
            else if (panel.GetComponent<RedactorBlocksVisual>() != null)
            {

            }
        }
    }
}
