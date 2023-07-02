using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;

//Load text from language
static public partial class Language {
    //Путь до папки с языками
    public const string directive = "Language";
    const string mainKey = "|=|";

    //текущий язык
    static string name = null;
    static float timeUpdate = 0;
    static public string Name
    {
        get
        {
            return name;
        }
    } //Получить текущий язык
    static public float TimeUpdate {
        get { return timeUpdate; }
    }

    const string EnglishStr = "English";

    static KeyAndText[] KATEnglish; //Базовый текст
    static KeyAndText[] KATSelect; //Основной текст для вывода

    //шрифты
    static Font fontEnglish;
    static Font fontSelect;

    const int MaximumKeyOneSumbol = 250; //Размерность массива для одного символа

    //загрузить указанный язык из файлов
    public static bool LoadLanguage(string language)
    {
        bool complite = false;

        //Если этот язык уже загружен
        if (name == language) {
            //выходим
            return complite = true;
        }

        //Язык требует загрузки
        name = language;
        //Запоминаем время последней смены языка
        timeUpdate = Time.unscaledTime;

        //Заполняем выбранный язык
        //Сперва зачищаем старые данные
        KATSelect = new KeyAndText[char.MaxValue * 2 * MaximumKeyOneSumbol];
        GetLanguage(false, directive + "/" + name);

        //Заполняем запасной язык
        KATEnglish = new KeyAndText[char.MaxValue * 2 * MaximumKeyOneSumbol];
        GetLanguage(true, directive + "/" + EnglishStr);

        return complite;

        //получить текст из файла
        void GetLanguage(bool isEnglish, string folder)
        {

            //создаем путь к файлу
            string path = folder + "/text.txt";

            if (File.Exists(path))
            {
                string encodeText = "Test Тест テスト 測試 تست mitä";
                //получаем текст из файла
                string[] fileText = File.ReadAllLines(path, System.Text.Encoding.GetEncoding(1201));

                //Получили строки файла, теперь разделяем по ключу и заполняем
                foreach (string textFull in fileText)
                {
                    SetText(textFull);
                }

                //Теперь надо получить файл шрифта
                /*
                string pathFont = folder + "/font.ttf";
                if (File.Exists(pathFont))
                {

                    font = (Font)Resources.Load(pathFont);
                }
                */
            }
            else
            {
                Debug.LogError("File " + path + " Not Found");
            }


            void SetText(string textFull) {
                string[] separate = { mainKey };
                string[] textSplite = textFull.Split(separate, System.StringSplitOptions.None);

                //Если текст разделен не на 2 части то это ошибка
                if (textSplite.Length != 2) {
                    return;
                }

                string key = textSplite[0];
                string text = textSplite[1];

                //Текст разделен по ключу убираем пробелы в ключе
                string keyNew = "";
                foreach (char symbol in key) {
                    //Добавляем символ
                    keyNew += symbol;

                    //Если символ не пробел
                    if (symbol != ' ') {
                        //Перезаписываем старый текст - новым
                        key = keyNew;
                    }
                }

                //Теперь ключ без пробелов
                //Создаем связку ключ - значение
                KeyAndText keyAndTextNew = new KeyAndText(key, text);

                //находим стартовый номер в массиве по первому и последнему символу ключа
                int numStart = GetStartNum(keyAndTextNew.key);

                if (isEnglish)
                {
                    for (int num = numStart; num < KATEnglish.Length; num++)
                    {
                        //если ячейка не свободна переключается дальше
                        if (KATEnglish[num] != null)
                        {
                            continue;
                        }

                        //Запоминаем
                        KATEnglish[num] = keyAndTextNew;
                        //Завершаем цикл
                        break;
                    }
                }
                else {
                    for (int num = numStart; num < KATSelect.Length; num++)
                    {
                        //если ячейка не свободна переключается дальше
                        if (KATSelect[num] != null)
                        {
                            continue;
                        }

                        //Запоминаем
                        KATSelect[num] = keyAndTextNew;
                        //Завершаем цикл
                        break;
                    }
                }
            }
        }
    }

