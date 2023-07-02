using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Redactor
{

    public class RedactorBiomeRightPanelsCTRL : MonoBehaviour
    {
        static public RedactorBiomeRightPanelsCTRL main;

        [SerializeField]
        SliderCTRL sliderPanels;

        [SerializeField]
        PanelRightRedactorBiome[] panels;

        [Header("Panels Name")]
        [SerializeField]
        string conditionsPlanet = "Planet Conditions";
        [SerializeField]
        string ruleParameters = "Rule parameters";

        // Start is called before the first frame update
        void Start()
        {
            main = this;
        }

        // Update is called once per frame
        void Update()
        {
            iniPanels();
        }

        void iniPanels()
        {
            //������ ������������ ����� ���������� ������� ������� ���������� � ����������� �� ���� �����
            int maxPanels = 0;

            foreach (PanelRightRedactorBiome panel in panels)
            {
                if (IsCorrectPanelType(panel))
                    maxPanels++;
            }

            sliderPanels.slider.minValue = 0;
            sliderPanels.slider.maxValue = maxPanels - 1;
        }

        public void clickSliderPanels() {
            //��������� ��� ������
            CloseAllPanels();

            //�������� ������ ������ � �������� ��
            PanelRightRedactorBiome panel = GetSelectPanel();
            panel.gameObject.SetActive(true);

            updateTextSliderPanels(panel);
        }

        void CloseAllPanels()
        {
            foreach (PanelRightRedactorBiome panel in panels)
            {
                panel.gameObject.SetActive(false);
            }
        }
        PanelRightRedactorBiome GetSelectPanel() {
            int SelectNum = 0;
            PanelRightRedactorBiome panelLastCorrect = null;
            for (int x = 0; x < panels.Length; x++)
            {
                if (IsCorrectPanelType(panels[x]))
                {
                    panelLastCorrect = panels[x];
                    if (sliderPanels.slider.value == SelectNum)
                    {
                        return panels[x];
                    }
                    SelectNum++;
                }
            }

            return panelLastCorrect;
        }

        public void updateTextSliderPanels(PanelRightRedactorBiome panel) {
            //��������� ����� ������ ������ �������
            if (panel as PanelRightRuleParameters)
                sliderPanels.SetValueText(ruleParameters);
            else if (panel as PanelRightConditionsTPlanet)
                sliderPanels.SetValueText(conditionsPlanet);
        }

        //��������� ��� ������ � � ����������� �� ����
        bool IsCorrectPanelType(PanelRightRedactorBiome panel)
        {
            //���� ������ ������� ���������
            if (panel as PanelRightRuleParameters)
                return true;
            //���� ������ ������� �������
            else if (panel as PanelRightConditionsTPlanet) {
                if (RedactorBiomeCTRL.main.biomeData as BiomeTypeSurface != null || 
                    RedactorBiomeCTRL.main.biomeData as BiomeTypeUnderground != null)
                    return true;
            }


            return false;
        }
    }
}