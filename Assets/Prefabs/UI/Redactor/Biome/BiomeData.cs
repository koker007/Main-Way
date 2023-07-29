using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cosmos;

public class BiomeData
{
    //Биом проинициализирован или нет
    bool isInicialized = false;
    public bool IsInicialized { get { return isInicialized; } }

    public string name;
    public string mod;

    /// <summary>
    /// Основной цвет биома
    /// </summary>
    public string color;

    //Коофиценты изменения приоритета биомов в зависимости от расположения на карте планеты
    public float coofPolus; //Коофицент полюсов
    public float coofZeroX; //Коофицент нулевой координаты экватора, например для планет с приливным захватом
    public float coofHeight; //Коофицент высоты
    public float coofHeightMax; //Коофицент максимальной высоты биома
    public float coofHeightMin; //Коофицент минимальной высоты биома
    public SeaPriority seaPriority; //Биом подводный, надводный

    public enum SeaPriority {
        everywhere = 0,
        onlyUnderSea = 1,
        onlyOverSea = 2
    }



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
            //Создаем путь к папке блоков
            string pathRules = path + "/" + StrC.rules;

            //Если директория есть, необходимо ее удалить создать заного
            if (!Directory.Exists(pathRules))
            {
                Directory.CreateDirectory(pathRules);
            }

            //Получаем все файлы и удаляем их
            string[] files = Directory.GetFiles(pathRules);
            foreach (string file in files)
                File.Delete(file);


            //Обрабатываем все правила
            for (int num = 0; num < biomeData.genRules.Count; num++) {
                SetRule(biomeData.genRules[num], num);
            }

            void SetRule(GenRule genRule, int num) {
                //создаем путь к правилу
                string pathRule = pathRules + "/" + num + StrC.formatTXT;
                if (File.Exists(pathRule))
                    File.Delete(pathRule);

                //Получаем имя блока
                BlockData blockData = GameData.Blocks.GetData(genRule.blockID, 0);
                if (blockData == null)
                {
                    //Если блока нет значит пустота
                    blockData = new TypeBlock();
                    blockData.mod = "";
                    blockData.name = "";
                }

                //Сохранить надо в текстовый файл
                //создаем список того что надо запомнить
                List<string> dataList = new List<string>();

                string data = "";
                //Запоминаем мод и имя блока
                data = StrC.blocks + StrC.mod + StrC.SEPARATOR + blockData.mod;
                dataList.Add(data);
                data = StrC.blocks + StrC.name + StrC.SEPARATOR + blockData.name;
                dataList.Add(data);

                data = StrC.perlin + StrC.scale + StrC.SEPARATOR + genRule.scaleAll;
                dataList.Add(data);
                data = StrC.perlin + StrC.octaves + StrC.SEPARATOR + genRule.octaves;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.x + StrC.SEPARATOR + genRule.scaleX;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.y + StrC.SEPARATOR + genRule.scaleY;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.z + StrC.SEPARATOR + genRule.scaleZ;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.frequency + StrC.SEPARATOR + genRule.freq;
                dataList.Add(data);

                //Сохраняем в файл
                File.WriteAllLines(pathRule, dataList.ToArray());
            }
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

        biomeData.mod = mod;
        biomeData.name = name;

        //Если хоть что-то не загрузилось, то отмена
        if (!LoadRules())
            return null;

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
            name = pathMass[pathMass.Length - 1];

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
        bool LoadRules() {
            //Создаем путь к папке блоков
            string pathRule = pathBiome + "/" + StrC.rules;

            if (!Directory.Exists(pathRule))
                return false;

            //получили все папки с правилами
            string[] pathRulesAll = Directory.GetFiles(pathRule);

            //Обрабатываем все
            List<GenRule> ruleDatas = new List<GenRule>();
            foreach (string path in pathRulesAll) {
                GetRule(path);
            }

            biomeData.genRules = ruleDatas;

            return true;

            void GetRule(string path) {
                //Вытаскиваем номер правила
                string[] spliters = new string[3];
                spliters[0] = "/";
                spliters[1] = "\\";
                spliters[2] = StrC.formatTXT;

                string[] pathArray = path.Split(spliters, System.StringSplitOptions.RemoveEmptyEntries);

                string[] DatasStr = File.ReadAllLines(path);
                GenRule genRule = new GenRule();

                string mod = "";
                string name = "";

                foreach (string DataStr in DatasStr) {
                    SetData(DataStr);
                }

                genRule.blockID = GameData.Blocks.GetBlockID(mod, name);

                //Правило создано, добавляем
                ruleDatas.Add(genRule);

                void SetData(string DataStr) {
                    string[] data = DataStr.Split(StrC.SEPARATOR);

                    if (data.Length != 2) {
                        Debug.LogError("Bad biome data:" + path);
                        return;
                    }

                    switch (data[0]) {
                        case StrC.blocks + StrC.mod:
                            mod = data[1];
                            break;
                        case StrC.blocks + StrC.name:
                            name = data[1];
                            break;
                        case StrC.perlin + StrC.scale:
                            genRule.scaleAll = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.octaves:
                            genRule.octaves = System.Convert.ToInt32(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.x:
                            genRule.scaleX = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.y:
                            genRule.scaleY = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.z:
                            genRule.scaleZ = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.frequency:
                            genRule.freq = (float)System.Convert.ToDouble(data[1]);
                            break;
                    }
                }
            }
        }
    }
}

