using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class TypeBlock: BlockData
{
    public Wall wallFace;
    public Wall wallBack;
    public Wall wallLeft;
    public Wall wallRight;
    public Wall wallUp;
    public Wall wallDown;

    public TypeBlock(BlockData blockData): base(blockData) {
        TypeBlock typeBlock = blockData as TypeBlock;

        if (typeBlock == null)
            return;

        wallFace = typeBlock.wallFace;
        wallBack = typeBlock.wallBack;
        wallLeft = typeBlock.wallLeft;
        wallRight = typeBlock.wallRight;
        wallUp = typeBlock.wallUp;
        wallDown = typeBlock.wallDown;
    }

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

            string pathWall = path + "/" + StrC.wall;
            string pathTexture = path + "/" + StrC.texture;

            if (side == Side.face)
            {
                pathWall += StrC.face;
                pathTexture += StrC.face;
            }
            else if (side == Side.back)
            {
                pathWall += StrC.back;
                pathTexture += StrC.back;
            }
            else if (side == Side.left)
            {
                pathWall += StrC.left;
                pathTexture += StrC.left;
            }
            else if (side == Side.right)
            {
                pathWall += StrC.right;
                pathTexture += StrC.right;
            }
            else if (side == Side.up)
            {
                pathWall += StrC.up;
                pathTexture += StrC.up;
            }
            else
            {
                pathWall += StrC.down;
                pathTexture += StrC.down;
            }
            pathTexture += StrC.formatPNG;

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

            string pathTexture = path + "/" + StrC.texture;
            string pathWall = path + "/" + StrC.wall;

            if (side == Side.face)
            {
                pathTexture += StrC.face;
                pathWall += StrC.face;
            }
            else if (side == Side.back)
            {
                pathTexture += StrC.back;
                pathWall += StrC.back;
            }
            else if (side == Side.left)
            {
                pathTexture += StrC.left;
                pathWall += StrC.left;
            }
            else if (side == Side.right)
            {
                pathTexture += StrC.right;
                pathWall += StrC.right;
            }
            else if (side == Side.up)
            {
                pathTexture += StrC.up;
                pathWall += StrC.up;
            }
            else
            {
                pathTexture += StrC.down;
                pathWall += StrC.down;
            }

            pathTexture += StrC.formatPNG;

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

    public TypeBlock()
    {
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

    public override Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down)
    {
        Mesh meshResult = new Mesh();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        List<Vector3> listVert = new List<Vector3>();
        List<Vector2> listUV = new List<Vector2>();
        List<Vector2> listUV2 = new List<Vector2>();

        //int addNum = 0;

        if (face)
        {
            listVert.AddRange(wallFace.forms.vertices);
            listUV.AddRange(wallFace.forms.uv);
            listUV2.AddRange(wallFace.forms.uv);
        }
        if (back)
        {
            listVert.AddRange(wallBack.forms.vertices);
            listUV.AddRange(wallBack.forms.uv);
            listUV2.AddRange(wallBack.forms.uv);
        }
        if (right)
        {
            listVert.AddRange(wallRight.forms.vertices);
            listUV.AddRange(wallRight.forms.uv);
            listUV2.AddRange(wallRight.forms.uv);
        }
        if (left)
        {
            listVert.AddRange(wallLeft.forms.vertices);
            listUV.AddRange(wallLeft.forms.uv);
            listUV2.AddRange(wallLeft.forms.uv);
        }
        if (up)
        {
            listVert.AddRange(wallUp.forms.vertices);
            listUV.AddRange(wallUp.forms.uv);
            listUV2.AddRange(wallUp.forms.uv);
        }
        if (down)
        {
            listVert.AddRange(wallDown.forms.vertices);
            listUV.AddRange(wallDown.forms.uv);
            listUV2.AddRange(wallDown.forms.uv);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////

        meshResult.vertices = listVert.ToArray();
        meshResult.uv = listUV.ToArray();
        meshResult.uv2 = listUV2.ToArray();

        meshResult.subMeshCount = 6;

        int addNum = 0;
        if (face)
        {
            meshResult.SetTriangles(wallFace.forms.triangles, 0);
            addNum += wallFace.forms.triangles.Length;
        }

        if (back)
        {
            int[] trianglesBack = Calc.Array.changeEvery(wallBack.forms.triangles, addNum);
            meshResult.SetTriangles(trianglesBack, 1);
            addNum += wallBack.forms.triangles.Length;
        }

        if (right)
        {
            int[] trianglesRight = Calc.Array.changeEvery(wallRight.forms.triangles, addNum);
            meshResult.SetTriangles(trianglesRight, 2);
            addNum += wallRight.forms.triangles.Length;
        }

        if (left)
        {
            int[] trianglesLeft = Calc.Array.changeEvery(wallLeft.forms.triangles, addNum);
            meshResult.SetTriangles(trianglesLeft, 3);
            addNum += wallLeft.forms.triangles.Length;
        }

        if (up) {
            int[] trianglesUp = Calc.Array.changeEvery(wallUp.forms.triangles, addNum);
            meshResult.SetTriangles(trianglesUp, 4);
            addNum += wallUp.forms.triangles.Length;
        }

        if (down)
        {
            int[] trianglesDown = Calc.Array.changeEvery(wallDown.forms.triangles, addNum);
            meshResult.SetTriangles(trianglesDown, 5);
        }

        stopwatch.Stop();
        Debug.Log("Stopwatch processor: " + stopwatch.ElapsedMilliseconds);

        return meshResult;
    }

    public void saveBlock(string pathBlock) {
        saveBlockWall(pathBlock);
    }

    public void loadBlock(string path) {
        loadBlockWall(path);
    }

    void saveBlockWall(string pathBlock)
    {
        string pathWalls = pathBlock + "/" + StrC.wall;

        wallFace.SaveTo(pathWalls);
        wallBack.SaveTo(pathWalls);
        wallLeft.SaveTo(pathWalls);
        wallRight.SaveTo(pathWalls);
        wallUp.SaveTo(pathWalls);
        wallDown.SaveTo(pathWalls);
    }
    void loadBlockWall(string path)
    {
        string pathWalls = path + "/" + StrC.wall;

        wallFace.LoadFrom(pathWalls);
        wallBack.LoadFrom(pathWalls);
        wallLeft.LoadFrom(pathWalls);
        wallRight.LoadFrom(pathWalls);
        wallUp.LoadFrom(pathWalls);
        wallDown.LoadFrom(pathWalls);

        wallFace.calcVertices();
        wallBack.calcVertices();
        wallRight.calcVertices();
        wallLeft.calcVertices();
        wallUp.calcVertices();
        wallDown.calcVertices();
    }
}

public class BlockForms
{
    public float[,] voxel = new float[16, 16];
    //Стена 16 на 16
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;
    public Vector2[] uvShadow;

    [System.Serializable]
    public class voxels
    {
        public float[,] height = new float[16, 16];
    }
}
