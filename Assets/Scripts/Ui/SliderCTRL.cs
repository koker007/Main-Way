using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderCTRL : MonoBehaviour
{
    [SerializeField]
    public Slider slider;

    float timeLastUpdate = -1;

    [SerializeField]
    TextMeshProUGUI[] texts;
    [SerializeField]
    string key;

    [SerializeField]
    bool defaultTextUse = true;
    string defaultText = "";
    string valueText = "";

    [SerializeField]
    KeyAndText[] KeysValue; //для замены значения текстом перевода по ключу

    void Start()
    {
        iniText();

        LoadValue();

        if (Language.Name == null)
            Invoke("TestUpdateInvoke", 0.01f);
        else
        {
            TestUpdateInvoke();
        }
    }

    //Добавить тексты если их нет
    void iniText()
    {
        //Если первый текст есть. выходим
        if (texts != null && texts.Length > 0 && texts[0] != null)
        {
            SetDefauldText();
            return;
        }

        //Ищем все тексты которые есть
        texts = GetComponentsInChildren<TextMeshProUGUI>();
        defaultText = texts[0].text;
    }

    //установить текст по умолчанию взяв из самого текста
    void SetDefauldText()
    {
        if (!defaultTextUse)
            return;

        defaultText = texts[0].text;
    }

    public void SetDefaultText(string defaultKey, string defoltText) {

        string text = Language.GetTextFromKey(defaultKey, defoltText);
        if (text == "")
            text = defoltText;

        defaultText = text;
    }
    public void SetDefaultText(string defaultKey) {
        SetDefaultText(defaultKey, "");
    }

    //проверить требуется ли обновление перевода для этого текста
    void TestUpdateInvoke()
    {
        //Выполнять только если этот объект еще существует
        if (gameObject == null)
            return;

        //проверяем нужно ли менять текст
        if (Language.TimeUpdate != timeLastUpdate && Language.Name != null)
            UpdateText();

        //Новая проверка текста в течении 10 секунд
        Invoke("TestUpdateInvoke", Random.Range(5, 10));
    }

    public void UpdateText()
    {
        string textStr = "";
        if (defaultTextUse)
            textStr = Language.GetTextFromKey(key, defaultText);
        else textStr = Language.GetTextFromKey(key);

        //Если есть какой-то текст
        if (textStr != null && textStr != "")
        {
            defaultText = textStr;

            UpdateSliderText();
        }

        //проверяем массив ключей для замены конкретного значения на текст перевода
        foreach (KeyAndText KAT in KeysValue) {
            if (KAT == null || KAT.key == "") 
                continue;

            string text = Language.GetTextFromKey(KAT.key);
            if (text != "")
                KAT.text = text;
        }

        //Запоминаем время последнего обновления этих текстов
        timeLastUpdate = Language.TimeUpdate;
    }


    void UpdateSliderText()
    {
        foreach (TextMeshProUGUI text in texts) {
            text.text = defaultText + ": " + valueText;
        }
    }

    public void SetValueText(string valueText, string valueKey) {
        //Выполняем поиск текста по ключу
        foreach (KeyAndText KaT in KeysValue) {
            //Если ключи не совпадают продолжаем поиск
            if (KaT == null || KaT.key != valueKey || KaT.text == "") 
                continue;

            //Если ключи совпали у нас есть текст
            SetValueText(KaT.text);
            //Перевод выполнен выходим
            return;
        }

        //Если текст ключа не был найден, ставим значение по умолчанию
        SetValueText(valueText);
        
    }

    public void SetValueText(string valueText)
    {
        //Если не была произведенна илициализация
        if (timeLastUpdate < 0) 
            return;

        this.valueText = valueText;
        UpdateSliderText();
    }
    public void SetValueText() {
        if (timeLastUpdate < 0)
            return;

        valueText = ((int)slider.value).ToString();
        UpdateSliderText();
    }

    //Сохраняет значение слайдера по ключу в файл настроек пользователя
    public void SaveValue() {
        PlayerPrefs.SetInt(key, (int)slider.value);
    }

    //Загрузить данные из файла
    public void LoadValue() {
        slider.value = PlayerPrefs.GetInt(key, (int)slider.value);
    }
}
