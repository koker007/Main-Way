using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Класс галлактики
public class Galaxy {
    public readonly string Seed;

    public CellS[,,] cells;

    GraficData.Perlin[,,] Perlins;

    Size size;

    public Galaxy(Size size, string seedFunc) {
        Debug.Log("Create New galaxy: " + seedFunc);

        //Сперва удаляем старую галактику
        GalaxyCtrl.main.GalaxyClear();

        //Запоминаем новую
        GalaxyCtrl.galaxy = this;

        Seed = seedFunc;

        //Создали массив ячеек галактики
        cells = new CellS[(int)size,(int)size/4, (int)size];

        int perlinMaxX = cells.GetLength(0) / 8;
        int perlinMaxY = cells.GetLength(1) / 8;
        int perlinMaxZ = cells.GetLength(2) / 8;

        if (cells.GetLength(0) % 8 > 0)
            perlinMaxX++;
        if (cells.GetLength(1) % 8 > 0)
            perlinMaxY++;
        if (cells.GetLength(2) % 8 > 0)
            perlinMaxZ++;

        //Получаем шум по размеру галактики
        Perlins = new GraficData.Perlin[perlinMaxX, perlinMaxY, perlinMaxZ];

        float OffSetX = Calc.GetSeedNum(Seed+ Seed[0], Seed[0] * Seed[1]);
        float OffSetY = Calc.GetSeedNum(Seed+ Seed[1], Seed[1] * Seed[2]);
        float OffSetZ = Calc.GetSeedNum(Seed+ Seed[2], Seed[2] * Seed[0]);

        for (int x = 0; x < perlinMaxX; x++) {
            for (int y = 0; y < perlinMaxY; y++) {
                for (int z = 0; z < perlinMaxZ; z++) {
                    Perlins[x, y, z] = new GraficData.Perlin(16f, 1, x - OffSetX, y + OffSetY, z + OffSetZ, 5, true);
                    Perlins[x, y, z].Calculate();
                }
            }
        }
        //Получили шум


        //нужно заполнить данными каждую ячейку
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int y = 0; y < cells.GetLength(1); y++) {
                for (int z = 0; z < cells.GetLength(2); z++) {
                    GenerateCell(new Vector3Int(x,y,z));
                }
            }
        }

        
    }

    public CellS GenerateCell(Vector3Int pos) {
        int Px = pos.x / 8;
        int Py = pos.y / 8;
        int Pz = pos.z / 8;

        int Lx = pos.x % 8;
        int Ly = pos.y % 8;
        int Lz = pos.z % 8;

        //Получаем шум
        float noise = Perlins[Px, Py, Pz].result[Lx, Ly, Lz];

        //Создаем ячейку
        cells[pos.x, pos.y, pos.z] = new CellS(pos, noise, this);
        return cells[pos.x, pos.y, pos.z];
    }


    public enum Size {
        cells15 = 15,
        cells31 = 31,
        cells61 = 61
    }
}









//Класс космической ячейки
public class CellS
{
    //Размер одной ячейки постоянный
    public const int size = 1000000;
    public const int sizeZone = 1000; //Игровые зоны ячеек
    public const int sizeVisual = 1000;

    public Galaxy galaxy;
    public Vector3 pos; //Позиция ячейки (целое) и расположение центрального объекта (остаток)

    public SpaceObjData mainObjs; //Базовый объект во круг которого существует звездная система
    public GalaxyObjCtrl visual; //Визуальная часть базового объекта

    float perlinGlobal; //Глобальный перлин благодаря которому сгенерируется локальный
    public float perlinGlob {
        get { return perlinGlobal; }
    }

    //Конструктор ячейки
    public CellS(Vector3 pos, float perlin, Galaxy galaxy) {
        this.galaxy = galaxy;
        this.pos = pos;



        //Запоминаем глобальный перлин этой ячейки
        perlinGlobal = perlin;

        genMainObj();

    }

