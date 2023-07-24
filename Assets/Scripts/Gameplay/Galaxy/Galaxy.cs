using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;


//����� ����������
public class Galaxy {
    public readonly string Seed;

    public CellS[,,] cells;

    GraficData.Perlin[,,] Perlins;

    Size size;

    public Galaxy(Size size, string seedFunc) {
        Debug.Log("Create New galaxy: " + seedFunc);

        //������ ������� ������ ���������
        GalaxyCtrl.main.GalaxyClear();

        //���������� �����
        GalaxyCtrl.galaxy = this;

        Seed = seedFunc;

        //������� ������ ����� ���������
        cells = new CellS[(int)size,(int)size/4, (int)size];

        int perlinMaxX = cells.GetLength(0) / 8;
        int perlinMaxY = cells.GetLength(1) / 8;
        int perlinMaxZ = cells.GetLength(2) / 8;

        if (cells.GetLength(0) % 8 > 0)
            perlinMaxX++;
        if (cells.GetLength(1) % 8 > 0)
            perlinMaxY++;
        if (cells.GetLength(2) % 8 > 0)
            perlinMaxZ++;

        //�������� ��� �� ������� ���������
        Perlins = new GraficData.Perlin[perlinMaxX, perlinMaxY, perlinMaxZ];

        float OffSetX = Calc.GetSeedNum(Seed+ Seed[0], Seed[0] * Seed[1]);
        float OffSetY = Calc.GetSeedNum(Seed+ Seed[1], Seed[1] * Seed[2]);
        float OffSetZ = Calc.GetSeedNum(Seed+ Seed[2], Seed[2] * Seed[0]);

        for (int x = 0; x < perlinMaxX; x++) {
            for (int y = 0; y < perlinMaxY; y++) {
                for (int z = 0; z < perlinMaxZ; z++) {
                    Perlins[x, y, z] = new GraficData.Perlin(16f, 1, x - OffSetX, y + OffSetY, z + OffSetZ, 5, true);
                    Perlins[x, y, z].Calculate();
                }
            }
        }
        //�������� ���


        //����� ��������� ������� ������ ������
        for (int x = 0; x < cells.GetLength(0); x++) {
            for (int y = 0; y < cells.GetLength(1); y++) {
                for (int z = 0; z < cells.GetLength(2); z++) {
                    GenerateCell(new Vector3Int(x,y,z));
                }
            }
        }

        
    }

    public CellS GenerateCell(Vector3Int pos) {
        int Px = pos.x / 8;
        int Py = pos.y / 8;
        int Pz = pos.z / 8;

        int Lx = pos.x % 8;
        int Ly = pos.y % 8;
        int Lz = pos.z % 8;

        //�������� ���
        float noise = Perlins[Px, Py, Pz].result[Lx, Ly, Lz];

        //������� ������
        cells[pos.x, pos.y, pos.z] = new CellS(pos, noise, this);
        return cells[pos.x, pos.y, pos.z];
    }


    public enum Size {
        cells15 = 15,
        cells31 = 31,
        cells61 = 61
    }
}




//����� ����������� ������
public class CellS
{
    //������ ����� ������ ����������
    public const int size = 1000000;
    public const int sizeZone = 1000; //������� ���� �����
    public const int sizeVisual = 1000;

    public Galaxy galaxy;
    public Vector3 pos; //������� ������ (�����) � ������������ ������������ ������� (�������)

    public ObjData mainObjs; //������� ������ �� ���� �������� ���������� �������� �������
    public GalaxyObjCtrl visual; //���������� ����� �������� �������

    List<StarData> StarsInCell;
    public List<StarData> Stars { get {
            return StarsInCell ?? new List<StarData>();
        } }

    float perlinGlobal; //���������� ������ ��������� �������� ������������� ���������
    public float perlinGlob {
        get { return perlinGlobal; }
    }

    //����������� ������
    public CellS(Vector3 pos, float perlin, Galaxy galaxy) {
        this.galaxy = galaxy;
        this.pos = pos;

        StarsInCell = new List<StarData>();

        //���������� ���������� ������ ���� ������
        perlinGlobal = perlin;

        genMainObj();

    }

