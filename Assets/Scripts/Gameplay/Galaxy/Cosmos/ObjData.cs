using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    //Родительский класс всех космических объектов
    public abstract class ObjData
    {
        //визуальная часть
        public SpaceObjCtrl visual;

        public CellS cell; //Ячейка родитель

        public ObjData parent; //Космический объект - родитель
        public List<ObjData> childs; //Луны

        /// <summary>
        /// Размер объекта
        /// </summary>
        public Size size;
        
        public int radiusOrbit;
        public int radiusChildZone;
        public int radiusGravity;
        public int radiusVoid;

        private float locPerlin = 0;
        public float[,,] perlin; //Перлин

        public Color color;
        public float time360Rotate; //Время одного полного оборота
        public float bright; //яркость
        public float mass; //масса
        public float atmosphere; //плотность атмосферы

        //Для генератора
        protected float randMass = 0;
        protected float randSize = 0;


        public virtual void GenData(ObjData parent, float perlin)
        {
            this.parent = parent;
            this.locPerlin = perlin;

            randMass = 0;
            randSize = 0;

            //Если родитель есть
            if (parent != null)
            {
                //Изменяем массу и размер
            }

            randMass += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[0]));
            randSize += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[1]));

            randMass = Mathf.Abs(randMass);
            randSize = Mathf.Abs(randSize);

            randMass += perlin;
            randMass /= 2;

            randSize += perlin;
            randSize /= 2;

        }

        /// <summary>
        /// Получить чанк карты высот данного мира
        /// </summary>
        /// <returns></returns>
        public abstract HeightMap[,] GetHeightMap(Size quarity);

        /// <summary>
        /// Получить перлин на основе индекса
        /// </summary>
        /// <param name="indexMax512"></param>
        /// <returns></returns>
        public float GetPerlinFromIndex(int indexMax512)
        {
            if (perlin == null) {
                GenPerlinLoc(locPerlin);
            }

            int perlinX = indexMax512 % this.perlin.GetLength(0);
            int perlinY = indexMax512 / this.perlin.GetLength(0);
            perlinY = perlinY % this.perlin.GetLength(1);
            int perlinZ = indexMax512 / (this.perlin.GetLength(0) * this.perlin.GetLength(1));

            return perlin[perlinX, perlinY, perlinZ];
        }


        //Сгенериновать перлин для космического объекта
        public void GenPerlinLoc(float perlinGen)
        {
            //

            //Начальный размер 
            float scale = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen) % 5 + 1;
            float freq = 1;
            float offsetX = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 10));
            float offsetY = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 100));
            float offsetZ = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 1000));
            int octaves = (int)Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 10000)) % 6 + 1; //Максимум до 6 октав

            GraficData.Perlin dataPerlinLoc = new GraficData.Perlin(scale, freq, offsetX, offsetY, offsetZ, octaves, false);
            dataPerlinLoc.Calculate();
            perlin = dataPerlinLoc.result;

        }

        //Сгенерировать детей, планеты, луны и тд. на основе доступного пространства
        public void GenChilds(float distGenMax, float perlin)
        {
            //Создаем массив планет
            childs = new List<ObjData>();

            //Если дистанция для генерации планет слишком мала, выходим
            if (distGenMax < 512)
                return;


            //Расчитать локальный перлин на основе глобального
            GenPerlinLoc((perlin * 1000) % 1);

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
            while (distGenNow < distGenMax && childs.Count < childMaximum && numTryAdd < 512)
            {

                //Доступное пространство для генерации лун
                float distFree = distGenMax - distGenNow;
                AddPlanet(distFree);
            }

            //Перебираем детей чтобы создать у них детей
            for (int num = 0; num < childs.Count; num++)
            {
                childs[num].GenChilds(childs[num].radiusChildZone, GetPerlinFromIndex(num));
            }

            void AddPlanet(float distFree)
            {
                //Создаем планету
                ObjData objData = new PlanetData(cell);
                childs.Add(objData);

                int index = childs.Count - 1;

                //Инициализируем ее
                childs[index].GenData(this, GetPerlinFromIndex(index));

                //Когда планета инициализированна (есть масса и размер) прибавляем растояние генерации
                distGenNow += childs[index].radiusGravity + childs[index].radiusVoid;


            }
        }
    }

    public enum TidalLocking
    {
        No,
        Yes
    }

    public class HeightMap
    {
        public const float factor = 0.875170906246f;

        static public float timeLastGen = 0;

        //какой размер одного пикселя в блоках
        int sizePixel = 65536;
        public float[,] map;

        //Сгенерировать карту
        public HeightMap(ObjData data, Size sizeTexture)
        {

            //Создаем текстуру указанного разрешения
            //Сперва определяемся с размерами карты вершин
            int height = Calc.GetSizeInt(sizeTexture);
            int width = height * 2;

            //получаем размер планеты
            int sizePlanet = Calc.GetSizeInt(data.size);

            //Узнаем сколько блоков в одном текселе
            this.sizePixel = sizePlanet / height;

            //Шумим перлином на видяхе
            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

            float scale = 1.0f / this.sizePixel * 8000;

            map = GenMap(width, height, scale, scale, scale, 5, offsetX, offsetY, offsetZ, 5, false, false);
        }

        //Сгенерировать участок карты размером 32 на 32
        public HeightMap(ObjData data, Size sizeTexture, Vector2Int partPos) {
            //Создаем текстуру размером 32 32 = 1024 пикселей

            //Находим размер всей поверхности планеты
            int height = Calc.GetSizeInt(sizeTexture);
            int width = height * 2;

            //получаем размер планеты
            int sizePlanet = Calc.GetSizeInt(data.size);

            //Узнаем сколько блоков в одном текселе
            this.sizePixel = sizePlanet / height;

            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), Mathf.Pow(data.GetPerlinFromIndex(105), data.GetPerlinFromIndex(373))) * 1000;
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), Mathf.Pow(data.GetPerlinFromIndex(281), data.GetPerlinFromIndex(255))) * 1000;
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), Mathf.Pow(data.GetPerlinFromIndex(110), data.GetPerlinFromIndex(304))) * 1000;

            float sizeContinent =  0.75f + (float)((data.GetPerlinFromIndex(159) * 1000) % 0.5f);

            float scale = (sizePlanet * sizeContinent) / sizePixel;

            map = GenPart(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 10, false, false, partPos.x, partPos.y);
        }

        float[,] GenMap(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ) {
            //Создаем новый массив
            float[,] arrayMap = new float[mapSizeX, mapSizeY];

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

                    float[,] partMap = GenPart(mapSizeX, mapSizeY, ScaleX, ScaleY, ScaleZ, Freq, offSetX, OffSetY, OffSetZ, Octaves, TimeX, TimeZ, chankX, chankY);

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
                            arrayMap[posMapX, posMapY] = partMap[x, y];
                        }
                    }
                }
            }
            return arrayMap;
        }
        float[,] GenPart(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ, int chankX, int chankY)
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
            float[,] arrayPart = new float[Chank.Size, Chank.Size];

            //Ищем фактор чанка
            float FactorChankX = (factor / ScaleX) * Chank.Size;
            float FactorChankY = (factor / ScaleY) * Chank.Size;
            float FactorChankZ = (factor / ScaleZ) * Chank.Size;

            float offSetX = OffSetX + FactorChankX * chankX;
            float offSetY = OffSetY + FactorChankY * chankY;
            float offSetZ = OffSetZ;

            if (TimeZ)
                offSetZ += Time.time * 0.1f;

            if (TimeX)
                offSetX += Time.time * 0.1f;

            float regionX = (chankX * Chank.Size) / (float)mapSizeX;
            float regionY = (chankY * Chank.Size) / (float)mapSizeY;

            GraficData.Perlin2D dataPerlin2D = new GraficData.Perlin2D(ScaleX, ScaleY, ScaleZ, Freq, offSetX, offSetY, offSetZ, Octaves, mapSizeX, mapSizeY, regionX, regionY);
            dataPerlin2D.Calculate();

            //Запихиваем копируем данные в массив
            for (int x = 0; x < Chank.Size; x++)
            {
                for (int y = 0; y < Chank.Size; y++)
                {
                    arrayPart[x, y] = dataPerlin2D.result[x, y, 0];
                }
            }
            return arrayPart;
        }
    }
}
