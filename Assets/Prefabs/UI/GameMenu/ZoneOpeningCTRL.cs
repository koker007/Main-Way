using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneOpeningCTRL : MonoBehaviour
{
    [SerializeField]
    SliderCTRL sliderOffOn;
    [SerializeField]
    RectTransform rect;

    float heightOpen = 0;
    float heightClose = 0;

    void Start()
    {
        IniZone();
    }

    void IniZone() {
        rect = gameObject.GetComponent<RectTransform>();

        RectTransform sliderRect = sliderOffOn.GetComponent<RectTransform>();
        heightClose = Mathf.Abs(sliderRect.sizeDelta.y);

        //получаем все рект трансформы детей
        RectTransform[] rectTransforms = gameObject.GetComponentsInChildren<RectTransform>();

        //перебирем детей и ищем того кто ниже всех от нуля
        heightOpen = heightClose;
        foreach (RectTransform rect in rectTransforms) {
            if (rect.parent != this.rect.transform)
                continue;

            float posY = Mathf.Abs(rect.localPosition.y);
            posY += Mathf.Abs(rect.sizeDelta.y);
            if (heightOpen < posY)
                heightOpen = posY;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOpenCloseZone();
    }



    void UpdateOpenCloseZone() {
        bool needOpen = false;
        
        if (sliderOffOn.slider.value != 0)
            needOpen = true;

        //Если надо открыть зону
        if (needOpen)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, heightOpen);
        }
        else {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, heightClose);
        }
    }
}