    //Получить текст перевода по ключу
    public static string GetTextFromKey(string key, string defoltText) {
        string text = "";

        //получаем номер поиска
        int numStart = GetStartNum(key);

        if (numStart < 0)
            return defoltText;

        //Проверяем в выбранном языке
        int numTry = 0;
        for (int num = numStart; KATSelect != null && num < KATSelect.Length && numTry <= MaximumKeyOneSumbol; num++) {
            numTry++;

            //Если эта ячейка пустая или в ней не совпадают ключи ищем дальше
            if (KATSelect[num] == null || KATSelect[num].key != key) {
                continue;
            }

            //Ключи совпали вытаскиваем текст
            text = KATSelect[num].text;

            //завершаем цикл
            break;
        }

        //Если текст обнаружен передаем
        if (text.Length > 0) {
            return text;
        }

        //ищем дальше в английском языке
        numTry = 0;
        for (int num = numStart; KATEnglish != null && num < KATEnglish.Length && numTry <= MaximumKeyOneSumbol; num++)
        {
            numTry++;

            //Если эта ячейка пустая или в ней не совпадают ключи ищем дальше
            if (KATEnglish[num] == null || KATEnglish[num].key != key)
            {
                continue;
            }

            //Ключи совпали вытаскиваем текст
            text = KATEnglish[num].text;

            //завершаем цикл
            break;
        }

        //Если текст обнаружен передаем
        if (text.Length > 0)
        {
            return text;
        }

        //Иначе оставляем текст по умолчанию
        text = defoltText;

        return text;
    }
    public static string GetTextFromKey(string key) {
        return GetTextFromKey(key, "");
    }
    public static void SetTextFromKey(string key, string textNew) {
        //получаем номер поиска
        int numStart = GetStartNum(key);

        if (numStart < 0)
            return;

        //Проверяем в выбранном языке
        int numTry = 0;
        for (int num = numStart; num < KATSelect.Length && numTry <= MaximumKeyOneSumbol; num++)
        {
            numTry++;

            //Если эта ячейка не пустая и в ней не совпадают ключи ищем дальше
            if (KATSelect[num] != null && KATSelect[num].key != key)
            {
                continue;
            }

            //Теперь ключ без пробелов
            //Создаем связку ключ - значение
            KeyAndText keyAndTextNew = new KeyAndText(key, textNew);
            //Запихиваем
            KATSelect[num] = keyAndTextNew;

            //завершаем цикл
            break;
        }
    }

    //получить стартовый номер поиска указанного ключа
    static int GetStartNum(string key) {
        if (key == null ||
            key.Length == 0)
            return -1;


        int numStart = 0;

        //находим стартовый номер в массиве по первому и последнему символу ключа
        numStart = key[0] + key[key.Length-1];

        return numStart;
    }
}


[Serializable]
class KeyAndText {
    public string key;
    public string text;

    public KeyAndText(string key, string text) {
        this.key = key;
        this.text = text;
    }

    static public KeyAndText LineToKaT(string textLine, string separator)
    {
        string[] separate = { separator };
        string[] textSplite = textLine.Split(separate, System.StringSplitOptions.None);

        //Если текст разделен не на 2 части то это ошибка
        if (textSplite.Length != 2)
        {
            return null;
        }

        string key = textSplite[0];
        string text = textSplite[1];

        //Текст разделен по ключу убираем пробелы в ключе
        string keyNew = "";
        foreach (char symbol in key)
        {
            //Добавляем символ
            keyNew += symbol;

            //Если символ не пробел
            if (symbol != ' ')
            {
                //Перезаписываем старый текст - новым
                key = keyNew;
            }
        }

        //Теперь ключ без пробелов
        //Создаем связку ключ - значение
        KeyAndText keyAndTextNew = new KeyAndText(key, text);

        return keyAndTextNew;
    }
    static public KeyAndText[] GetKATs(string[] dataLines, string separator) {
        List<KeyAndText> keyAndTextsList = new List<KeyAndText>();

        foreach (string data in dataLines) {
            KeyAndText keyAndTextNew = LineToKaT(data, separator);
            if (keyAndTextNew == null)
                continue;

            keyAndTextsList.Add(keyAndTextNew);
        }

        return keyAndTextsList.ToArray();
    }
}
