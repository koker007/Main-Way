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
    KeyAndText[] KeysValue; //��� ������ �������� ������� �������� �� �����

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

    //�������� ������ ���� �� ���
    void iniText()
    {
        //���� ������ ����� ����. �������
        if (texts != null && texts.Length > 0 && texts[0] != null)
        {
            SetDefauldText();
            return;
        }

        //���� ��� ������ ������� ����
        texts = GetComponentsInChildren<TextMeshProUGUI>();
        defaultText = texts[0].text;
    }

    //���������� ����� �� ��������� ���� �� ������ ������
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

    //��������� ��������� �� ���������� �������� ��� ����� ������
    void TestUpdateInvoke()
    {
        //��������� ������ ���� ���� ������ ��� ����������
        if (gameObject == null)
            return;

        //��������� ����� �� ������ �����
        if (Language.TimeUpdate != timeLastUpdate && Language.Name != null)
            UpdateText();

        //����� �������� ������ � ������� 10 ������
        Invoke("TestUpdateInvoke", Random.Range(5, 10));
    }

    public void UpdateText()
    {
        string textStr = "";
        if (defaultTextUse)
            textStr = Language.GetTextFromKey(key, defaultText);
        else textStr = Language.GetTextFromKey(key);

        //���� ���� �����-�� �����
        if (textStr != null && textStr != "")
        {
            defaultText = textStr;

            UpdateSliderText();
        }

        //��������� ������ ������ ��� ������ ����������� �������� �� ����� ��������
        foreach (KeyAndText KAT in KeysValue) {
            if (KAT == null || KAT.key == "") 
                continue;

            string text = Language.GetTextFromKey(KAT.key);
            if (text != "")
                KAT.text = text;
        }

        //���������� ����� ���������� ���������� ���� �������
        timeLastUpdate = Language.TimeUpdate;
    }


    void UpdateSliderText()
    {
        foreach (TextMeshProUGUI text in texts) {
            text.text = defaultText + ": " + valueText;
        }
    }

    public void SetValueText(string valueText, string valueKey) {
        //��������� ����� ������ �� �����
        foreach (KeyAndText KaT in KeysValue) {
            //���� ����� �� ��������� ���������� �����
            if (KaT == null || KaT.key != valueKey || KaT.text == "") 
                continue;

            //���� ����� ������� � ��� ���� �����
            SetValueText(KaT.text);
            //������� �������� �������
            return;
        }

        //���� ����� ����� �� ��� ������, ������ �������� �� ���������
        SetValueText(valueText);
        
    }

    public void SetValueText(string valueText)
    {
        //���� �� ���� ������������ �������������
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

    //��������� �������� �������� �� ����� � ���� �������� ������������
    public void SaveValue() {
        PlayerPrefs.SetInt(key, (int)slider.value);
    }

    //��������� ������ �� �����
    public void LoadValue() {
        slider.value = PlayerPrefs.GetInt(key, (int)slider.value);
    }
}
