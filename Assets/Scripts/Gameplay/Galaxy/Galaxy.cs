using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public SpaceObjData mainObjs; //������� ������ �� ���� �������� ���������� �������� �������
    public GalaxyObjCtrl visual; //���������� ����� �������� �������

    float perlinGlobal; //���������� ������ ��������� �������� ������������� ���������
    public float perlinGlob {
        get { return perlinGlobal; }
    }

    //����������� ������
    public CellS(Vector3 pos, float perlin, Galaxy galaxy) {
        this.galaxy = galaxy;
        this.pos = pos;



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
        mainObjs = new SpaceObjData(this);
        mainObjs.GenObjData(null, perlinGlobal,0);

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
        mainObjs.GenPlanets(distGenMax, perlinGlobal);
        
        Debug.Log("Cell Gen: " + pos + " Planets:" + mainObjs.childs.Length);

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
    }

    public void VisualMainObj() {
    
    }


}










//����� ������������ ������� (������ ��� ������� ��� ����)
public class SpaceObjData {
    //���������� �����
    public SpaceObjCtrl visual;

    public CellS cell; //������ ��������
    public SpaceObjData parent; //����������� ������ - ��������
    public SpaceObjData[] childs; //����
    public int radiusOrbit;
    public int radiusChildZone;
    public int radiusGravity;
    public int radiusVoid;

    public float[,,] perlin; //������ �������

    public Color color;
    public float time360Rotate; //����� ������ ������� �������
    public float bright; //�������
    public float mass; //�����
    public float atmosphere; //��������� ���������
    public float liting;

    /// <summary>
    /// ������� ��������� �������
    /// </summary>
    public PatternPlanet patternPlanet;
    //�������� �������� �������
    public SpaceObjMap[] MainTextures;
    

    public Size size;
    public TidalLocking tidalLocking;

    public enum TidalLocking
    {
        No,
        Yes
    }

    public SpaceObjData(CellS cell) {
        this.cell = cell;

        
    }

    public float GetPerlinFromIndex(int indexMax512) {

        int perlinX = indexMax512 % this.perlin.GetLength(0);
        int perlinY = indexMax512 / this.perlin.GetLength(0);
        perlinY = perlinY % this.perlin.GetLength(1);
        int perlinZ = indexMax512 / (this.perlin.GetLength(0) * this.perlin.GetLength(1));

        return perlin[perlinX, perlinY, perlinZ];
    }