    //������������� ������ ������
    public void genMainObj() {
        if (mainObjs != null) 
            return;

        //����� �� ���������� ���������
        if (!isGenFormGalaxy()) 
            return;

        //��� ���� �������� ��� ������ � ������������ ���� ������ ���������� �� ���������� � ������
        mainObjs = new StarData(this);
        mainObjs.GenData(null, perlinGlobal);

        //����� ��� ������ ��������� ������� �����������
        //���������� ������� ������ ������
        pos = GetPos();

        bool isGenFormGalaxy() {
            float shance = perlinGlobal;
            int centerX = galaxy.cells.GetLength(0) / 2;
            int centerY = galaxy.cells.GetLength(1) / 2;
            int centerZ = galaxy.cells.GetLength(2) / 2;

            //�������� ������ �� ������
            Vector3 offset = new Vector3(pos.x - centerX, pos.y - centerY, pos.z - centerZ);

            float distDisc = new Vector3(offset.x, offset.y * 8, offset.z).magnitude;
            float distCentr = new Vector3(offset.x * 2, offset.y * 4, offset.z * 2).magnitude;

            float coofDis� = distDisc / centerX;
            float coofCentr = distCentr / centerX;

            if (distCentr < centerX)
                shance += (1 - coofCentr);
            else if (coofDis� < 1)
                shance += 0.05f;
            else if (coofDis� > 1)
                shance += (1 - coofDis�) * 0.3f;

            if (shance > 0.7f)
                return true;

            return false;
        }
        bool isGenPerlin() {
            Debug.Log(pos + " P " + perlinGlobal );

            if (perlinGlobal > 0.7f)
                return true;

            return false;
        }


        Vector3 GetPos() {
            Vector3 result = pos;
            //��������� ������� �� ������ ����

            float num = (int)pos.x * galaxy.cells.GetLength(1) * galaxy.cells.GetLength(2) + (int)pos.y * galaxy.cells.GetLength(2) + (int)pos.z;

            float numX = (perlinGlobal * 10 % 1.3f) * (perlinGlobal * 100 % 1.174f);
            float numY = (perlinGlobal * 100 % 1.53f) * (perlinGlobal * 1000 % 1.472f);
            float numZ = (perlinGlobal * 1000 % 1.124f) * (perlinGlobal * 10000 % 1.854f);

            float randX = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numX)) % 0.5f;
            float randY = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numY)) % 0.5f;
            float randZ = Calc.GetSeedNum(galaxy.Seed, (int)(num * perlinGlobal * numZ)) % 0.5f;

            result += new Vector3(0.5f, 0.5f, 0.5f); //������������ �� ������ ������

            //������������
            result.x += randX;
            result.y += randY;
            result.z += randZ;

            //Debug.Log(rand);

            return result;
        }
    }

    //������������� ������� � ������
    public void genPlanets() {

        //���� �������� ������� ��� ������������ �������
        if (mainObjs == null)
            return;

        //���� ���� ��� ����, ��������� �� ���������
        if (mainObjs.childs != null)
            return;

        //������ ��������� ��������� ��� ��������� ������
        float distGenMax = GetDistToGen();

        //�������� ������ �������� ����� � ��������
        distGenMax *= CellS.size; 

        //��������� ��������� ������ ������������ ������� ������
        mainObjs.GenChilds(distGenMax, perlinGlobal);

        iniAllStars(); //��������� ��� ������� � ������


        Debug.Log("Cell Gen: " + pos + " Planets:" + mainObjs.childs.Count);

        float GetDistToGen()
        {
            //����� ���������� �� ������ �������� �� ���� ������� ���������� � ������� �� ��� ���������������� � ��������

            //������ ��������� �� ������ �� ������� �������� ������
            Vector3 dist = new Vector3();
            dist.x = pos.x % 1;
            dist.y = pos.y % 1;
            dist.z = pos.z % 1;

            dist -= new Vector3(0.5f, 0.5f, 0.5f);

            //�������� �������� �� ������ �� ������
            float distPlanetMax = 1;
            dist.x = Mathf.Abs(dist.x);
            dist.y = Mathf.Abs(dist.y);
            dist.z = Mathf.Abs(dist.z);

            //�������� ��������� �� ������� ������
            dist.x = 0.5f - dist.x;
            dist.y = 0.5f - dist.y;
            dist.z = 0.5f - dist.z;

            if (distPlanetMax > dist.x)
                distPlanetMax = dist.x;
            if (distPlanetMax > dist.y)
                distPlanetMax = dist.y;
            if (distPlanetMax > dist.z)
                distPlanetMax = dist.z;

            //������������ ��������� ��� ������ ������ ��������
            Debug.Log("Generate Planets, Cell:" + pos + " Gen dist" + distPlanetMax);

            return distPlanetMax;
        }

        void iniAllStars() {

            //���� � ������ ���� ���-�� �������
            if (StarsInCell != null || mainObjs == null)
                return;

            //��������� ������
            StarsInCell = new List<StarData>();

            //��������� ������ � �� �����
            TestStar(mainObjs);

            void TestStar(ObjData objData) {
                TestAdd(objData);

                foreach (ObjData child in mainObjs.childs) {
                    TestStar(child);
                }
            }

            void TestAdd(ObjData objData) {
                StarData starData = objData as StarData;

                if (starData == null)
                    return;

                StarsInCell.Add(starData);
            }

        }
    }

    public void VisualMainObj() {
    
    }


}