    //Сгенерировать основу ячейки
    public void genMainObj() {
        if (mainObjs != null) 
            return;

        //Нужно ли прекратить генерацию
        if (!isGenFormGalaxy()) 
            return;

        //тут надо выбирать тип звезды и относительно типа звезды установить ее светимость и размер
        mainObjs = new SpaceObjData(this);
        mainObjs.GenObjData(null, perlinGlobal,0);

        //Когда все данные основного объекта определенны
        //Определяем позицию внутри ячейки
        pos = GetPos();

        bool isGenFormGalaxy() {
            float shance = perlinGlobal;
            int centerX = galaxy.cells.GetLength(0) / 2;
            int centerY = galaxy.cells.GetLength(1) / 2;
            int centerZ = galaxy.cells.GetLength(2) / 2;

            //Смещение ячейки от центра
            Vector3 offset = new Vector3(pos.x - centerX, pos.y - centerY, pos.z - centerZ);

            float distDisc = new Vector3(offset.x, offset.y * 8, offset.z).magnitude;
            float distCentr = new Vector3(offset.x * 2, offset.y * 4, offset.z * 2).magnitude;

            float coofDisс = distDisc / centerX;
            float coofCentr = distCentr / centerX;

            if (distCentr < centerX)
                shance += (1 - coofCentr);
            else if (coofDisс < 1)
                shance += 0.05f;
            else if (coofDisс > 1)
                shance += (1 - coofDisс) * 0.3f;

            if (shance > 0.7f)
                return true;

            return false;
        }
        bool isGenPerlin() {
            Debug.Log(pos + " P " + perlinGlobal );

            if (perlinGlobal > 0.7f)
                return true;

            return false;
        }


        Vector3 GetPos() {
            Vector3 result = pos;
            //Вычисляем позицию на основе сида

            float num = (int)pos.x * galaxy.cells.GetLength(1) * galaxy.cells.GetLength(2) + (int)pos.y * galaxy.cells.GetLength(2) + (int)pos.z;

            float numX = (perlinGlobal * 10 % 1.3f) * (perlinGlobal * 100 % 1.174f);
            float numY = (perlinGlobal * 100 % 1.53f) * (perlinGlobal * 1000 % 1.472f);
            float numZ = (perlinGlobal * 1000 % 1.124f) * (perlinGlobal * 10000 % 1.854f);

            float randX = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numX)) % 0.5f;
            float randY = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numY)) % 0.5f;
            float randZ = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numZ)) % 0.5f;

            result += new Vector3(0.5f, 0.5f, 0.5f); //Расположение по центру ячейки

            //рандомизация
            result.x += randX;
            result.y += randY;
            result.z += randZ;

            //Debug.Log(rand);

            return result;
        }
    }

    //Сгенерировать планеты в ячейке
    public void genPlanets() {

        //Если звездная система без центрального объекта
        if (mainObjs == null)
            return;

        //Если дети уже есть, генерация не требуется
        if (mainObjs.childs != null)
            return;

        //Узнаем доступное растояние для генерации планет
        float distGenMax = GetDistToGen();

        //приводим размер звездных ячеек в блоковый
        distGenMax *= CellS.size; 

        //Запускаем генерацию планет относительно главной звезды
        mainObjs.GenPlanets(distGenMax, perlinGlobal);
        
        Debug.Log("Cell Gen: " + pos + " Planets:" + mainObjs.childs.Length);

        float GetDistToGen()
        {
            //Может вызываться не только сервером но если планеты существуют у клиента то они синхронизируются с сервером

            //Узнаем растояние от звезды до границы звездной ячейки
            Vector3 dist = new Vector3();
            dist.x = pos.x % 1;
            dist.y = pos.y % 1;
            dist.z = pos.z % 1;

            dist -= new Vector3(0.5f, 0.5f, 0.5f);

            //Получаем смещение от центра по модулю
            float distPlanetMax = 1;
            dist.x = Mathf.Abs(dist.x);
            dist.y = Mathf.Abs(dist.y);
            dist.z = Mathf.Abs(dist.z);

            //Получаем растояние до границы ячейки
            dist.x = 0.5f - dist.x;
            dist.y = 0.5f - dist.y;
            dist.z = 0.5f - dist.z;

            if (distPlanetMax > dist.x)
                distPlanetMax = dist.x;
            if (distPlanetMax > dist.y)
                distPlanetMax = dist.y;
            if (distPlanetMax > dist.z)
                distPlanetMax = dist.z;

            //максимальное растояние для спавна планет найденно
            Debug.Log("Generate Planets, Cell:" + pos + " Gen dist" + distPlanetMax);

            return distPlanetMax;
        }
    }

    public void VisualMainObj() {
    
    }


}










//Класс комсического объекта (звезды или планеты или луны)
public class SpaceObjData {
    //визуальная часть
    public SpaceObjCtrl visual;

    public CellS cell; //Ячейка родитель
    public SpaceObjData parent; //Космический объект - родитель
    public SpaceObjData[] childs; //Луны
    public int radiusOrbit;
    public int radiusChildZone;
    public int radiusGravity;
    public int radiusVoid;

