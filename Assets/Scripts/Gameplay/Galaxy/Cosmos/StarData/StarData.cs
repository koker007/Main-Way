using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace Cosmos
{
    public class StarData : ObjData, IGetMainTexture
    {
        //������ ���� �����
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

                //��� ������ ���� ������, �������� �� 5 �������� �� 13-17 �������
                int massPower = (int)((randMass * 1000) % 5) + 13;
                this.mass = Calc.GetSizeInt((Size)massPower);

                //������
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


                //���������� �� ������ ��������
                radiusOrbit += Calc.GetSizeInt(parent.size);

                //���������� ������ ������ ��� �������
                int index = 0;
                foreach (ObjData objData in parent.childs)
                {
                    if (objData != this)
                    {
                        //���������� ������ ������� �������
                        radiusOrbit += objData.radiusGravity + objData.radiusVoid;
                        //radiusOrbit += Calc.GetSizeInt(objData.size);
                        index++;
                    }
                    else
                    {
                        //������ ����� ��������� �������� ��� ������� ���� �������
                        radiusGravity = Calc.GetSizeInt(objData.size) * 4;
                        //���������!! ���� ��������� ��� ��������� ���������� �� ����� ������ distanceChildFree

                        radiusChildZone = (int)(radiusGravity * perlin);


                        //������������ � �������� �������
                        radiusVoid = (int)(radiusGravity * ((perlin * 100) % 10));

                        //������ ������� ���������� ����� �������� � ��������


                        radiusOrbit += radiusGravity / 2 + radiusVoid / 2;
                        //radiusOrbit += Calc.GetSizeInt(objData.size)/2;
                        break;
                    }
                }
            }

            void GenHeightMap() {
                //�� ������ ������� ������� ������� ������ ��� ��������� ���������� ������� �������
                int needTextures = (int)size;

                //������� ������ �������
                heightMaps = new HeightMap[needTextures];
            }


            //�� ������ ����� � ������� ������������ � ���������� ������� �������
            void iniData(float size, float mass)
            {
                //��� ������� ������� ���� �������
                //���� ������

                if (size >= Calc.GetSizeInt(Size.s65536)) ini17();
                else if (size >= Calc.GetSizeInt(Size.s32768)) ini16();
                else if (size >= Calc.GetSizeInt(Size.s16384)) ini15();
                else if (size >= Calc.GetSizeInt(Size.s8192)) ini14();
                else if (size >= Calc.GetSizeInt(Size.s4096)) ini13();

                void ini17()
                {
                    //������ �� ������� ��� ����� ������ ��� �������
                    float massCoof = mass / size;

                    //���� ����� �������� ����������� � ��������
                    //������� ������
                    if (massCoof >= 1)
                    {
                        StarBlue();
                    }
                    //���� �������
                    else if (massCoof >= 1.0f / 4)
                    {
                        StarBlueWhite();
                    }
                    //��������� ������
                    else if (massCoof >= 1.0f / 8)
                    {
                        StarOrange();
                    }
                    //������� ������
                    else
                    {
                        StarRed();
                    }
                }
                void ini16()
                {
                    //������ �� ������� ��� ����� ������ ��� �������
                    float massCoof = mass / size;

                    //���� ����� �������� ����������� � ��������
                    //�������
                    if (massCoof >= 2)
                    {
                        StarBlue();
                    }
                    //���� �������
                    else if (massCoof >= 1)
                    {
                        StarBlueWhite();
                    }
                    //�����
                    else if (massCoof >= 1.0f / 2)
                    {
                        StarWhite();
                    }
                    //������
                    else if (massCoof >= 1.0f / 8)
                    {
                        StarYellow();
                    }
                    //���������
                    else
                    {
                        StarOrange();
                    }
                }
                void ini15()
                {
                    //������ �� ������� ��� ����� ������ ��� �������
                    float massCoof = mass / size;

                    //���� ����� �������� ����������� � ��������
                    //�����
                    if (massCoof >= 1)
                    {
                        StarWhite();
                    }
                    //����� �����
                    else if (massCoof >= 1.0f / 2)
                    {
                        StarWhiteYellow();
                    }
                    //������
                    else
                    {
                        StarYellow();
                    }
                }
                void ini14()
                {
                    //������ �� ������� ��� ����� ������ ��� �������
                    float massCoof = mass / size;

                    //���� ����� �������� ����������� � ��������
                    //�������
                    if (massCoof >= 8)
                    {
                        StarBlue();
                    }
                    //���� �������
                    else if (massCoof >= 4)
                    {
                        StarBlueWhite();
                    }
                    //������
                    else if (massCoof >= 1.0f / 2)
                    {
                        StarYellow();
                    }
                    //���������
                    else if (massCoof >= 1.0f / 4)
                    {
                        StarOrange();
                    }
                    //�������
                    else if (massCoof >= 1.0f / 8)
                    {
                        StarRed();
                    }
                    //����������
                    else
                    {
                        StarBrown();
                    }
                }
                void ini13()
                {
                    //������ �� ������� ��� ����� ������ ��� �������
                    float massCoof = mass / size;

                    //���� ����� �������� ����������� � ��������
                    //�������
                    if (massCoof >= 16)
                    {
                        StarBlue();
                    }
                    //���� �������
                    else if (massCoof >= 8)
                    {
                        StarBlueWhite();
                    }
                    //�����
                    else if (massCoof >= 4)
                    {
                        StarWhite();
                    }
                    //�����-�����
                    else if (massCoof >= 2)
                    {
                        StarWhiteYellow();
                    }
                    //������
                    else if (massCoof >= 1.0f)
                    {
                        StarYellow();
                    }
                    //���������
                    else if (massCoof >= 1.0f / 2)
                    {
                        StarOrange();
                    }
                    //�������
                    else if (massCoof >= 1.0f / 4)
                    {
                        StarRed();
                    }
                    //����������
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
