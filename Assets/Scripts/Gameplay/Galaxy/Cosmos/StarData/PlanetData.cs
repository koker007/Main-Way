using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    public class PlanetData : ObjData, IGetMainTexture
    {
        //����� 
        //����� �����
        HeightMap[][,] heightMaps;
        //������ ������
        BiomeMaps[][,] biomesMaps;
        int[][,] biomeNum;

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

        public Texture2D GetMainTexture(Size quality)
        {
            TextureMaps ??= new Texture2D[(int)size];

            //��������� ��� �������� ����
            if (TextureMaps[(int)quality] != null)
            {
                return TextureMaps[(int)quality];
            }
            Debug.Log("PlanetSize " + quality);

            //������� ������
            int q = (int)quality;


            int sizeTexture = Calc.GetSizeInt(size) / Calc.GetSizeInt(quality);

            //������������ � ����������� ������ � ������ ��������
            int chankXMax = (sizeTexture * 2) / Chank.Size;
            int chankYMax = (sizeTexture) / Chank.Size;

            TextureMaps[q] ??= new Texture2D(sizeTexture * 2, sizeTexture);

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

            //���������� �����
            biomesMaps[q][chankPos.x, chankPos.y] ??= new BiomeMaps(this, quality, chankPos, 3);

            //����� ���� ������ ������� ��������

            //������ ����� ����� ����������
            int[,] biomeNum = new int[biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(0), biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1)];
            float[,] biomeZeroPower = new float[biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(0), biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1)];

            //���������� ��� ����� � � ����������� �� ���� ����� �������� ���� ��������
            for (int x = 0; x < biomesMaps[q][chankPos.x,chankPos.y].maps.GetLength(0); x++) {
                for (int y = 0; y < biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(1); y++) {

                    for (int arrayNum = 0; arrayNum < biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(2); arrayNum++) {

                        //���� ���� ����� ������
                        if (biomeZeroPower[x,y] > biomesMaps[q][chankPos.x, chankPos.y].maps[x, y, arrayNum])
                            continue;

                        //���������� ���� �������� �����
                        biomeZeroPower[x,y] = biomesMaps[q][chankPos.x, chankPos.y].maps[x, y, arrayNum];
                        //����������� ����� ����� ����������
                        biomeNum[x,y] = arrayNum;

                    }
                }
            }

            //��������� ��������
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
                        TextureMaps[(int)quality].SetPixel(globalX, globalY, new Color(biomeZeroPower[x, y], biomeZeroPower[x, y], biomeZeroPower[x, y], 1));
                    }
                    else
                        TextureMaps[(int)quality].SetPixel(globalX, globalY, colors[biomeNum[x, y]]);
                }
            }

            

        }

        void iniBiomeMaps() {
            biomesMaps ??= new BiomeMaps[(int)size][,];



            for (int qualityNum = 0; qualityNum < biomesMaps.Length; qualityNum++) {
                if (biomesMaps[qualityNum] != null)
                    continue;

                int sizeBiome = Calc.GetSizeInt(size) / Calc.GetSizeInt((Size)qualityNum+1);

                //������������ � ����������� ������ � ������ ��������
                int chankXMax = (sizeBiome * 2) / Chank.Size;
                int chankYMax = (sizeBiome) / Chank.Size;

                biomesMaps[qualityNum] = new BiomeMaps[chankXMax, chankYMax];
            }
        }

        static public PlanetData GetRandomPlanet() {

            PlanetData result = new PlanetData(GalaxyCtrl.galaxy.cells[0,0,0]);
            result.GenData(null, Random.RandomRange(0, 1.0f));

            return result;
        }
    }


}