//������ ������ ������� � �����������
public class SpaceObjMap {
    static public float timeLastGen = 0;

    //����� ������ ������ ������� � ������
    int sizePixel = 65536;
    public float[,] map;

    //�������� �������
    public Texture2D texture;

    //������������� ��������
    public SpaceObjMap(ObjData data, Size sizeTexture) {

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

        CreateTexture();

        void GenPerlin() {
            float offsetX = Mathf.Pow(data.GetPerlinFromIndex(130), data.GetPerlinFromIndex(105) + data.GetPerlinFromIndex(373));
            float offsetY = Mathf.Pow(data.GetPerlinFromIndex(333), data.GetPerlinFromIndex(281) + data.GetPerlinFromIndex(255));
            float offsetZ = Mathf.Pow(data.GetPerlinFromIndex(304), data.GetPerlinFromIndex(110) + data.GetPerlinFromIndex(304));

            float scale = 1.0f / this.sizePixel * 4000;

            map = GraficData.Perlin2D.GetArrayMap(width, height, scale, scale, scale, 2, offsetX, offsetY, offsetZ, 1, false, false);

        }
        
        void CreateTexture() {

            texture = new Texture2D(map.GetLength(0), map.GetLength(1) * 2);

            //��������� �������� �������� - ������ ��������
            for (int x = 0; x < map.GetLength(0); x++) {
                for (int y = 0; y < map.GetLength(1); y++) {
                    Color color = new Color(map[x, y], map[x, y], map[x, y]);
                    //if (map[x, y] < 0.1f)
                    //    color = Color.blue;

                    texture.SetPixel(x,y, color);
                }
            }
            //��������� ������������� �������� ������� ��������
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {

                    //�� � ������� �� ��������
                    int mirrorX = x - (map.GetLength(0)/2);
                    //�� � �������������
                    int mirrorY = map.GetLength(1)-1 - y;

                    if (mirrorX < 0)
                        mirrorX += map.GetLength(0);

                    Color color = texture.GetPixel(mirrorX, mirrorY);

                    texture.SetPixel(x, y + map.GetLength(1), color);
                }
            }


            texture.anisoLevel = 0;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;

            texture.Apply();
        }
    }
}

//
public enum Size
{
    s1 = 1,
    s2 = 2,
    s4 = 3,
    s8 = 4,
    s16 = 5,
    s32 = 6,
    s64 = 7,
    s128 = 8,
    s256 = 9,
    s512 = 10,
    s1024 = 11,
    s2048 = 12,
    s4096 = 13,
    s8192 = 14,
    s16384 = 15,
    s32768 = 16,
    s65536 = 17
}

