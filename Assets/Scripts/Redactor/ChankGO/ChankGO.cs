using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using System.Diagnostics;

namespace Game
{
    namespace Space
    {
        public class ChankGO : MonoBehaviour
        {
            static List<ChankGO> buffer = new List<ChankGO>();

            Chank data;

            [SerializeField]
            MeshFilter meshFilterBasic;
            [SerializeField]
            MeshRenderer meshRendererBasic;

            static class MeshData {
                const byte sizeXTextureColor = 180;
                const byte sizeYTextureColor = 200; //183 ���� ��� �������

                static readonly Vector3[] vertices32;
                static readonly Vector3[] normals32;

                static readonly Vector2[] uvTexBlock;
                static public readonly Texture2D textureShadow;

                static MeshData()
                {

                    List<Vector3> vertices32List = new List<Vector3>();
                    List<Vector3> normals32List = new List<Vector3>();

                    //���������� ��� ������� ������
                    for (int z = 0; z < Chank.Size; z++)
                    {
                        for (int y = 0; y < Chank.Size; y++)
                        {
                            for (int x = 0; x < Chank.Size; x++)
                            {
                                //1 ���� 8 ������
                                Vector3 LDB = new Vector3(x + 0, y + 0, z + 0);
                                Vector3 RDB = new Vector3(x + 1, y + 0, z + 0);

                                Vector3 LUB = new Vector3(x + 0, y + 1, z + 0);
                                Vector3 RUB = new Vector3(x + 1, y + 1, z + 0);

                                Vector3 LDF = new Vector3(x + 0, y + 0, z + 1);
                                Vector3 RDF = new Vector3(x + 1, y + 0, z + 1);

                                Vector3 LUF = new Vector3(x + 0, y + 1, z + 1);
                                Vector3 RUF = new Vector3(x + 1, y + 1, z + 1);

                                //1 ���� 3 ������ � 4 ������� �� ������ 
                                //������� ������� ������

                                //���� ��� �����
                                vertices32List.Add(LDB);
                                vertices32List.Add(LUB);
                                vertices32List.Add(LUF);
                                vertices32List.Add(LDF);

                                for(int num = 0; num < 4; num++)
                                    normals32List.Add(Vector3.right);

                                //��� ��� ����
                                vertices32List.Add(LDB);
                                vertices32List.Add(LDF);
                                vertices32List.Add(RDF);
                                vertices32List.Add(RDB);

                                for (int num = 0; num < 4; num++)
                                    normals32List.Add(Vector3.up);

                                //��� � �����
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

                    //������� ������� ������� �������� ��������
                    List<Vector2> uvTextureList = new List<Vector2>();
                    uvTextureList.Add(new Vector2(0, 0));
                    uvTextureList.Add(new Vector2(0, 1));
                    uvTextureList.Add(new Vector2(1, 1));

                    uvTextureList.Add(new Vector2(1, 1));
                    uvTextureList.Add(new Vector2(1, 0));
                    uvTextureList.Add(new Vector2(0, 0));

                    uvTexBlock = uvTextureList.ToArray();

                    //������� ��������� ����
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

                    //���������� ��� �����
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

                                //�������� ������ ��� ���� 12 ������

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

                                //������� ������� ����� ����� �� ��������
                                char pixX = (char)(blockNum % texture2Dnew.width);
                                char pixY = (char)(blockNum / texture2Dnew.width);

                                //��������� �������
                                //������ ������ ����� �����

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorLeft.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(X1, X2, X3, X4, colorThis, false, pixX, pixY);
                                }
                                //������ ������� ������ ����
                                else if (colorLeft.a > 0.9f && colorThis.a <= 0.9f)
                                {
                                    int blockNumLeft = (x + y * Chank.Size + z * Chank.Size * Chank.Size);
                                    char pixXL = (char)(blockNumLeft % texture2Dnew.width);
                                    char pixYL = (char)(blockNumLeft / texture2Dnew.width);
                                    AddPlane(X1, X2, X3, X4, colorLeft, true, pixXL, pixYL);
                                }

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorDown.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(Y1, Y2, Y3, Y4, colorThis, false, pixX, pixY);
                                }
                                //������� ������� ������� ����
                                else if (colorDown.a > 0.9f && colorThis.a <= 0.9f)
                                {
                                    int blockNumDown = (x + y * Chank.Size + z * Chank.Size * Chank.Size);
                                    char pixXD = (char)(blockNumDown % texture2Dnew.width);
                                    char pixYD = (char)(blockNumDown / texture2Dnew.width);
                                    AddPlane(Y1, Y2, Y3, Y4, colorDown, true, pixXD, pixYD);
                                }

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorBack.a <= 0.9f && colorThis.a > 0.9f)
                                {
                                    AddPlane(Z1, Z2, Z3, Z4, colorThis, false, pixX, pixY);
                                }
                                //�������� ������� ������� ����
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

                        //��������� ����
                        texture2Dnew.SetPixel(colorPixX, colorPisY, new Color(color.a, color.g, color.b));

                        //��������� ���������� ����������
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

                static public void CalcMeshColorShader(Chank data, out Mesh mesh, out Texture2D texture) {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    mesh = new Mesh();

                    Grafic.Data.MeshChankColor MeshData = new Grafic.Data.MeshChankColor();
                    Grafic.Calc.MeshChankColor.calculate(MeshData, data);

                    PlusNeibourChanksData(ref MeshData, data);

                    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mesh.vertices = vertices32;
                    mesh.triangles = MeshData.triangles;
                    mesh.normals = MeshData.normals;
                    mesh.uv = MeshData.uv;
                    mesh.uv2 = MeshData.uv2;


                    texture = MeshData.mainTexture;

                    stopwatch.Stop();
                    UnityEngine.Debug.Log("GenChankGO: " + data.index +
                        " stopwatch: " + stopwatch.ElapsedMilliseconds);
                }

                //��������� ��������� � ������ ������ � �������� ������
                static public void PlusNeibourChanksData(ref Grafic.Data.MeshChankColor meshData,in Chank data) {

                    byte xEnd = (byte)(data.Colors.GetLength(0) - 1);
                    byte yEnd = (byte)(data.Colors.GetLength(0) - 1);
                    byte zEnd = (byte)(data.Colors.GetLength(0) - 1);

                    //�������� �������
                    Chank chank = data;
                    Chank left = data.GetNeighbour(Side.left);
                    Chank right = data.GetNeighbour(Side.right);
                    Chank down = data.GetNeighbour(Side.down);
                    Chank up = data.GetNeighbour(Side.up);
                    Chank back = data.GetNeighbour(Side.back);
                    Chank forward = data.GetNeighbour(Side.face);

                    int indexMax = 32 * 32 * 32;

                    //�������� �������� � ���
                    for (int x = 0; x < 32; x++) {
                        for (int y = 0; y < 32; y++) {
                            for (int z = 0; z < 32; z++) {
                                if (x > 0 && x < 32 && y > 0 && y < 32 && z > 0 && z < 32)
                                    break;

                                DrawBlock(ref meshData, x,y,z);
                            }
                        }
                    }

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

                        if (color.a > 0.9f && x != 0 && y > 10 && z == 0)
                        {
                            bool test = false;
                        }

                        getColorBlock(out colorL, posL);
                        getColorBlock(out colorD, posD);
                        getColorBlock(out colorB, posB);

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
                            if (colorD.a <= 0.9f
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
                            if (colorB.a <= 0.9f
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

                                meshData.uv2[numVertStart + 0 + 8] = new Vector2(getLightB_RD(posD), 0);
                                meshData.uv2[numVertStart + 1 + 8] = new Vector2(getLightB_RU(posD), 1);
                                meshData.uv2[numVertStart + 2 + 8] = new Vector2(getLightB_LU(posD), 1);
                                meshData.uv2[numVertStart + 3 + 8] = new Vector2(getLightB_LD(posD), 0);
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

                                int numL = (x - 1) + y * 32 + z * 32 * 32;
                                Vector2Int texturePosL = new Vector2Int(numL % sizeXTextureColor, numL / sizeXTextureColor);
                                float uvXL = texturePosL.x * uvXsize;
                                float uvYL = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0] = new Vector2(uvXL, uvYL);
                                meshData.uv[numVertStart + 1] = new Vector2(uvXL, uvYL + uvYsize);
                                meshData.uv[numVertStart + 2] = new Vector2(uvXL + uvXsize, uvYL + uvYsize);
                                meshData.uv[numVertStart + 3] = new Vector2(uvXL + uvXsize, uvYL);

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

                                int numD = x + (y - 1) * 32 + z * 32 * 32;
                                Vector2Int texturePosL = new Vector2Int(numD % sizeXTextureColor, numD / sizeXTextureColor);
                                float uvXD = texturePosL.x * uvXsize;
                                float uvYD = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0 + 4] = new Vector2(uvXD, uvYD);
                                meshData.uv[numVertStart + 1 + 4] = new Vector2(uvXD, uvYD + uvYsize);
                                meshData.uv[numVertStart + 2 + 4] = new Vector2(uvXD + uvXsize, uvYD + uvYsize);
                                meshData.uv[numVertStart + 3 + 4] = new Vector2(uvXD + uvXsize, uvYD);

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

                                int numB = x + y * 32 + (z - 1) * 32 * 32;
                                Vector2Int texturePosL = new Vector2Int(numB % sizeXTextureColor, numB / sizeXTextureColor);
                                float uvXB = texturePosL.x * uvXsize;
                                float uvYB = texturePosL.y * uvYsize;
                                meshData.uv[numVertStart + 0 + 8] = new Vector2(uvXB, uvYB);
                                meshData.uv[numVertStart + 1 + 8] = new Vector2(uvXB, uvYB + uvYsize);
                                meshData.uv[numVertStart + 2 + 8] = new Vector2(uvXB + uvXsize, uvYB + uvYsize);
                                meshData.uv[numVertStart + 3 + 8] = new Vector2(uvXB + uvXsize, uvYB);

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
                        if (c1.a > 0.9) {
                            count++;
                            result += s1;
                        }
                        if (c2.a > 0.9) {
                            count++;
                            result += s2;
                        }
                        if (c3.a > 0.9) {
                            count++;
                            result += s3;
                        }
                        if (c4.a > 0.9)
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
                        light = 0;

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
                            light = left.Illumination[31, pos.y, pos.z];
                        }
                        else if (pos.x > 31)
                        {
                            if (right == null)
                                return;
                            color = right.Colors[0, pos.y, pos.z];
                            light = right.Illumination[0, pos.y, pos.z];
                        }
                        else if (pos.y < 0)
                        {
                            if (down == null)
                                return;
                            color = down.Colors[pos.x, 31, pos.z];
                            light = down.Illumination[pos.x, 31, pos.z];
                        }
                        else if (pos.y > 31)
                        {
                            if (up == null)
                                return;
                            color = up.Colors[pos.x, 0, pos.z];
                            light = up.Illumination[pos.x, 0, pos.z];
                        }
                        else if (pos.z < 0)
                        {
                            if (back == null)
                                return;
                            color = back.Colors[pos.x, pos.y, 31];
                            light = back.Illumination[pos.x, pos.y, 31];
                        }
                        else if (pos.z > 31)
                        {
                            if (forward == null)
                                return;
                            color = forward.Colors[pos.x, pos.y, 0];
                            light = forward.Illumination[pos.x, pos.y, 0];
                        }
                        else  {
                            color = chank.Colors[pos.x, pos.y, pos.z];
                            light = chank.Illumination[pos.x, pos.y, pos.z];
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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        result = calcShadow4(c1,s1,c2,s1,c3,s3,c4,s4);

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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        result = calcShadow4(c1, s1, c2, s1, c3, s3, c4, s4);

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

                        return result;
                    }

                    float getLightB_LD(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y - 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y - 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

                        return result;
                    }
                    float getLightB_LU(Vector3Int pos) {
                        float result = 0;

                        Color c1, c2, c3, c4;
                        float s1, s2, s3, s4;

                        getColorAndLight(new Vector3Int(pos.x, pos.y, pos.z), out c1, out s1);
                        getColorAndLight(new Vector3Int(pos.x, pos.y + 1, pos.z), out c2, out s2);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y + 1, pos.z), out c3, out s3);
                        getColorAndLight(new Vector3Int(pos.x + 1, pos.y, pos.z), out c4, out s4);

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