    //������������� ����������� ������ �� ������ ������� �������� ��� ��� �������� ���� ������ �������� ����������� � ������
    public void GenObjData(SpaceObjData parent, float perlin, float distanceChildFree) {
        GenObjData(parent, perlin, distanceChildFree, null);
    }
    public void GenObjData(SpaceObjData parent, float perlin, float distanceChildFree, PatternPlanet patternPlanet) {
        this.parent = parent;

        //����� ������������ � ������������� ���������� ����� � �������, ��� 2 ��������� ���������� ��� �������
        float startMass = 65536;
        float startSize = 65536;
        float randMass = 0;
        float randSize = 0;


        //���� �������� ����
        if (parent != null) {
            //������ ������������ ����� � ������ �� 2 ������� ����
            startMass = parent.mass / 4;
            startSize = (int)parent.size - 2;
        }

        randMass += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[0]));
        randSize += Calc.GetSeedNum(cell.galaxy.Seed + cell.pos.x + cell.pos.y + cell.pos.z, (int)((cell.pos.x + cell.pos.y + cell.pos.z) * randMass * cell.galaxy.Seed[1]));

        randMass = Mathf.Abs(randMass);
        randSize = Mathf.Abs(randSize);

        randMass += perlin;
        randMass /= 2;

        randSize += perlin;
        randSize /= 2;



        //��������� �������� �����������
        //���������� ������
        GenStandart();

        //�������� �������
        ChoosePatternPlanet();
        //���������� �� �������� ���� ��������
        GenPatternPlanet();

        //����� ������ ������� ������� � ��� �������� ������� ������ �������������� ���������
        inicializeLast();

        void GenStandart() {

            //���� �������� ���
            if (parent == null)
            {
                IniOrbitRadius();

                //��� ������ ���� ������, �������� �� 5 �������� �� 13-17 �������
                int massPower = (int)((randMass * 1000) % 5) + 10;
                this.mass = Calc.GetSizeInt((Size)massPower);

                //������
                int sizePower = (int)((randSize * 1000) % 5) + 10;
                this.size = (Size)sizePower;

                iniData(Calc.GetSizeInt(size), mass);

                this.time360Rotate = perlin;
            }
            else
            {
                //��������� ������ � ���


                color = new Color(Random.Range(0.5f, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1));

                //������
                int sizePower = (int)(((randSize * 1000) % startSize - 3) + 3); //������ ������ 3-�� ������� �� �����
                this.size = (Size)sizePower;

                IniOrbitRadius();
            }

            void IniOrbitRadius() {
                radiusOrbit = 0;
                radiusChildZone = 0;

                if (parent == null)
                    return;


                //���������� �� ������ ��������
                radiusOrbit += Calc.GetSizeInt(parent.size);

                //���������� ������ ������ ��� �������
                int index = 0;
                foreach (SpaceObjData objData in parent.childs) {
                    if (objData != this)
                    {
                        //���������� ������ ������� �������
                        radiusOrbit += objData.radiusGravity + objData.radiusVoid;
                        //radiusOrbit += Calc.GetSizeInt(objData.size);
                        index++;
                    }
                    else {
                        //������ ����� ��������� �������� ��� ������� ���� �������
                        radiusGravity = Calc.GetSizeInt(objData.size) * 4;
                        //���������!! ���� ��������� ��� ��������� ���������� �� ����� ������ distanceChildFree

                        radiusChildZone = (int)(radiusGravity * perlin);


                        //������������ � �������� �������
                        radiusVoid = (int)(radiusGravity * ((perlin * 100) % 10));

                        //������ ������� ���������� ����� �������� � ��������


                        radiusOrbit += radiusGravity / 2 + radiusVoid /2;
                        //radiusOrbit += Calc.GetSizeInt(objData.size)/2;
                        break;
                    }
                }
            }
        }

        //�������� �������
        void ChoosePatternPlanet() {
            //���� ������� �������������� ����
            if (patternPlanet != null) {
                //����������
                this.patternPlanet = patternPlanet;

                //����� ���������� ������ �� �������
                size = patternPlanet.termsGenerate.sizeMax;
                atmosphere = patternPlanet.parameters.AtmosphereMax;

                return;
            }

            //���� �������� ���, ���� ���������� �� ������ ����������� ���������

        }
        void GenPatternPlanet() {
            if (parent == null && this.patternPlanet != null)
                return;

            //�������������� ��� ������� ��� ������ ����������� �� ������� � ����������� � ������� ������ � ������

            //������������ � ���������
            //������������ � ����������
            //
        }

        void inicializeLast() {
            //�� ������ ������� ������� ������� ������ ��� ��������� ���������� ������� �������
            int needTextures = (int)size;

            //������� ������ �������
            MainTextures = new SpaceObjMap[needTextures];
        }
    }

    //�� ������ ����� � ������� ������������ � ���������� ������� �������
    void iniData(float size, float mass)
    {
        //��� ������� ������� ���� �������
        //���� ������

        if(size >= Calc.GetSizeInt(Size.s8192o)) ini17();
        else if (size >= Calc.GetSizeInt(Size.s4096o)) ini16();
        else if (size >= Calc.GetSizeInt(Size.s2048o)) ini15();
        else if (size >= Calc.GetSizeInt(Size.s1024o)) ini14();
        else if (size >= Calc.GetSizeInt(Size.s512o)) ini13();
        else if (size >= Calc.GetSizeInt(Size.s256o)) ini12();
        else if (size >= Calc.GetSizeInt(Size.s128o)) ini11();
        else if (size >= Calc.GetSizeInt(Size.s64o)) ini10();
        else if (size >= Calc.GetSizeInt(Size.s32o)) ini09();
        else if (size >= Calc.GetSizeInt(Size.s16o)) ini08();
        else if (size >= Calc.GetSizeInt(Size.s8o)) ini07();

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
            else if (massCoof >= 1.0f / 4) {
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
        void ini12()
        {

        }
        void ini11()
        {

        }
        void ini10()
        {

        }
        void ini09()
        {

        }
        void ini08()
        {

        }
        void ini07()
        {

        }

        void StarBlue() {
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
        void StarYellow() {
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

    //������������� ������ ��� �������
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

    //������������� ������� �� ������ ���������� ������������
    public void GenPlanets(float distGenMax, float perlin) {
        //������� ������ ������
        childs = new SpaceObjData[0];

        //���� ��������� ��� ��������� ������ ������� ����, �������
        if (distGenMax < 512)
            return;


        //��������� ��������� ������ �� ������ �����������
        GenPerlinLoc((perlin*1000)%1);

        //����� ��������� ������������ ��������� ��� ��������� ������ ���� �������� ���
        //if(parent == null)
            //distGenMax *= (0.5f * (perlin * 0.5f));

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
        while (distGenNow < distGenMax && childs.Length < childMaximum && numTryAdd < 512) {

            //��������� ������������ ��� ��������� ���
            float distFree = distGenMax - distGenNow;
            AddPlanet(distFree);
        }

        //���������� ����� ����� ������� � ��� �����
        for (int num = 0; num < childs.Length; num++) {
            childs[num].GenPlanets(childs[num].radiusChildZone, GetPerlinFromIndex(num));
        }
        
        void AddPlanet(float distFree) {
            //������� �������
            SpaceObjData spaceObj = new SpaceObjData(cell);
            //��������� �� ��������
            SpaceObjData[] childOld = childs;

            childs = new SpaceObjData[childs.Length + 1];
            for (int num = 0; num < childOld.Length; num++) {
                childs[num] = childOld[num];
            }

            int childNum = childOld.Length;
            childs[childNum] = spaceObj;

            //�������������� ��
            childs[childNum].GenObjData(this, GetPerlinFromIndex(childNum), distFree);

            //����� ������� ����������������� (���� ����� � ������) ���������� ��������� ���������
            distGenNow += childs[childNum].radiusGravity + childs[childNum].radiusVoid; //Calc.GetSizeInt(child[child.Length - 1].size);


        }
    }

    /// <summary>
    /// �������� �������� �������� ������� ��� ���� �������� ��� ������ ��������
    /// </summary>
    /// <param name="quarity"></param>
    /// <returns></returns>
    public Texture2D GetMainTexture(int quarity) {
        //��������� ���� �� �������� ����� ��������
        if (MainTextures[quarity] != null)
            return MainTextures[quarity].texture;

        //���� ��� ��������� �� �����
        if (SpaceObjMap.timeLastGen == Time.time) {
            //���������� ��������� �����������������������
            for (int num = MainTextures.Length - 1; num > 0; num--) {
                if (MainTextures[num] != null)
                    return MainTextures[num].texture;
            }
        }
        //���� ������������ �����
        else {
            //���� ����������������� �������� ������� � ������� ��������
            for (int num = 0; num < MainTextures.Length; num++) {
                if (MainTextures[num] != null)
                    continue;

                //����������
                MainTextures[num] = new SpaceObjMap(this, (Size)num);

                //���������� ����� ���������
                SpaceObjMap.timeLastGen = Time.time;

                //����������
                return MainTextures[num].texture;
            }
        }

        //���������� ������� ��������
        if (MainTextures[0] == null)
        {
            MainTextures[0] = new SpaceObjMap(this, Size.s1o);
        }
        return MainTextures[0].texture;
    }

    public float[,] GetHeightMap(int quarity) {

        if (MainTextures[quarity] != null)
            return MainTextures[quarity].map;

        //���� ��������� ����� ����� �� ���������� � ����� ������
        //����������
        MainTextures[quarity] = new SpaceObjMap(this, (Size)quarity);
        //���������� ����� ���������
        SpaceObjMap.timeLastGen = Time.time;


        return MainTextures[quarity].map;
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
    public SpaceObjMap(SpaceObjData data, Size sizeTexture) {

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
    s1o = 1,
    s2o = 2,
    s4o = 3,
    s8o = 4,
    s16o = 5,
    s32o = 6,
    s64o = 7,
    s128o = 8,
    s256o = 9,
    s512o = 10,
    s1024o = 11,
    s2048o = 12,
    s4096o = 13,
    s8192o = 14
}

