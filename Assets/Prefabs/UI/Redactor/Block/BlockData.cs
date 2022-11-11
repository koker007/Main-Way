using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BlockData
{
    public string name;
    public string mod;
    public int variant;

    /// <summary>
    /// Внешний вид
    /// </summary>
    //прозрачность
    TypeBlockTransparent transparentType = TypeBlockTransparent.NoTransparent;
    float transparentPower = 0.5f;

    //Рисовать соседские стены или нет
    public bool drawNeighbourWall = false;

    //Светимость
    public int lighting = 0;

    public BlockWall wallFace;
    public BlockWall wallBack;
    public BlockWall wallLeft;
    public BlockWall wallRight;
    public BlockWall wallUp;
    public BlockWall wallDown;

    public BlockPhysics physics;

    public class Str
    {
        public const string mod = "mod";
        public const string block = "blocks";
        public const string name = "name";

        public const string wall = "Wall";

        public const string face = "face";
        public const string back = "back";
        public const string left = "left";
        public const string right = "right";
        public const string up = "up";
        public const string down = "down";

        public const string physics = "physics";
        public const string collidersZone = "collidersZone";

        public const string texture = "texture";
        public const string height = "height";
        public const string formatPNG = ".png";
    }

    //Получить меш куба на основе того какие стены нужно отрисовать
    public Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down) {
        Mesh meshResult = new Mesh();

        //Есл нужно сделать лицевую сторону, создаем ее
        int resultMax = 0;

        meshResult.vertices = GraficCalc.main.mergeVector3(wallFace.forms.vertices, wallBack.forms.vertices, wallRight.forms.vertices, wallLeft.forms.vertices, wallUp.forms.vertices, wallDown.forms.vertices);
        //meshResult.triangles = GraficCalc.main.mergeTriangleNum(wallFace.forms.triangles, wallBack.forms.triangles, wallRight.forms.triangles, wallLeft.forms.triangles, wallUp.forms.triangles, wallDown.forms.triangles);
        meshResult.uv = GraficCalc.main.mergeVector2(wallFace.forms.uv, wallBack.forms.uv, wallRight.forms.uv, wallLeft.forms.uv, wallUp.forms.uv, wallDown.forms.uv);


        meshResult.subMeshCount = 6;
        /////////////////////////////////////////////////////////////
        //Нужно в данных треугольников надо сдвигать счет примитивов
        /////////////////////////////////////////////////////////////
        int addNum = 0;
        meshResult.SetTriangles(wallFace.forms.triangles, 0);
        addNum += wallFace.forms.triangles.Length;

        int[] trianglesBack = GraficCalc.main.addToInt(wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesBack, 1);
        addNum += wallBack.forms.triangles.Length;

        int[] trianglesRight = GraficCalc.main.addToInt(wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesRight, 2);
        addNum += wallRight.forms.triangles.Length;

        int[] trianglesLeft = GraficCalc.main.addToInt(wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesLeft, 3);
        addNum += wallLeft.forms.triangles.Length;

        int[] trianglesUp = GraficCalc.main.addToInt(wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesUp, 4);
        addNum += wallUp.forms.triangles.Length;

        int[] trianglesDown = GraficCalc.main.addToInt(wallBack.forms.triangles, addNum);
        meshResult.SetTriangles(trianglesDown, 5);

        return meshResult;
    }

    public BlockData() {
        wallFace = new BlockWall(Side.face);
        wallBack = new BlockWall(Side.back);
        wallRight = new BlockWall(Side.right);
        wallLeft = new BlockWall(Side.left);
        wallUp = new BlockWall(Side.up);
        wallDown = new BlockWall(Side.down);

        wallFace.SetTextureTest();
        wallBack.SetTextureTest();
        wallRight.SetTextureTest();
        wallLeft.SetTextureTest();
        wallUp.SetTextureTest();
        wallDown.SetTextureTest();

        wallFace.calcVertices();
        wallBack.calcVertices();
        wallRight.calcVertices();
        wallLeft.calcVertices();
        wallUp.calcVertices();
        wallDown.calcVertices();

        //Инициализация блока по умолчанию
        physics = new BlockPhysics();
        physics.parameters = new BlockPhysics.Parameters();
    }


    public void SetRandomVoxel()
    {
        //wallFace = new BlockWall(Side.face);
        //wallBack = new BlockWall(Side.back);
        //wallRight = new BlockWall(Side.right);
        //wallLeft = new BlockWall(Side.left);
        //wallUp = new BlockWall(Side.up);
        //wallDown = new BlockWall(Side.down);

        for (int x = 0; x < wallFace.forms.voxel.GetLength(0); x++)
        {
            for (int y = 0; y < wallFace.forms.voxel.GetLength(1); y++)
            {
                wallFace.forms.voxel[x, y] = Random.Range(0, 1.0f);
                wallBack.forms.voxel[x, y] = Random.Range(0, 1.0f);
                wallRight.forms.voxel[x, y] = Random.Range(0, 1.0f);
                wallLeft.forms.voxel[x, y] = Random.Range(0, 1.0f);
                wallUp.forms.voxel[x, y] = Random.Range(0, 1.0f);
                wallDown.forms.voxel[x, y] = Random.Range(0, 1.0f);
            }
        }

        //Установить тестовую текстуру
        //wallFace.SetTextureTest();
        //wallBack.SetTextureTest();
        //wallRight.SetTextureTest();
        //wallLeft.SetTextureTest();
        //wallUp.SetTextureTest();
        //wallDown.SetTextureTest();

        //wallFace.calcVertices();
        //wallBack.calcVertices();
        //wallRight.calcVertices();
        //wallLeft.calcVertices();
        //wallUp.calcVertices();
        //wallDown.calcVertices();
    }

    //Сохранить все данные блока который отправляется
    static public void SaveData(BlockData blockData) {
        //Создаем путь к папке блоке
        string path = GameData.pathMod + "/" + blockData.mod + "/" + Str.block + "/" + blockData.name + "/" + blockData.variant;
         
        //Проверяем есть ли папка
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        //Сохраняем стены
        saveBlockWall(path);
        //Сохраняем физику
        saveBlockPhysics(path);

        void saveBlockWall(string pathBlock) {
            string pathWalls = pathBlock + "/" + Str.wall;

            blockData.wallFace.SaveTo(pathWalls);
            blockData.wallBack.SaveTo(pathWalls);
            blockData.wallLeft.SaveTo(pathWalls);
            blockData.wallRight.SaveTo(pathWalls);
            blockData.wallUp.SaveTo(pathWalls);
            blockData.wallDown.SaveTo(pathWalls);
        }       
        void saveBlockPhysics(string pathBlock) {
            string pathPhysics = pathBlock + "/" + Str.physics;

            blockData.physics.saveColliderZone(pathPhysics);
        }
    }
    static public BlockData LoadData(string pathBlockVariant) {
        //Если пипки блока нет - выходим
        if (!Directory.Exists(pathBlockVariant))
        {
            Debug.Log(pathBlockVariant + " Not exist");
            return null;
        }

        BlockData resultData = new BlockData();

        //Загрузка основных данных блока
        loadBlockMain(pathBlockVariant);

        loadBlockWall(pathBlockVariant);
        loadBlockPhysics(pathBlockVariant);

        return resultData;

        void loadBlockMain(string path) {
            //Вытаскиваем путь
            string[] pathParts1 = path.Split("/");

            List<string> pathList = new List<string>();
            foreach (string pathCut in pathParts1) {
                string[] pathParts2 = pathCut.Split("\\");
                foreach (string part in pathParts2) {
                    pathList.Add(part);
                }
            }

            string[] pathMass = new string[pathList.Count];
            for (int num = 0; num < pathList.Count; num++) {
                pathMass[num] = pathList[num];
            }



            if (pathMass.Length <= 3)
            {
                pathMass = path.Split("\\");
            }

            if (pathMass.Length <= 3)
            {
                Debug.LogError(path + " load name error");
                return;
            }

            resultData.mod = pathMass[pathMass.Length - 3];
            resultData.name = pathMass[pathMass.Length - 2];
            resultData.variant = System.Convert.ToInt32(pathMass[pathMass.Length - 1]);
        }
        void loadBlockWall(string path) {
            string pathWalls = path + "/" + Str.wall;

            resultData.wallFace.LoadFrom(pathWalls);
            resultData.wallBack.LoadFrom(pathWalls);
            resultData.wallLeft.LoadFrom(pathWalls);
            resultData.wallRight.LoadFrom(pathWalls);
            resultData.wallUp.LoadFrom(pathWalls);
            resultData.wallDown.LoadFrom(pathWalls);
        }
        void loadBlockPhysics(string path)
        {
            string pathPhysics = path + "/" + Str.physics;

            resultData.physics.loadColliderZone(pathPhysics);
        }
    }
    static public BlockData[] LoadDatas(string pathBlock) {
        //Нужно узнать сколько есть вариантов в папке блока
        string[] pathVariants = Directory.GetDirectories(pathBlock);

        int maxVar = 0;
        //Создаем список блоков и вытаскиваем данные в него
        List<BlockData> blockDatasList = new List<BlockData>();
        foreach (string pathVar in pathVariants) {
            BlockData blockData = LoadData(pathVar);
            blockDatasList.Add(blockData);

            //обновляем максимум
            if (maxVar <= blockData.variant)
                maxVar = blockData.variant + 1;
        }

        //Создаем массив вариантов
        BlockData[] blockDatas = new BlockData[maxVar];

        //Запихиваем
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

/// <summary>
/// Визуальная часть стены
/// </summary>
public class BlockWall
{
    public Texture2D texture;
    public BlockForms forms;

    Side side;

    public BlockWall(Side side) {
        this.side = side;

        forms = new BlockForms();
    }

    public void LoadFrom(string path) {
        if (!Directory.Exists(path)) {
            Debug.Log(path + " Block wall not Exist");
            return;
        }

        string pathWall = path + "/" + BlockData.Str.wall;
        string pathTexture = path + "/" + BlockData.Str.texture;

        if (side == Side.face)
        {
            pathWall += BlockData.Str.face;
            pathTexture += BlockData.Str.face;
        }
        else if (side == Side.back)
        {
            pathWall += BlockData.Str.back;
            pathTexture += BlockData.Str.back;
        }
        else if (side == Side.left)
        {
            pathWall += BlockData.Str.left;
            pathTexture += BlockData.Str.left;
        }
        else if (side == Side.right)
        {
            pathWall += BlockData.Str.right;
            pathTexture += BlockData.Str.right;
        }
        else if (side == Side.up)
        {
            pathWall += BlockData.Str.up;
            pathTexture += BlockData.Str.up;
        }
        else {
            pathWall += BlockData.Str.down;
            pathTexture += BlockData.Str.down;
        }
        pathTexture += BlockData.Str.formatPNG;

        Texture2D texture = new Texture2D(16,16);

        if (File.Exists(pathTexture)) {
            byte[] data = File.ReadAllBytes(pathTexture);
            texture.LoadImage(data);
        }

        if (File.Exists(pathWall)) {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(pathWall, FileMode.Open);
            BlockForms.voxels voxs = (BlockForms.voxels)bf.Deserialize(fileStream);
            fileStream.Close();


            forms.voxel = voxs.height;
        }

        this.texture = texture;
    }
    public void SaveTo(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string pathTexture = path + "/" + BlockData.Str.texture;
        string pathWall = path + "/" + BlockData.Str.wall;

        if (side == Side.face)
        {
            pathTexture += BlockData.Str.face;
            pathWall += BlockData.Str.face;
        }
        else if (side == Side.back)
        {
            pathTexture += BlockData.Str.back;
            pathWall += BlockData.Str.back;
        }
        else if (side == Side.left)
        {
            pathTexture += BlockData.Str.left;
            pathWall += BlockData.Str.left;
        }
        else if (side == Side.right)
        {
            pathTexture += BlockData.Str.right;
            pathWall += BlockData.Str.right;
        }
        else if (side == Side.up)
        {
            pathTexture += BlockData.Str.up;
            pathWall += BlockData.Str.up;
        }
        else {
            pathTexture += BlockData.Str.down;
            pathWall += BlockData.Str.down;
        }

        pathTexture += BlockData.Str.formatPNG;

        byte[] textureData = texture.EncodeToPNG();
        FileStream textureStream = File.Open(pathTexture, FileMode.OpenOrCreate);
        textureStream.Write(textureData);
        textureStream.Close();

        BlockForms.voxels voxels = new BlockForms.voxels();
        voxels.height = forms.voxel;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream voxStream = File.OpenWrite(pathWall);
        bf.Serialize(voxStream, voxels);
        voxStream.Close();
    }

    //Установить цвет тестовую текстуру
    public void SetTextureTest() {
        texture = new Texture2D(16, 16);

        float one = 1.0f / 16.0f;

        //x = red //y = green
        if (side == Side.face) {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color(one * x, one * y, 0.0f));
                }
            }
        }
        else if (side == Side.back)
        {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color(1 - (one * x), one * y, 1.0f));
                }
            }
        }
        //x = blue //y = green
        else if (side == Side.left)
        {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color( 0.0f, one * y, 1 - (one * x)));
                }
            }
        }
        else if (side == Side.right)
        {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color( 1.0f , one * y, one * x));
                }
            }
        }
        //x = r // y = blue
        else if (side == Side.up)
        {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color(x * one, 1.0f , one * y));
                }
            }
        }
        //x = r // y = blue
        else if (side == Side.down)
        {
            for (int x = 0; x < forms.voxel.GetLength(0); x++)
            {
                for (int y = 0; y < forms.voxel.GetLength(1); y++)
                {
                    texture.SetPixel(x, y, new Color((x * one), 0.0f, 1 - one * y));
                }
            }
        }
        else
        {
            Debug.LogError("Error cube wall side");
        }

        texture.Apply();
        texture.filterMode = FilterMode.Point;
    }

    public void calcVertices()
    {
        //создаем плоскость стены изходя из глубины вокселей

        GraficBlockWall.main.calculate(new GraficData.BlockWall(forms, side));
    }
}
public class BlockForms {
    public float[,] voxel = new float[16, 16];
    //Стена 16 на 16
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

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
        public Vector3S pos; //Позиция старта
        public Vector3S size; //Размер относительно старта
    }
    public class Light {
        float lightRange;
    }
    public class Parameters {
        float viscosity = 1; //Вязкость

    }

    public ColliderZone[] loadColliderZone(string pathPhysics)
    {

        ColliderZone[] colliderZones = null;

        string pathFileColliders = pathPhysics + '/' + BlockData.Str.collidersZone;

        //Если файла нет
        if (!File.Exists(pathFileColliders))
            return colliderZones;

        //Если файл есть - загружаем
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(pathFileColliders, FileMode.Open);
        colliderZones = (ColliderZone[])bf.Deserialize(fileStream);
        fileStream.Close();

        zones = colliderZones;



        return colliderZones;

    }
    public void saveColliderZone(string pathPhysics)
    {
        string pathFileColliders = pathPhysics + '/' + BlockData.Str.collidersZone;

        //Если файл есть - удаляем
        if (File.Exists(pathFileColliders))
            File.Delete(pathFileColliders);

        //Если создавать файл не надо выходим
        if (zones == null)
            return;

        if (!Directory.Exists(pathPhysics)) {
            Directory.CreateDirectory(pathPhysics);
        }


        //Создаем файл
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
