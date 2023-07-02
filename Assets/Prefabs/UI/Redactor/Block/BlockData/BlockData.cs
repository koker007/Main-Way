using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BlockData
{
    public string name;
    public string mod;
    public int variant;

    /// <summary>
    /// ������� ���
    /// </summary>
    //������������
    TypeBlockTransparent transparentType = TypeBlockTransparent.NoTransparent;
    float transparentPower = 0.5f;

    //�������� ��������� ����� ��� ���
    public bool drawNeighbourWall = false;

    //����������
    public int lighting = 0;

    public Type type;

    public BlockPhysics physics;

    //������ ������ ����� ������
    public TypeBlock TBlock;
    public TypeVoxel TVoxels;
    public TypeLiquid TLiquid;

    public enum Type {
        block = 0,
        voxels = 1,
        liquid = 2
    }

    //�������� ��� ���� �� ������ ���� ����� ����� ����� ����������
    public Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down) {
        Mesh meshResult = new Mesh();

        meshResult.vertices = GraficCalc.main.mergeVector3(TBlock.wallFace.forms.vertices, TBlock.wallBack.forms.vertices, TBlock.wallRight.forms.vertices, TBlock.wallLeft.forms.vertices, TBlock.wallUp.forms.vertices, TBlock.wallDown.forms.vertices);
        //meshResult.triangles = GraficCalc.main.mergeTriangleNum(wallFace.forms.triangles, wallBack.forms.triangles, wallRight.forms.triangles, wallLeft.forms.triangles, wallUp.forms.triangles, wallDown.forms.triangles);
        meshResult.uv = GraficCalc.main.mergeVector2(TBlock.wallFace.forms.uv, TBlock.wallBack.forms.uv, TBlock.wallRight.forms.uv, TBlock.wallLeft.forms.uv, TBlock.wallUp.forms.uv, TBlock.wallDown.forms.uv);
        meshResult.uv2 = GraficCalc.main.mergeVector2(TBlock.wallFace.forms.uvShadow, TBlock.wallBack.forms.uvShadow, TBlock.wallRight.forms.uvShadow, TBlock.wallLeft.forms.uvShadow, TBlock.wallUp.forms.uvShadow, TBlock.wallDown.forms.uvShadow);

        meshResult.subMeshCount = 6;
        /////////////////////////////////////////////////////////////
        //����� � ������ ������������� ���� �������� ���� ����������
        /////////////////////////////////////////////////////////////
        int addNum = 0;
        meshResult.SetTriangles(TBlock.wallFace.forms.triangles, 0);
        addNum += TBlock.wallFace.forms.triangles.Length;

        int[] trianglesBack = GraficCalc.main.addToInt(TBlock.wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesBack, 1);
        addNum += TBlock.wallBack.forms.triangles.Length;

        int[] trianglesRight = GraficCalc.main.addToInt(TBlock.wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesRight, 2);
        addNum += TBlock.wallRight.forms.triangles.Length;

        int[] trianglesLeft = GraficCalc.main.addToInt(TBlock.wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesLeft, 3);
        addNum += TBlock.wallLeft.forms.triangles.Length;

        int[] trianglesUp = GraficCalc.main.addToInt(TBlock.wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesUp, 4);
        addNum += TBlock.wallUp.forms.triangles.Length;

        int[] trianglesDown = GraficCalc.main.addToInt(TBlock.wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesDown, 5);

        return meshResult;
    }

    public void TestCreateVoxel() {
        if (TVoxels != null)
            return;

        TVoxels = new TypeVoxel();
        TVoxels.RandomizeData();
    }
    public void TestCreateLiquid() {
        if (TLiquid != null)
            return;

        TLiquid = new TypeLiquid();
    }
    public Mesh GetMeshVoxel() {
        return TVoxels.GetMesh();
    }
    public Mesh GetMeshLiquid(bool face, bool back, bool left, bool right, bool up, bool down, int lvlUp, int lvlDown) {
        Mesh mesh = new Mesh();

        mesh = TLiquid.GetMesh(face, back, left, right, up, down, lvlUp, lvlDown);

        return mesh;
    }

    public BlockData() {
        //������� �� ��������� ��� �����
        TBlock = new TypeBlock();

        //������������� ����� �� ���������
        physics = new BlockPhysics();
        physics.parameters = new BlockPhysics.Parameters();
    }


    public void SetRandomVoxel()
    {
        for (int x = 0; x < TBlock.wallFace.forms.voxel.GetLength(0); x++)
        {
            for (int y = 0; y < TBlock.wallFace.forms.voxel.GetLength(1); y++)
            {
                TBlock.wallFace.forms.voxel[x, y] = Random.Range(0, 1.0f);
                TBlock.wallBack.forms.voxel[x, y] = Random.Range(0, 1.0f);
                TBlock.wallRight.forms.voxel[x, y] = Random.Range(0, 1.0f);
                TBlock.wallLeft.forms.voxel[x, y] = Random.Range(0, 1.0f);
                TBlock.wallUp.forms.voxel[x, y] = Random.Range(0, 1.0f);
                TBlock.wallDown.forms.voxel[x, y] = Random.Range(0, 1.0f);
            }
        }
    }

    //��������� ��� ������ ����� ������� ������������
    static public void SaveData(BlockData blockData) {
        //������� ���� � ����� �����
        string path = GameData.GameData.pathMod + "/" + blockData.mod + "/" + StrC.blocks + "/" + blockData.name + "/" + blockData.variant;
         
        //��������� ���� �� �����
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        //��������� ������� ������ �����
        saveBlockMain(path);

        //��������� � ����������� �� ���� �����
        if (blockData.type == Type.block) {
            //��������� �����
            saveBlockWall(path);
        }
        //��������� ���������� �����
        else if (blockData.type == Type.voxels) {
            saveTVoxelForm(path);
        }
        else if (blockData.type == Type.liquid) {
            saveTLiquidVisual(path);
        }

        //��������� ������
        saveBlockPhysics(path);

        void saveBlockMain(string path) {
            string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

            //��������� ���� � ��������� ����
            //������� ������ ���� ��� ���� ���������
            List<string> dataList = new List<string>();

            string dataOne = "";
            //���������� ���
            dataOne = StrC.type + StrC.SEPARATOR;
            if (blockData.type == Type.block)
                dataOne += StrC.TBlock;
            else if (blockData.type == Type.voxels)
                dataOne += StrC.TVoxels;
            else if (blockData.type == Type.liquid)
                dataOne += StrC.TLiquid;

            dataList.Add(dataOne);

            //��������� � ����
            FileStream fileStream;
            //���� ����� ��� - �������
            if (!File.Exists(pathMainStr))
            {
                fileStream = File.Create(pathMainStr);
                fileStream.Close();
            }
            File.WriteAllLines(pathMainStr, dataList.ToArray());
        }
        void saveBlockWall(string pathBlock) {
            string pathWalls = pathBlock + "/" + StrC.wall;

            blockData.TBlock.wallFace.SaveTo(pathWalls);
            blockData.TBlock.wallBack.SaveTo(pathWalls);
            blockData.TBlock.wallLeft.SaveTo(pathWalls);
            blockData.TBlock.wallRight.SaveTo(pathWalls);
            blockData.TBlock.wallUp.SaveTo(pathWalls);
            blockData.TBlock.wallDown.SaveTo(pathWalls);
        }       
        void saveBlockPhysics(string pathBlock) {
            string pathPhysics = pathBlock + "/" + StrC.physics;

            blockData.physics.saveColliderZone(pathPhysics);
        }

        void saveTVoxelForm(string pathBlock) {
            string pathTVoxelForm = pathBlock + "/" + StrC.TVoxels;
            blockData.TVoxels.SaveTo(pathTVoxelForm);
        }
        void saveTLiquidVisual(string pathBlock) {
            string pathTLiquidVisual = pathBlock + "/" + StrC.TLiquid;
            blockData.TLiquid.SaveTo(pathTLiquidVisual);
        }
    }
    static public BlockData LoadData(string pathBlockVariant) {
        //���� ����� ����� ��� - �������
        if (!Directory.Exists(pathBlockVariant))
        {
            Debug.Log(pathBlockVariant + " Not exist");
            return null;
        }

        BlockData resultData = new BlockData();

        //�������� �������� ������ �����
        loadBlockMain(pathBlockVariant);

        if (resultData.type == Type.block)
        {
            loadBlockWall(pathBlockVariant);
        }
        else if (resultData.type == Type.voxels)
        {
            loadTVoxelsForm(pathBlockVariant);
        }
        else if (resultData.type == Type.liquid) {
            loadTLiquidForm(pathBlockVariant);
        }

        loadBlockPhysics(pathBlockVariant);

        return resultData;

        void loadBlockMain(string path) {
            //����������� ����
            string[] pathParts1 = path.Split("/");

            List<string> pathList = new List<string>();
            foreach (string pathCut in pathParts1) {
                string[] pathParts2 = pathCut.Split("\\");
                foreach (string part in pathParts2) {
                    pathList.Add(part);
                }
            }

            string[] pathMass = pathList.ToArray();
            //for (int num = 0; num < pathList.Count; num++) {
            //    pathMass[num] = pathList[num];
            //}



            if (pathMass.Length <= 3)
            {
                pathMass = path.Split("\\");
            }

            if (pathMass.Length <= 3)
            {
                Debug.LogError(path + " load name error");
                return;
            }

            resultData.mod = pathMass[pathMass.Length - 4];
            resultData.name = pathMass[pathMass.Length - 2];
            resultData.variant = System.Convert.ToInt32(pathMass[pathMass.Length - 1]);

            //����� ��������� ���� � ��������� �������
            loadMainTXT();

            void loadMainTXT() {
                string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

                //��������� ������������� �����
                if (!File.Exists(pathMainStr)) {
                    //����� ���, ������
                    Debug.LogError("File main.txt not exist " + pathMainStr);
                    return;
                }

                //����������� ������ �����
                string[] datasStr = File.ReadAllLines(pathMainStr);

                //��������� ��� ������ �� ������
                foreach (string dataStr in datasStr) {
                    string[] data = dataStr.Split(StrC.SEPARATOR);

                    if (data.Length > 2)
                    {
                        Debug.LogError("Bad parametr: " + dataStr + " in " + pathMainStr);
                        continue;
                    }

                    testParametr(data[0], data[1]);
                }
                //////////////////////////////////////////////////////////////////////////////////////
                ///

                void testParametr(string name, string value) {
                    if (name == StrC.type)
                    {
                        if (value == StrC.TBlock)
                            resultData.type = Type.block;
                        else if (value == StrC.TVoxels)
                            resultData.type = Type.voxels;
                        else if (value == StrC.TLiquid)
                            resultData.type = Type.liquid;
                        else
                            Debug.LogError("Bad parametr of " + name + ": " + value);

                    }
                }
            }
        }
        void loadBlockWall(string path) {
            string pathWalls = path + "/" + StrC.wall;

            resultData.TBlock.wallFace.LoadFrom(pathWalls);
            resultData.TBlock.wallBack.LoadFrom(pathWalls);
            resultData.TBlock.wallLeft.LoadFrom(pathWalls);
            resultData.TBlock.wallRight.LoadFrom(pathWalls);
            resultData.TBlock.wallUp.LoadFrom(pathWalls);
            resultData.TBlock.wallDown.LoadFrom(pathWalls);
        }
        void loadTVoxelsForm(string path) {
            string pathTVoxelForm = path + "/" + StrC.TVoxels;

            resultData.TVoxels = new TypeVoxel();
            resultData.TVoxels.LoadFrom(pathTVoxelForm);
        }
        void loadTLiquidForm(string path) {
            string pathTLiquidForm = path + "/" + StrC.TLiquid;

            resultData.TLiquid = new TypeLiquid();
            resultData.TLiquid.LoadFrom(pathTLiquidForm);
        }
        void loadBlockPhysics(string path)
        {
            string pathPhysics = path + "/" + StrC.physics;
            resultData.physics.loadColliderZone(pathPhysics);
        }
    }
    static public BlockData[] LoadDatas(string pathBlock) {
        //����� ������ ������� ���� ��������� � ����� �����
        string[] pathVariants = Directory.GetDirectories(pathBlock);

        int maxVar = 0;
        //������� ������ ������ � ����������� ������ � ����
        List<BlockData> blockDatasList = new List<BlockData>();
        foreach (string pathVar in pathVariants) {
            BlockData blockData = LoadData(pathVar);
            blockDatasList.Add(blockData);

            //��������� ��������
            if (maxVar <= blockData.variant)
                maxVar = blockData.variant + 1;
        }

        //������� ������ ���������
        BlockData[] blockDatas = new BlockData[maxVar];

        //����������
        foreach (BlockData blockData in blockDatasList) {
            if (blockDatas[blockData.variant] != null) {
                Debug.LogError("Block load Error: have variant| " + blockData.mod + " " + blockData.name + " " + blockData.variant);
                continue;
            }

            blockDatas[blockData.variant] = blockData;
        }

        return blockDatas;
    }
}

