using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Game.Space;

//Отвечает за загрузку всех данных в игру
namespace Game
{
    public class GameData : MonoBehaviour
    {
        static public GameData main;
        //Количество символов принадлежности к моду
        public const int charsMod = 3;
        //количество символов блока
        public const int charsName = 3;

        static public string pathMod = "Mods";

        static public string nameBlock = "Blocks"; //Имя папки блоков
        static public string nameRecipes = "Recipes"; //Имя папки рецептов крафтов

        [SerializeField]
        public ChankGO prefabChankGO;
        [SerializeField]
        public PlanetGO prefabPlanetGO;

        // Start is called before the first frame update
        void Start()
        {
            main = this;
            Blocks.Ini();

            AllReload();
        }

        // Update is called once per frame
        void Update()
        {
            TestGarbageCollector();
        }
        /*
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
        */
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
            //Загружаем блоки
            loadModBlocks(pathMod);

            //Загружаем биомы

        }

        void loadModBlocks(string pathMod) {

            //Проверяем список содержимого мода
            string pathModBlocks = pathMod + "\\" + StrC.blocks;

            //Проверяем что папка блоков существует
            if (!Directory.Exists(pathModBlocks))
                return;

            //Получаем список блоков
            string[] pathsBlockdata = Directory.GetDirectories(pathModBlocks);

            //Загружаем все блоки
            foreach (string pathBlockData in pathsBlockdata) {
                BlockData[] BlockVariants = BlockData.LoadDatas(pathBlockData);

                Blocks.SetData(BlockVariants);
            }
        }


        void TestGarbageCollector() {
            if (!GarbageCollector.isTimeToClear())
                return;

            GarbageCollector.GC.Execute();
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
        static BlockData[][] blockData;

        static public void Ini()
        {
            //Нужно определиться сколько данных хранить
            //M = Mod | N - nameBlock | 
            //[MMMNNN]

            blockMax = char.MaxValue * (GameData.charsMod + GameData.charsName);

            blockData = new BlockData[blockMax][];

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
                abbreviaturaID += abbreviatura[num] * (int)Mathf.Pow(5, num);
            }

            while (abbreviaturaID >= blockData.Length) {
                abbreviaturaID -= blockData.Length;
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
                    idNow -= blockData.Length;

                //Проверяем по id что блок есть и у него совпадает мод и имя
                if (blockData[idNow] != null && blockData[idNow][0].mod == ModName && blockData[idNow][0].name == BlockName)
                    return idNow;


            }

            if (!onlyExistData) {
                //возвращаем к старту
                idNow = idStart;
                for (int num = 0; num < tryingMAX; num++, idNow++)
                {
                    //Проверяем что не вышли за пределы массива
                    if (idNow >= blockData.Length)
                        idNow = 0;

                    //Проверяем по id что блок есть и у него совпадает мод и имя
                    if (blockData[idNow] == null || blockData[idNow][0].mod == ModName && blockData[idNow][0].name == BlockName)
                        return idNow;

                }
            }

            return -1;
        }

        static public BlockData GetData(string ModName, string BlockName, uint variant)
        {
            //Получаем ID
            int id = GetBlockID(ModName, BlockName);

            return GetData(id, variant);
        }
        //Получить информацию о блоке по его id
        static public BlockData GetData(int blockID, uint variant) {
            BlockData[] blockVariants = GetDatas(blockID);

            //Если блока не существует, выходим
            if (blockVariants == null)
                return null;

            //если вариант не существует
            if (variant >= blockVariants.Length)
                variant = 0;

            return blockVariants[variant];
        }
        static public BlockData[] GetDatas(int blockID)
        {
            return blockData[blockID];
        }
        static public BlockData[] GetDatas(string ModName, string BlockName) {
            //Получаем ID
            int id = GetBlockID(ModName, BlockName);
            
            if (id < 0)
                return null;

            return blockData[id];
        }

        //Записать данные блока в список
        static public void SetData(BlockData[] data) {
            if (data == null)
                return;

            int id = GetBlockID(data[0].mod, data[0].name, false);

            if (id < 0)
                Debug.LogError("Not can to load block data: " + data[0].mod + " " + data[0].name);

            blockData[id] = data;
            countLoaded++;
        }

        static public Color GetColor(int blockID) {
            Color result = new Color(1,1,1,0);

            BlockData block = GetData(blockID, 0);

            //Если меньше 10 то это тестовые блоки
            if(blockID < 10)
               return result;

            return block.GetColor();
        }
    }
}