    public float[,,] perlin; //Перлин планеты

    public Color color;
    public float time360Rotate; //Время одного полного оборота
    public float bright; //яркость
    public float mass; //масса
    public float atmosphere; //плотность атмосферы
    public float liting;

    /// <summary>
    /// Паттерн генерации планеты
    /// </summary>
    public PatternPlanet patternPlanet;
    //Основные текстуры планеты
    public SpaceObjMap[] MainTextures;
    

    public Size size;
    public TidalLocking tidalLocking;

    public enum TidalLocking
    {
        No,
        Yes
    }

    public SpaceObjData(CellS cell) {
        this.cell = cell;

        
    }

    public float GetPerlinFromIndex(int indexMax512) {

        int perlinX = indexMax512 % this.perlin.GetLength(0);
        int perlinY = indexMax512 / this.perlin.GetLength(0);
        perlinY = perlinY % this.perlin.GetLength(1);
        int perlinZ = indexMax512 / (this.perlin.GetLength(0) * this.perlin.GetLength(1));

        return perlin[perlinX, perlinY, perlinZ];
    }

    //Сгенерировать космический объект на основе объекта родителя или без родителя если объект является центральным в ячейке
    public void GenObjData(SpaceObjData parent, float perlin, float distanceChildFree) {
        GenObjData(parent, perlin, distanceChildFree, null);
    }
    public void GenObjData(SpaceObjData parent, float perlin, float distanceChildFree, PatternPlanet patternPlanet) {
        this.parent = parent;

        //Нужно определиться с максимальными значениями массы и размера, эти 2 параметра определяют тип обьекта
        float startMass = 65536;
        float startSize = 65536;
        float randMass = 0;
        float randSize = 0;


        //Если родитель есть
        if (parent != null) {
            //Делаем максимальную массу и размер на 2 порядка ниже
            startMass = parent.mass / 4;
            startSize = (int)parent.size - 2;
        }

        randMass += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[0]));
        randSize += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[1]));

        randMass = Mathf.Abs(randMass);
        randSize = Mathf.Abs(randSize);

        randMass += perlin;
        randMass /= 2;

        randSize += perlin;
        randSize /= 2;



        //Стартовые значения опеределены
        //Генерируем данные
        GenStandart();

        //Выбираем паттерн
        ChoosePatternPlanet();
        //Генерируем по паттерну если выбранно
        GenPatternPlanet();

        //Когда выбран паттерн планеты и уже известны размеры планет инициализируем остальное
        inicializeLast();

        void GenStandart() {

            //Если родителя нет
            if (parent == null)
            {
                IniOrbitRadius();

                //Это должна быть звезда, выбираем из 5 размеров от 13-17 порядка
                int massPower = (int)((randMass * 1000) % 5) + 10;
                this.mass = Calc.GetSizeInt((Size)massPower);

                //Размер
                int sizePower = (int)((randSize * 1000) % 5) + 10;
                this.size = (Size)sizePower;

                iniData(Calc.GetSizeInt(size), mass);

                this.time360Rotate = perlin;
            }
            else
            {
                //Генерация планет и лун


                color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));

                //Размер
                int sizePower = (int)(((randSize * 1000) % startSize - 3) + 3); //размер меньше 3-го порядка не нужен
                this.size = (Size)sizePower;

                IniOrbitRadius();
            }

            void IniOrbitRadius() {
                radiusOrbit = 0;
                radiusChildZone = 0;

                if (parent == null)
                    return;


                //прибавляем на радиус родителя
                radiusOrbit += Calc.GetSizeInt(parent.size);

                //прибавляем радиус планет что впереди
                int index = 0;
                foreach (SpaceObjData objData in parent.childs) {
                    if (objData != this)
                    {
                        //Прибавляем радиус влияния планеты
                        radiusOrbit += objData.radiusGravity + objData.radiusVoid;
                        //radiusOrbit += Calc.GetSizeInt(objData.size);
                        index++;
                    }
                    else {
                        //Узнаем какой возможный максимум для влияния этой планеты
                        radiusGravity = Calc.GetSizeInt(objData.size) * 4;
                        //Проблемма!! надо проверять что растояние гравитации не будет больше distanceChildFree

                        radiusChildZone = (int)(radiusGravity * perlin);


                        //Определяемся с размером пустоты
                        radiusVoid = (int)(radiusGravity * ((perlin * 100) % 10));

                        //Узнаем сколько свободного места осталось у родителя


                        radiusOrbit += radiusGravity / 2 + radiusVoid /2;
                        //radiusOrbit += Calc.GetSizeInt(objData.size)/2;
                        break;
                    }
                }
            }
        }

        //Получить паттерн
        void ChoosePatternPlanet() {
            //Если паттерн принудительный есть
            if (patternPlanet != null) {
                //используем
                this.patternPlanet = patternPlanet;

                //Нужно определять размер по перлину
                size = patternPlanet.termsGenerate.sizeMax;
                atmosphere = patternPlanet.parameters.AtmosphereMax;

                return;
            }

            //Если паттерна нет, ищем подходящий из списка планетарных паттернов

        }
        void GenPatternPlanet() {
            if (parent == null && this.patternPlanet != null)
                return;

            //предполагается что паттерн уже выбран совпадающий по размеру и температуре и спектру звезды и прочее

            //Определяемся с жидкостью
            //Определяемся с атмосферой
            //
        }

        void inicializeLast() {
            //на основе размера планеты Создаем массив под указанное количество текстур планеты
            int needTextures = (int)size;

            //Создали массив текстур
            MainTextures = new SpaceObjMap[needTextures];
        }
    }

    //На основе массы и размера определяемся с остальными данными объекта
    void iniData(float size, float mass)
    {
        //Для каждого размера свои правила
        //Если размер

        if(size >= Calc.GetSizeInt(Size.s8192o)) ini17();
        else if (size >= Calc.GetSizeInt(Size.s4096o)) ini16();
        else if (size >= Calc.GetSizeInt(Size.s2048o)) ini15();
        else if (size >= Calc.GetSizeInt(Size.s1024o)) ini14();
        else if (size >= Calc.GetSizeInt(Size.s512o)) ini13();
        else if (size >= Calc.GetSizeInt(Size.s256o)) ini12();
        else if (size >= Calc.GetSizeInt(Size.s128o)) ini11();
        else if (size >= Calc.GetSizeInt(Size.s64o)) ini10();
        else if (size >= Calc.GetSizeInt(Size.s32o)) ini09();
        else if (size >= Calc.GetSizeInt(Size.s16o)) ini08();
        else if (size >= Calc.GetSizeInt(Size.s8o)) ini07();

        void ini17()
        {
            //Узнаем во сколько раз массы меньше чем размера
            float massCoof = mass / size;

            //если масса примерно соотносится с размером
            //Голубой гигант
            if (massCoof >= 1)
            {
                StarBlue();
            }
            //Бело голубой
            else if (massCoof >= 1.0f / 4) {
                StarBlueWhite();
            }
            //Оранжевый гигант
            else if (massCoof >= 1.0f / 8)
            {
                StarOrange();
            }
            //Красный гигант
            else
            {
                StarRed();
            }
        }
        void ini16()
        {
            //Узнаем во сколько раз массы меньше чем размера
            float massCoof = mass / size;

            //если масса примерно соотносится с размером
            //Голубой
            if (massCoof >= 2)
            {
                StarBlue();
            }
            //Бело голубой
            else if (massCoof >= 1)
            {
                StarBlueWhite();
            }
            //белый
            else if (massCoof >= 1.0f / 2)
            {
                StarWhite();
            }
            //желтый
            else if (massCoof >= 1.0f / 8)
            {
                StarYellow();
            }
            //оранжевый
            else
            {
                StarOrange();
            }
        }
        void ini15()
        {
            //Узнаем во сколько раз массы меньше чем размера
            float massCoof = mass / size;

            //если масса примерно соотносится с размером
            //белый
            if (massCoof >= 1)
            {
                StarWhite();
            }
            //желто белый
            else if (massCoof >= 1.0f / 2)
            {
                StarWhiteYellow();
            }
            //желтый
            else
            {
                StarYellow();
            }
        }
        void ini14()
        {
            //Узнаем во сколько раз массы меньше чем размера
            float massCoof = mass / size;

            //если масса примерно соотносится с размером
            //Голубой
            if (massCoof >= 8)
            {
                StarBlue();
            }
            //Бело голубой
            else if (massCoof >= 4)
            {
                StarBlueWhite();
            }
            //желтый
            else if (massCoof >= 1.0f / 2)
            {
                StarYellow();
            }
            //оранжевый
            else if (massCoof >= 1.0f / 4)
            {
                StarOrange();
            }
            //красный
            else if (massCoof >= 1.0f / 8)
            {
                StarRed();
            }
            //коричневый
            else
            {
                StarBrown();
            }
        }
        void ini13()
        {
            //Узнаем во сколько раз массы меньше чем размера
            float massCoof = mass / size;

            //если масса примерно соотносится с размером
            //Голубой
            if (massCoof >= 16)
            {
                StarBlue();
            }
            //Бело голубой
            else if (massCoof >= 8)
            {
                StarBlueWhite();
            }
            //Белый
            else if (massCoof >= 4)
            {
                StarWhite();
            }
            //желто-белый
            else if (massCoof >= 2)
            {
                StarWhiteYellow();
            }
            //желтый
            else if (massCoof >= 1.0f)
            {
                StarYellow();
            }
            //оранжевый
            else if (massCoof >= 1.0f / 2)
            {
                StarOrange();
            }
            //красный
            else if (massCoof >= 1.0f / 4)
            {
                StarRed();
            }
            //коричневый
            else
            {
                StarBrown();
            }
        }
        void ini12()
        {

        }
        void ini11()
        {

        }
        void ini10()
        {

        }
        void ini09()
        {

        }
        void ini08()
        {

        }
        void ini07()
        {

        }

        void StarBlue() {
            color = SpaceColors.main.Blue;
            bright = 0.99f;
        }
        void StarBlueWhite()
        {
            color = SpaceColors.main.BlueWhite;
            bright = 0.95f;
        }
        void StarWhite()
        {
            color = SpaceColors.main.White;
            bright = 0.90f;
        }
        void StarWhiteYellow()
        {
            color = SpaceColors.main.WhiteYellow;
            bright = 0.85f;
        }
        void StarYellow() {
            color = SpaceColors.main.Yellow;
            bright = 0.8f;
        }
        void StarOrange()
        {
            color = SpaceColors.main.Orange;
            bright = 0.7f;
        }
        void StarRed()
        {
            color = SpaceColors.main.Red;
            bright = 0.5f;
        }
        void StarBrown()
        {
            color = SpaceColors.main.Brown;
            bright = 0.25f;
        }
    }

    //Сгенериновать перлин для планеты
    public void GenPerlinLoc(float perlinGen)
    {
        //

        //Начальный размер 
        float scale = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen) % 5 + 1;
        float freq = 1;
        float offsetX = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10);
        float offsetY = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 100);
        float offsetZ = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 1000);
        int octaves = (int)Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10000) % 6 + 1; //Максимум до 6 октав

        GraficData.Perlin dataPerlinLoc = new GraficData.Perlin(scale, freq, offsetX, offsetY, offsetZ, octaves, false);
        dataPerlinLoc.Calculate();
        perlin = dataPerlinLoc.result;

    }

    //Сгенерировать планету на основе доступного пространства
    public void GenPlanets(float distGenMax, float perlin) {
        //Создаем массив планет
        childs = new SpaceObjData[0];

        //Если дистанция для генерации планет слишком мала, выходим
        if (distGenMax < 512)
            return;


        //Расчитать локальный перлин на основе глобального
        GenPerlinLoc((perlin*1000)%1);

        //Нужно уменьшить максимальное растояние для генерации планет если родителя нет
        //if(parent == null)
            //distGenMax *= (0.5f * (perlin * 0.5f));

        int sizeInt = Calc.GetSizeInt(size);
        //Текущее растояние генерации
        //Прибавляем радиус планеты
        int distGenNow = sizeInt;

        //Определяемся с максимальным количеством спутников
        int childMaximum;
        if (parent == null)
            childMaximum = (int)(512 * perlin);
        else childMaximum = (int)(10 * perlin);

        //Создаем планеты пока не закончится пространство или планет будет слишком дохера
        int numTryAdd = 0;
        while (distGenNow < distGenMax && childs.Length < childMaximum && numTryAdd < 512) {

            //Доступное пространство для генерации лун
            float distFree = distGenMax - distGenNow;
            AddPlanet(distFree);
        }

        //Перебираем детей чтобы создать у них детей
        for (int num = 0; num < childs.Length; num++) {
            childs[num].GenPlanets(childs[num].radiusChildZone, GetPerlinFromIndex(num));
        }
        
        void AddPlanet(float distFree) {
            //Создали планету
            SpaceObjData spaceObj = new SpaceObjData(cell);
            //Добавляем ее родителю
            SpaceObjData[] childOld = childs;

            childs = new SpaceObjData[childs.Length + 1];
            for (int num = 0; num < childOld.Length; num++) {
                childs[num] = childOld[num];
            }

            int childNum = childOld.Length;
            childs[childNum] = spaceObj;

            //Инициализируем ее
            childs[childNum].GenObjData(this, GetPerlinFromIndex(childNum), distFree);

            //Когда планета инициализированна (есть масса и размер) прибавляем растояние генерации
            distGenNow += childs[childNum].radiusGravity + childs[childNum].radiusVoid; //Calc.GetSizeInt(child[child.Length - 1].size);


        }
    }

    /// <summary>
    /// Получить основную текстуру планеты чем выше качество тем больше текстура
    /// </summary>
    /// <param name="quarity"></param>
    /// <returns></returns>
    public Texture2D GetMainTexture(int quarity) {
        //Проверяем есть ли текстура этого качества
        if (MainTextures[quarity] != null)
            return MainTextures[quarity].texture;

        //Если для генерации не время
        if (SpaceObjMap.timeLastGen == Time.time) {
            //Возвращяем последнюю сгенерированнуютекстуру
            for (int num = MainTextures.Length - 1; num > 0; num--) {
                if (MainTextures[num] != null)
                    return MainTextures[num].texture;
            }
        }
        //Если генерировать можно
        else {
            //ищем несгенерированную текстуру начиная с низкого качества
            for (int num = 0; num < MainTextures.Length; num++) {
                if (MainTextures[num] != null)
                    continue;

                //Генерируем
                MainTextures[num] = new SpaceObjMap(this, (Size)num);

                //Запоминаем время генерации
                SpaceObjMap.timeLastGen = Time.time;

                //Возвращяем
                return MainTextures[num].texture;
            }
        }

        //возвращяем нулевую текстуру
        if (MainTextures[0] == null)
        {
            MainTextures[0] = new SpaceObjMap(this, Size.s1o);
        }
        return MainTextures[0].texture;
    }

    public float[,] GetHeightMap(int quarity) {

        if (MainTextures[quarity] != null)
            return MainTextures[quarity].map;

        //Если требуется карта высот то возвратить в любом случае
        //Генерируем
        MainTextures[quarity] = new SpaceObjMap(this, (Size)quarity);
        //Запоминаем время генерации
        SpaceObjMap.timeLastGen = Time.time;


        return MainTextures[quarity].map;
    }
}

