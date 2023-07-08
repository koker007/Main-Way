using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class BiomeData
{
    //Биом проинициализирован или нет
    bool isInicialized = false;
    public bool IsInicialized { get { return isInicialized; } }

    public string name;
    public string mod;

    //Список ID блоков в этом биоме и их правила генерации
    public List<GenRule> genRules = new List<GenRule>();

    //Конструктор
    public BiomeData() {
        //Должно быть хотябы одно правило генерации
        genRules.Add(new GenRule());
    }

    //одно правило генерации блока
    public class GenRule {
        public int blockID = 0;

        //Perlin
        public float scaleAll = 16;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;
        public float freq = 3;
        public int octaves = 1;
    }

    //Набор правил для генерации одного конкретного блока
    public class BiomeRuleData
    {
        bool inicialized = false;
        public bool INICIALIZED { get { return inicialized; } }

        //имя блока к которому применяются правила генерации
        string blockMod;
        string blockName;
        int blockID = -1; //id блока вычисляемый в процессе загрузки игры

        GenRule genRule; //правило генерации этого блока в биоме

        public void ReInicialize() {
            ReInicialize(blockMod, blockName);
        }
        public void ReInicialize(string blockMod, string blockName) {
            //Сброс инициализации
            inicialized = false;

            //Запоминаем имя блока
            this.blockMod = blockMod;
            this.blockName = blockName;

            //Получаем ID блока
            blockID = GameData.Blocks.GetBlockID(this.blockMod, this.blockName);

            //if block not exist - exit
            if (blockID < 0)
                return;

            inicialized = true;
        }
    }

    public static string GetDataPath(string mod, string name)
    {
        //Создаем путь к папке биома
        string path = GameData.GameData.pathMod + "/" + mod + "/" + StrC.biomes + "/" + name;

        return path;
    }
    public string GetDataPath()
    {
        return GetDataPath(mod, name);
    }

    static public void SaveData(BiomeData biomeData) {
        string path = biomeData.GetDataPath();

        //Проверяем есть ли папка
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        SaveMain();
        SaveRules();

        void SaveMain() {
            string pathMain = path + "/" + StrC.main + StrC.formatTXT;

            //Сохраняем тип блока
            //Сохранить надо в текстовый файл
            //создаем список того что надо запомнить
            List<string> dataList = new List<string>();

            string dataOne = "";
            //Запоминаем тип
            dataOne = StrC.type + StrC.SEPARATOR;
            if (biomeData as BiomeTypeSurface != null)
                dataOne += StrC.TSurface;
            else if (biomeData as BiomeTypeUnderground != null)
                dataOne += StrC.TUnderground;
            else if (biomeData as BiomeTypeDwarf != null)
                dataOne += StrC.TDwarf;
            else if (biomeData as BiomeTypeRings != null)
                dataOne += StrC.TRings;

            dataList.Add(dataOne);

            //Если файла нет - создаем
            if (!File.Exists(pathMain))
            {
                FileStream fileStream;
                fileStream = File.Create(pathMain);
                fileStream.Close();
            }
            //Сохраняем в файл
            File.WriteAllLines(pathMain, dataList.ToArray());
        }
        void SaveRules() {
            
        }
    }
    static public BiomeData LoadData(string pathBiome) {

        //проверяем существует ли мамка
        if (!Directory.Exists(pathBiome))
        {
            Debug.Log(pathBiome + " Not exist");
            return null;
        }

        string mod = "";
        string name = "";

        BiomeData biomeData = null;

        LoadMain();

        //Обязательно должен быть уже определен тип биома
        if (biomeData == null)
            return null;

        LoadRules();

        return biomeData;

        void LoadMain() {
            //Вытаскиваем путь
            string[] pathParts1 = pathBiome.Split("/");

            List<string> pathList = new List<string>();
            foreach (string pathCut in pathParts1)
            {
                string[] pathParts2 = pathCut.Split("\\");
                foreach (string part in pathParts2)
                {
                    pathList.Add(part);
                }
            }

            string[] pathMass = pathList.ToArray();


            if (pathMass.Length <= 3)
            {
                pathMass = pathBiome.Split("\\");
            }

            if (pathMass.Length <= 3)
            {
                Debug.LogError(pathBiome + " load name error");
                return;
            }

            mod = pathMass[pathMass.Length - 4];
            name = pathMass[pathMass.Length - 2];

            //Нужно загрузить файл с основными данными
            loadMainTXT();

            void loadMainTXT()
            {
                string pathMainStr = pathBiome + "/" + StrC.main + StrC.formatTXT;

                //проверяем существование файла
                if (!File.Exists(pathMainStr))
                {
                    //Файла нет, ошибка
                    Debug.LogError("File main.txt not exist " + pathMainStr);
                    return;
                }

                //Вытаскиваем данные файла
                string[] datasStr = File.ReadAllLines(pathMainStr);

                //Проверяем все строки на данные
                foreach (string dataStr in datasStr)
                {
                    string[] data = dataStr.Split(StrC.SEPARATOR);

                    if (data.Length > 2)
                    {
                        Debug.LogError("Bad parametr: " + dataStr + " in " + pathMainStr);
                        continue;
                    }

                    GetType(data[0], data[1]);
                }
                //////////////////////////////////////////////////////////////////////////////////////
                ///

                void GetType(string name, string value)
                {
                    if (name == StrC.type)
                    {
                        if (value == StrC.TSurface)
                            biomeData = new BiomeTypeSurface();
                        else if (value == StrC.TUnderground)
                            biomeData = new BiomeTypeUnderground();
                        else if (value == StrC.TDwarf)
                            biomeData = new BiomeTypeDwarf();
                        else if (value == StrC.TDwarf)
                            biomeData = new BiomeTypeRings();
                        else
                            Debug.LogError("Bad parametr of " + name + ": " + value);

                    }
                }
            }
        }
        void LoadRules() {
            
        }
    }
}
