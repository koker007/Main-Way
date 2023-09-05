using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


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

            static bool isCalculateNow = false;



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

                for (int x = 25; x < 32; x++) {
                    for (int y = 20; y < 32; y++) {
                        for (int z = 0; z < 32; z++) {
                            dataChank.Illumination[x, y, z] = 0.2f;
                        }
                    }
                }

                //Ожидаем завершения работы шейдера в другой job system задаче
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //while (isCalculateNow && stopwatch.ElapsedMilliseconds < 1000)
                //{

                //}
                isCalculateNow = true;

                // Создание буферов для хранения данных
                //ComputeBuffer verticesBuffer = new ComputeBuffer(dataMesh.vertices.Length, Grafic.Data.MeshChankColor.sizeOfVertices);
                ComputeBuffer normalsBuffer = new ComputeBuffer(dataMesh.normals.Length, Grafic.Data.MeshChankColor.sizeOfNormals);
                ComputeBuffer uvBuffer = new ComputeBuffer(dataMesh.uv.Length, Grafic.Data.MeshChankColor.sizeOfUV);
                ComputeBuffer uv2Buffer = new ComputeBuffer(dataMesh.uv2.Length, Grafic.Data.MeshChankColor.sizeOfUV);
                ComputeBuffer trianglesBuffer = new ComputeBuffer(dataMesh.triangles.Length, Grafic.Data.MeshChankColor.sizeOfTriangles);

                Color[,,] colors = dataChank.GetColors();
                ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(float) * 4);

                ComputeBuffer llluminationBuffer = new ComputeBuffer(dataChank.Illumination.Length, sizeof(float));

                //Подготовка буфера основной текстуры
                RenderTexture mainTexture = new RenderTexture(dataMesh.mainTexture.width, dataMesh.mainTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                mainTexture.enableRandomWrite = true;
                mainTexture.Create();

                // Заполнение буферов данными
                colorsBuffer.SetData(colors);
                llluminationBuffer.SetData(dataChank.Illumination);

                // Установка данных буферов в вычислительный шейдер
                //main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_vertices", verticesBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_normals", normalsBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_uv", uvBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_uv2", uv2Buffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_triangles", trianglesBuffer);

                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_colors", colorsBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_lllumination", llluminationBuffer);

                main.ShaderMeshChankColor.SetTexture(main._kernelIndexMeshChankColor, "_mainTexture", mainTexture);

                main.ShaderMeshChankColor.SetInt("_mainTextureWight", dataMesh.mainTexture.width);
                main.ShaderMeshChankColor.SetInt("_mainTextureHeight", dataMesh.mainTexture.height);

                //Начать вычисления шейдера
                main.ShaderMeshChankColor.Dispatch(main._kernelIndexMeshChankColor, 1, 1, 1);

                //Вытащить данные из шейдера
                //verticesBuffer.GetData(dataMesh.vertices);
                normalsBuffer.GetData(dataMesh.normals);
                uvBuffer.GetData(dataMesh.uv);
                uv2Buffer.GetData(dataMesh.uv2);
                trianglesBuffer.GetData(dataMesh.triangles);

                //вытаскиваем текстуру из рендера в 2Д
                RenderTexture.active = mainTexture;
                dataMesh.mainTexture.ReadPixels(new Rect(0,0, dataMesh.mainTexture.width, dataMesh.mainTexture.height),0,0);
                dataMesh.mainTexture.Apply();

                //Сказать что данные закончили вычисления и готовы к работе
                dataMesh.calculated = true;

                //Высвободить видео память
                //verticesBuffer.Dispose();
                normalsBuffer.Dispose();
                uvBuffer.Dispose();
                uv2Buffer.Dispose();
                trianglesBuffer.Dispose();

                colorsBuffer.Dispose();

                isCalculateNow = false;
            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
