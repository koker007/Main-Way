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
        sliderPanels.slider.minValue = 0;
        sliderPanels.slider.maxValue = Panels.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {

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
        GameObject panelSelect = Panels[(int)sliderPanels.slider.value];
        panelSelect.SetActive(true);

        string strVoxel = "Voxel";
        string strPhysics = "Physics";
        string strVisual = "Visual";

        //Узнаем какого рода эта панель
        if (panelSelect.GetComponent<RedactorBlocksVoxel>() != null)
        {
            sliderPanels.SetValueText(strVoxel);
        }
        else if (panelSelect.GetComponent<RedactorBlocksPhysics>() != null)
        {
            sliderPanels.SetValueText(strPhysics);
        }
        else if (panelSelect.GetComponent<RedactorBlocksVisual>() != null) {
            sliderPanels.SetValueText(strVisual);
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
