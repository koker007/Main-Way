using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using System.Diagnostics;

namespace Game
{
    namespace Space
    {
        public class ChankGO : MonoBehaviour
        {
            static List<ChankGO> buffer = new List<ChankGO>();
            //Очересь чанков требующих инициализации
            static Queue<ChankGO> inicializeQueue = new Queue<ChankGO>();

            Chank data;

            [SerializeField]
            MeshFilter meshFilterBasic;
            [SerializeField]
            MeshRenderer meshRendererBasic;
            Mesh meshColor;

            bool JobRedrawExist = false;
            JobRedraw jobRedraw;
            JobHandle jobRedrawHandle;

            static public void tryInicialize() {
                if (inicializeQueue.Count == 0)
                    return;

                ChankGO chankGO = inicializeQueue.Peek();

                    inicializeQueue.Dequeue();
            }

            static class MeshData {
                const byte sizeXTextureColor = 180;
                const byte sizeYTextureColor = 200; //183 если без соседей

                static public readonly Vector3[] vertices32;
                static readonly Vector3[] normals32;

                static readonly Vector2[] uvTexBlock;
                static public readonly Texture2D textureShadow;

                static MeshData()
                {

                    List<Vector3> vertices32List = new List<Vector3>();
                    List<Vector3> normals32List = new List<Vector3>();

                    //Перебираем все позиции блоков
                    for (int z = 0; z < Chank.Size; z++)
                    {
                        for (int y = 0; y < Chank.Size; y++)
                        {
                            for (int x = 0; x < Chank.Size; x++)
                            {
                                //1 блок 8 вершин
                                Vector3 LDB = new Vector3(x + 0, y + 0, z + 0);
                                Vector3 RDB = new Vector3(x + 1, y + 0, z + 0);

                                Vector3 LUB = new Vector3(x + 0, y + 1, z + 0);
                                Vector3 RUB = new Vector3(x + 1, y + 1, z + 0);

                                Vector3 LDF = new Vector3(x + 0, y + 0, z + 1);
                                Vector3 RDF = new Vector3(x + 1, y + 0, z + 1);

                                Vector3 LUF = new Vector3(x + 0, y + 1, z + 1);
                                Vector3 RUF = new Vector3(x + 1, y + 1, z + 1);

                                //1 блок 3 сторон и 4 вершины на каждую 
                                //Лицевая сторона внутри

                                //лево или право
                                vertices32List.Add(LDB);
                                vertices32List.Add(LUB);
                                vertices32List.Add(LUF);
                                vertices32List.Add(LDF);

                                for(int num = 0; num < 4; num++)
                                    normals32List.Add(Vector3.right);

                                //низ или верх
                                vertices32List.Add(LDB);
                                vertices32List.Add(LDF);
                                vertices32List.Add(RDF);
                                vertices32List.Add(RDB);

                                for (int num = 0; num < 4; num++)
                                    normals32List.Add(Vector3.up);

                                //зад и перед
                                vertices32List.Add(RDB);
                                vertices32List.Add(RUB);
                                vertices32List.Add(LUB);
                                vertices32List.Add(LDB);

                                for (int num = 0; num < 4; num++)
                                    normals32List.Add(Vector3.forward);
                            }
                        }
                    }

                    vertices32 = vertices32List.ToArray();
                    normals32 = vertices32List.ToArray();

                    //Создаем базовые позиции основной текстуры
                    List<Vector2> uvTextureList = new List<Vector2>();
                    uvTextureList.Add(new Vector2(0, 0));
                    uvTextureList.Add(new Vector2(0, 1));
                    uvTextureList.Add(new Vector2(1, 1));

                    uvTextureList.Add(new Vector2(1, 1));
                    uvTextureList.Add(new Vector2(1, 0));
                    uvTextureList.Add(new Vector2(0, 0));

                    uvTexBlock = uvTextureList.ToArray();

                    //Создаем текструру тени
                    textureShadow = new Texture2D(255, 1);
                    for (int x = 0; x < textureShadow.width; x++) {
                        float coof = (float)x / textureShadow.width;
                        Color color = new Color(coof, coof, coof);
                        textureShadow.SetPixel(x, 0, color);
                    }
                    textureShadow.filterMode = FilterMode.Point;
                    textureShadow.Apply();
                }

                static public void CalcMeshColor(Chank data, out Mesh mesh, out Texture2D texture2D)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    mesh = new Mesh();

                    List<Vector3> vert = new List<Vector3>();
                    List<Vector3> normals = new List<Vector3>();
                    List<int> triangles = new List<int>();
                    List<Vector2> textureUV = new List<Vector2>();


                    Texture2D texture2Dnew = new Texture2D(sizeXTextureColor, sizeYTextureColor);

                    int vertSize = 4 * 3;

                    //Перебираем все блоки
                    int blockNum = 0;
                    for (int z = 0; z < data.BlocksID.GetLength(1); z++)
                    {
                        for (int y = 0; y < data.BlocksID.GetLength(1); y++)
                        {
                            for (int x = 0; x < data.BlocksID.GetLength(0); x++, blockNum++)
                            {
                                Vector3Int pos = new Vector3Int(x, y, z);
                                Color colorThis = data.GetColor(pos);
                                Color colorLeft = data.GetColor(pos, Chank.Placement.Left);
                                Color colorDown = data.GetColor(pos, Chank.Placement.Downer);
                                Color colorBack = data.GetColor(pos, Chank.Placement.Back);

                                //получаем индекс для всех 12 вершин

                                int X1 = blockNum * vertSize;
                                int X2 = X1 + 1;
                                int X3 = X1 + 2;
                                int X4 = X1 + 3;

                                int Y1 = X1 + 4;
                                int Y2 = X1 + 5;
                                int Y3 = X1 + 6;
                                int Y4 = X1 + 7;

                                int Z1 = X1 + 8;
                                int Z2 = X1 + 9;
                                int Z3 = X1 + 10;
                                int Z4 = X1 + 11;

                                //Находим пиксель этого блока на текстуре
                                char pixX = (char)(blockNum % texture2Dnew.width);
                                char pixY = (char)(blockNum / texture2Dnew.width);

                                //Проверяем стороны
                                //делаем стенки этого блока

                                //Если слева ничего и текущий цветоблок есть (достаточно не прозрачный)
                                if (colorLeft.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(X1, X2, X3, X4, colorThis, false, pixX, pixY);
                                }
                                //Правая сторона левого куба
                                else if (colorLeft.a > 0.9f && colorThis.a <= 0.9f)
                                {
                                    int blockNumLeft = (x + y * Chank.Size + z * Chank.Size * Chank.Size);
                                    char pixXL = (char)(blockNumLeft % texture2Dnew.width);
                                    char pixYL = (char)(blockNumLeft / texture2Dnew.width);
                                    AddPlane(X1, X2, X3, X4, colorLeft, true, pixXL, pixYL);
                                }

                                //Если снизу ничего и текущий цветоблок есть (достаточно не прозрачный)
                                if (colorDown.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(Y1, Y2, Y3, Y4, colorThis, false, pixX, pixY);
                                }
                                //Верхняя сторона нижнего куба
                                else if (colorDown.a > 0.9f && colorThis.a <= 0.9f)
                                {
                                    int blockNumDown = (x + y * Chank.Size + z * Chank.Size * Chank.Size);
                                    char pixXD = (char)(blockNumDown % texture2Dnew.width);
                                    char pixYD = (char)(blockNumDown / texture2Dnew.width);
                                    AddPlane(Y1, Y2, Y3, Y4, colorDown, true, pixXD, pixYD);
                                }

                                //Если сзади ничего и текущий цветоблок есть (достаточно не прозрачный)
                                if (colorBack.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(Z1, Z2, Z3, Z4, colorThis, false, pixX, pixY);
                                }
                                //Передняя сторона заднего куба
                                else if (colorBack.a > 0.9f && colorThis.a <= 0.9f)
                                {
                                    int blockNumBack = (x + y * Chank.Size + z * Chank.Size * Chank.Size);
                                    char pixXB = (char)(blockNumBack % texture2Dnew.width);
                                    char pixYB = (char)(blockNumBack / texture2Dnew.width);
                                    AddPlane(Z1, Z2, Z3, Z4, colorBack, true, pixXB, pixYB);
                                }

                            }
                        }
                    }

                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.vertices = vert.ToArray();
                    mesh.triangles = triangles.ToArray();
                    mesh.normals = normals.ToArray();
                    //mesh.Optimize();

