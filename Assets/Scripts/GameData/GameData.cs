using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Отвечает за загрузку всех данных в игру
public class GameData : MonoBehaviour
{
    int blockMax = 100000;
    //Количество символов принадлежности к моду
    const int charsMod = 3;
    //количество символов блока
    const int charsName = 3;

    static public string pathMod = "Mods";

    static public string nameBlock = "Blocks"; //Имя папки блоков
    static public string nameRecipes = "Recipes"; //Имя папки рецептов крафтов

    //путь загрузки блоков
    BlockData[] blockData;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("char max " + (int)char.MaxValue);
        iniBlockData();

        AllReload();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void iniBlockData() {
        //Нужно определиться сколько данных хранить
        //M = Mod | N - nameBlock | 
        //[MMMNNN]

        blockMax = char.MaxValue * (charsMod + charsName);

        blockData = new BlockData[blockMax];

        //теперь надо загрузить все блоки
    }

    static public uint GetBlockID(string ModName, string BlockName) {
        //Получаем текстовое сокрашение
        string abbreviatura = "";
        //Первые 3 символа мода
        for (int num = 0; num < charsMod; num++) {
            //Если текущий символ больше чем имя
            if (num >= ModName.Length)
                abbreviatura += "_";
            else abbreviatura += ModName[num];
        }

        //Первые 3 символа имени
        for (int num = 0; num < charsMod; num++)
        {
            //Если текущий символ больше чем имя
            if (num >= BlockName.Length)
                abbreviatura += "_";
            else abbreviatura += BlockName[num];
        }

        //Теперь есть Абревиатура получаем ID
        uint abbreviaturaID = 0;
        //Перебираем каждый символ
        for (int num = 0; num < abbreviatura.Length; num++) {
            abbreviaturaID += abbreviatura[num];
        }

        return abbreviaturaID;
    }

    void AllReload()
    {
        //Проверяем есть ли папка мод
        if (!Directory.Exists(pathMod)) {
            //папка мод должна быть
            Directory.CreateDirectory(pathMod);
        }

        //вытаскиваем все папки модов
        string[] directoriesMod = Directory.GetDirectories(pathMod);
        foreach (string directoryMod in directoriesMod) {
            loadMod(directoryMod);
        }
    }

    void loadMod(string pathMod) {
        //Проверяем список блоков
        string[] directoriesBlock = Directory.GetDirectories(pathMod);
        foreach (string blockDirectory in directoriesBlock) {
            BlockData.LoadDatas(blockDirectory);
        }

        bool test = false;
    }
}
