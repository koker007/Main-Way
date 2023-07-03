using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TypeVoxel: BlockData
{

    Visual visual;
    Data data = new Data();
    Texture2D texture = new Texture2D(16 * 16, 16);

    [System.Serializable]
    public class Data
    {
        public int[] exist = new int[16 * 16 * 16];
    }
    public class Visual
    {
        public int[] triangles;
        public Vector3[] vert;
        public Vector2[] uv;
    }

    public TypeVoxel() {
        RandomizeData();
    }
    public TypeVoxel(BlockData blockData): base(blockData) {
        
    }

    public void loadVoxel(string path) {

        string pathTVoxelForm = path + "/" + StrC.TVoxels;
        LoadFrom(pathTVoxelForm);
    }
    public void saveVoxel(string pathBlock) {
        string pathTVoxelForm = pathBlock + "/" + StrC.TVoxels;
        SaveTo(pathTVoxelForm);
    }

    void existsToAlphaTexture()
    {
        //Запихнуть существование вокселя в альфу текстуры
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    int textureX = x + z * 16;
                    int idVoxel = x + y * 16 + z * 16 * 16;

                    if (data.exist[idVoxel] == 0)
                        texture.SetPixel(textureX, y, new Color(1, 1, 1, 0));
                }
            }
        }

        texture.Apply();
    }
    void existFromAlphaTexture()
    {
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
    public void RandomizeData()
    {
        for (int num = 0; num < data.exist.Length; num++)
        {
            if (Random.Range(0, 100) < 10)
                data.exist[num] = 1;

            if (num == 0)
                data.exist[num] = 1;

            else if (num == data.exist.Length - 1)
            {
                data.exist[num] = 1;
            }
        }

        for (int x = 0; x < 16; x++)
        {
            for (int z = 0; z < 16; z++)
            {
                int posX = 16 * z + x;
                for (int y = 0; y < 16; y++)
                {
                    texture.SetPixel(posX, y, new Color(x / 16.0f, y / 16.0f, z / 16.0f));
                }
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    Mesh GetMesh(Data data)
    {
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

    public Mesh GetMesh()
    {
        return GetMesh(this.data);
    }
    public Texture2D GetTexture()
    {
        return texture;
    }
    public Data GetData()
    {
        return data;
    }

    public void SaveTo(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string pathTexture = path + "/" + StrC.texture;
        //string pathForm = path + "/" + Str.form;

        pathTexture += StrC.formatPNG;

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
    public void LoadFrom(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Path not exixt " + path);
            return;
        }

        string pathTexture = path + "/" + StrC.texture + StrC.formatPNG;
        if (!File.Exists(pathTexture))
        {
            Debug.LogError("File not exist: " + pathTexture);
            return;
        }


        byte[] textureData = File.ReadAllBytes(pathTexture);
        texture.LoadImage(textureData);
        existFromAlphaTexture();

    }
}