public class BlockForms {
    public float[,] voxel = new float[16, 16];
    //����� 16 �� 16
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;
    public Vector2[] uvShadow;

    [System.Serializable]
    public class voxels {
        public float[,] height = new float[16, 16];
    }
}

public class BlockPhysics {

    public ColliderZone[] zones;
    public Light light;
    public Parameters parameters;

    [System.Serializable]
    public class ColliderZone
    {
        public Vector3S pos; //������� ������
        public Vector3S size; //������ ������������ ������
    }
    public class Light {
        float lightRange;
    }
    public class Parameters {
        float viscosity = 1; //��������

    }

    public ColliderZone[] loadColliderZone(string pathPhysics)
    {

        ColliderZone[] colliderZones = null;

        string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

        //���� ����� ���
        if (!File.Exists(pathFileColliders))
            return colliderZones;

        //���� ���� ���� - ���������
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(pathFileColliders, FileMode.Open);
        colliderZones = (ColliderZone[])bf.Deserialize(fileStream);
        fileStream.Close();

        zones = colliderZones;



        return colliderZones;

    }
    public void saveColliderZone(string pathPhysics)
    {
        string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

        //���� ���� ���� - �������
        if (File.Exists(pathFileColliders))
            File.Delete(pathFileColliders);

        //���� ��������� ���� �� ���� �������
        if (zones == null)
            return;

        if (!Directory.Exists(pathPhysics)) {
            Directory.CreateDirectory(pathPhysics);
        }


        //������� ����
        BinaryFormatter bf = new BinaryFormatter();
        FileStream collidersZoneStream = File.OpenWrite(pathFileColliders);
        bf.Serialize(collidersZoneStream, zones);
        collidersZoneStream.Close();
    }


}


public enum Side
{
    face = 0,
    back = 1,
    right = 2,
    left = 3,
    up = 4,
    down = 5
}
