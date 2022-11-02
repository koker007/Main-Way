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

    //���� �������� ��� ������ �� ������ � ���������� ������� �������
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
        //��������� ��� ������
        CloseAllPanels();

        //�������� ������ ��������� ���������
        GameObject panelSelect = Panels[(int)sliderPanels.slider.value];
        panelSelect.SetActive(true);

        string strVoxel = "Voxel";
        string strPhysics = "Physics";
        string strVisual = "Visual";

        //������ ������ ���� ��� ������
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
        //���������� ������
        foreach (GameObject panel in Panels) {
            //������ ������ ���� ��� ������
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
