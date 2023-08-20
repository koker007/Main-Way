using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

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
                static readonly Vector3[] vertices32;
                static readonly Vector2[] uvTexture;
                static readonly Texture2D textureShadow;

                static MeshData()
                {
                    List<Vector3> vertices32List = new List<Vector3>();

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

                                //��� ��� ����
                                vertices32List.Add(LDB);
                                vertices32List.Add(LDF);
                                vertices32List.Add(RDF);
                                vertices32List.Add(RDB);

                                //��� � �����
                                vertices32List.Add(RDB);
                                vertices32List.Add(RUB);
                                vertices32List.Add(LUB);
                                vertices32List.Add(LDB);
                            }
                        }
                    }

                    vertices32 = vertices32List.ToArray();

                    //������� ������� ������� �������� ��������
                    List<Vector2> uvTextureList = new List<Vector2>();
                    uvTextureList.Add(new Vector2(0, 0));
                    uvTextureList.Add(new Vector2(0, 1));
                    uvTextureList.Add(new Vector2(1, 1));

                    uvTextureList.Add(new Vector2(1, 1));
                    uvTextureList.Add(new Vector2(1, 0));
                    uvTextureList.Add(new Vector2(0, 0));

                    uvTexture = uvTextureList.ToArray();

                    //������� ��������� ����
                    textureShadow = new Texture2D(255, 1);
                    for (int x = 0; x < textureShadow.width; x++) {
                        Color color = new Color(0,0,0, (float)x / textureShadow.width);
                        textureShadow.SetPixel(x, 0, color);
                    }
                    textureShadow.Apply();
                }

                static public void CalcMesh�olor(Chank data, out Mesh mesh, out Texture2DArray texture2DArray)
                {
                    mesh = new Mesh();

                    List<int> triangles = new List<int>();
                    List<Vector2> uvTex = new List<Vector2>();
                    List<Vector2> uvShadow = new List<Vector2>();
                    List<Texture2D> texture2Ds = new List<Texture2D>();

                    int vertSize = 4 * 3;

                    //���������� ��� �����
                    for (int x = 0; x < data.BlocksID.GetLength(0); x++)
                    {
                        for (int y = 0; y < data.BlocksID.GetLength(1); y++)
                        {
                            for (int z = 0; z < data.BlocksID.GetLength(2); z++)
                            {
                                Vector3Int pos = new Vector3Int(x, y, z);
                                Color colorThis = data.GetColor(pos);
                                Color colorLeft = data.GetColor(pos, Chank.Placement.Left);
                                Color colorDown = data.GetColor(pos, Chank.Placement.Downer);
                                Color colorBack = data.GetColor(pos, Chank.Placement.Back);

                                //�������� ������ ��� ���� 12 ������

                                int X1 = (x + y * Chank.Size + z * Chank.Size * Chank.Size) * vertSize;
                                int X2 = X1 + 1;
                                int X3 = X1 + 2;
                                int X4 = X1 + 3;

                                int Y1 = X1 + 4;
                                int Y2 = Y1 + 1;
                                int Y3 = Y1 + 2;
                                int Y4 = Y1 + 3;

                                int Z1 = X1 + 8;
                                int Z2 = Z1 + 1;
                                int Z3 = Z1 + 2;
                                int Z4 = Z1 + 3;

                                //��������� �������
                                //������ ������ ����� �����

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorLeft.a <= 0.9f && colorThis.a > 0.9f)
                                    AddPlane(X4, X3, X2, X2, X1, X3, colorThis);
                                //������ ������� ������ ����
                                else if (colorLeft.a > 0.9f && colorThis.a <= 0.9f)
                                    AddPlane(X1, X2, X3, X3, X4, X1, colorLeft);

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorDown.a <= 0.9f && colorThis.a > 0.9f)
                                    AddPlane(Y4, Y3, Y2, Y2, Y1, Y3, colorThis);
                                //������� ������� ������� ����
                                else if (colorDown.a > 0.9f && colorThis.a <= 0.9f)
                                    AddPlane(Y1, Y2, Y3, Y3, Y4, Y1, colorDown);

                                //���� ����� ������ � ������� ��������� ���� (���������� �� ����������)
                                if (colorBack.a <= 0.9f && colorThis.a > 0.9f)
                                    AddPlane(Z4, Z3, Z2, Z2, Z1, Z3, colorThis);
                                //�������� ������� ������� ����
                                else if (colorBack.a > 0.9f && colorThis.a <= 0.9f)
                                    AddPlane(Z1, Z2, Z3, Z3, Z4, Z1, colorBack);

                            }
                        }
                    }



                    mesh.vertices = vertices32;
                    mesh.triangles = triangles.ToArray();
                    mesh.uv = uvTex.ToArray();

                    texture2DArray = new Texture2DArray(1,1, texture2Ds.Count, TextureFormat.RGBA32, false, false);
                    for (int num = 0; num < texture2Ds.Count; num++) {
                        Graphics.CopyTexture(texture2Ds[num], 0, texture2DArray, num);
                    }

                    void AddPlane(int P1, int P2, int P3, int P4, int P5, int P6, Color color) {
                        //������ ������ ������ ����� �����
                        triangles.Add(P1);
                        triangles.Add(P2);
                        triangles.Add(P3);

                        triangles.Add(P4);
                        triangles.Add(P5);
                        triangles.Add(P6);

                        uvTex.Add(uvTexture[0]);
                        uvTex.Add(uvTexture[1]);
                        uvTex.Add(uvTexture[2]);
                        uvTex.Add(uvTexture[3]);
                        uvTex.Add(uvTexture[4]);
                        uvTex.Add(uvTexture[5]);

                        Texture2D texture = new Texture2D(1, 1);
                        texture.SetPixel(0, 0, new Color(color.a, color.g, color.b));
                        texture2Ds.Add(texture);

                        //������������ //������ ���������� � ������������ ����� � ����������
                        //0.0f-����� 1.0f-������
                        uvShadow.Add(new Vector2(1.0f, 0));
                        uvShadow.Add(new Vector2(1.0f, 0));
                        uvShadow.Add(new Vector2(1.0f, 0));
                        uvShadow.Add(new Vector2(1.0f, 0));
                        uvShadow.Add(new Vector2(1.0f, 0));
                        uvShadow.Add(new Vector2(1.0f, 0));
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

                Texture2DArray texture2DArray;
                Mesh mesh;

                //���� ������ ����� ������ 1 �� ������ �� �����
                MeshData.CalcMesh�olor(data, out mesh, out texture2DArray);
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
                int sizeOneBlock = Calc.GetSizeInt(chank.sizeBlock);

                transform.localScale = new Vector3(sizeOneBlock, sizeOneBlock, sizeOneBlock);

                //������������ ����
                JobRedraw jobRedraw = new JobRedraw(this);
                jobRedraw.Execute();
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
