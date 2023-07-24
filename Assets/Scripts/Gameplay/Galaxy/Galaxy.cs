using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;


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

    public ObjData mainObjs; //Базовый объект во круг которого существует звездная система
    public GalaxyObjCtrl visual; //Визуальная часть базового объекта

    List<StarData> StarsInCell;
    public List<StarData> Stars { get {
            return StarsInCell ?? new List<StarData>();
        } }

    float perlinGlobal; //Глобальный перлин благодаря которому сгенерируется локальный
    public float perlinGlob {
        get { return perlinGlobal; }
    }

    //Конструктор ячейки
    public CellS(Vector3 pos, float perlin, Galaxy galaxy) {
        this.galaxy = galaxy;
        this.pos = pos;

        StarsInCell = new List<StarData>();

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
        mainObjs = new StarData(this);
        mainObjs.GenData(null, perlinGlobal);

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
        mainObjs.GenChilds(distGenMax, perlinGlobal);

        iniAllStars(); //Добавляем все светила в список


        Debug.Log("Cell Gen: " + pos + " Planets:" + mainObjs.childs.Count);

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

        void iniAllStars() {

            //Если в списке есть что-то выходим
            if (StarsInCell != null || mainObjs == null)
                return;

            //Проверяем список
            StarsInCell = new List<StarData>();

            //Проверяем звезду и ее детей
            TestStar(mainObjs);

            void TestStar(ObjData objData) {
                TestAdd(objData);

                foreach (ObjData child in mainObjs.childs) {
                    TestStar(child);
                }
            }

            void TestAdd(ObjData objData) {
                StarData starData = objData as StarData;

                if (starData == null)
                    return;

                StarsInCell.Add(starData);
            }

        }
    }

    public void VisualMainObj() {
    
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
    public SpaceObjMap(ObjData data, Size sizeTexture) {

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
    s1 = 1,
    s2 = 2,
    s4 = 3,
    s8 = 4,
    s16 = 5,
    s32 = 6,
    s64 = 7,
    s128 = 8,
    s256 = 9,
    s512 = 10,
    s1024 = 11,
    s2048 = 12,
    s4096 = 13,
    s8192 = 14,
    s16384 = 15,
    s32768 = 16,
    s65536 = 17
}

