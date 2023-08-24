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
                const int sizeXTextureColor = 180;
                const int sizeYTextureColor = 183;

                static readonly Vector3[] vertices32;
                static readonly Vector3[] normals32;

                static readonly Vector2[] uvTexBlock;
                static readonly Texture2D textureShadow;

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
                        Color color = new Color(0,0,0, (float)x / textureShadow.width);
                        textureShadow.SetPixel(x, 0, color);
                    }
                    textureShadow.Apply();
                }
                static public void CalcMeshColor2(Chank data, out Mesh mesh, out Texture2D texture2D)
                {
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

                Texture2D texture2D;
                Mesh mesh;

                //Если размер биома больше 1 то рисуем по цвету
                MeshData.CalcMeshColor2(data, out mesh, out texture2D);

                meshRendererBasic.materials[0].mainTexture = texture2D;
                meshFilterBasic.mesh = mesh;
                

                //Если размер биома 1 то рисуем по блокам
            }


            public void Awake()
            {
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

                //перерисовать чанк
                JobRedraw jobRedraw = new JobRedraw(this);
                jobRedraw.Execute();
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