                        return result;
                    }
                }
            }

            struct JobRedraw : IJob
            {
                ChankGO chankGO;

                public JobRedraw(ChankGO chankGO)
                {
                    this.chankGO = chankGO;
                }

                public void Execute()
                {
                    chankGO.JobStartRedraw();
                }
            }

            void JobStartRedraw() {

                Texture2D texture;
                Mesh mesh;

                //���� ������ ����� ������ 1 �� ������ �� �����
                MeshData.CalcMeshColorShader(data, out mesh, out texture);

                //meshRendererBasic.materials[0].mainTexture = texture;
                meshRendererBasic.materials[0].SetTexture("_MainTex", texture);
                meshRendererBasic.materials[0].SetTexture("_llluminationTex", MeshData.textureShadow);
                meshFilterBasic.mesh = mesh;
                

                //���� ������ ����� 1 �� ������ �� ������
            }


            public void Awake()
            {
                //�������� ����� ������� ������ �����, ���������� � �����
                buffer.Add(this);
            }

            /// <summary>
            /// �������� ��������� ����
            /// </summary>
            /// <returns></returns>
            static public ChankGO GetChankGO()
            {
                //���������� ��� �����, ���� ���������
                foreach (ChankGO chankGO in buffer)
                {
                    if (chankGO.data == null)
                        return chankGO;
                }

                //���������� ������� ����� ������ �����
                ChankGO chankNew = Instantiate(GameData.main.prefabChankGO);
                return chankNew;
            }

            public void Inicialize(Chank chank) {
                data = chank;
                //������ ������ �� ������ �����
                int sizeBlock = Calc.GetSizeInt(chank.sizeBlock);
                int sizeChank = sizeBlock * Chank.Size;

                transform.localScale = new Vector3(sizeBlock, sizeBlock, sizeBlock);

                Vector3 position = chank.index * sizeChank;
                transform.localPosition = position;


                //������������ ����
                JobStartRedraw();
                //JobRedraw jobRedraw = new JobRedraw(this);
                //jobRedraw.Execute();
            }
            /// <summary>
            /// �������� ���� ��� ����������� �������������
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
