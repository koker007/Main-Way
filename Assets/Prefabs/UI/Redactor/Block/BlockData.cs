using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BlockData
{
    public string name;
    public string mod;

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
        //Инициализация блока по умолчанию
        physics = new BlockPhysics();
        physics.parameters = new BlockPhysics.Parameters();
    }

    static public void SaveData(BlockData blockData) {
        //Создаем путь к папке блоке
        string path = GameData.pathMod + "/" + blockData.mod + "/" + blockData.name;

        //Проверяем есть ли папка
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        
    }
    static public BlockData LoadData(string pathBlock) {
        BlockData resultData = new BlockData();
        return resultData;
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

}

public class BlockPhysics {

    public ColliderZone[] zones;
    public Parameters parameters;

    public class ColliderZone
    {
        public Vector3 pos; //Позиция старта
        public Vector3 size; //Размер относительно старта        
    }
    public class Parameters {
        float viscosity = 1; //Вязкость

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
