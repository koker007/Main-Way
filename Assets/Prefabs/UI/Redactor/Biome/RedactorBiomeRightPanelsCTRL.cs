using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBiomeRightPanelsCTRL : MonoBehaviour
{
    static public RedactorBiomeRightPanelsCTRL main;

    [SerializeField]
    SliderCTRL sliderPanels;

    [SerializeField]
    GameObject[] panels;

    [Header("Panels Name")]
    [SerializeField]
    string planetConditions = "Planet Conditions";
    [SerializeField]
    string planetBlockList = "Blocks List";

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClickPanelsNext() {
    
    }
    void ClickPanelsBack() {
        
    }

    //ѕринимает тип панели и в зависимости от типа
    bool IsCorrectPanelType(GameObject panel)
    {
        bool result = false;



        return result;
    }
}
