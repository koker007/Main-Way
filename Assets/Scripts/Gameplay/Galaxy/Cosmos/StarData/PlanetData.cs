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

        public PatternPlanet pattern;

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

        public override HeightMap[,] GetHeightMaps(Size quarity)
        {
            iniHeightMaps();

            int q = (int)quarity -1;


            //��������� ��� ����� ����� ������
            //���� ��������� ����� ����� �� ���������� � ����� ������
            //����������
            for (int x = 0; x < heightMaps[q].GetLength(0); x++) {
                for (int y = 0; y < heightMaps[q].GetLength(1); y++) {
                    if (heightMaps[q][x, y] != null)
                        continue;

                    GenHeightMap(quarity, new Vector2Int(x,y));
                }
            }

            return heightMaps[q];
        }

        /// <summary>
        /// �������� ����� ����� ���������� ����� - ���� ����, ���������
        /// </summary>
        /// <param name="quarity"></param>
        /// <param name="chankPos"></param>
        /// <returns></returns>
        public HeightMap GetHeightMap(Size quarity, Vector2Int chankPos) {
            iniHeightMaps();

            int q = (int)quarity - 1;

            //���� ����� ����� ���� ���������� ��
            if (heightMaps[q][chankPos.x, chankPos.y] != null)
                return heightMaps[q][chankPos.x, chankPos.y];

            //����������
            GenHeightMap(quarity, chankPos);

            return heightMaps[q][chankPos.x, chankPos.y];
        }

        /// <summary>
        /// ���������������� ����� ����� ���������� �����
        /// </summary>
        /// <param name="quarity"></param>
        /// <param name="chankPos"></param>
        public void GenHeightMap(Size quarity, Vector2Int chankPos) {
            int q = (int)quarity - 1;

            heightMaps[q][chankPos.x, chankPos.y] = new HeightMap(this, quarity, new Vector2Int(chankPos.x, chankPos.y));

            //���������� ����� ���������
            SpaceObjMap.timeLastGen = Time.time;
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
            int chankYMax = sizeTexture / Chank.Size;
            int chankXMax = chankYMax * 2;

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
            iniHeightMaps();
            iniBiomeMaps();

            int q = (int)quality - 1;

            //������� ����� ��������
            BiomeTypeSurface[] biomeDatas = new BiomeTypeSurface[6];
            for (int num = 0; num < biomeDatas.Length; num++) {
                biomeDatas[num] = new BiomeTypeSurface();
                if (num == 0) {
                    biomeDatas[num].seaPriority = BiomeData.SeaPriority.onlyOverSea;
                    biomeDatas[num].coofHeight = 0.5f;
                    biomeDatas[num].coofPolus = 0.2f;
                    //biomeDatas[num].coofZeroX = 0.5f;
                }
            }

            //������� ����� ����� ���� ���
            heightMaps[q][chankPos.x, chankPos.y] ??= new HeightMap(this, quality, chankPos);

            //���������� �����
            biomesMaps[q][chankPos.x, chankPos.y] ??= new BiomeMaps(this, quality, chankPos, biomeDatas, heightMaps[q][chankPos.x, chankPos.y]);

            //����� ���� ������ ������� ��������

            //������ ����� ����� ����������
            int[,] biomeWinerNum = biomesMaps[q][chankPos.x, chankPos.y].winers;
            float[,] biomeIntensive = biomesMaps[q][chankPos.x, chankPos.y].winersIntensive;

            //��������� ��������
            TextureMaps ??= new Texture2D[(int)size];

            int sizeTexture = Calc.GetSizeInt(size) / Calc.GetSizeInt(quality);
            

            TextureMaps[(int)quality] ??= new Texture2D(sizeTexture * 2, sizeTexture);

            Color[] colors = new Color[10];
            for (int num = 0; num < colors.GetLength(0); num++) {
                colors[num] = Color.black;

                if (num == 0)
                    colors[num] = Color.white;
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
                    colors[num] = Color.yellow;

            }

            for (int x = 0; x < biomeWinerNum.GetLength(0); x++) {
                int globalX = biomeWinerNum.GetLength(0) * chankPos.x + x;
                for (int y = 0; y < biomeWinerNum.GetLength(1); y++)
                {
                    int globalY = biomeWinerNum.GetLength(1) * chankPos.y + y;

                    if (biomesMaps[q][chankPos.x, chankPos.y].maps.GetLength(2) == 1)
                    {
                        float intensive = biomeIntensive[x, y];
                        if (intensive * 20 % 2 > 1)
                            intensive = 1;
                        else
                            intensive = 0;

                        TextureMaps[(int)quality].SetPixel(globalX, globalY, new Color(intensive, intensive, intensive, 1));
                    }
                    else
                    {
                        Color color = colors[biomeWinerNum[x, y]] * (heightMaps[q][chankPos.x, chankPos.y].map[x, y] * 2 - 0.5f);
                        if (heightMaps[q][chankPos.x, chankPos.y].map[x, y] < 0.5f)
                            color = Color.blue * (heightMaps[q][chankPos.x, chankPos.y].map[x, y] * 2 - 0.5f);

                        TextureMaps[(int)quality].SetPixel(globalX, globalY, new Color(color.r, color.g, color.b));
                    }
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
        void iniHeightMaps() {
            heightMaps ??= new HeightMap[(int)size][,];

            for (int qualityNum = 0; qualityNum < heightMaps.Length; qualityNum++) {
                if (heightMaps[qualityNum] != null)
                    continue;

                int sizeMaps = Calc.GetSizeInt(size) / Calc.GetSizeInt((Size)qualityNum + 1);

                //������������ � ����������� ������ � ������ ��������
                int chankXMax = (sizeMaps * 2) / Chank.Size;
                int chankYMax = (sizeMaps) / Chank.Size;

                heightMaps[qualityNum] = new HeightMap[chankXMax, chankYMax];
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

        /// <summary>
        /// �������� ��������� ���� �����
        /// </summary>
        /// <param name="quality"></param>
        /// <param name="chankPos"></param>
        /// <returns></returns>
        public BiomeMaps GetBiomePart(Size quality, Vector2Int chankPos) {
            iniHeightMaps();
            iniBiomeMaps();

            int q = (int)quality - 1;

            if (pattern == null)
                SetTestPattern();

            //������� ����� ����� ���� ���
            heightMaps[q][chankPos.x, chankPos.y] ??= new HeightMap(this, quality, chankPos);

            //���������� �����
            biomesMaps[q][chankPos.x, chankPos.y] ??= new BiomeMaps(this, quality, chankPos, pattern.biomesSurface.ToArray(), heightMaps[q][chankPos.x, chankPos.y]);

            return biomesMaps[q][chankPos.x, chankPos.y];
        }

        void SetTestPattern() {
            pattern = PatternPlanet.GetTestPattern();
            Debug.Log("Use a test planet pattern");
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
