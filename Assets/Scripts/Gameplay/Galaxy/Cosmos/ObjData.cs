using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos
{
    //������������ ����� ���� ����������� ��������
    public abstract class ObjData
    {
        //���������� �����
        public SpaceObjCtrl visual;

        public CellS cell; //������ ��������

        public ObjData parent; //����������� ������ - ��������
        public List<ObjData> childs; //����

        /// <summary>
        /// ������ �������
        /// </summary>
        public Size size;
        
        public int radiusOrbit;
        public int radiusChildZone;
        public int radiusGravity;
        public int radiusVoid;

        private float locPerlin = 0;
        public float[,,] perlin; //������

        public Color color;
        public float time360Rotate; //����� ������ ������� �������
        public float bright; //�������
        public float mass; //�����
        public float atmosphere; //��������� ���������

        //��� ����������
        protected float randMass = 0;
        protected float randSize = 0;


        public virtual void GenData(ObjData parent, float perlin)
        {
            this.parent = parent;
            this.locPerlin = perlin;

            randMass = 0;
            randSize = 0;

            //���� �������� ����
            if (parent != null)
            {
                //�������� ����� � ������
            }

            randMass += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[0]));
            randSize += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[1]));

            randMass = Mathf.Abs(randMass);
            randSize = Mathf.Abs(randSize);

            randMass += perlin;
            randMass /= 2;

            randSize += perlin;
            randSize /= 2;

        }

        /// <summary>
        /// �������� ���� ����� ����� ������� ����
        /// </summary>
        /// <returns></returns>
        public abstract HeightMap[,] GetHeightMap(Size quarity);

        /// <summary>
        /// �������� ������ �� ������ �������
        /// </summary>
        /// <param name="indexMax512"></param>
        /// <returns></returns>
        public float GetPerlinFromIndex(int indexMax512)
        {
            if (perlin == null) {
                GenPerlinLoc(locPerlin);
            }

            int perlinX = indexMax512 % this.perlin.GetLength(0);
            int perlinY = indexMax512 / this.perlin.GetLength(0);
            perlinY = perlinY % this.perlin.GetLength(1);
            int perlinZ = indexMax512 / (this.perlin.GetLength(0) * this.perlin.GetLength(1));

            return perlin[perlinX, perlinY, perlinZ];
        }


        //������������� ������ ��� ������������ �������
        public void GenPerlinLoc(float perlinGen)
        {
            //

            //��������� ������ 
            float scale = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen) % 5 + 1;
            float freq = 1;
            float offsetX = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 10));
            float offsetY = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 100));
            float offsetZ = Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 1000));
            int octaves = (int)Calc.GetSeedNum(cell.galaxy.Seed, (int)(perlinGen * 10000)) % 6 + 1; //�������� �� 6 �����

            GraficData.Perlin dataPerlinLoc = new GraficData.Perlin(scale, freq, offsetX, offsetY, offsetZ, octaves, false);
            dataPerlinLoc.Calculate();
            perlin = dataPerlinLoc.result;

        }

        //������������� �����, �������, ���� � ��. �� ������ ���������� ������������
        public void GenChilds(float distGenMax, float perlin)
        {
            //������� ������ ������
            childs = new List<ObjData>();

            //���� ��������� ��� ��������� ������ ������� ����, �������
            if (distGenMax < 512)
                return;


            //��������� ��������� ������ �� ������ �����������
            GenPerlinLoc((perlin * 1000) % 1);

            int sizeInt = Calc.GetSizeInt(size);
            //������� ��������� ���������
            //���������� ������ �������
            int distGenNow = sizeInt;

            //������������ � ������������ ����������� ���������
            int childMaximum;
            if (parent == null)
                childMaximum = (int)(512 * perlin);
            else childMaximum = (int)(10 * perlin);

            //������� ������� ���� �� ���������� ������������ ��� ������ ����� ������� ������
            int numTryAdd = 0;
            while (distGenNow < distGenMax && childs.Count < childMaximum && numTryAdd < 512)
            {

                //��������� ������������ ��� ��������� ���
                float distFree = distGenMax - distGenNow;
                AddPlanet(distFree);
            }

            //���������� ����� ����� ������� � ��� �����
            for (int num = 0; num < childs.Count; num++)
            {
                childs[num].GenChilds(childs[num].radiusChildZone, GetPerlinFromIndex(num));
            }

            void AddPlanet(float distFree)
            {
                //������� �������
                ObjData objData = new PlanetData(cell);
                childs.Add(objData);

                int index = childs.Count - 1;

                //�������������� ��
                childs[index].GenData(this, GetPerlinFromIndex(index));

                //����� ������� ����������������� (���� ����� � ������) ���������� ��������� ���������
                distGenNow += childs[index].radiusGravity + childs[index].radiusVoid;


            }
        }
    }

    public enum TidalLocking
    {
        No,
        Yes
    }

    public class HeightMap
    {
        public const float factor = 0.875170906246f;

        static public float timeLastGen = 0;

        //����� ������ ������ ������� � ������
        int sizePixel = 65536;
        public float[,] map;

        //������������� �����
        public HeightMap(ObjData data, Size sizeTexture)
        {

            //������� �������� ���������� ����������
            //������ ������������ � ��������� ����� ������
            int height = Calc.GetSizeInt(sizeTexture);
            int width = height * 2;

            //�������� ������ �������
            int sizePlanet = Calc.GetSizeInt(data.size);

            //������ ������� ������ � ����� �������
            this.sizePixel = sizePlanet / height;

            //����� �������� �� ������
            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

            float scale = 1.0f / this.sizePixel * 8000;

            map = GenMap(width, height, scale, scale, scale, 5, offsetX, offsetY, offsetZ, 5, false, false);
        }

        //������������� ������� ����� �������� 32 �� 32
        public HeightMap(ObjData data, Size sizeTexture, Vector2Int partPos) {
            //������� �������� �������� 32 32 = 1024 ��������

            //������� ������ ���� ����������� �������
            int height = Calc.GetSizeInt(sizeTexture);
            int width = height * 2;

            //�������� ������ �������
            int sizePlanet = Calc.GetSizeInt(data.size);

            //������ ������� ������ � ����� �������
            this.sizePixel = sizePlanet / height;

            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), Mathf.Pow(data.GetPerlinFromIndex(105), data.GetPerlinFromIndex(373))) * 1000;
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), Mathf.Pow(data.GetPerlinFromIndex(281), data.GetPerlinFromIndex(255))) * 1000;
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), Mathf.Pow(data.GetPerlinFromIndex(110), data.GetPerlinFromIndex(304))) * 1000;

            float sizeContinent =  0.75f + (float)((data.GetPerlinFromIndex(159) * 1000) % 0.5f);

            float scale = (sizePlanet * sizeContinent) / sizePixel;

            map = GenPart(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 10, false, false, partPos.x, partPos.y);
        }

        float[,] GenMap(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ) {
            //������� ����� ������
            float[,] arrayMap = new float[mapSizeX, mapSizeY];

            //���� ������ �����
            float FactorChankX = (factor / ScaleX) * Chank.Size;
            float FactorChankY = (factor / ScaleY) * Chank.Size;
            float FactorChankZ = (factor / ScaleZ) * Chank.Size;

            //���������� ���������� ������ � ���������� ������� ���� �� ����
            int chankXMax = mapSizeX / Chank.Size;
            int chankXremain = mapSizeX % Chank.Size;
            if (chankXremain > 0)
                chankXMax++;

            int chankYMax = mapSizeY / Chank.Size;
            int chankYremain = mapSizeY % Chank.Size;
            if (chankYremain > 0)
                chankYMax++;

            //���������� ������ ����
            for (int chankX = 0; chankX < chankXMax; chankX++)
            {
                int chankPixelStartX = chankX * Chank.Size;

                for (int chankY = 0; chankY < chankYMax; chankY++)
                {
                    int chankPixelStartY = chankY * Chank.Size;

                    float offSetX = OffSetX + FactorChankX * chankX;
                    float offSetY = OffSetY + FactorChankY * chankY;
                    float offSetZ = OffSetZ;

                    if (TimeZ)
                        offSetZ += Time.time * 0.1f;

                    if (TimeX)
                        offSetX += Time.time * 0.1f;

                    float[,] partMap = GenPart(mapSizeX, mapSizeY, ScaleX, ScaleY, ScaleZ, Freq, offSetX, OffSetY, OffSetZ, Octaves, TimeX, TimeZ, chankX, chankY);

                    //���� ������� ���� � ��������
                    int maxX = 32;
                    int maxY = 32;
                    if (chankX == chankXMax - 1 && chankXremain > 0)
                        maxX = chankXremain;
                    if (chankY == chankYMax - 1 && chankYremain > 0)
                        maxY = chankYremain;

                    //���������� ������ � ��������
                    for (int x = 0; x < maxX; x++)
                    {
                        int posMapX = chankPixelStartX + x;
                        for (int y = 0; y < maxY; y++)
                        {
                            int posMapY = chankPixelStartY + y;
                            arrayMap[posMapX, posMapY] = partMap[x, y];
                        }
                    }
                }
            }
            return arrayMap;
        }
        float[,] GenPart(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ, int chankX, int chankY)
        {
            //���������� ���������� ������
            int chankXMax = mapSizeX / Chank.Size;
            int chankXremain = mapSizeX % Chank.Size;
            if (chankXremain > 0)
                chankXMax++;

            int chankYMax = mapSizeY / Chank.Size;
            int chankYremain = mapSizeY % Chank.Size;
            if (chankYremain > 0)
                chankYMax++;

            //���� ��������� ���� �� ���������� - ����������
            if (chankX < 0 ||
                chankX >= chankXMax ||
                chankY < 0 ||
                chankY >= chankYMax)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            //������� ����� ������
            float[,] arrayPart = new float[Chank.Size, Chank.Size];

            //���� ������ �����
            float FactorChankX = (factor / ScaleX) * Chank.Size;
            float FactorChankY = (factor / ScaleY) * Chank.Size;
            float FactorChankZ = (factor / ScaleZ) * Chank.Size;

            float offSetX = OffSetX + FactorChankX * chankX;
            float offSetY = OffSetY + FactorChankY * chankY;
            float offSetZ = OffSetZ;

            if (TimeZ)
                offSetZ += Time.time * 0.1f;

            if (TimeX)
                offSetX += Time.time * 0.1f;

            float regionX = (chankX * Chank.Size) / (float)mapSizeX;
            float regionY = (chankY * Chank.Size) / (float)mapSizeY;

            GraficData.Perlin2D dataPerlin2D = new GraficData.Perlin2D(ScaleX, ScaleY, ScaleZ, Freq, offSetX, offSetY, offSetZ, Octaves, mapSizeX, mapSizeY, regionX, regionY);
            dataPerlin2D.Calculate();

            //���������� �������� ������ � ������
            for (int x = 0; x < Chank.Size; x++)
            {
                for (int y = 0; y < Chank.Size; y++)
                {
                    arrayPart[x, y] = dataPerlin2D.result[x, y, 0];
                }
            }
            return arrayPart;
        }
    }
}
