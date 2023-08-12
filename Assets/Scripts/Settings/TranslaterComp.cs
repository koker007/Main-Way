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

    //ƒобавить тексты если их нет
    void iniText() {
        //≈сли первый текст есть. выходим
        if (texts != null && texts.Length > 0 && texts[0] != null) {
            SetDefauldText();
            return;
        }

        //»щем все тексты которые есть
        texts = GetComponentsInChildren<TextMeshProUGUI>();
    }

    void SetDefauldText() {
        if (!defaultTextUse) 
            return;

        defaultText = texts[0].text;
    }

    //проверить требуетс€ ли обновление перевода дл€ этого текста
    void TestUpdateInvoke() {
        //¬ыполн€ть только если этот объект еще существует
        if (gameObject == null && !isNeedUpdate) 
            return;

        //провер€ем нужно ли мен€ть текст
        if (Language.TimeUpdate != timeLastUpdate && Language.Name != null)
            UpdateText();

        //Ќова€ проверка текста в течении 10 секунд
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

        //≈сли есть какой-то текст
        if (textStr != null && textStr != "")
        {
            //«амен€ем новым переводом все тексты
            foreach (TextMeshProUGUI text in texts)
            {
                text.text = textStr;
            }
        }


        //«апоминаем врем€ последнего обновлени€ этих текстов
        timeLastUpdate = Language.TimeUpdate;
    }
}