                    texture2Dnew.Apply();
                    texture2Dnew.filterMode = FilterMode.Point;
                    texture2D = texture2Dnew;

                    stopwatch.Stop();
                    UnityEngine.Debug.Log("GenChankGO: " + data.index +
                        " stopwatch: " + stopwatch.ElapsedMilliseconds);

                    void AddPlane(int P1, int P2, int P3, int P4, Color color, bool inside, char colorPixX, char colorPisY)
                    {
                        int vertNum1 = vert.Count;
                        int vertNum2 = vertNum1 + 1;
                        int vertNum3 = vertNum2 + 1;
                        int vertNum4 = vertNum3 + 1;

                        Vector3 vert1 = vertices32[P1];
                        Vector3 vert2 = vertices32[P2];
                        Vector3 vert3 = vertices32[P3];
                        Vector3 vert4 = vertices32[P4];

                        vert.Add(vertices32[P1]);
                        vert.Add(vertices32[P2]);
                        vert.Add(vertices32[P3]);
                        vert.Add(vertices32[P4]);

                        //Вставляем цвет
                        texture2Dnew.SetPixel(colorPixX, colorPisY, new Color(color.a, color.g, color.b));

                        //Вычисляем текстурные координаты
                        float posUVX = colorPixX / (float)texture2Dnew.width;
                        float posUVY = colorPisY / (float)texture2Dnew.width;
                        float posUVXP = posUVX + (1 / (float)texture2Dnew.width);
                        float posUVYP = posUVY + (1 / (float)texture2Dnew.height);

                        textureUV.Add(new Vector2(posUVX, posUVY));
                        textureUV.Add(new Vector2(posUVX, posUVYP));
                        textureUV.Add(new Vector2(posUVXP, posUVYP));
                        textureUV.Add(new Vector2(posUVXP, posUVY));


                        if (inside)
                        {
                            triangles.Add(vertNum1);
                            triangles.Add(vertNum2);
                            triangles.Add(vertNum3);

                            triangles.Add(vertNum3);
                            triangles.Add(vertNum4);
                            triangles.Add(vertNum1);

                            normals.Add(normals32[P1]);
                            normals.Add(normals32[P2]);
                            normals.Add(normals32[P3]);
                            normals.Add(normals32[P4]);
                        }
                        else
                        {
                            triangles.Add(vertNum1);
                            triangles.Add(vertNum4);
                            triangles.Add(vertNum3);

                            triangles.Add(vertNum3);
                            triangles.Add(vertNum2);
                            triangles.Add(vertNum1);

                            normals.Add(normals32[P1] * -1);
                            normals.Add(normals32[P2] * -1);
                            normals.Add(normals32[P3] * -1);
                            normals.Add(normals32[P4] * -1);
                        }
                    }
                }

