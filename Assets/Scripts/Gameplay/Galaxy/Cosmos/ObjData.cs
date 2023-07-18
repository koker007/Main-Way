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
        /// Получить текстуру поверхности мира, указанного качества
        /// </summary>
        /// <param name="quarity"></param>
        /// <returns></returns>
        public abstract Texture2D GetMainTexture(Size quality);
        /// <summary>
        /// Получить карту высот данного мира
        /// </summary>
        /// <returns></returns>
        public abstract float[,] GetHeightMap();

        /// <summary>
        /// Получить перлин на основе индекса
        /// </summary>
        /// <param name="indexMax512"></param>
        /// <returns></returns>
        public float GetPerlinFromIndex(int indexMax512)
        {

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
            float offsetX = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10);
            float offsetY = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 100);
            float offsetZ = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 1000);
            int octaves = (int)Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10000) % 6 + 1; //Максимум до 6 октав

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
            GenPerlin();

            void GenPerlin()
            {
                float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
                float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
                float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

                float scale = 1.0f / this.sizePixel * 4000;

                map = GraficData.Perlin2D.GetArrayMap(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 1, false, false);

            }
        }
    }
}
