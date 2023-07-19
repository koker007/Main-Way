using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class PlanetData : ObjData
    {
        //����� ����� �����
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
                //�� ������ ������� ������� ������� ������ ��� ��������� ���������� ������� �������
                int needTextures = (int)size;

                //������� ������ �������
                heightMaps = new HeightMap[needTextures][,];
            }

            void GenStandart() {
                float startMass = 65536;
                float startSize = 65536;
                //���� �������� ����
                if (parent != null)
                {
                    //������ ������������ ����� � ������ �� 2 ������� ����
                    startMass = parent.mass / 4;
                    startSize = (int)parent.size - 2;
                }

                //��������� ������ � ���
                color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));

                //������
                int sizePower = (int)(((randSize * 1000) % startSize - 3) + 3); //������ ������ 7-�� ������� �� �����
                this.size = (Size)sizePower;
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
        }

        public override HeightMap[,] GetHeightMap(Size quarity)
        {
            int q = (int)quarity;

            //������� ������ ���� �� ����
            if (heightMaps[q] == null) {
                //������������ � ����������� ������ � ������ ��������
                int height = Calc.GetSizeInt(quarity);
                int width = height * 2;

                int chankXMax = width / Chank.Size;
                int chankYMax = height / Chank.Size;

                heightMaps[q] = new HeightMap[chankXMax, chankYMax];
            }


            //��������� ��� ����� ����� ������
            //���� ��������� ����� ����� �� ���������� � ����� ������
            //����������
            for (int x = 0; x < heightMaps[q].GetLength(0); x++) {
                for (int y = 0; y < heightMaps[q].GetLength(1); y++) {
                    if (heightMaps[q][x, y] != null)
                        continue;

                    heightMaps[q][x, y] = new HeightMap(this, quarity, new Vector2Int(x, y));

                    //���������� ����� ���������
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