                /*
                public static void CalcMeshColorShader(Chank data, out Mesh mesh, out Texture2D texture, in Grafic.Calc.MeshChankColor2 graficMesh) {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    mesh = graficMesh.dataMesh.mesh;

                    graficMesh.calculate(data);

                    //PlusNeibourChanksData(ref graficMesh.dataMesh, data);

                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.vertices = vertices32;
                    mesh.triangles = graficMesh.dataMesh.triangles;
                    mesh.normals = graficMesh.dataMesh.normals;
                    mesh.uv = graficMesh.dataMesh.uv;
                    mesh.uv2 = graficMesh.dataMesh.uv2;

                    texture = graficMesh.dataMesh.mainTexture;

                    stopwatch.Stop();
                    UnityEngine.Debug.Log("GenChankGO: " + data.index +
                        " stopwatch: " + stopwatch.ElapsedMilliseconds);
                }
                */
                //Применить изменения с учетом данных о соседних чанках
                /*
                static public void PlusNeibourChanksData(ref Grafic.Data.MeshChankColor meshData,in Chank data) {

                    byte xEnd = (byte)(data.Colors.GetLength(0) - 1);
                    byte yEnd = (byte)(data.Colors.GetLength(0) - 1);
                    byte zEnd = (byte)(data.Colors.GetLength(0) - 1);

                    //Получаем соседей
                    Chank chank = data;
                    Chank left = data.GetNeighbour(Side.left);
                    Chank right = data.GetNeighbour(Side.right);
                    Chank down = data.GetNeighbour(Side.down);
                    Chank up = data.GetNeighbour(Side.up);
                    Chank back = data.GetNeighbour(Side.back);
                    Chank forward = data.GetNeighbour(Side.face);

                    int indexMax = 32 * 32 * 32;

                    //изменяем текстуру и меш
                    for (int x = 0; x < 32; x++) {
                        for (int y = 0; y < 32; y++) {
                            for (int z = 0; z < 32; z++) {
                                if (z < 31 && x > 0 && x < 31 && y > 0 && y < 31 && z > 0)
                                {
                                    break;
                                }

                                DrawBlock(ref meshData, x,y,z);
                            }
                        }
                    }
                    meshData.mainTexture.Apply();

                    void DrawBlock(ref Grafic.Data.MeshChankColor meshData, int x, int y, int z)
                    {
                        //get global index
                        int index = x + y * 32 + z * 32 * 32;

                        Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                        if (index >= 0 && index < indexMax)
                        {
                            color = chank.Colors[x,y,z];
                        }

                        //get color if Neibour exist
                        Color colorL, colorD, colorB;
                        Vector3Int pos = new Vector3Int(x,y,z);
                        Vector3Int posL = new Vector3Int(pos.x - 1, pos.y, pos.z);
                        Vector3Int posD = new Vector3Int(pos.x, pos.y - 1, pos.z);
                        Vector3Int posB = new Vector3Int(pos.x, pos.y, pos.z - 1);

                        getColorBlock(out colorL, posL);
                        getColorBlock(out colorD, posD);
                        getColorBlock(out colorB, posB);

                        if (color.a > 0.9f && colorD.a > 0.9f && x >= 0 && y >= 0 && z == 31 && forward == null)
                        {
                            bool test = false;
                        }

                        int num = x + y * 32 + z * 32 * 32;
                        int numVertStart = num * 4 * 3;
                        int numTrianStart = num * 3 * 2 * 3;

                        Vector2 texturePos = new Vector2(num % sizeXTextureColor, num / sizeXTextureColor);
                        float uvXsize = 1.0f / sizeXTextureColor;
                        float uvYsize = 1.0f / sizeYTextureColor;
                        float uvXnow = texturePos.x * uvXsize;
                        float uvYnow = texturePos.y * uvYsize;


                        //Left, Down, Back
                        if (color.a > 0.9f)
                        {

                            //Left
                            if (colorL.a <= 0.9f && pos.x == 0
                                )
                            {
                                meshData.triangles[numTrianStart + 0] = numVertStart;
                                meshData.triangles[numTrianStart + 1] = numVertStart + 3;
                                meshData.triangles[numTrianStart + 2] = numVertStart + 2;

                                meshData.triangles[numTrianStart + 3] = numVertStart + 2;
                                meshData.triangles[numTrianStart + 4] = numVertStart + 1;
                                meshData.triangles[numTrianStart + 5] = numVertStart;

                                for (byte numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm] = new Vector3(-1.0f, 0.0f, 0.0f);

                                meshData.uv[numVertStart + 0] = new Vector2(uvXnow, uvYnow);
                                meshData.uv[numVertStart + 1] = new Vector2(uvXnow, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 2] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 3] = new Vector2(uvXnow + uvXsize, uvYnow);

                                meshData.uv2[numVertStart + 0] = new Vector2(getLightL_DB(posL), 0);
                                meshData.uv2[numVertStart + 1] = new Vector2(getLightL_UB(posL), 1);
                                meshData.uv2[numVertStart + 2] = new Vector2(getLightL_UF(posL), 1);
                                meshData.uv2[numVertStart + 3] = new Vector2(getLightL_DF(posL), 0);
                            }
                            //Down
                            if (colorD.a <= 0.9f && pos.y == 0
                                )
                            {
                                meshData.triangles[numTrianStart + 6 + 0] = numVertStart + 4;
                                meshData.triangles[numTrianStart + 6 + 1] = numVertStart + 4 + 3;
                                meshData.triangles[numTrianStart + 6 + 2] = numVertStart + 4 + 2;

                                meshData.triangles[numTrianStart + 6 + 3] = numVertStart + 4 + 2;
                                meshData.triangles[numTrianStart + 6 + 4] = numVertStart + 4 + 1;
                                meshData.triangles[numTrianStart + 6 + 5] = numVertStart + 4;

                                for (int numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm + 4] = new Vector3(0.0f, -1.0f, 0.0f);

                                meshData.uv[numVertStart + 0 + 4] = new Vector2(uvXnow, uvYnow);
                                meshData.uv[numVertStart + 1 + 4] = new Vector2(uvXnow, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 2 + 4] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 3 + 4] = new Vector2(uvXnow + uvXsize, uvYnow);

                                meshData.uv2[numVertStart + 0 + 4] = new Vector2(getLightD_LB(posD), 0);
                                meshData.uv2[numVertStart + 1 + 4] = new Vector2(getLightD_LF(posD), 1);
                                meshData.uv2[numVertStart + 2 + 4] = new Vector2(getLightD_RF(posD), 1);
                                meshData.uv2[numVertStart + 3 + 4] = new Vector2(getLightD_RB(posD), 0);
                            }
                            //Back
                            if (colorB.a <= 0.9f && pos.z == 0
                                )
                            {
                                meshData.triangles[numTrianStart + 12 + 0] = numVertStart + 8;
                                meshData.triangles[numTrianStart + 12 + 1] = numVertStart + 8 + 3;
                                meshData.triangles[numTrianStart + 12 + 2] = numVertStart + 8 + 2;

                                meshData.triangles[numTrianStart + 12 + 3] = numVertStart + 8 + 2;
                                meshData.triangles[numTrianStart + 12 + 4] = numVertStart + 8 + 1;
                                meshData.triangles[numTrianStart + 12 + 5] = numVertStart + 8;

                                for (int numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm + 8] = new Vector3(0.0f, 0.0f, -1.0f);

                                meshData.uv[numVertStart + 0 + 8] = new Vector2(uvXnow, uvYnow);
                                meshData.uv[numVertStart + 1 + 8] = new Vector2(uvXnow, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 2 + 8] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                                meshData.uv[numVertStart + 3 + 8] = new Vector2(uvXnow + uvXsize, uvYnow);

                                meshData.uv2[numVertStart + 0 + 8] = new Vector2(getLightB_RD(posB), 0);
                                meshData.uv2[numVertStart + 1 + 8] = new Vector2(getLightB_RU(posB), 1);
                                meshData.uv2[numVertStart + 2 + 8] = new Vector2(getLightB_LU(posB), 1);
                                meshData.uv2[numVertStart + 3 + 8] = new Vector2(getLightB_LD(posB), 0);
                            }

                        }
                        //Neibour
                        else
                        {
                            //Left
                            if (colorL.a > 0.9f)
                            {
                                meshData.triangles[numTrianStart + 0] = numVertStart;
                                meshData.triangles[numTrianStart + 1] = numVertStart + 1;
                                meshData.triangles[numTrianStart + 2] = numVertStart + 2;

                                meshData.triangles[numTrianStart + 3] = numVertStart + 2;
                                meshData.triangles[numTrianStart + 4] = numVertStart + 3;
                                meshData.triangles[numTrianStart + 5] = numVertStart;

                                for (int numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm] = new Vector3(1.0f, 0.0f, 0.0f);

                                int numL = 0;
                                if (x != 0) numL = (x - 1) + y * 32 + z * 32 * 32;
                                else {
                                    numL = indexMax + y + z * 32;
                                }

                                Vector2Int texturePosL = new Vector2Int(numL % sizeXTextureColor, numL / sizeXTextureColor);
                                float uvXL = texturePosL.x * uvXsize;
                                float uvYL = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0] = new Vector2(uvXL, uvYL);
                                meshData.uv[numVertStart + 1] = new Vector2(uvXL, uvYL + uvYsize);
                                meshData.uv[numVertStart + 2] = new Vector2(uvXL + uvXsize, uvYL + uvYsize);
                                meshData.uv[numVertStart + 3] = new Vector2(uvXL + uvXsize, uvYL);

                                if (x == 0) {
                                    meshData.mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorL.r, colorL.g, colorL.b, 1.0f));
                                }

                                meshData.uv2[numVertStart + 0] = new Vector2(getLightL_DB(pos), 0);
                                meshData.uv2[numVertStart + 1] = new Vector2(getLightL_UB(pos), 1);
                                meshData.uv2[numVertStart + 2] = new Vector2(getLightL_UF(pos), 1);
                                meshData.uv2[numVertStart + 3] = new Vector2(getLightL_DF(pos), 0);
                            }
                            else
                            {
                                for (int numNorm = 0; numNorm < 6; numNorm++)
                                    meshData.triangles[numTrianStart + numNorm] = 0;
                            }
                            //Down
                            if (colorD.a > 0.9f)
                            {
                                meshData.triangles[numTrianStart + 6 + 0] = numVertStart + 4;
                                meshData.triangles[numTrianStart + 6 + 1] = numVertStart + 4 + 1;
                                meshData.triangles[numTrianStart + 6 + 2] = numVertStart + 4 + 2;

                                meshData.triangles[numTrianStart + 6 + 3] = numVertStart + 4 + 2;
                                meshData.triangles[numTrianStart + 6 + 4] = numVertStart + 4 + 3;
                                meshData.triangles[numTrianStart + 6 + 5] = numVertStart + 4;

                                for (int numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm + 4] = new Vector3(0.0f, 1.0f, 0.0f);

                                int numD = 0;
                                if (y != 0) numD = x + (y - 1) * 32 + z * 32 * 32;
                                else
                                {
                                    numD = indexMax + 32 * 32 + x + z * 32;
                                }

                                Vector2Int texturePosL = new Vector2Int(numD % sizeXTextureColor, numD / sizeXTextureColor);
                                float uvXD = texturePosL.x * uvXsize;
                                float uvYD = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0 + 4] = new Vector2(uvXD, uvYD);
                                meshData.uv[numVertStart + 1 + 4] = new Vector2(uvXD, uvYD + uvYsize);
                                meshData.uv[numVertStart + 2 + 4] = new Vector2(uvXD + uvXsize, uvYD + uvYsize);
                                meshData.uv[numVertStart + 3 + 4] = new Vector2(uvXD + uvXsize, uvYD);

                                if (y == 0)
                                    meshData.mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorD.r, colorD.g, colorD.b, 1.0f));

                                meshData.uv2[numVertStart + 0 + 4] = new Vector2(getLightD_LB(pos), 0);
                                meshData.uv2[numVertStart + 1 + 4] = new Vector2(getLightD_LF(pos), 1);
                                meshData.uv2[numVertStart + 2 + 4] = new Vector2(getLightD_RF(pos), 1);
                                meshData.uv2[numVertStart + 3 + 4] = new Vector2(getLightD_RB(pos), 0);
                            }
                            else
                            {
                                for (int numNorm = 0; numNorm < 6; numNorm++)
                                    meshData.triangles[numTrianStart + numNorm + 6] = 0;
                            }

                            //Back
                            if (colorB.a > 0.9f)
                            {
                                meshData.triangles[numTrianStart + 12 + 0] = numVertStart + 8;
                                meshData.triangles[numTrianStart + 12 + 1] = numVertStart + 8 + 1;
                                meshData.triangles[numTrianStart + 12 + 2] = numVertStart + 8 + 2;

                                meshData.triangles[numTrianStart + 12 + 3] = numVertStart + 8 + 2;
                                meshData.triangles[numTrianStart + 12 + 4] = numVertStart + 8 + 3;
                                meshData.triangles[numTrianStart + 12 + 5] = numVertStart + 8;

                                for (int numNorm = 0; numNorm < 4; numNorm++)
                                    meshData.normals[numVertStart + numNorm + 8] = new Vector3(0.0f, 0.0f, 1.0f);

                                int numB = 0;
                                if (z != 0) numB = x + y * 32 + (z - 1) * 32 * 32;
                                else
                                {
                                    numB = indexMax + 32 * 32 * 2 + x + y * 32;
                                }

                                Vector2Int texturePosL = new Vector2Int(numB % sizeXTextureColor, numB / sizeXTextureColor);
                                float uvXB = texturePosL.x * uvXsize;
                                float uvYB = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0 + 8] = new Vector2(uvXB, uvYB);
                                meshData.uv[numVertStart + 1 + 8] = new Vector2(uvXB, uvYB + uvYsize);
                                meshData.uv[numVertStart + 2 + 8] = new Vector2(uvXB + uvXsize, uvYB + uvYsize);
                                meshData.uv[numVertStart + 3 + 8] = new Vector2(uvXB + uvXsize, uvYB);

                                if (z == 0)
                                    meshData.mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorB.r, colorB.g, colorB.b, 1.0f));

                                meshData.uv2[numVertStart + 0 + 8] = new Vector2(getLightB_RD(pos), 0);
                                meshData.uv2[numVertStart + 1 + 8] = new Vector2(getLightB_RU(pos), 1);
                                meshData.uv2[numVertStart + 2 + 8] = new Vector2(getLightB_LU(pos), 1);
                                meshData.uv2[numVertStart + 3 + 8] = new Vector2(getLightB_LD(pos), 0);
                            }
                            else
                            {
                                for (int numNorm = 0; numNorm < 6; numNorm++)
                                    meshData.triangles[numTrianStart + numNorm + 12] = 0;
                            }
                        }
                    }

                    void getColorBlock(out Color color, Vector3Int pos) {
                        //Left
                        if (pos.x < 0)
                        {
                            if (left == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = left.Colors[pos.x + Chank.Size, pos.y, pos.z];
                        }
                        //Right
                        else if (pos.x >= Chank.Size)
                        {
                            if (right == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = right.Colors[pos.x - Chank.Size, pos.y, pos.z];
                        }
                        //Down
                        else if (pos.y < 0)
                        {
                            if (down == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = down.Colors[pos.x, pos.y + Chank.Size, pos.z];
                        }
                        //Up
                        else if (pos.y >= Chank.Size)
                        {
                            if (up == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = up.Colors[pos.x, pos.y - Chank.Size, pos.z];
                        }
                        //Down
                        else if (pos.z < 0)
                        {
                            if (back == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = back.Colors[pos.x, pos.y, pos.z + Chank.Size];
                        }
                        //Up
                        else if (pos.z >= Chank.Size)
                        {
                            if (forward == null)
                                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                            else
                                color = forward.Colors[pos.x, pos.y, pos.z - Chank.Size];
                        }
                        else {
                            color = chank.Colors[pos.x, pos.y, pos.z];
                        }

                    }

                    float calcShadow4(in Color c1, in float s1, in Color c2, in float s2, in Color c3, in float s3, in Color c4, in float s4) {
                        float result = 0.001f;

                        byte count = 0;
                        if (c1.a <= 0.9) {
                            count++;
                            result += s1;
                        }
                        if (c2.a <= 0.9) {
                            count++;
                            result += s2;
                        }
                        if (c3.a <= 0.9f && c2.a <= 0.9f && c4.a <= 0.9f) {
                            count++;
                            result += s3;
                        }
                        if (c4.a <= 0.9)
                        {
                            count++;
                            result += s4;
                        }

                        if (count > 0)
                            result /= count;
                        else result = 0.99f;

                        return result;
                    }
                    void getColorAndLight(Vector3Int pos, out Color color, out float light) {
                        color = new Color(0, 0, 0, 0);
                        light = 0.99f;

                        byte countOutRange = 0;
                        if (pos.x < 0 || pos.x > 31)
                            countOutRange++;
                        if (pos.y < 0 || pos.y > 31)
                            countOutRange++;
                        if (pos.z < 0 || pos.z > 31)
                            countOutRange++;

                        if (countOutRange > 1)
                            return;

                        if (pos.x < 0)
                        {
                            if (left == null)
                                return;

                            color = left.Colors[31, pos.y, pos.z];
                            light = left.Light[31, pos.y, pos.z];
                        }
                        else if (pos.x > 31)
                        {
                            if (right == null)
                                return;
                            color = right.Colors[0, pos.y, pos.z];
                            light = right.Light[0, pos.y, pos.z];
                        }
                        else if (pos.y < 0)
                        {
                            if (down == null)
                                return;
                            color = down.Colors[pos.x, 31, pos.z];
                            light = down.Light[pos.x, 31, pos.z];
                        }
                        else if (pos.y > 31)
                        {
                            if (up == null)
                                return;
                            color = up.Colors[pos.x, 0, pos.z];
                            light = up.Light[pos.x, 0, pos.z];
                        }
                        else if (pos.z < 0)
                        {
                            if (back == null)
                                return;
                            color = back.Colors[pos.x, pos.y, 31];
                            light = back.Light[pos.x, pos.y, 31];
                        }
                        else if (pos.z > 31)
                        {
                            if (forward == null)
                                return;
                            color = forward.Colors[pos.x, pos.y, 0];
                            light = forward.Light[pos.x, pos.y, 0];
                        }
                        else  {
                            color = chank.Colors[pos.x, pos.y, pos.z];
                            light = chank.Light[pos.x, pos.y, pos.z];
                        }


                    }
                    float getLightL_DB(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z - 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }
                    float getLightL_UB(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x, pos.y+1, pos.z - 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x, pos.y+1, pos.z), out c4, out s4);

                        result = calcShadow4(c1,s1,c2,s2,c3,s3,c4,s4);

                        return result;
                    }
                    float getLightL_UF(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z + 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }
                    float getLightL_DF(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z + 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }

                    float getLightD_LB(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z - 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }
                    float getLightD_LF(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z + 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }
                    float getLightD_RF(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z + 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }
                    float getLightD_RB(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z - 1), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                        return result;
                    }

                    float getLightB_LD(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y - 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);
                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                        return result;
                    }
                    float getLightB_LU(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y + 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);
                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                        return result;
                    }
                    float getLightB_RU(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y + 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);
                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                        return result;
                    }
                    float getLightB_RD(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y - 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);
                        result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                        return result;
                    }
                }
                */
            }

