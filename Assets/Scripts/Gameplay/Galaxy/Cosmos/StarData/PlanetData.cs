using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class PlanetData : ObjData
    {
        //Чанки карты высот
        HeightMap[][,] heightMaps;


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
                int width = height * 2;

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

        public override Texture2D GetMainTexture(Size quality)
        {
            throw new System.NotImplementedException();
        }
    }
}
