using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Cosmos
{
    public class StarData : ObjData, IGetMainTexture
    {
        //Массив карт высот
        HeightMap[] heightMaps;

        public StarData(CellS cell) {
            this.cell = cell;
        }

        public override HeightMap[,] GetHeightMap(Size quarity)
        {
            throw new System.NotImplementedException();
        }

        public Texture2D GetMainTexture(Size quality)
        {
            throw new System.NotImplementedException();
        }

        public override void GenData(ObjData parent, float perlin)
        {
            base.GenData(parent, perlin);

            GenStandart();
            IniOrbitRadius();
            GenHeightMap();

            void GenStandart()
            {

                //Это должна быть звезда, выбираем из 5 размеров от 13-17 порядка
                int massPower = (int)((randMass * 1000) % 5) + 13;
                this.mass = Calc.GetSizeInt((Size)massPower);

                //Размер
                int sizePower = (int)((randSize * 1000) % 5) + 13;
                this.size = (Size)sizePower;

                iniData(Calc.GetSizeInt(size), mass);

                this.time360Rotate = perlin;
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

            void GenHeightMap() {
                //на основе размера планеты Создаем массив под указанное количество текстур планеты
                int needTextures = (int)size;

                //Создали массив текстур
                heightMaps = new HeightMap[needTextures];
            }


            //На основе массы и размера определяемся с остальными данными объекта
            void iniData(float size, float mass)
            {
                //Для каждого размера свои правила
                //Если размер

                if (size >= Calc.GetSizeInt(Size.s65536)) ini17();
                else if (size >= Calc.GetSizeInt(Size.s32768)) ini16();
                else if (size >= Calc.GetSizeInt(Size.s16384)) ini15();
                else if (size >= Calc.GetSizeInt(Size.s8192)) ini14();
                else if (size >= Calc.GetSizeInt(Size.s4096)) ini13();

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
                    else if (massCoof >= 1.0f / 4)
                    {
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

                void StarBlue()
                {
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
                void StarYellow()
                {
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

        }
    }


}