            struct JobRedraw : IJob
            {
                //ChankGO chankGO;
                const byte sizeXTextureColor = 180;
                const byte sizeYTextureColor = 200; //183 если без соседей

                const int indexMax = 32 * 32 * 32;
                const int countSide = 32 * 32;
                const int countTriangles = indexMax * 3 * 2 * 3;
                const int countVert = indexMax * 3 * 4;

                public NativeArray<int> triangles;
                public NativeArray<Vector2> uvMain;
                public NativeArray<Vector2> uvLight;
                public NativeArray<Vector3> normals;

                public NativeArray<Color> colors;
                public NativeArray<float> lights;
                public NativeArray<Color> neighbourColors;
                public NativeArray<float> neighbourLight;

                public JobRedraw(NativeArray<Color> colors, NativeArray<float> lights, NativeArray<Color> neighbourColors, NativeArray<float> neighbourLight)
                {
                    triangles = new NativeArray<int>(countTriangles, Allocator.TempJob);
                    uvMain = new NativeArray<Vector2>(countVert, Allocator.TempJob);
                    uvLight = new NativeArray<Vector2>(countVert, Allocator.TempJob);
                    normals = new NativeArray<Vector3>(countVert, Allocator.TempJob);

                    this.colors = colors;
                    this.lights = lights;
                    this.neighbourColors = neighbourColors;
                    this.neighbourLight = neighbourLight;

                }

