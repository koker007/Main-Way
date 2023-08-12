using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class TranslaterComp : MonoBehaviour
{
    float timeLastUpdate = -1;

    [SerializeField]
    TextMeshProUGUI[] texts;
    [SerializeField]
    string key;

    [SerializeField]
    bool defaultTextUse = true;
    string defaultText = "";

    bool isNeedUpdate = true;

    void Start()
    {
        iniText();

        if (Language.Name == null)
            Invoke("TestUpdateInvoke", 0.01f);
        else {
            TestUpdateInvoke();
        }
    }

    //�������� ������ ���� �� ���
    void iniText() {
        //���� ������ ����� ����. �������
        if (texts != null && texts.Length > 0 && texts[0] != null) {
            SetDefauldText();
            return;
        }

        //���� ��� ������ ������� ����
        texts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    void SetDefauldText() {
        if (!defaultTextUse) 
            return;

        defaultText = texts[0].text;
    }

    //��������� ��������� �� ���������� �������� ��� ����� ������
    void TestUpdateInvoke() {
        //��������� ������ ���� ���� ������ ��� ����������
        if (gameObject == null && !isNeedUpdate) 
            return;

        //��������� ����� �� ������ �����
        if (Language.TimeUpdate != timeLastUpdate && Language.Name != null)
            UpdateText();

        //����� �������� ������ � ������� 10 ������
        Invoke("TestUpdateInvoke", Random.Range(5,10));
    }

    public void UpdateText() {
        string textStr = "";
        if (defaultTextUse)
            textStr = Language.GetTextFromKey(key, defaultText);
        else textStr = Language.GetTextFromKey(key);

        SetText(textStr);
    }

    public void SetText(string textStr, bool needUpdate = true) {
        if (needUpdate)
            isNeedUpdate = needUpdate;

        //���� ���� �����-�� �����
        if (textStr != null && textStr != "")
        {
            //�������� ����� ��������� ��� ������
            foreach (TextMeshProUGUI text in texts)
            {
                text.text = textStr;
            }
        }


        //���������� ����� ���������� ���������� ���� �������
        timeLastUpdate = Language.TimeUpdate;
    }
}