public class BiomeMaps {
    public const float factor = 0.875170906246f;

    public float[,,] maps;

    //какой размер одного пикселя в блоках
    int sizePixel = 65536;

    //Сгенерировать участок карты размером 32 на 32
    public BiomeMaps(ObjData data, Size sizeTexture, Vector2Int partPos, BiomeData[] biomeData, HeightMap heightMap)
    {
        //Создаем текстуру

        //Находим размер всей поверхности планеты
        int height = Calc.GetSizeInt(data.size) / Calc.GetSizeInt(sizeTexture);
        int width = height * 2;

        //получаем размер планеты
        int sizePlanet = Calc.GetSizeInt(data.size);

        //Узнаем сколько блоков в одном текселе
        this.sizePixel = Calc.GetSizeInt(sizeTexture);

        float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), Mathf.Pow(data.GetPerlinFromIndex(105), data.GetPerlinFromIndex(373))) * 1000;
        float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), Mathf.Pow(data.GetPerlinFromIndex(281), data.GetPerlinFromIndex(255))) * 1000;
        float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), Mathf.Pow(data.GetPerlinFromIndex(110), data.GetPerlinFromIndex(304))) * 1000;

        float sizeContinent = 0.75f + (float)((data.GetPerlinFromIndex(159) * 1000) % 0.5f);

        float scale = (sizePlanet * sizeContinent) / sizePixel * 0.8f;

        maps = GenPart(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 3, false, false, partPos.x, partPos.y, biomeData, heightMap);
    }

    float[,,] GenMap(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ, BiomeData[] biomeData, HeightMap heightMap)
    {
        //Создаем новый массив
        float[,,] arrayMap = new float[mapSizeX, mapSizeY, biomeData.Length];

        //Ищем фактор чанка
        float FactorChankX = (factor / ScaleX) * Chank.Size;
        float FactorChankY = (factor / ScaleY) * Chank.Size;
        float FactorChankZ = (factor / ScaleZ) * Chank.Size;

        //Определяем количество чанков и запоминаем остаток если он есть
        int chankXMax = mapSizeX / Chank.Size;
        int chankXremain = mapSizeX % Chank.Size;
        if (chankXremain > 0)
            chankXMax++;

        int chankYMax = mapSizeY / Chank.Size;
        int chankYremain = mapSizeY % Chank.Size;
        if (chankYremain > 0)
            chankYMax++;

        //Генерируем каждый чанк
        for (int chankX = 0; chankX < chankXMax; chankX++)
        {
            int chankPixelStartX = chankX * Chank.Size;

            for (int chankY = 0; chankY < chankYMax; chankY++)
            {
                int chankPixelStartY = chankY * Chank.Size;

                float offSetX = OffSetX + FactorChankX * chankX;
                float offSetY = OffSetY + FactorChankY * chankY;
                float offSetZ = OffSetZ;

                if (TimeZ)
                    offSetZ += Time.time * 0.1f;

                if (TimeX)
                    offSetX += Time.time * 0.1f;

                float[,,] partMap = GenPart(mapSizeX, mapSizeY, ScaleX, ScaleY, ScaleZ, Freq, offSetX, OffSetY, OffSetZ, Octaves, TimeX, TimeZ, chankX, chankY, biomeData, heightMap);

                //Если крайний чанк с остатком
                int maxX = 32;
                int maxY = 32;
                if (chankX == chankXMax - 1 && chankXremain > 0)
                    maxX = chankXremain;
                if (chankY == chankYMax - 1 && chankYremain > 0)
                    maxY = chankYremain;

                //Запихиваем данные в текстуру
                for (int x = 0; x < maxX; x++)
                {
                    int posMapX = chankPixelStartX + x;
                    for (int y = 0; y < maxY; y++)
                    {
                        int posMapY = chankPixelStartY + y;

                        for (int z = 0; z < partMap.GetLength(2); z++) {
                            arrayMap[posMapX, posMapY, z] = partMap[x, y, z];
                        }
                    }
                }
            }
        }
        return arrayMap;
    }
    float[,,] GenPart(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ, int chankX, int chankY, BiomeData[] biomeDatas, HeightMap heightMap)
    {
        //Определяем количество чанков
        int chankXMax = mapSizeX / Chank.Size;
        int chankXremain = mapSizeX % Chank.Size;
        if (chankXremain > 0)
            chankXMax++;

        int chankYMax = mapSizeY / Chank.Size;
        int chankYremain = mapSizeY % Chank.Size;
        if (chankYremain > 0)
            chankYMax++;

        //Если требуемый чанк не правильный - исключение
        if (chankX < 0 ||
            chankX >= chankXMax ||
            chankY < 0 ||
            chankY >= chankYMax)
        {
            throw new System.ArgumentOutOfRangeException();
        }

        //Создаем новый массив
        //float[,,] arrayPart = new float[Chank.Size, Chank.Size];

        //Ищем фактор чанка
        float FactorChankX = (factor / ScaleX) * Chank.Size;
        float FactorChankY = (factor / ScaleY) * Chank.Size;
        float FactorChankZ = (factor / ScaleZ) * Chank.Size;

        float offSetX = OffSetX * biomeDatas.Length + FactorChankX * chankX;
        float offSetY = OffSetY * biomeDatas.Length + FactorChankY * chankY;
        float offSetZ = OffSetZ;

        if (TimeZ)
            offSetZ += Time.time * 0.1f;

        if (TimeX)
            offSetX += Time.time * 0.1f;

        float regionX = (chankX * Chank.Size) / (float)mapSizeX;
        float regionY = (chankY * Chank.Size) / (float)mapSizeY;

        //Вычисляем перлин на шейдере
        GraficData.Perlin2DArray dataPerlin2DArray = new GraficData.Perlin2DArray(ScaleX, ScaleY, ScaleZ, Freq, offSetX, offSetY, offSetZ, Octaves, mapSizeX, mapSizeY, regionX, regionY, biomeDatas.Length);
        dataPerlin2DArray.Calculate();

        //Смещение биомов
        CalcCoofBiome();

        //Запихиваем копируем данные в массив
        return dataPerlin2DArray.result;

        
        //Вычислить смещение с учетом требований биомов
        void CalcCoofBiome() {
            //Проходимся по каждому пикселю
            for (int x = 0; x < dataPerlin2DArray.result.GetLength(0); x++) {
                float regionPixX = regionX + (x / (float)mapSizeX);
                for (int y = 0; y < dataPerlin2DArray.result.GetLength(1); y++) {
                    float regionPixY = regionY + (y / (float)mapSizeY);
                    for (int bioNum = 0; bioNum < dataPerlin2DArray.result.GetLength(2); bioNum++) {
                        //Считаем смещение по полюсам
                        //На полюсах надо увеличить на экваторе уменьшить
                        float coofPolus = 0;

                        //Поправка на высоту
                        float coofHeight = 0;
                        if (biomeDatas[bioNum].seaPriority == BiomeData.SeaPriority.everywhere)
                            coofHeight = (heightMap.map[x, y] - 0.5f) * 2;
                        else if (biomeDatas[bioNum].seaPriority == BiomeData.SeaPriority.onlyOverSea)
                            coofHeight = (heightMap.map[x, y] - 0.5f) * 2;
                        else if (biomeDatas[bioNum].seaPriority == BiomeData.SeaPriority.onlyUnderSea)
                            coofHeight = (heightMap.map[x, y] - 0.5f) * -2;

                        coofHeight *= biomeDatas[bioNum].coofHeight;


                        if (biomeDatas[bioNum].seaPriority == BiomeData.SeaPriority.onlyOverSea && heightMap.map[x, y] < 0.5f ||
                            biomeDatas[bioNum].seaPriority == BiomeData.SeaPriority.onlyUnderSea && heightMap.map[x, y] > 0.5f)
                            coofHeight = 0;
                        

                        //выше экватора
                        if (regionPixY > 0.5f) 
                            coofPolus = ((regionPixY - 0.5f) * 4 - 1) * biomeDatas[bioNum].coofPolus;
                        //ниже экватора
                        else if (regionPixY <= 0.5f) {
                            coofPolus = ((0.5f - regionPixY) * 4 - 1) * biomeDatas[bioNum].coofPolus;
                        }

                        //Считаем смещение по экватору
                        float coofX = 0;
                        if (regionPixX > 0.5f)
                            coofX = ((regionPixX - 0.5f) * 4 - 1) * biomeDatas[bioNum].coofZeroX;
                        else if (regionPixX <= 0.5f)
                        {
                            coofX = ((0.5f - regionPixX) * 4 - 1) * biomeDatas[bioNum].coofZeroX;
                        }

                        //Умножаем на коофицент изменения
                        float coofSum = coofPolus + coofX + coofHeight;

                        dataPerlin2DArray.result[x, y, bioNum] += coofSum;
                    }
                }
            }
        }
    }
}