                void calcMesh() {
                    for (byte x = 0; x < 32; x++) {
                        for (byte y = 0; y < 32; y++) {
                            for(byte z = 0; z < 32; z++)
                                calcBlock(x,y,z);
                        }
                    }
                }

                void calcBlock(byte x, byte y, byte z) {
                    //get global index
                    int index = x + y * 32 + z * 32 * 32;

                    Color color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    if (index >= 0 && index < indexMax)
                    {
                        color = colors[index];
                    }

                    if (color.a > 0.9 && x >= 31) {
                        bool test = true;
                    }

                    //get color if Neibour exist
                    Color colorL, colorD, colorB;

                    Vector3Int pos = new Vector3Int(x, y, z);
                    Vector3Int posL = new Vector3Int(pos.x - 1, pos.y, pos.z);
                    Vector3Int posD = new Vector3Int(pos.x, pos.y - 1, pos.z);
                    Vector3Int posB = new Vector3Int(pos.x, pos.y, pos.z - 1);

                    getDataColor(posL, out colorL);
                    getDataColor(posD, out colorD);
                    getDataColor(posB, out colorB);

                    int num = x + y * 32 + z * 32 * 32;
                    int numVertStart = num * 4 * 3;
                    int numTrianStart = num * 3 * 2 * 3;

                    Vector2 texturePos = new Vector2(num % sizeXTextureColor, num / sizeXTextureColor);
                    float uvXsize = 1.0f / sizeXTextureColor;
                    float uvYsize = 1.0f / sizeYTextureColor;
                    float uvXnow = texturePos.x * uvXsize;
                    float uvYnow = texturePos.y * uvYsize;


                    //Left, Down, Back
                    if (color.a > 0.9f)
                    {

                        //Left
                        if (colorL.a <= 0.9f
                            )
                        {
                            triangles[numTrianStart + 0] = numVertStart;
                            triangles[numTrianStart + 1] = numVertStart + 3;
                            triangles[numTrianStart + 2] = numVertStart + 2;

                            triangles[numTrianStart + 3] = numVertStart + 2;
                            triangles[numTrianStart + 4] = numVertStart + 1;
                            triangles[numTrianStart + 5] = numVertStart;

                            for (byte numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm] = new Vector3(-1.0f, 0.0f, 0.0f);

                            uvMain[numVertStart + 0] = new Vector2(uvXnow, uvYnow);
                            uvMain[numVertStart + 1] = new Vector2(uvXnow, uvYnow + uvYsize);
                            uvMain[numVertStart + 2] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                            uvMain[numVertStart + 3] = new Vector2(uvXnow + uvXsize, uvYnow);

                            uvLight[numVertStart + 0] = new Vector2(getLightL_DB(posL), 0);
                            uvLight[numVertStart + 1] = new Vector2(getLightL_UB(posL), 1);
                            uvLight[numVertStart + 2] = new Vector2(getLightL_UF(posL), 1);
                            uvLight[numVertStart + 3] = new Vector2(getLightL_DF(posL), 0);
                        }
                        //Down
                        if (colorD.a <= 0.9f
                            )
                        {
                            triangles[numTrianStart + 6 + 0] = numVertStart + 4;
                            triangles[numTrianStart + 6 + 1] = numVertStart + 4 + 3;
                            triangles[numTrianStart + 6 + 2] = numVertStart + 4 + 2;

                            triangles[numTrianStart + 6 + 3] = numVertStart + 4 + 2;
                            triangles[numTrianStart + 6 + 4] = numVertStart + 4 + 1;
                            triangles[numTrianStart + 6 + 5] = numVertStart + 4;

                            for (int numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm + 4] = new Vector3(0.0f, -1.0f, 0.0f);

                            uvMain[numVertStart + 0 + 4] = new Vector2(uvXnow, uvYnow);
                            uvMain[numVertStart + 1 + 4] = new Vector2(uvXnow, uvYnow + uvYsize);
                            uvMain[numVertStart + 2 + 4] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                            uvMain[numVertStart + 3 + 4] = new Vector2(uvXnow + uvXsize, uvYnow);

                            uvLight[numVertStart + 0 + 4] = new Vector2(getLightD_LB(posD), 0);
                            uvLight[numVertStart + 1 + 4] = new Vector2(getLightD_LF(posD), 1);
                            uvLight[numVertStart + 2 + 4] = new Vector2(getLightD_RF(posD), 1);
                            uvLight[numVertStart + 3 + 4] = new Vector2(getLightD_RB(posD), 0);
                        }
                        //Back
                        if (colorB.a <= 0.9f
                            )
                        {
                            triangles[numTrianStart + 12 + 0] = numVertStart + 8;
                            triangles[numTrianStart + 12 + 1] = numVertStart + 8 + 3;
                            triangles[numTrianStart + 12 + 2] = numVertStart + 8 + 2;

                            triangles[numTrianStart + 12 + 3] = numVertStart + 8 + 2;
                            triangles[numTrianStart + 12 + 4] = numVertStart + 8 + 1;
                            triangles[numTrianStart + 12 + 5] = numVertStart + 8;

                            for (int numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm + 8] = new Vector3(0.0f, 0.0f, -1.0f);

                            uvMain[numVertStart + 0 + 8] = new Vector2(uvXnow, uvYnow);
                            uvMain[numVertStart + 1 + 8] = new Vector2(uvXnow, uvYnow + uvYsize);
                            uvMain[numVertStart + 2 + 8] = new Vector2(uvXnow + uvXsize, uvYnow + uvYsize);
                            uvMain[numVertStart + 3 + 8] = new Vector2(uvXnow + uvXsize, uvYnow);

                            uvLight[numVertStart + 0 + 8] = new Vector2(getLightB_RD(posB), 0);
                            uvLight[numVertStart + 1 + 8] = new Vector2(getLightB_RU(posB), 1);
                            uvLight[numVertStart + 2 + 8] = new Vector2(getLightB_LU(posB), 1);
                            uvLight[numVertStart + 3 + 8] = new Vector2(getLightB_LD(posB), 0);
                        }

                    }
                    //Neibour
                    else
                    {
                        //Left
                        if (colorL.a > 0.9f)
                        {
                            triangles[numTrianStart + 0] = numVertStart;
                            triangles[numTrianStart + 1] = numVertStart + 1;
                            triangles[numTrianStart + 2] = numVertStart + 2;

                            triangles[numTrianStart + 3] = numVertStart + 2;
                            triangles[numTrianStart + 4] = numVertStart + 3;
                            triangles[numTrianStart + 5] = numVertStart;

                            for (int numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm] = new Vector3(1.0f, 0.0f, 0.0f);

                            int numL = 0;
                            if (x != 0) numL = (x - 1) + y * 32 + z * 32 * 32;
                            else
                            {
                                numL = indexMax + y + z * 32;
                            }

                            Vector2Int texturePosL = new Vector2Int(numL % sizeXTextureColor, numL / sizeXTextureColor);
                            float uvXL = texturePosL.x * uvXsize;
                            float uvYL = texturePosL.y * uvYsize;
                            uvMain[numVertStart + 0] = new Vector2(uvXL, uvYL);
                            uvMain[numVertStart + 1] = new Vector2(uvXL, uvYL + uvYsize);
                            uvMain[numVertStart + 2] = new Vector2(uvXL + uvXsize, uvYL + uvYsize);
                            uvMain[numVertStart + 3] = new Vector2(uvXL + uvXsize, uvYL);

                            if (x == 0)
                            {
                                //mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorL.r, colorL.g, colorL.b, 1.0f));
                            }

                            uvLight[numVertStart + 0] = new Vector2(getLightL_DB(pos), 0);
                            uvLight[numVertStart + 1] = new Vector2(getLightL_UB(pos), 1);
                            uvLight[numVertStart + 2] = new Vector2(getLightL_UF(pos), 1);
                            uvLight[numVertStart + 3] = new Vector2(getLightL_DF(pos), 0);
                        }
                        else
                        {
                            for (int numNorm = 0; numNorm < 6; numNorm++)
                                triangles[numTrianStart + numNorm] = 0;
                        }
                        //Down
                        if (colorD.a > 0.9f)
                        {
                            triangles[numTrianStart + 6 + 0] = numVertStart + 4;
                            triangles[numTrianStart + 6 + 1] = numVertStart + 4 + 1;
                            triangles[numTrianStart + 6 + 2] = numVertStart + 4 + 2;

                            triangles[numTrianStart + 6 + 3] = numVertStart + 4 + 2;
                            triangles[numTrianStart + 6 + 4] = numVertStart + 4 + 3;
                            triangles[numTrianStart + 6 + 5] = numVertStart + 4;

                            for (int numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm + 4] = new Vector3(0.0f, 1.0f, 0.0f);

                            int numD = 0;
                            if (y != 0) numD = x + (y - 1) * 32 + z * 32 * 32;
                            else
                            {
                                numD = indexMax + 32 * 32 + x + z * 32;
                            }

                            Vector2Int texturePosL = new Vector2Int(numD % sizeXTextureColor, numD / sizeXTextureColor);
                            float uvXD = texturePosL.x * uvXsize;
                            float uvYD = texturePosL.y * uvYsize;
                            uvMain[numVertStart + 0 + 4] = new Vector2(uvXD, uvYD);
                            uvMain[numVertStart + 1 + 4] = new Vector2(uvXD, uvYD + uvYsize);
                            uvMain[numVertStart + 2 + 4] = new Vector2(uvXD + uvXsize, uvYD + uvYsize);
                            uvMain[numVertStart + 3 + 4] = new Vector2(uvXD + uvXsize, uvYD);

                            if (y == 0)
                            {
                                //mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorD.r, colorD.g, colorD.b, 1.0f));
                            }
                            uvLight[numVertStart + 0 + 4] = new Vector2(getLightD_LB(pos), 0);
                            uvLight[numVertStart + 1 + 4] = new Vector2(getLightD_LF(pos), 1);
                            uvLight[numVertStart + 2 + 4] = new Vector2(getLightD_RF(pos), 1);
                            uvLight[numVertStart + 3 + 4] = new Vector2(getLightD_RB(pos), 0);
                        }
                        else
                        {
                            for (int numNorm = 0; numNorm < 6; numNorm++)
                                triangles[numTrianStart + numNorm + 6] = 0;
                        }

                        //Back
                        if (colorB.a > 0.9f)
                        {
                            triangles[numTrianStart + 12 + 0] = numVertStart + 8;
                            triangles[numTrianStart + 12 + 1] = numVertStart + 8 + 1;
                            triangles[numTrianStart + 12 + 2] = numVertStart + 8 + 2;

                            triangles[numTrianStart + 12 + 3] = numVertStart + 8 + 2;
                            triangles[numTrianStart + 12 + 4] = numVertStart + 8 + 3;
                            triangles[numTrianStart + 12 + 5] = numVertStart + 8;

                            for (int numNorm = 0; numNorm < 4; numNorm++)
                                normals[numVertStart + numNorm + 8] = new Vector3(0.0f, 0.0f, 1.0f);

                            int numB = 0;
                            if (z != 0) numB = x + y * 32 + (z - 1) * 32 * 32;
                            else
                            {
                                numB = indexMax + 32 * 32 * 2 + x + y * 32;
                            }

                            Vector2Int texturePosL = new Vector2Int(numB % sizeXTextureColor, numB / sizeXTextureColor);
                            float uvXB = texturePosL.x * uvXsize;
                            float uvYB = texturePosL.y * uvYsize;
                            uvMain[numVertStart + 0 + 8] = new Vector2(uvXB, uvYB);
                            uvMain[numVertStart + 1 + 8] = new Vector2(uvXB, uvYB + uvYsize);
                            uvMain[numVertStart + 2 + 8] = new Vector2(uvXB + uvXsize, uvYB + uvYsize);
                            uvMain[numVertStart + 3 + 8] = new Vector2(uvXB + uvXsize, uvYB);

                            if (z == 0)
                            {
                                //mainTexture.SetPixel(texturePosL.x, texturePosL.y, new Color(colorB.r, colorB.g, colorB.b, 1.0f));
                            }

                            uvLight[numVertStart + 0 + 8] = new Vector2(getLightB_RD(pos), 0);
                            uvLight[numVertStart + 1 + 8] = new Vector2(getLightB_RU(pos), 1);
                            uvLight[numVertStart + 2 + 8] = new Vector2(getLightB_LU(pos), 1);
                            uvLight[numVertStart + 3 + 8] = new Vector2(getLightB_LD(pos), 0);
                        }
                        else
                        {
                            for (int numNorm = 0; numNorm < 6; numNorm++)
                                triangles[numTrianStart + numNorm + 12] = 0;
                        }
                    }
                }

