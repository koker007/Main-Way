using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Отвечает за загрузку всех данных в игру
namespace GameData
{
    public class GameData : MonoBehaviour
    {
        //Количество символов принадлежности к моду
        public const int charsMod = 3;
        //количество символов блока
        public const int charsName = 3;

        static public string pathMod = "Mods";

        static public string nameBlock = "Blocks"; //Имя папки блоков
        static public string nameRecipes = "Recipes"; //Имя папки рецептов крафтов


        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("char max " + (int)char.MaxValue);
            Blocks.Ini();

            AllReload();
        }

        // Update is called once per frame
        void Update()
        {

        }

        static public uint GetBlockID(string ModName, string BlockName)
        {
            //Получаем текстовое сокрашение
            string abbreviatura = "";
            //Первые 3 символа мода
            for (int num = 0; num < charsMod; num++)
            {
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
            for (int num = 0; num < abbreviatura.Length; num++)
            {
                abbreviaturaID += abbreviatura[num];
            }

            return abbreviaturaID;
        }

        void AllReload()
        {
            //Проверяем есть ли папка мод
            if (!Directory.Exists(pathMod))
            {
                //папка мод должна быть
                Directory.CreateDirectory(pathMod);
            }

            //вытаскиваем все папки модов
            string[] directoriesMod = Directory.GetDirectories(pathMod);
            foreach (string directoryMod in directoriesMod)
            {
                loadMod(directoryMod);
            }
        }

        void loadMod(string pathMod)
        {
            //Проверяем список блоков
            string[] directoriesBlock = Directory.GetDirectories(pathMod);
            foreach (string blockDirectory in directoriesBlock)
            {
                BlockData[] datas = BlockData.LoadDatas(blockDirectory);

                foreach (BlockData data in datas) {
                    Blocks.SetData(data);
                }
            }

            bool test = false;
        }
    }


    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// /////////////////////////////////////////////////////      Blocks data      ////////////////////////////////////////////////////////////
    /// /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    static class Blocks {
        const int tryingMAX = 100;
        static int countLoaded = 0;


        static int blockMax = 100000;
        //путь загрузки блоков
        static BlockData[] blockData;

        static public void Ini()
        {
            //Нужно определиться сколько данных хранить
            //M = Mod | N - nameBlock | 
            //[MMMNNN]

            blockMax = char.MaxValue * (GameData.charsMod + GameData.charsName);

            blockData = new BlockData[blockMax];

            //теперь надо загрузить все блоки
        }

        static private int GetBlockFirstID(string ModName, string BlockName)
        {
            //Получаем текстовое сокрашение
            string abbreviatura = "";
            //Первые 3 символа мода
            for (int num = 0; num < GameData.charsMod; num++)
            {
                //Если текущий символ больше чем имя
                if (num >= ModName.Length)
                    abbreviatura += "_";
                else abbreviatura += ModName[num];
            }

            //Первые 3 символа имени
            for (int num = 0; num < GameData.charsName; num++)
            {
                //Если текущий символ больше чем имя
                if (num >= BlockName.Length)
                    abbreviatura += "_";
                else abbreviatura += BlockName[num];
            }

            //Теперь есть Абревиатура получаем ID
            int abbreviaturaID = 0;
            //Перебираем каждый символ
            for (int num = 0; num < abbreviatura.Length; num++)
            {
                abbreviaturaID += abbreviatura[num] + (int)Mathf.Pow(4, num);
            }

            return abbreviaturaID;
        }

        //Получить фактический ID блока, если он загружен
        static public int GetBlockID(string ModName, string BlockName, bool onlyExistData = true) {
            //Получаем первичный id
            int idStart = GetBlockFirstID(ModName, BlockName);

            int idNow = idStart;


            for (int num = 0; num < tryingMAX; num++, idNow++)
            {
                //Проверяем что не вышли за пределы массива
                if (idNow >= blockData.Length)
                    idNow = 0;

                //Проверяем по id что блок есть и у него совпадает мод и имя
                if (blockData[idNow] != null && blockData[idNow].mod == ModName && blockData[idNow].name == BlockName)
                    return idNow;


            }

            if (!onlyExistData) {
                for (int num = 0; num < tryingMAX; num++, idNow++)
                {
                    //Проверяем что не вышли за пределы массива
                    if (idNow >= blockData.Length)
                        idNow = 0;

                    //Проверяем по id что блок есть и у него совпадает мод и имя
                    if (blockData[idNow] == null || blockData[idNow].mod == ModName && blockData[idNow].name == BlockName)
                        return idNow;

                }
            }

            return -1;
        }

        //Получить информацию о блоке по его id
        static public BlockData GetData(int blockID) {
            return blockData[blockID];
        }
        static public BlockData GetData(string ModName, string BlockName) {
            //Получаем ID
            int id = GetBlockID(ModName, BlockName);
            
            if (id < 0)
                return null;

            return blockData[id];
        }

        //Записать данные блока в список
        static public void SetData(BlockData data) {
            if (data == null)
                return;

            int id = GetBlockID(data.mod, data.name, false);

            if (id < 0)
                Debug.LogError("Not can to load block data: " + data.mod + " " + data.name);

            blockData[id] = data;
            countLoaded++;
        }
    }
}