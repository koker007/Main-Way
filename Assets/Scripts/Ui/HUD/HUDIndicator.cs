using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDIndicator : MonoBehaviour
{
    [SerializeField]
    public Type type;

    [SerializeField]
    float value = 100;
    [SerializeField]
    float valueMax = 100;
    [SerializeField]
    float second = 100;

    [SerializeField]
    Position position;

    [SerializeField]
    HUDIndicatorOne PrefabIndicatorOne;

    [SerializeField]
    List<HUDIndicatorOne> Indicators = new List<HUDIndicatorOne>();

    public enum Type {
        Health, //Здоровье
        Oxygen,    //Кислород
        Hunger //Голод
    }

    //Позиция индикатора
    public enum Position {
        Left,
        Right
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicator();
    }

    public void SetValue(int value, int valueMax) {
        this.value = value;
        this.valueMax = valueMax;
    }
    public void SetValueSecond() {
    
    }

    void UpdateIndicator() {

        switch(type){
            case Type.Health:
                UpdateHealth();
                break;

            case Type.Oxygen:
                UpdateOxygen();
                break;

            case Type.Hunger:
                UpdateHunger();
                break;
        }

        //Добавить один индикатор
        void AddIndicatorOne()
        {
            GameObject indicatorOneObj = Instantiate(PrefabIndicatorOne.gameObject, transform);
            HUDIndicatorOne indicatorOne = indicatorOneObj.GetComponent<HUDIndicatorOne>();
            RectTransform rect = indicatorOneObj.GetComponent<RectTransform>();

            //Перемещаем
            int lines = Indicators.Count / 10;
            int posX = 0;
            if (position == Position.Left)
                posX = Indicators.Count % 10;
            else
                posX = 9 - Indicators.Count % 10;

            indicatorOne.SetStartPos(position);

            rect.pivot = new Vector2(-posX, -lines);

            Indicators.Add(indicatorOne);
        }

        void UpdateHealth() {
            //Узнаем сколько надо показать сердец
            int hearthCount = (int)(valueMax / 10);
            if (valueMax % 10 > 0)
                hearthCount++;

            //Добавляем сердце если не хватает
            int needAdd = hearthCount - Indicators.Count;
            for (int num = 0; num < needAdd; num++) {
                AddIndicatorOne();
            }

            //Отображаем здоровье
            //Сколько здоровья есть
            int healthHave = (int)value/10;
            for (int num = 0; num < Indicators.Count; num++) {
                if (num < healthHave)
                {
                    Indicators[num].SetFirstPercent(100);
                }
                else if (num == healthHave)
                {
                    //Узнаем остаток
                    float balance = value % 10;
                    balance *= 10;

                    Indicators[num].SetFirstPercent((int)balance);
                }
                else {
                    Indicators[num].SetFirstPercent(0);
                }
            }
        }
        void UpdateOxygen() {
            //Узнаем сколько надо показать кислорода
            int oxygenCount = (int)(valueMax / 10);
            if (valueMax % 10 > 0)
                oxygenCount++;

            //Добавляем кислород если не хватает
            int needAdd = oxygenCount - Indicators.Count;
            for (int num = 0; num < needAdd; num++)
            {
                AddIndicatorOne();
            }

            //Отображаем кислород
            //Сколько кислорода есть
            int healthHave = (int)value / 10;
            for (int num = 0; num < Indicators.Count; num++)
            {
                if (num < healthHave)
                {
                    Indicators[num].SetFirstPercent(100);
                }
                else if (num == healthHave)
                {
                    //Узнаем остаток
                    float balance = value % 10;
                    balance *= 10;

                    Indicators[num].SetFirstPercent((int)balance);
                }
                else
                {
                    Indicators[num].SetFirstPercent(0);
                }
            }
        }
        void UpdateHunger() {
        
        }

    }
}