                void getDataColor(Vector3Int pos, out Color color) {

                    byte neightbours = 0;
                    Side side = Side.left;

                    if (pos.x < 0) {
                        neightbours++;
                        side = Side.left;
                    }
                    else if (pos.x > 31)
                    {
                        neightbours++;
                        side = Side.right;
                    }

                    if (pos.y < 0)
                    {
                        neightbours++;
                        side = Side.down;
                    }
                    else if (pos.y > 31)
                    {
                        neightbours++;
                        side = Side.up;
                    }

                    if (pos.z < 0)
                    {
                        neightbours++;
                        side = Side.back;
                    }
                    else if (pos.z > 31)
                    {
                        neightbours++;
                        side = Side.face;
                    }

                    //Данные из этого чанка
                    if (neightbours == 0)
                    {
                        int index = pos.x + pos.y * 32 + pos.z * 32 * 32;
                        color = colors[index];
                    }
                    //Данные какого-то соседа
                    else if (neightbours == 1)
                    {
                        int index = 0;

                        switch (side) {
                            case Side.left: index = pos.z + pos.y * 32 + countSide * 0; break;
                            case Side.right: index = pos.z + pos.y * 32 + countSide * 1; break;
                            case Side.down: index = pos.x + pos.z * 32 + countSide * 2; break;
                            case Side.up: index = pos.x + pos.z * 32 + countSide * 3; break;
                            case Side.back: index = pos.x + pos.y * 32 + countSide * 4; break;
                            case Side.face: index = pos.x + pos.y * 32 + countSide * 5; break;
                        }

                        color = neighbourColors[index];
                    }
                    //Слишком сбоку
                    else {
                        color = new Color();
                    }
                }
                void getColorAndLight(Vector3Int pos, out Color color, out float light)
                {

                    byte neightbours = 0;
                    Side side = Side.left;

                    if (pos.x < 0)
                    {
                        neightbours++;
                        side = Side.left;
                    }
                    else if (pos.x > 31)
                    {
                        neightbours++;
                        side = Side.right;
                    }

                    if (pos.y < 0)
                    {
                        neightbours++;
                        side = Side.down;
                    }
                    else if (pos.y > 31)
                    {
                        neightbours++;
                        side = Side.up;
                    }

                    if (pos.z < 0)
                    {
                        neightbours++;
                        side = Side.back;
                    }
                    else if (pos.z > 31)
                    {
                        neightbours++;
                        side = Side.face;
                    }

                    //Данные из этого чанка
                    if (neightbours == 0)
                    {
                        int index = pos.x + pos.y * 32 + pos.z * 32 * 32;
                        color = colors[index];
                        light = lights[index];
                    }
                    //Данные какого-то соседа
                    else if (neightbours == 1)
                    {
                        int index = 0;

                        switch (side)
                        {
                            case Side.left: index = pos.z + pos.y * 32 + countSide * 0; break;
                            case Side.right: index = pos.z + pos.y * 32 + countSide * 1; break;
                            case Side.down: index = pos.x + pos.z * 32 + countSide * 2; break;
                            case Side.up: index = pos.x + pos.z * 32 + countSide * 3; break;
                            case Side.back: index = pos.x + pos.y * 32 + countSide * 4; break;
                            case Side.face: index = pos.x + pos.y * 32 + countSide * 5; break;
                        }

                        color = neighbourColors[index];
                        light = neighbourLight[index];
                    }
                    //Слишком сбоку
                    else
                    {
                        color = new Color(0,0,0,1.0f);
                        light = 0;
                    }
                }

