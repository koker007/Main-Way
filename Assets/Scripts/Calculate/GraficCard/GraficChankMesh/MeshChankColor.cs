using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Grafic
{
    namespace Calc
    {
        public class MeshChankColor : MonoBehaviour
        {
            static MeshChankColor main;

            [SerializeField]
            ComputeShader ShaderMeshChankColor;

            uint lenghtX = 0;
            uint lenghtY = 0;
            uint lenghtZ = 0;

            int _kernelIndexMeshChankColor;



            // Start is called before the first frame update
            void Start()
            {
                main = this;
                iniMeshChankColor();
            }

            void iniMeshChankColor()
            {
                //Ищем в шейдере программу по расчету перлина
                _kernelIndexMeshChankColor = ShaderMeshChankColor.FindKernel("CSMain");

                //Получаем информацию из шейдера какая возможная длина за один раcчет
                ShaderMeshChankColor.GetKernelThreadGroupSizes(_kernelIndexMeshChankColor, out lenghtX, out lenghtY, out lenghtZ);

            }

            static public void calculate(Grafic.Data.MeshChankColor dataMesh, Chank dataChank)
            {
                if (dataMesh.calculated)
                    return;

                /*
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        for (int z = 0; z < 31; z++)
                        {
                            dataChank.Colors[x, y, z] = new Color(0, 0, 0, 0);
                        }
                    }
                }
                
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 31; y < 32; y++)
                    {
                        for (int z = 0; z < 32; z++)
                        {
                            dataChank.Colors[x, y, z] = new Color(0, 0, 0, 0);
                        }
                    }
                }
                */


                for (int x = 0; x < 4; x++) {
                    for (int y = 31; y < 32; y++) {
                        for (int z = 0; z < 6; z++) {
                            dataChank.Colors[x, y, z] = new Color(0, 0, 0, 1);
                        }
                    }
                }

                // Создание буферов для хранения данных
                //ComputeBuffer verticesBuffer = new ComputeBuffer(dataMesh.vertices.Length, Grafic.Data.MeshChankColor.sizeOfVertices);
                ComputeBuffer normalsBuffer = new ComputeBuffer(dataMesh.normals.Length, Grafic.Data.MeshChankColor.sizeOfNormals);
                ComputeBuffer uvBuffer = new ComputeBuffer(dataMesh.uv.Length, Grafic.Data.MeshChankColor.sizeOfUV);
                ComputeBuffer trianglesBuffer = new ComputeBuffer(dataMesh.triangles.Length, Grafic.Data.MeshChankColor.sizeOfTriangles);

                Color[,,] colors = dataChank.GetColors();
                ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(float) * 4);

                // Заполнение буферов данными из меша
                //verticesBuffer.SetData(dataMesh.vertices);
                normalsBuffer.SetData(dataMesh.normals);
                uvBuffer.SetData(dataMesh.uv);
                trianglesBuffer.SetData(dataMesh.triangles);

                colorsBuffer.SetData(colors);

                // Установка данных буферов в вычислительный шейдер
                //main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_vertices", verticesBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_normals", normalsBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_uv", uvBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_triangles", trianglesBuffer);

                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_colors", colorsBuffer);

                //main.ShaderMeshChankColor.SetInt("_RulesMax", DataPerlin3DArray.genRules.Length);

                //Начать вычисления шейдера
                main.ShaderMeshChankColor.Dispatch(main._kernelIndexMeshChankColor, 1, 1, 1);

                //Вытащить данные из шейдера
                //verticesBuffer.GetData(dataMesh.vertices);
                normalsBuffer.GetData(dataMesh.normals);
                uvBuffer.GetData(dataMesh.uv);
                trianglesBuffer.GetData(dataMesh.triangles);

                //Сказать что данные закончили вычисления и готовы к работе
                dataMesh.calculated = true;

                //Высвободить видео память
                //verticesBuffer.Dispose();
                normalsBuffer.Dispose();
                uvBuffer.Dispose();
                trianglesBuffer.Dispose();

                colorsBuffer.Dispose();
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
