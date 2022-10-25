using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Отвечает за загрузку всех данных в игру
public class GameData : MonoBehaviour
{
    int blockMax = 100000;
    //Количество символов принадлежности к моду
    int charsMod = 3;
    //количество символов блока
    int charsName = 3;
    //символ блока сумарное среднее
    int charsNameSum = 1;

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
        //[MMMNNNn]

        blockMax = char.MaxValue * charsMod * charsName * charsNameSum;

        blockData = new BlockData[blockMax];

        //теперь надо загрузить все блоки
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
        
        }

        bool test = false;
    }
}
