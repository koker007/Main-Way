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
    /// Внешний вид
    /// </summary>
    //прозрачность
    TypeBlockTransparent transparentType = TypeBlockTransparent.NoTransparent;
    float transparentPower = 0.5f;

    //Рисовать соседские стены или нет
    public bool drawNeighbourWall = false;

    //Светимость
    public int lighting = 0;

    public Type type;

    public BlockPhysics physics;

    //Данные разных типов блоков
    public TypeBlock TBlock;
    public TypeVoxel TVoxels;
    public TypeLiquid TLiquid;

    public class Str
    {
        public const string SEPARATOR = ":=";

        public const string mod = "mod";
        public const string blocks = "blocks";
        public const string voxels = "voxels";
        public const string liquid = "liquid";
        public const string name = "name";
        public const string main = "main";

        public const string wall = "Wall";

        public const string face = "face";
        public const string back = "back";
        public const string left = "left";
        public const string right = "right";
        public const string up = "up";
        public const string down = "down";

        public const string physics = "physics";
        public const string collidersZone = "collidersZone";

        public const string form = "form";
        public const string texture = "texture";
        public const string height = "height";
        public const string formatPNG = ".png";
        public const string formatTXT = ".txt";

        public const string type = "type";
        public const string TBlock = "TBlock";
        public const string TVoxels = "TVoxels";
        public const string TLiquid = "TLiquid";
    }

    public enum Type {
        block = 0,
        voxels = 1,
        liquid = 2
    }

    //Получить меш куба на основе того какие стены нужно отрисовать
    public Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down) {
        Mesh meshResult = new Mesh();

        meshResult.vertices = GraficCalc.main.mergeVector3(TBlock.wallFace.forms.vertices, TBlock.wallBack.forms.vertices, TBlock.wallRight.forms.vertices, TBlock.wallLeft.forms.vertices, TBlock.wallUp.forms.vertices, TBlock.wallDown.forms.vertices);
        //meshResult.triangles = GraficCalc.main.mergeTriangleNum(wallFace.forms.triangles, wallBack.forms.triangles, wallRight.forms.triangles, wallLeft.forms.triangles, wallUp.forms.triangles, wallDown.forms.triangles);
        meshResult.uv = GraficCalc.main.mergeVector2(TBlock.wallFace.forms.uv, TBlock.wallBack.forms.uv, TBlock.wallRight.forms.uv, TBlock.wallLeft.forms.uv, TBlock.wallUp.forms.uv, TBlock.wallDown.forms.uv);
        meshResult.uv2 = GraficCalc.main.mergeVector2(TBlock.wallFace.forms.uvShadow, TBlock.wallBack.forms.uvShadow, TBlock.wallRight.forms.uvShadow, TBlock.wallLeft.forms.uvShadow, TBlock.wallUp.forms.uvShadow, TBlock.wallDown.forms.uvShadow);

        meshResult.subMeshCount = 6;
        /////////////////////////////////////////////////////////////
        //Нужно в данных треугольников надо сдвигать счет примитивов
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
        //Создаем по умолчанию тип блока
        TBlock = new TypeBlock();

        //Инициализация блока по умолчанию
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

    //Сохранить все данные блока который отправляется
    static public void SaveData(BlockData blockData) {
        //Создаем путь к папке блоке
        string path = GameData.pathMod + "/" + blockData.mod + "/" + Str.blocks + "/" + blockData.name + "/" + blockData.variant;
         
        //Проверяем есть ли папка
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        //Сохраняем главные данные блока
        saveBlockMain(path);

        //Сохраняем в зависимости от типа блока
        if (blockData.type == Type.block) {
            //Сохраняем стены
            saveBlockWall(path);
        }
        //Сохраняем воксельную форму
        else if (blockData.type == Type.voxels) {
            saveTVoxelForm(path);
        }
        else if (blockData.type == Type.liquid) {
            saveTLiquidVisual(path);
        }

        //Сохраняем физику
        saveBlockPhysics(path);

        void saveBlockMain(string path) {
            string pathMainStr = path + "/" + Str.main + Str.formatTXT;

            //Сохранить надо в текстовый файл
            //создаем список того что надо запомнить
            List<string> dataList = new List<string>();

            string dataOne = "";
            //Запоминаем тип
            dataOne = Str.type + Str.SEPARATOR;
            if (blockData.type == Type.block)
                dataOne += Str.TBlock;
            else if (blockData.type == Type.voxels)
                dataOne += Str.TVoxels;
            else if (blockData.type == Type.liquid)
                dataOne += Str.TLiquid;

            dataList.Add(dataOne);

            //Сохраняем в файл
            FileStream fileStream;
            //Если файла нет - создаем
            if (!File.Exists(pathMainStr))
            {
                fileStream = File.Create(pathMainStr);
                fileStream.Close();
            }
            File.WriteAllLines(pathMainStr, dataList.ToArray());
        }
        void saveBlockWall(string pathBlock) {
            string pathWalls = pathBlock + "/" + Str.wall;

            blockData.TBlock.wallFace.SaveTo(pathWalls);
            blockData.TBlock.wallBack.SaveTo(pathWalls);
            blockData.TBlock.wallLeft.SaveTo(pathWalls);
            blockData.TBlock.wallRight.SaveTo(pathWalls);
            blockData.TBlock.wallUp.SaveTo(pathWalls);
            blockData.TBlock.wallDown.SaveTo(pathWalls);
        }       
        void saveBlockPhysics(string pathBlock) {
            string pathPhysics = pathBlock + "/" + Str.physics;

            blockData.physics.saveColliderZone(pathPhysics);
        }

        void saveTVoxelForm(string pathBlock) {
            string pathTVoxelForm = pathBlock + "/" + Str.TVoxels;
            blockData.TVoxels.SaveTo(pathTVoxelForm);
        }
        void saveTLiquidVisual(string pathBlock) {
            string pathTLiquidVisual = pathBlock + "/" + Str.TLiquid;
            //blockData
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

        if (resultData.type == Type.block)
        {
            loadBlockWall(pathBlockVariant);
        }
        else if (resultData.type == Type.voxels) {
            loadTVoxelsForm(pathBlockVariant);
        }

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

            //Нужно загрузить файл с основными данными
            loadMainTXT();

            void loadMainTXT() {
                string pathMainStr = path + "/" + Str.main + Str.formatTXT;

                //проверяем существование файла
                if (!File.Exists(pathMainStr)) {
                    //Файла нет, ошибка
                    Debug.LogError("File main.txt not exist " + pathMainStr);
                    return;
                }

                //Вытаскиваем данные файла
                string[] datasStr = File.ReadAllLines(pathMainStr);

                //Проверяем все строки на данные
                foreach (string dataStr in datasStr) {
                    string[] data = dataStr.Split(Str.SEPARATOR);

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
                    if (name == Str.type)
                    {
                        if (value == Str.TBlock)
                            resultData.type = Type.block;
                        else if (value == Str.TVoxels)
                            resultData.type = Type.voxels;
                        else if (value == Str.TLiquid)
                            resultData.type = Type.liquid;
                        else
                            Debug.LogError("Bad parametr of " + name + ": " + value);

                    }
                }
            }
        }
        void loadBlockWall(string path) {
            string pathWalls = path + "/" + Str.wall;

            resultData.TBlock.wallFace.LoadFrom(pathWalls);
            resultData.TBlock.wallBack.LoadFrom(pathWalls);
            resultData.TBlock.wallLeft.LoadFrom(pathWalls);
            resultData.TBlock.wallRight.LoadFrom(pathWalls);
            resultData.TBlock.wallUp.LoadFrom(pathWalls);
            resultData.TBlock.wallDown.LoadFrom(pathWalls);
        }
        void loadTVoxelsForm(string path) {
            string pathTVoxelForm = path + "/" + Str.TVoxels;

            resultData.TVoxels = new TypeVoxel();
            resultData.TVoxels.LoadFrom(pathTVoxelForm);
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

public class TypeBlock {
    public Wall wallFace;
    public Wall wallBack;
    public Wall wallLeft;
    public Wall wallRight;
    public Wall wallUp;
    public Wall wallDown;

    /// <summary>
    /// Визуальная часть стены
    /// </summary>
    public class Wall
    {
        public Texture2D texture;
        public BlockForms forms;

        Side side;

        public Wall(Side side)
        {
            this.side = side;
            forms = new BlockForms();
        }

        public void LoadFrom(string path)
        {
            if (!Directory.Exists(path))
            {
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
            else
            {
                pathWall += BlockData.Str.down;
                pathTexture += BlockData.Str.down;
            }
            pathTexture += BlockData.Str.formatPNG;

            Texture2D texture = new Texture2D(16, 16);
            texture.filterMode = FilterMode.Point;

            if (File.Exists(pathTexture))
            {
                byte[] data = File.ReadAllBytes(pathTexture);
                texture.LoadImage(data);
            }

            if (File.Exists(pathWall))
            {

                BinaryFormatter bf = new BinaryFormatter();
                FileStream fileStream = File.Open(pathWall, FileMode.Open);
                BlockForms.voxels voxs = (BlockForms.voxels)bf.Deserialize(fileStream);
                fileStream.Close();


                forms.voxel = voxs.height;
            }

            this.texture = texture;
        }
        public void SaveTo(string path)
        {
            if (!Directory.Exists(path))
            {
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
            else
            {
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
        public void SetTextureTest()
        {
            texture = new Texture2D(16, 16);

            float one = 1.0f / 16.0f;

            //x = red //y = green
            if (side == Side.face)
            {
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
                        texture.SetPixel(x, y, new Color(0.0f, one * y, 1 - (one * x)));
                    }
                }
            }
            else if (side == Side.right)
            {
                for (int x = 0; x < forms.voxel.GetLength(0); x++)
                {
                    for (int y = 0; y < forms.voxel.GetLength(1); y++)
                    {
                        texture.SetPixel(x, y, new Color(1.0f, one * y, one * x));
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
                        texture.SetPixel(x, y, new Color(x * one, 1.0f, one * y));
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

    public TypeBlock() {
        wallFace = new Wall(Side.face);
        wallBack = new Wall(Side.back);
        wallRight = new Wall(Side.right);
        wallLeft = new Wall(Side.left);
        wallUp = new Wall(Side.up);
        wallDown = new Wall(Side.down);
        
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
    }
}
public class BlockForms {
    public float[,] voxel = new float[16, 16];
    //Стена 16 на 16
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;
    public Vector2[] uvShadow;

    [System.Serializable]
    public class voxels {
        public float[,] height = new float[16, 16];
    }
}

public class TypeVoxel {

    Visual visual;
    Data data = new Data();
    Texture2D texture = new Texture2D(16*16, 16);

    [System.Serializable]
    public class Data {
        public int[] exist = new int[16 * 16 * 16];
    }
    public class Visual{
        public int[] triangles;
        public Vector3[] vert;
        public Vector2[] uv;
    }

    void existsToAlphaTexture() {
        //Запихнуть существование вокселя в альфу текстуры
        for (int x = 0; x < 16; x++) {
            for (int y = 0; y < 16; y++) {
                for (int z = 0; z < 16; z++) {
                    int textureX = x + z * 16;
                    int idVoxel = x + y * 16 + z * 16 * 16;

                    if (data.exist[idVoxel] == 0)
                        texture.SetPixel(textureX, y, new Color(1,1,1,0));
                }
            }
        }

        texture.Apply();
    }
    void existFromAlphaTexture() {
        //Узнать существование вокселя на основе текстуры
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    int textureX = x + z * 16;
                    int idVoxel = x + y * 16 + z * 16 * 16;

                    if (texture.GetPixel(textureX, y).a != 0)
                        data.exist[idVoxel] = 1;
                    else data.exist[idVoxel] = 0;
                }
            }
        }
    }
    public void RandomizeData() {
        for (int num = 0; num < data.exist.Length; num++) {
            if (Random.Range(0, 100) < 10)
                data.exist[num] = 1;

            if (num == 0)
                data.exist[num] = 1;

            else if (num == data.exist.Length - 1) {
                data.exist[num] = 1;
            }
        }

        for (int x = 0; x < 16; x++){
            for (int z = 0; z < 16; z++){
                int posX = 16 * z + x;
                for (int y = 0; y < 16; y++){
                    texture.SetPixel(posX, y, new Color(x/16.0f, y/16.0f, z/16.0f));
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    Mesh GetMesh(Data data) {
        this.data = data;

        Mesh mesh = new Mesh();

        //создаем 8 под мешей
        //D - Down | U - Up
        //F - face | B - back
        //L - left | R - Right

        GraficData.BlockVoxel blockVoxelData = new GraficData.BlockVoxel();

        GraficBlockTVoxel.main.calculate(blockVoxelData, data.exist);

        int[] intm = Calc.Mesh.DelZeroTriangles(blockVoxelData.triangles);

        int[] triangles = intm;

        
        for (int x = 0; x < triangles.Length; x++)
        {
            if (triangles[x] >= blockVoxelData.vertices.Length)
            {
                Debug.Log("bad " + x + " " + triangles[x]);

            }
        }

        mesh.vertices = blockVoxelData.vertices;
        mesh.uv = blockVoxelData.uv;
        mesh.triangles = blockVoxelData.triangles;
        //mesh.RecalculateNormals();
        mesh.normals = blockVoxelData.normals;

        //mesh.Optimize();
        visual = new Visual();
        visual.vert = mesh.vertices;
        visual.triangles = mesh.triangles;
        visual.uv = mesh.uv;

        return mesh; 
    }
    Mesh GetMeshOld(Data data)
    {
        this.data = data;

        Mesh mesh = new Mesh();

        //создаем 8 под мешей
        //D - Down | U - Up
        //F - face | B - back
        //L - left | R - Right

        GraficData.BlockVoxelPart DFL = new GraficData.BlockVoxelPart(0, 0, 0);
        GraficData.BlockVoxelPart DFR = new GraficData.BlockVoxelPart(1, 0, 0);
        GraficData.BlockVoxelPart DBL = new GraficData.BlockVoxelPart(0, 0, 1);
        GraficData.BlockVoxelPart DBR = new GraficData.BlockVoxelPart(1, 0, 1);

        GraficData.BlockVoxelPart UFL = new GraficData.BlockVoxelPart(0, 1, 0);
        GraficData.BlockVoxelPart UFR = new GraficData.BlockVoxelPart(1, 1, 0);
        GraficData.BlockVoxelPart UBL = new GraficData.BlockVoxelPart(0, 1, 1);
        GraficData.BlockVoxelPart UBR = new GraficData.BlockVoxelPart(1, 1, 1);

        GraficBlockTVoxel.main.calculate(DFL, data.exist);
        GraficBlockTVoxel.main.calculate(DFR, data.exist);
        GraficBlockTVoxel.main.calculate(DBL, data.exist);
        GraficBlockTVoxel.main.calculate(DBR, data.exist);
        GraficBlockTVoxel.main.calculate(UFL, data.exist);
        GraficBlockTVoxel.main.calculate(UFR, data.exist);
        GraficBlockTVoxel.main.calculate(UBL, data.exist);
        GraficBlockTVoxel.main.calculate(UBR, data.exist);

        int[] intm01234567 = Calc.Mesh.MergeTriangles(
            DFL.triangles, DFL.vertices.Length,
            DFR.triangles, DFR.vertices.Length,
            UFL.triangles, UFL.vertices.Length,
            UFR.triangles, UFR.vertices.Length,
            DBL.triangles, DBL.vertices.Length,
            DBR.triangles, DBR.vertices.Length,
            UBL.triangles, UBL.vertices.Length,
            UBR.triangles, UBR.vertices.Length);

        int[] intm = Calc.Mesh.DelZeroTriangles(intm01234567);

        Vector2[] uv = Calc.Mesh.MergeVec2(DFL.uv, DFR.uv, UFL.uv, UFR.uv, DBL.uv, DBR.uv, UBL.uv, UBR.uv);
        Vector3[] vertices = Calc.Mesh.MergeVec3(DFL.vertices, DFR.vertices, UFL.vertices, UFR.vertices, DBL.vertices, DBR.vertices, UBL.vertices, UBR.vertices);
        Vector3[] normals = Calc.Mesh.MergeVec3(DFL.normals, DFR.normals, UFL.normals, UFR.normals, DBL.normals, DBR.normals, UBL.normals, UBR.normals);
        int[] triangles = intm;

        for (int x = 0; x < triangles.Length; x++)
        {
            if (triangles[x] >= vertices.Length)
            {
                Debug.Log("bad " + x + " " + triangles[x]);

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //mesh.RecalculateNormals();
        mesh.normals = normals;

        mesh.Optimize();
        visual = new Visual();
        visual.vert = mesh.vertices;
        visual.triangles = mesh.triangles;
        visual.uv = mesh.uv;

        return mesh;
    }

    public Mesh GetMesh() {
        return GetMesh(this.data);
    }
    public Texture2D GetTexture() {
        return texture;
    }
    public Data GetData() {
        return data;
    }

    public void SaveTo(string path) {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string pathTexture = path + "/" + BlockData.Str.texture;
        //string pathForm = path + "/" + BlockData.Str.form;

        pathTexture += BlockData.Str.formatPNG;

        //Подготавливаем текстуру
        existsToAlphaTexture();

        byte[] textureData = texture.EncodeToPNG();
        FileStream textureStream = File.Open(pathTexture, FileMode.OpenOrCreate);
        textureStream.Write(textureData);
        textureStream.Close();

        //Data voxels = new Data();
        //voxels = data;
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream voxStream = File.OpenWrite(pathForm);
        //bf.Serialize(voxStream, voxels);
        //voxStream.Close();
    }
    public void LoadFrom(string path) {
        if (!Directory.Exists(path)) {
            Debug.LogError("Path not exixt " + path);
            return;
        }

        string pathTexture = path + "/" + BlockData.Str.texture + BlockData.Str.formatPNG;
        if (!File.Exists(pathTexture)) {
            Debug.LogError("File not exist: " + pathTexture);
            return;
        }


        byte[] textureData = File.ReadAllBytes(pathTexture);
        texture.LoadImage(textureData);
        existFromAlphaTexture();

    }
}
public class TypeLiquid {

    static public Mesh[,,] meshs;

    public Data data = new Data();
    List<Texture2D[]> textures;

    public class Data {
        //Базовый цвет объекта
        public float colorHue = 0.0f;
        public float colorSaturation = 0.0f;
        public float colorValue = 0.0f;
        public float colorHueEnd = 0.0f;
        public float colorSaturationEnd = 0.0f;
        public float colorValueEnd = 0.0f;
        
        public float perlinScale = 1;
        public int perlinOctaves = 1;
        public float perlinScaleX = 1;
        public float perlinScaleY = 1;
        public float perlinScaleZ = 1;
        public int texturesMax = 10;
        public float animSpeed = 30;
        public float animSpeedX = 1;
        public float animSpeedY = 1;
        public float animSpeedZ = 4;
        public float animSizeX = 0.1f;
        public float animSizeY = 0.1f;
        public float animSizeZ = 0.1f;


    }

    public TypeLiquid() {
        iniMeshs();

        void iniMeshs() {
            //Если уже проинициализированно
            if (meshs != null)
                return;

            //Up //Down //Side
            meshs = new Mesh[16, 16, 6];

            for (int up = 0; up < 16; up++) {
                float voxUp = (up+1) / 16.0f;
                for (int down = 0; down < 16; down++) {
                    if (down > up)
                        continue;

                    float voxDown = down / 16.0f;

                    meshs[up, down, 0] = getFace(voxUp, voxDown);
                    meshs[up, down, 1] = getBack(voxUp, voxDown);
                    meshs[up, down, 2] = getLeft(voxUp, voxDown);
                    meshs[up, down, 3] = getRight(voxUp, voxDown);
                    meshs[up, down, 4] = getUp(voxUp);
                    meshs[up, down, 5] = getDown(voxDown);
                }
            }

            Mesh getFace(float voxUp, float voxDown) {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 0.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 0.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 0.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getBack(float voxUp, float voxDown) {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(1.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(0.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getLeft(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[3] = new Vector3(0.0f, voxDown, 0.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getRight(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(1.0f, voxDown, 0.0f);
                vertices[1] = new Vector3(1.0f, voxUp, 0.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getUp(float voxUp)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(1.0f, voxUp, 0.0f);

                uv[0] = new Vector2(0.0f, 0.0f);
                uv[1] = new Vector2(0.0f, 1.0f);
                uv[2] = new Vector2(1.0f, 1.0f);
                uv[3] = new Vector2(1.0f, 0.0f);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getDown(float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(0.0f, voxDown, 0.0f);
                vertices[2] = new Vector3(1.0f, voxDown, 0.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, 0.0f);
                uv[1] = new Vector2(0.0f, 1.0f);
                uv[2] = new Vector2(1.0f, 1.0f);
                uv[3] = new Vector2(1.0f, 0.0f);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
        }
    }

    public Texture2D GetTexture(int tick, Side side) {
        IniTextures();

        //Высчитываем нужный тик
        int numTick = tick % textures.Count;
        if (side == Side.face)
            return textures[numTick][0];
        else if (side == Side.back)
            return textures[numTick][1];
        else if (side == Side.left)
            return textures[numTick][2];
        else if (side == Side.right)
            return textures[numTick][3];
        else if (side == Side.up)
            return textures[numTick][4];
        else
            return textures[numTick][5];


        void IniTextures() {
            if (textures != null)
                return;


            textures = new List<Texture2D[]>();

            Color colorStart = Color.HSVToRGB(data.colorHue, data.colorSaturation, data.colorValue);
            Color colorEnd = Color.HSVToRGB(data.colorHueEnd, data.colorSaturationEnd, data.colorValueEnd);

            float offsetXStart = Random.Range(-999999, 999999);
            float offsetYStart = Random.Range(-999999, 999999);
            float offsetZStart = Random.Range(-999999, 999999);

            //Ищем минимальную скорость
            float minSpeed = 1;
            if (data.animSpeedX != 0)
                minSpeed = 1;
            else if (minSpeed > data.animSpeedY && data.animSpeedY != 0)
                minSpeed = data.animSpeedY;
            else if (minSpeed > data.animSpeedZ && data.animSpeedZ != 0);
                minSpeed = data.animSpeedZ;
            //Ищем минимальное время анимации
            float minTime = 1 / minSpeed;

            //Узнаем разницу во времени между 2-мя кадрами
            float timeOneFrame = minTime / data.texturesMax;

            float scaleX = data.perlinScale * data.perlinScaleX;
            float scaleY = data.perlinScale * data.perlinScaleY;
            float scaleZ = data.perlinScale * data.perlinScaleZ;

            //Перебираем все кадры цикла
            for (int frameNum = 0; frameNum < data.texturesMax; frameNum++) {
                float timeFrameNow = timeOneFrame * frameNum;

                float offSetX = Mathf.Sin(timeFrameNow * data.animSpeedX * Mathf.PI * 2) / data.animSizeX + offsetXStart;
                float offSetY = Mathf.Cos(timeFrameNow * data.animSpeedY * Mathf.PI * 2) / data.animSizeY + offsetYStart;
                float offSetZ = Mathf.Cos(timeFrameNow * data.animSpeedZ * Mathf.PI * 2) / data.animSizeZ + offsetZStart;

                //Получаем 3д шум
                float[,,] map = GraficData.PerlinCube.GetArrayMap(scaleX, scaleY, scaleZ, 3.0f, offSetX, offSetY, offSetZ, data.perlinOctaves);

                textures.Add(CreateTextures(map));
            }


            Texture2D[] CreateTextures(float[,,] perlin)
            {
                Texture2D[] textures = new Texture2D[6];

                textures[0] = new Texture2D(16, 16);
                textures[1] = new Texture2D(16, 16);
                textures[2] = new Texture2D(16, 16);
                textures[3] = new Texture2D(16, 16);
                textures[4] = new Texture2D(16, 16);
                textures[5] = new Texture2D(16, 16);

                //Face
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[x, y, 0];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[0].SetPixel(x, y, color);
                    }
                }
                //back
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[15 - x, y, 15];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[1].SetPixel(x, y, color);
                    }
                }
                //left
                for (int z = 0; z < perlin.GetLength(0); z++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[0, y, 15 - z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[2].SetPixel(z, y, color);
                    }
                }
                //right
                for (int z = 0; z < perlin.GetLength(0); z++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[15, y, z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[3].SetPixel(z, y, color);
                    }
                }
                //down
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int z = 0; z < perlin.GetLength(1); z++)
                    {
                        float perl = perlin[x, 0, 15 - z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[4].SetPixel(x, z, color);
                    }
                }
                //up
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int z = 0; z < perlin.GetLength(1); z++)
                    {
                        float perl = perlin[x, 15, z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[5].SetPixel(x, z, color);
                    }
                }

                //Применение изменений
                for (int num = 0; num < textures.Length; num++) {
                    textures[num].filterMode = FilterMode.Point;
                    textures[num].Apply();
                }

                return textures;
            }

            /*
            Texture2D[] CreateTextures() {
                Texture2D[] textures = new Texture2D[6];
                for (int num = 0; num < 6; num++) {
                    textures[num] = new Texture2D(16,16);

                    for (int x = 0; x < 16; x++) {
                        for (int y = 0; y < 16; y++) {
                            textures[num].SetPixel(x, y, Color.Lerp(colorStart, colorMax, Random.Range(0.0f, 1.0f)));
                        }
                    }
                    textures[num].filterMode = FilterMode.Point;
                    textures[num].Apply();
                }
                return textures;
            }
            */
        }
    }
    public void ClearTextures() {
        textures = null;
    }
    public Mesh GetMesh(Data data, bool face, bool back, bool left, bool right, bool up, bool down, int lvlUp, int lvlDown) {
        this.data = data;

        Mesh meshResult = new Mesh();

        meshResult.vertices = GraficCalc.main.mergeVector3(meshs[lvlUp, lvlDown, 0].vertices, meshs[lvlUp, lvlDown, 1].vertices, meshs[lvlUp, lvlDown, 2].vertices, meshs[lvlUp, lvlDown, 3].vertices, meshs[lvlUp, lvlDown, 4].vertices, meshs[lvlUp, lvlDown, 5].vertices);
        //meshResult.triangles = GraficCalc.main.mergeTriangleNum(wallFace.forms.triangles, wallBack.forms.triangles, wallRight.forms.triangles, wallLeft.forms.triangles, wallUp.forms.triangles, wallDown.forms.triangles);
        meshResult.uv = GraficCalc.main.mergeVector2(meshs[lvlUp, lvlDown, 0].uv, meshs[lvlUp, lvlDown, 1].uv, meshs[lvlUp, lvlDown, 2].uv, meshs[lvlUp, lvlDown, 3].uv, meshs[lvlUp, lvlDown, 4].uv, meshs[lvlUp, lvlDown, 5].uv);


        meshResult.subMeshCount = 6;
        /////////////////////////////////////////////////////////////
        //Нужно в данных треугольников надо сдвигать счет примитивов
        /////////////////////////////////////////////////////////////
        int addNum = 0;
        meshResult.SetTriangles(meshs[lvlUp, lvlDown, 0].triangles, 0);
        addNum += meshs[lvlUp, lvlDown, 0].vertices.Length;

        int[] trianglesBack = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 1].triangles, addNum);
        meshResult.SetTriangles(trianglesBack, 1);
        addNum += meshs[lvlUp, lvlDown, 1].vertices.Length;

        int[] trianglesRight = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 2].triangles, addNum);
        meshResult.SetTriangles(trianglesRight, 2);
        addNum += meshs[lvlUp, lvlDown, 2].vertices.Length;

        int[] trianglesLeft = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 3].triangles, addNum);
        meshResult.SetTriangles(trianglesLeft, 3);
        addNum += meshs[lvlUp, lvlDown, 3].vertices.Length;

        int[] trianglesUp = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 4].triangles, addNum);
        meshResult.SetTriangles(trianglesUp, 4);
        addNum += meshs[lvlUp, lvlDown, 4].vertices.Length;

        int[] trianglesDown = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 5].triangles, addNum);
        meshResult.SetTriangles(trianglesDown, 5);

        return meshResult;

    }

    public Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down, int lvlUp, int lvlDown) {
        return GetMesh(this.data, face, back, left, right, up, down, lvlUp, lvlDown);
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
