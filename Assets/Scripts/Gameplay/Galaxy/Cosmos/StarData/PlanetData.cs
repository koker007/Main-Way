using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class PlanetData : ObjData
    {
        HeightMap[] heightMaps;

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
                //на основе размера планеты —оздаем массив под указанное количество текстур планеты
                int needTextures = (int)size;

                //—оздали массив текстур
                heightMaps = new HeightMap[needTextures];
            }

            void GenStandart() {
                float startMass = 65536;
                float startSize = 65536;
                //≈сли родитель есть
                if (parent != null)
                {
                    //ƒелаем максимальную массу и размер на 2 пор€дка ниже
                    startMass = parent.mass / 4;
                    startSize = (int)parent.size - 2;
                }

                //√енераци€ планет и лун
                color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));

                //–азмер
                int sizePower = (int)(((randSize * 1000) % startSize - 3) + 3); //размер меньше 7-го пор€дка не нужен
                this.size = (Size)sizePower;
            }

            void IniOrbitRadius()
            {
                radiusOrbit = 0;
                radiusChildZone = 0;

                if (parent == null)
                    return;


                //прибавл€ем на радиус родител€
                radiusOrbit += Calc.GetSizeInt(parent.size);

                //прибавл€ем радиус планет что впереди
                int index = 0;
                foreach (ObjData objData in parent.childs)
                {
                    if (objData != this)
                    {
                        //ѕрибавл€ем радиус вли€ни€ планеты
                        radiusOrbit += objData.radiusGravity + objData.radiusVoid;
                        //radiusOrbit += Calc.GetSizeInt(objData.size);
                        index++;
                    }
                    else
                    {
                        //”знаем какой возможный максимум дл€ вли€ни€ этой планеты
                        radiusGravity = Calc.GetSizeInt(objData.size) * 4;
                        //ѕроблемма!! надо провер€ть что расто€ние гравитации не будет больше distanceChildFree

                        radiusChildZone = (int)(radiusGravity * perlin);


                        //ќпредел€емс€ с размером пустоты
                        radiusVoid = (int)(radiusGravity * ((perlin * 100) % 10));

                        //”знаем сколько свободного места осталось у родител€


                        radiusOrbit += radiusGravity / 2 + radiusVoid / 2;
                        //radiusOrbit += Calc.GetSizeInt(objData.size)/2;
                        break;
                    }
                }
            }
        }

        public override float[,] GetHeightMap()
        {
            throw new System.NotImplementedException();
        }

        public override Texture2D GetMainTexture(Size quality)
        {
            throw new System.NotImplementedException();
        }
    }
}
