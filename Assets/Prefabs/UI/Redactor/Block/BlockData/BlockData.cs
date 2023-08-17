using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class BlockData
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
    /// <summary>
    /// Обший цвет блока
    /// </summary>
    protected Color color;

    //Рисовать соседские стены или нет
    public bool drawNeighbourWall = false;

    //Светимость
    public int lighting = 0;

    public BlockPhysics physics;

    public BlockData(BlockData blockData) {
        mod = blockData.mod;
        name = blockData.name;
        variant = blockData.variant;

        transparentType = blockData.transparentType;
        transparentPower = blockData.transparentPower;

        drawNeighbourWall = blockData.drawNeighbourWall;

        lighting = blockData.lighting;

        physics = blockData.physics;

    }

    abstract public Color GetColor();

    public enum Type {
        block = 0,
        voxels = 1,
        liquid = 2
    }

    //Получить меш куба на основе того какие стены нужно отрисовать
    public virtual Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down) {
        Mesh meshResult = new Mesh();
        return meshResult;
    }

    public BlockData() {
        //Инициализация блока по умолчанию
        physics = new BlockPhysics();
        physics.parameters = new BlockPhysics.Parameters();
    }

    //Сохранить все данные блока который отправляется
    static public void SaveData(BlockData blockData) {
        //Создаем путь к папке блоке
        string path = Game.GameData.pathMod + "/" + blockData.mod + "/" + StrC.blocks + "/" + blockData.name + "/" + blockData.variant;
         
        //Проверяем есть ли папка
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        //Сохраняем главные данные блока
        saveBlockMain(path);

        TypeBlock typeBlock = blockData as TypeBlock;
        TypeVoxel typeVoxel = blockData as TypeVoxel;
        TypeLiquid typeLiquid = blockData as TypeLiquid;

        //Сохраняем в зависимости от типа блока
        if (typeBlock != null) {
            //Сохраняем стены
            typeBlock.saveBlock(path);
        }
        //Сохраняем воксельную форму
        else if (typeVoxel != null) {
            typeVoxel.saveVoxel(path);
        }
        else if (typeLiquid != null) {
            typeLiquid.saveLiquid(path);
        }

        //Сохраняем физику
        saveBlockPhysics(path);

        void saveBlockMain(string path) {
            string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

            //Сохранить надо в текстовый файл
            //создаем список того что надо запомнить
            List<string> dataList = new List<string>();

            string dataOne = "";
            //Запоминаем тип
            dataOne = StrC.type + StrC.SEPARATOR;
            if (blockData as TypeBlock != null)
                dataOne += StrC.TBlock;
            else if (blockData as TypeVoxel != null)
                dataOne += StrC.TVoxels;
            else if (blockData as TypeLiquid != null)
                dataOne += StrC.TLiquid;

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
        void saveBlockPhysics(string pathBlock) {
            string pathPhysics = pathBlock + "/" + StrC.physics;

            blockData.physics.saveColliderZone(pathPhysics);
        }

    }
    static public BlockData LoadData(string pathBlockVariant) {
        //Если папки блока нет - выходим
        if (!Directory.Exists(pathBlockVariant))
        {
            Debug.Log(pathBlockVariant + " Not exist");
            return null;
        }

        string mod = "";
        string name = "";
        int variant = 0;
        Type type = Type.block;

        BlockData resultData;

        TypeBlock typeBlock = new TypeBlock();
        TypeVoxel typeVoxel = new TypeVoxel();
        TypeLiquid typeLiquid = new TypeLiquid();

        //Загрузка основных данных блока
        loadBlockMain(pathBlockVariant);

        if (type == Type.voxels)
        {
            typeVoxel.loadVoxel(pathBlockVariant);
            resultData = typeVoxel;
        }
        else if (type == Type.liquid) {
            typeLiquid.loadLiquid(pathBlockVariant);
            resultData = typeLiquid;
        }
        else //Type Block
        {
            typeBlock.loadBlock(pathBlockVariant);
            resultData = typeBlock;
        }

        resultData.mod = mod;
        resultData.name = name;
        resultData.variant = variant;

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

            mod = pathMass[pathMass.Length - 4];
            name = pathMass[pathMass.Length - 2];
            variant = System.Convert.ToInt32(pathMass[pathMass.Length - 1]);

            //Нужно загрузить файл с основными данными
            loadMainTXT();

            void loadMainTXT() {
                string pathMainStr = path + "/" + StrC.main + StrC.formatTXT;

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
                    string[] data = dataStr.Split(StrC.SEPARATOR);

                    if (data.Length > 2)
                    {
                        Debug.LogError("Bad parametr: " + dataStr + " in " + pathMainStr);
                        continue;
                    }

                    GetType(data[0], data[1]);
                }
                //////////////////////////////////////////////////////////////////////////////////////
                ///

                void GetType(string name, string value) {
                    if (name == StrC.type)
                    {
                        if (value == StrC.TBlock)
                            type = Type.block;
                        else if (value == StrC.TVoxels)
                            type = Type.voxels;
                        else if (value == StrC.TLiquid)
                            type = Type.liquid;
                        else
                            Debug.LogError("Bad parametr of " + name + ": " + value);

                    }
                }
            }
        }
        void loadBlockPhysics(string path)
        {
            string pathPhysics = path + "/" + StrC.physics;
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

        string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

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
        string pathFileColliders = pathPhysics + '/' + StrC.collidersZone;

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