                float calcShadow4(in Color c1, in float s1, in Color c2, in float s2, in Color c3, in float s3, in Color c4, in float s4)
                {
                    float result = 0.001f;

                    byte count = 0;
                    if (c1.a <= 0.9)
                    {
                        count++;
                        result += s1;
                    }
                    if (c2.a <= 0.9)
                    {
                        count++;
                        result += s2;
                    }
                    if (c3.a <= 0.9f && c2.a <= 0.9f && c4.a <= 0.9f)
                    {
                        count++;
                        result += s3;
                    }
                    if (c4.a <= 0.9)
                    {
                        count++;
                        result += s4;
                    }

                    if (count > 0)
                        result /= count;
                    else result = 0.99f;

                    return result;
                }

                float getLightL_DB(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z - 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightL_UB(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z - 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightL_UF(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z + 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightL_DF(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z + 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }

                float getLightD_LB(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z - 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightD_LF(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z + 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightD_RF(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z + 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z + 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }
                float getLightD_RB(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z - 1), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z - 1), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);

                    return result;
                }

                float getLightB_LD(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y - 1, pos.z), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);
                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                    return result;
                }
                float getLightB_LU(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y + 1, pos.z), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x - 1, pos.y, pos.z), out c4, out s4);
                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                    return result;
                }
                float getLightB_RU(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y + 1, pos.z), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);
                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                    return result;
                }
                float getLightB_RD(Vector3Int pos)
                {
                    float result = 0;

                    Color c1, c2, c3, c4;
                    float s1, s2, s3, s4;

                    getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                    getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c2, out s2);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y - 1, pos.z), out c3, out s3);
                    getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);
                    result = calcShadow4(c1, s1, c2, s2, c3, s3, c4, s4);
                    return result;
                }

                public void Execute()
                {
                    calcMesh();
                }

                
            }

            public void FixedReDraw()
            {
                int blocks = 32 * 32;

                int blocksAll = 32 * 32 * 32;
                int blocksNeighbour = blocks * 6;

                NativeArray<Color> colors;
                NativeArray<float> light;
                NativeArray<Color> neighbourColors;
                NativeArray<float> neighbourLight;

                Color[] colorList = new Color[blocksAll];
                float[] lightList = new float[blocksAll];

                for (byte z = 0; z < 32; z++)
                    for (byte x = 0; x < 32; x++)
                        for (byte y = 0; y < 32; y++) {
                            int index = x + y * 32 + z * 32 * 32;

                            colorList[index] = data.Colors[x, y, z];
                            lightList[index] = data.Light[x, y, z];
                        }



                colors = new NativeArray<Color>(colorList, Allocator.TempJob);
                light = new NativeArray<float>(lightList, Allocator.TempJob);
                neighbourColors = new NativeArray<Color>(data.neighbourColors, Allocator.TempJob);
                neighbourLight = new NativeArray<float>(data.neighbourLights, Allocator.TempJob);

                jobRedraw = new JobRedraw(colors, light, neighbourColors, neighbourLight);
                jobRedrawHandle = jobRedraw.Schedule();

                JobRedrawExist = true;
            }
            public bool LateReDraw() {
                //Если работа есть и она еще не законченна, удалять нельзя
                if (JobRedrawExist && !jobRedrawHandle.IsCompleted)
                    return false;                    

                // Ожидаем завершения всех задач с помощью JobHandle
                jobRedrawHandle.Complete();

                List<int> triangles = new List<int>();
                List<Vector2> uvMain = new List<Vector2>();
                List<Vector2> uvLight = new List<Vector2>();
                List<Vector3> normals = new List<Vector3>();

                // Теперь можно получить данные и освободить ресурсы

                triangles.AddRange(jobRedraw.triangles.ToArray());
                uvMain.AddRange(jobRedraw.uvMain.ToArray());
                uvLight.AddRange(jobRedraw.uvLight.ToArray());
                normals.AddRange(jobRedraw.normals.ToArray());

                jobRedraw.triangles.Dispose();
                jobRedraw.normals.Dispose();
                jobRedraw.uvMain.Dispose();
                jobRedraw.uvLight.Dispose();

                jobRedraw.colors.Dispose();
                jobRedraw.lights.Dispose();
                jobRedraw.neighbourColors.Dispose();
                jobRedraw.neighbourLight.Dispose();

                JobRedrawExist = false;

                meshColor.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                meshColor.vertices = MeshData.vertices32;
                meshColor.triangles = triangles.ToArray();
                meshColor.normals = normals.ToArray();
                meshColor.uv = uvMain.ToArray();
                meshColor.uv2 = uvLight.ToArray();

                //texture = graficMesh.dataMesh.mainTexture;

                //meshRendererBasic.materials[0].SetTexture("_MainTex", texture);
                meshRendererBasic.materials[0].SetTexture("_llluminationTex", MeshData.textureShadow);
                meshFilterBasic.mesh = meshColor;

                //Работа выполнена, удалять можно
                return true;
            }

            public void StartInicialize()
            {
                inicializeQueue.Enqueue(this);
            }

            public void Awake()
            {
                meshColor = new Mesh();
                //Создался новый игровой объект чанка, запихиваем в буфер
                buffer.Add(this);
            }

            /// <summary>
            /// Получить свободный чанк
            /// </summary>
            /// <returns></returns>
            static public ChankGO GetChankGO()
            {
                //Перебираем все чанки, ищем свободный
                foreach (ChankGO chankGO in buffer)
                {
                    if (chankGO.data == null)
                        return chankGO;
                }

                //Необходимо создать новый префаб чанка
                ChankGO chankNew = Instantiate(GameData.main.prefabChankGO);
                return chankNew;
            }

            public void Inicialize(Chank chank) {
                data = chank;
                //меняем размер на размер чанка
                int sizeBlock = Calc.GetSizeInt(chank.sizeBlock);
                int sizeChank = sizeBlock * Chank.Size;

                transform.localScale = new Vector3(sizeBlock, sizeBlock, sizeBlock);

                Vector3 position = chank.index * sizeChank;
                transform.localPosition = position;
            }


            /// <summary>
            /// Очистить чанк для дальнейшего использования
            /// </summary>
            public void Clear() {
                data = null;

                transform.localPosition = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(1, 1, 1);
            }

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
