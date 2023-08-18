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
                //���������� ��� �����, ���� ���������
                foreach (VisualZoneGO visualZone in buffer)
                {
                    PlanetGO planetGO = visualZone as PlanetGO;

                    if (planetGO != null &&
                        planetGO.data == null)
                        return planetGO;
                }

                //���������� ������� ����� ������ �����
                PlanetGO planetNew = Instantiate(GameData.main.prefabPlanetGO);
                return planetNew;
            }

            public override void iniChanks()
            {
                if (chanks != null)
                    return;

                PlanetData planetData = data as PlanetData;

                //������ ������ �������
                int planetSize = Calc.GetSizeInt(planetData.size);

                //������� ����� ��� ��� �����������
                chanks = new ChankGO[(int)planetData.size][,,];

                //����������� ��� ������ �����������
                for (int sizeNow = 0; sizeNow < chanks.Length; sizeNow++)
                {
                    int sizeInt = Calc.GetSizeInt((Size)(sizeNow + 1));
                    //������ ���������� ������ ��� z
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

                //��������� ���� ��� ����� �� �������
                if (planetData == null)
                    return;

                this.data = data;

                if (isNow)
                    now = this;

            }

            public override void JobStartGenerate(Size sizeGenMin)
            {
                if (chanks == null)
                    iniChanks();

                PlanetData planetData = data as PlanetData;

                //���������� ��� ����� c ������ �������� �������
                bool isGenMax = false;
                for (int num = chanks.Length - 1; num >= (int)(sizeGenMin - 1); num--)
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
                    //���������� ��� ����� ���������� �������
                    for (int x = 0; x < chanks[size].GetLength(0); x++)
                    {
                        for (int y = 0; y < chanks[size].GetLength(1); y++)
                        {
                            for (int z = 0; z < chanks[size].GetLength(2); z++)
                            {
                                if (chanks[size][x, y, z] != null)
                                    continue;

                                chanks[size][x, y, z] = ChankGO.GetChankGO();
                                chanks[size][x, y, z].Inicialize(planetData.GetChank(sizeChank, new Vector3Int(x,y,z)));
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
