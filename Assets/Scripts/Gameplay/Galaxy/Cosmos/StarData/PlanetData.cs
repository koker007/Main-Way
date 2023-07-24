using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class PlanetData : ObjData, IGetMainTexture
    {
        //Чанки 
        //карты высот
        HeightMap[][,] heightMaps;
        //Перлин биомов
        BiomeMaps[][,] biomesMaps;
        int[][,] biomeNum;
        Texture2D textureShadow;
        public Texture2D TextureShadow { get { return textureShadow; } }

        Texture2D[] TextureMaps;

        PatternPlanet pattern;

        public PlanetData(CellS cell)
        {
            this.cell = cell;
            
        }

        public override void GenData(ObjData parent, float perlin) {
            base.GenData(parent, perlin);

            GenStandart();
            IniOrbitRadius();

            GenHeightMap();

            void GenHeightMap()
            {
                //на основе размера планеты Создаем массив под указанное количество текстур планеты
                int needTextures = (int)size;

                //Создали массив текстур
                heightMaps = new HeightMap[needTextures][,];
            }

            void GenStandart() {
                float startMass = 65536;
                float startSize = 65536;
                //Если родитель есть
                if (parent != null)
                {
                    //Делаем максимальную массу и размер на 2 порядка ниже
                    startMass = parent.mass / 4;
                    startSize = (int)parent.size - 2;
                }

                //Генерация планет и лун
                color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));

                //Размер
                int sizePower = (int)(((randSize * 1000) % startSize - 3) + 3); //размер меньше 7-го порядка не нужен
                this.size = (Size)sizePower;
            }

            void IniOrbitRadius()
            {
                radiusOrbit = 0;
                radiusChildZone = 0;

                if (parent == null)
                    return;


                //прибавляем на радиус родителя
                radiusOrbit += Calc.GetSizeInt(parent.size);

                //прибавляем радиус планет что впереди
                int index = 0;
                foreach (ObjData objData in parent.childs)
                {
                    if (objData != this)
                    {
                        //Прибавляем радиус влияния планеты
                        radiusOrbit += objData.radiusGravity + objData.radiusVoid;
                        //radiusOrbit += Calc.GetSizeInt(objData.size);
                        index++;
                    }
                    else
                    {
                        //Узнаем какой возможный максимум для влияния этой планеты
                        radiusGravity = Calc.GetSizeInt(objData.size) * 4;
                        //Проблемма!! надо проверять что растояние гравитации не будет больше distanceChildFree

                        radiusChildZone = (int)(radiusGravity * perlin);


                        //Определяемся с размером пустоты
                        radiusVoid = (int)(radiusGravity * ((perlin * 100) % 10));

                        //Узнаем сколько свободного места осталось у родителя


                        radiusOrbit += radiusGravity / 2 + radiusVoid / 2;
                        //radiusOrbit += Calc.GetSizeInt(objData.size)/2;
                        break;
                    }
                }
            }
        }

        public override HeightMap[,] GetHeightMap(Size quarity)
        {
            int q = (int)quarity;

            //Создаем массив если он пуст
            if (heightMaps[q] == null) {
                //Определяемся с количеством чанков в данном качестве
                int height = Calc.GetSizeInt(quarity);
                int width = height;

                int chankXMax = width / Chank.Size;
                int chankYMax = height / Chank.Size;

                heightMaps[q] = new HeightMap[chankXMax, chankYMax];
            }


            //Проверяем что карта высот полная
            //Если требуется карта высот то возвратить в любом случае
            //Генерируем
            for (int x = 0; x < heightMaps[q].GetLength(0); x++) {
                for (int y = 0; y < heightMaps[q].GetLength(1); y++) {
                    if (heightMaps[q][x, y] != null)
                        continue;

                    heightMaps[q][x, y] = new HeightMap(this, quarity, new Vector2Int(x, y));

                    //Запоминаем время генерации
                    SpaceObjMap.timeLastGen = Time.time;
                }
            }

            return heightMaps[q];
        }

        public Texture2D GetMainTexture(Size quality)
        {
            TextureMaps ??= new Texture2D[(int)size];

            //Проверяем что текстуры нету
            if (TextureMaps[(int)quality] != null)
            {
                return TextureMaps[(int)quality];
            }
            Debug.Log("PlanetSize " + size);

            //Создаем массив
            int q = (int)quality;


            int sizeTexture = Calc.GetSizeInt(size) / Calc.GetSizeInt(quality);

            //Определяемся с количеством чанков в данном качестве
            int chankXMax = sizeTexture / Chank.Size;
            int chankYMax = chankXMax;

            TextureMaps[q] ??= new Texture2D(sizeTexture, sizeTexture);

            for (int chankX = 0; chankX < chankXMax; chankX++) {
                for (int chankY = 0; chankY < chankYMax; chankY++) {
                    GenTexturePart(quality, new Vector2Int(chankX, chankY));
                }
            }

            TextureMaps[(int)quality].filterMode = FilterMode.Point;
            TextureMaps[(int)quality].Apply();

            Texture2D result = TextureMaps[(int)quality];

            return result;


        }

        void GenTexturePart(Size quality, Vector2Int chankPos) {
            iniBiomeMaps();

            int q = (int)quality - 1;

            //Генерируем биомы
            biomesMaps[q][chankPos.x, chankPos.y] ??= new BiomeMaps(this, quality, chankPos, 6);

            //Биомы есть теперь создаем текстуру

            //Храним номер биома победителя
            int[,] biomeNum = new int[biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(0), biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1)];
            float[,] biomeZeroPower = new float[biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(0), biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1)];

            //Перебираем все биомы и в зависимости от силы биома выбираем цвет текстуры
            for (int x = 0; x < biomesMaps[q][chankPos.x,chankPos.y].maps.GetLength(0); x++) {
                for (int y = 0; y < biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1); y++) {

                    for (int arrayNum = 0; arrayNum < biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(2); arrayNum++) {

                        //Если сила биома слабая
                        if (biomeZeroPower[x,y] > biomesMaps[q][chankPos.x, chankPos.y].maps[x, y, arrayNum])
                            continue;

                        //запоминаем силу сильного биома
                        biomeZeroPower[x,y] = biomesMaps[q][chankPos.x, chankPos.y].maps[x, y, arrayNum];
                        //Запомнимаем номер биома победителя
                        biomeNum[x,y] = arrayNum;

                    }
                }
            }

            //Заполняем текстуру
            TextureMaps ??= new Texture2D[(int)size];

            int sizeTexture = Calc.GetSizeInt(size) / Calc.GetSizeInt(quality);
            

            TextureMaps[(int)quality] ??= new Texture2D(sizeTexture * 2, sizeTexture);

            Color[] colors = new Color[10];
            for (int num = 0; num < colors.GetLength(0); num++) {
                colors[num] = Color.black;

                if (num == 0)
                    colors[num] = Color.blue;
                else if (num == 1)
                    colors[num] = Color.cyan;
                else if (num == 2)
                    colors[num] = Color.gray;
                else if (num == 3)
                    colors[num] = Color.green;
                else if (num == 4)
                    colors[num] = Color.magenta;
                else if (num == 5)
                    colors[num] = Color.red;
                else if (num == 6)
                    colors[num] = Color.white;
                else if (num == 7)
                    colors[num] = Color.yellow;

            }

            for (int x = 0; x < biomeNum.GetLength(0); x++) {
                int globalX = biomeNum.GetLength(0) * chankPos.x + x;
                for (int y = 0; y < biomeNum.GetLength(1); y++)
                {
                    int globalY = biomeNum.GetLength(1) * chankPos.y + y;

                    if (biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(2) == 1)
                    {
                        float intensive = biomeZeroPower[x, y];
                        if (intensive * 20 % 2 > 1)
                            intensive = 1;
                        else
                            intensive = 0;

                        TextureMaps[(int)quality].SetPixel(globalX, globalY, new Color(intensive, intensive, intensive, 1));
                    }
                    else
                        TextureMaps[(int)quality].SetPixel(globalX, globalY, colors[biomeNum[x, y]]);
                }
            }

            

        }

        void UpdateTextureShadow() {
            //Если нет текстуры - создаем
            if (textureShadow == null) {
                //качество текстуры зависит от размера планеты
                int size = Calc.GetSizeInt(this.size) / 32;

                textureShadow = new Texture2D(size * 2, size);
                List<StarData> stars = cell.Stars;

                //Проходимся по каждому пикселю
                for (int pixX = 0; pixX < textureShadow.width; pixX++)
                {
                    float pixCoofX = (float)pixX / (float)textureShadow.width;

                    for (int pixY = 0; pixY < textureShadow.height; pixY++)
                    {
                        float pixCoofY = (float)pixY / (float)textureShadow.height;

                        float lightSum = 0;
                        //перебираем все светила если их несколько
                        foreach (StarData star in stars) {
                            //Вычисляем направление света на планету
                            Vector3 vecLight = visual.transform.position - star.visual.transform.position;
                            float distLight = vecLight.magnitude;
                            vecLight.Normalize();

                            //вычислаяем позицию светила относительно звезды
                            Vector2 sunPos = new Vector2();

                            sunPos.y = Mathf.Atan2(vecLight.x, vecLight.y);
                            sunPos.x = Mathf.Atan2(vecLight.y, Mathf.Sqrt(vecLight.x * vecLight.x + vecLight.z * vecLight.z));

                            lightSum += GetLightingPixel(sunPos, new Vector2(pixCoofX, pixCoofY));
                        }

                        lightSum /= stars.Count;

                        lightSum = 1 - lightSum;
                        textureShadow.SetPixel(pixX, pixY, new Color(0, 0, 0, lightSum));
                    }
                }

                textureShadow.Apply();
                textureShadow.filterMode = FilterMode.Point;

                float GetLightingPixel(Vector2 sunPos, Vector2 pixCoof) {
                    //Вычисляем освещенность пикселя
                    //Если коофицент больше 0.5 то это темная сторона солнца
                    float lightingCoof = 0;

                    float lightX = 1 - Mathf.Abs(sunPos.x - pixCoof.x);
                    float lightXM = 1 - Mathf.Abs(sunPos.x - (pixCoof.x - 1));
                    float lightXP = 1 - Mathf.Abs(sunPos.x - (pixCoof.x + 1));

                    if (lightX < lightXM)
                        lightX = lightXM;
                    if (lightX < lightXP)
                        lightX = lightXP;

                    lightingCoof = lightX;

                    float lightY = 1 - Mathf.Abs(sunPos.y - pixCoof.y);
                    lightY += 0.25f;

                    lightingCoof = lightX;
                    if (lightY < lightX)
                        lightingCoof = lightY;

                    //Узнаем освещение за пределами полюсов
                    float lightYP = 1 - Mathf.Abs(sunPos.y - (1 + (1 - pixCoof.y)));
                    float lightYM = 1 - Mathf.Abs(sunPos.y - pixCoof.y * -1);
                    lightYP += 0.25f;
                    lightYM += 0.25f;

                    if (lightingCoof < lightYM)
                        lightingCoof = lightYM;
                    if (lightingCoof < lightYP)
                        lightingCoof = lightYP;


                    lightingCoof = lightingCoof - 0.7f;
                    lightingCoof *= 5;

                    return lightingCoof;
                }
            }
        }

        void iniBiomeMaps() {
            biomesMaps ??= new BiomeMaps[(int)size][,];



            for (int qualityNum = 0; qualityNum < biomesMaps.Length; qualityNum++) {
                if (biomesMaps[qualityNum] != null)
                    continue;

                int sizeBiome = Calc.GetSizeInt(size) / Calc.GetSizeInt((Size)qualityNum+1);

                //Определяемся с количеством чанков в данном качестве
                int chankXMax = (sizeBiome * 2) / Chank.Size;
                int chankYMax = (sizeBiome) / Chank.Size;

                biomesMaps[qualityNum] = new BiomeMaps[chankXMax, chankYMax];
            }
        }

        static public PlanetData GetRandomPlanet() {

            StarData starData = new StarData(GalaxyCtrl.galaxy.cells[Random.Range(0,15), Random.Range(0, 15), Random.Range(0, 15)]);
            starData.GenData(null, Random.Range(0, 1.0f));
            starData.GenChilds(500000, Random.Range(0, 1.0f));

            PlanetData result = null;

            foreach (ObjData objData in starData.childs) {
                PlanetData planetData = objData as PlanetData;
                if (planetData == null)
                    continue;

                result = planetData;
                break;
            }

            return result;
        }
    }


}
