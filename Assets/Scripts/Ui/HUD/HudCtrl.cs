using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudCtrl : MonoBehaviour
{
    [SerializeField]
    GameObject migleCenter;

    [SerializeField]
    HUDMain HUDMain;
    [SerializeField]
    HUDInfoF3 HUDInfo;

    [SerializeField]
    List<HUDIndicator> IndicatorsLeft;
    [SerializeField]
    List<HUDIndicator> IndicatorsRight;

    [SerializeField]
    Transform ParentIndicatorsLeft;
    [SerializeField]
    Transform ParentIndocatorsRight;


    [Header("Prefabs")]
    [SerializeField]
    HUDIndicator PrefabHUD_Health;
    [SerializeField]
    HUDIndicator PrefabHUD_Oxygen;
    [SerializeField]
    HUDIndicator PrefabHUD_Hunger;

    // Start is called before the first frame update
    void Start()
    {
        Inicialize();
    }

    void Inicialize() {
        HUDMain.inicialize();
        HUDInfo.Inicialize();
    }

    // Update is called once per frame
    void Update()
    {
        TestKeys();

        TestIndicatorsLeft();
        TestIndicatorsRight();

        TestHide();
    }

    void TestHide() {
        if (PlayerCTRL.local == null)
        {
            if (migleCenter.gameObject.activeSelf)
                migleCenter.gameObject.SetActive(false);
        }
        else {
            if (!migleCenter.gameObject.activeSelf)
                migleCenter.gameObject.SetActive(true);
        }
    }
    
    void TestIndicatorsLeft()
    {
        //Если игрока нет, удаляем все индикаторы
        if (!PlayerCTRL.local)
        {
            if (IndicatorsLeft.Count > 0)
            {
                foreach (HUDIndicator indicator in IndicatorsLeft)
                {
                    Destroy(indicator.gameObject);
                }
            }


            return;
        }

        HUDIndicator HudHealth = null;

        List<HUDIndicator> indicatorsLeftNew = new List<HUDIndicator>();
        //Ищем индикаторы
        foreach (HUDIndicator indicator in IndicatorsLeft) {
            if (indicator == null)
                continue;

            //Если это здоровье
            if (indicator.type == HUDIndicator.Type.Health)
                HudHealth = indicator;

            //Добавляем элемент
            indicatorsLeftNew.Add(indicator);
        }
        //Запоминаем новый список
        IndicatorsLeft = indicatorsLeftNew;

        //Если здоровье есть
        if (PlayerCTRL.local.health > 0)
        {
            //Создаем здоровье если нет
            if (HudHealth == null) {
                GameObject HudHealthObj = Instantiate(PrefabHUD_Health.gameObject, ParentIndicatorsLeft);
                RectTransform rect = HudHealthObj.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f,0);
                
                HudHealth = HudHealthObj.GetComponent<HUDIndicator>();
                IndicatorsLeft.Add(HudHealth);


            }

            //Назначаем индикутору здоровья значение
            HudHealth.SetValue(PlayerCTRL.local.health, PlayerCTRL.local.healthMax);
        }
        else if(HudHealth) {
            //Удаляем индикатор если он не нужен
            Destroy(HudHealth.gameObject);
        }
    }
    void TestIndicatorsRight() {
        //Если игрока нет, удаляем все индикаторы
        if (!PlayerCTRL.local)
        {
            if (IndicatorsRight.Count > 0)
            {
                foreach (HUDIndicator indicator in IndicatorsRight)
                {
                    Destroy(indicator.gameObject);
                }
            }


            return;
        }

        HUDIndicator HudOxygen = null;

        List<HUDIndicator> indicatorsRightNew = new List<HUDIndicator>();
        //Ищем индикаторы
        foreach (HUDIndicator indicator in IndicatorsRight)
        {
            if (indicator == null)
                continue;

            //Если это здоровье
            if (indicator.type == HUDIndicator.Type.Oxygen)
                HudOxygen = indicator;

            //Добавляем элемент
            indicatorsRightNew.Add(indicator);
        }
        //Запоминаем новый список
        IndicatorsRight = indicatorsRightNew;

        //Если кислолод у игрока не полный
        if (PlayerCTRL.local.oxygen < 100)
        {
            //Создаем индикатор если нет
            if (HudOxygen == null)
            {
                GameObject HudOxygenObj = Instantiate(PrefabHUD_Oxygen.gameObject, ParentIndocatorsRight);
                RectTransform rect = HudOxygenObj.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0.5f, 0);

                HudOxygen = HudOxygenObj.GetComponent<HUDIndicator>();
                IndicatorsRight.Add(HudOxygen);


            }

            //Назначаем индикутору значение
            HudOxygen.SetValue(PlayerCTRL.local.oxygen, 100);
        }
        else if (HudOxygen)
        {
            //Удаляем индикатор если он не нужен
            Destroy(HudOxygen.gameObject);
        }
    }

    void TestKeys() {
        //Отключить или включить Основной HUD
        if (Input.GetKeyDown(KeyCode.F1))
            HUDMain.reActive();

        //Отключить или включить Информационную панель
        if (Input.GetKeyDown(KeyCode.F3))
            HUDInfoF3.reActiveF3();
    }

}