//Хранит данные планеты о поверхности
public class SpaceObjMap {
    static public float timeLastGen = 0;

    //какой размер одного пикселя в блоках
    int sizePixel = 65536;
    public float[,] map;

    //Текстура планеты
    public Texture2D texture;

    //Сгенерировать текстуру
    public SpaceObjMap(SpaceObjData data, Size sizeTexture) {

        //Создаем текстуру указанного разрешения
        //Сперва определяемся с размерами карты вершин
        int height = Calc.GetSizeInt(sizeTexture);
        int width = height * 2;

        //получаем размер планеты
        int sizePlanet = Calc.GetSizeInt(data.size);

        //Узнаем сколько блоков в одном текселе
        this.sizePixel = sizePlanet / height;

        //Шумим перлином на видяхе
        GenPerlin();

        CreateTexture();

        void GenPerlin() {
            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

            float scale = 1.0f / this.sizePixel * 4000;

            map = GraficData.Perlin2D.GetArrayMap(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 1, false, false);

        }
        
        void CreateTexture() {

            texture = new Texture2D(map.GetLength(0), map.GetLength(1) * 2);

            //рисование основной текстуры - нижняя половина
            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    Color color = new Color(map[x, y], map[x, y], map[x, y]);
                    //if (map[x, y] < 0.1f)
                    //    color = Color.blue;

                    texture.SetPixel(x,y, color);
                }
            }
            //Рисование отзеркаленной текстуры верхняя половина
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {

                    //По х смещяем на половину
                    int mirrorX = x - (map.GetLength(0)/2);
                    //По у отзеркаливаем
                    int mirrorY = map.GetLength(1)-1 - y;

                    if (mirrorX < 0)
                        mirrorX += map.GetLength(0);

                    Color color = texture.GetPixel(mirrorX, mirrorY);

                    texture.SetPixel(x, y + map.GetLength(1), color);
                }
            }


            texture.anisoLevel = 0;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;

            texture.Apply();
        }
    }
}

//
public enum Size
{
    s1o = 1,
    s2o = 2,
    s4o = 3,
    s8o = 4,
    s16o = 5,
    s32o = 6,
    s64o = 7,
    s128o = 8,
    s256o = 9,
    s512o = 10,
    s1024o = 11,
    s2048o = 12,
    s4096o = 13,
    s8192o = 14
}

