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
                int width = height;

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
            Debug.Log("PlanetSize " + size);

            //������� ������
            int q = (int)quality;


            int sizeTexture = Calc.GetSizeInt(size) / Calc.GetSizeInt(quality);

            //������������ � ����������� ������ � ������ ��������
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

            //���������� �����
            biomesMaps[q][chankPos.x, chankPos.y] ??= new BiomeMaps(this, quality, chankPos, 6);

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
            //���� ��� �������� - �������
            if (textureShadow == null) {
                //�������� �������� ������� �� ������� �������
                int size = Calc.GetSizeInt(this.size) / 32;

                textureShadow = new Texture2D(size * 2, size);
                List<StarData> stars = cell.Stars;

                //���������� �� ������� �������
                for (int pixX = 0; pixX < textureShadow.width; pixX++)
                {
                    float pixCoofX = (float)pixX / (float)textureShadow.width;

                    for (int pixY = 0; pixY < textureShadow.height; pixY++)
                    {
                        float pixCoofY = (float)pixY / (float)textureShadow.height;

                        float lightSum = 0;
                        //���������� ��� ������� ���� �� ���������
                        foreach (StarData star in stars) {
                            //��������� ����������� ����� �� �������
                            Vector3 vecLight = visual.transform.position - star.visual.transform.position;
                            float distLight = vecLight.magnitude;
                            vecLight.Normalize();

                            //���������� ������� ������� ������������ ������
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
                    //��������� ������������ �������
                    //���� ��������� ������ 0.5 �� ��� ������ ������� ������
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

                    //������ ��������� �� ��������� �������
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

                //������������ � ����������� ������ � ������ ��������
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
