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
        /// �������� �������� ����������� ����, ���������� ��������
        /// </summary>
        /// <param name="quarity"></param>
        /// <returns></returns>
        public abstract Texture2D GetMainTexture(Size quality);
        /// <summary>
        /// �������� ����� ����� ������� ����
        /// </summary>
        /// <returns></returns>
        public abstract float[,] GetHeightMap();

        /// <summary>
        /// �������� ������ �� ������ �������
        /// </summary>
        /// <param name="indexMax512"></param>
        /// <returns></returns>
        public float GetPerlinFromIndex(int indexMax512)
        {

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
            float offsetX = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10);
            float offsetY = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 100);
            float offsetZ = Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 1000);
            int octaves = (int)Calc.GetSeedNum(cell.galaxy.Seed, (int)perlinGen * 10000) % 6 + 1; //�������� �� 6 �����

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
            GenPerlin();

            void GenPerlin()
            {
                float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
                float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
                float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

                float scale = 1.0f / this.sizePixel * 4000;

                map = GraficData.Perlin2D.GetArrayMap(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 1, false, false);

            }
        }
    }
}
