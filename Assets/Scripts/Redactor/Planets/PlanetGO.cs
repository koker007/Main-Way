using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Game
{
    namespace Space
    {
        public class PlanetGO : VisualZoneGO
        {
            static public PlanetGO GetPlanetGO()
            {
                //Перебираем все чанки, ищем свободный
                foreach (VisualZoneGO visualZone in buffer)
                {
                    PlanetGO planetGO = visualZone as PlanetGO;

                    if (planetGO != null &&
                        planetGO.data == null)
                        return planetGO;
                }

                //Необходимо создать новый префаб чанка
                PlanetGO planetNew = Instantiate(GameData.main.prefabPlanetGO);
                return planetNew;
            }

            public override void iniChanks()
            {
                if (chanks != null)
                    return;

                PlanetData planetData = data as PlanetData;

                //Узнаем размер планеты
                int planetSize = Calc.GetSizeInt(planetData.size);
                int chankSize = (int)Calc.GetSize(Chank.Size);
                //Создаем место под все размерности
                chanks = new ChankGO[(int)(planetData.size-chankSize)][,,];

                //Расчитываем для каждой размерности
                for (int sizeNow = 0; sizeNow < chanks.Length; sizeNow++)
                {
                    int sizeInt = Calc.GetSizeInt((Size)(sizeNow + 1));
                    //Узнаем количество чанков для z
                    Vector3Int chanksCount = new Vector3Int();
                    chanksCount.z = planetSize / (Chank.Size * sizeInt);
                    chanksCount.x = chanksCount.z * 2;
                    chanksCount.y = chanksCount.z / 2;

                    chanks[sizeNow] = new ChankGO[chanksCount.x, chanksCount.y, chanksCount.z];
                }
            }

            public override void Inizialize(ObjData data, bool isNow = false)
            {
                PlanetData planetData = data as PlanetData;

                //Прерываем если тип даных не планета
                if (planetData == null)
                    return;

                this.data = data;

                if (isNow)
                    now = this;

                JobGenerate jobGenerate = new JobGenerate(this, data.size);
                jobGenerate.Execute();

            }

            public override void JobStartGenerate(Size sizeGenMin)
            {
                if (chanks == null)
                    iniChanks();

                PlanetData planetData = data as PlanetData;

                //Перебираем все чанки c самого большого размера
                bool isGenMax = false;
                int sizeGenInt = sizeGenMin - Calc.GetSize(Chank.Size);
                for (int num = chanks.Length - 1; num >= (int)(sizeGenInt - 1); num--)
                {
                    if (chanks[num].Length == 0)
                        continue;

                    if (!isGenMax)
                    {
                        isGenMax = true;
                        GenAll(num);
                    }
                }

                void GenAll(int size)
                {
                    Size sizeChank = (Size)(size + 1);
                    //Перебираем все чанки указанного размера
                    for (int x = 0; x < chanks[size].GetLength(0); x++)
                    {
                        for (int y = 0; y < chanks[size].GetLength(1); y++)
                        {
                            for (int z = 0; z < chanks[size].GetLength(2); z++)
                            {
                                if (chanks[size][x, y, z] != null)
                                    continue;

                                chanks[size][x, y, z] = ChankGO.GetChankGO();
                                if (size == 0)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks1.transform);
                                else if(size == 1)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks2.transform);
                                else if (size == 2)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks4.transform);
                                else if (size == 3)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks8.transform);
                                else if (size == 4)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks8.transform);
                                else if (size == 5)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks16.transform);
                                else if (size == 6)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks32.transform);
                                else if (size == 7)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks64.transform);
                                else if (size == 8)
                                    chanks[size][x, y, z].transform.SetParent(ParentChanks128.transform);

                                chanks[size][x, y, z].Inicialize(planetData.GetChank(sizeChank, new Vector3Int(x,y,z)));

                                AddReDraw(chanks[size][x, y, z]);
                            }
                        }
                    }
                }

            }



            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {
                TransformWorld();
            }
        }
    }
}
