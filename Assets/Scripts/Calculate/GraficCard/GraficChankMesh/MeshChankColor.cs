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
                //���� � ������� ��������� �� ������� �������
                _kernelIndexMeshChankColor = ShaderMeshChankColor.FindKernel("CSMain");

                //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
                ShaderMeshChankColor.GetKernelThreadGroupSizes(_kernelIndexMeshChankColor, out lenghtX, out lenghtY, out lenghtZ);

            }

            static public void calculate(Grafic.Data.MeshChankColor dataMesh, Chank dataChank)
            {
                if (dataMesh.calculated)
                    return;

                // �������� ������� ��� �������� ������
                //ComputeBuffer verticesBuffer = new ComputeBuffer(dataMesh.vertices.Length, Grafic.Data.MeshChankColor.sizeOfVertices);
                ComputeBuffer normalsBuffer = new ComputeBuffer(dataMesh.normals.Length, Grafic.Data.MeshChankColor.sizeOfNormals);
                ComputeBuffer uvBuffer = new ComputeBuffer(dataMesh.uv.Length, Grafic.Data.MeshChankColor.sizeOfUV);
                ComputeBuffer uv2Buffer = new ComputeBuffer(dataMesh.uv2.Length, Grafic.Data.MeshChankColor.sizeOfUV);
                ComputeBuffer trianglesBuffer = new ComputeBuffer(dataMesh.triangles.Length, Grafic.Data.MeshChankColor.sizeOfTriangles);

                Color[,,] colors = dataChank.GetColors();
                ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length, sizeof(float) * 4);

                ComputeBuffer llluminationBuffer = new ComputeBuffer(dataChank.Illumination.Length, sizeof(float));

                // ���������� ������� �������
                colorsBuffer.SetData(colors);
                llluminationBuffer.SetData(dataChank.Illumination);

                // ��������� ������ ������� � �������������� ������
                //main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_vertices", verticesBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_normals", normalsBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_uv", uvBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_uv2", uv2Buffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_triangles", trianglesBuffer);

                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_colors", colorsBuffer);
                main.ShaderMeshChankColor.SetBuffer(main._kernelIndexMeshChankColor, "_lllumination", llluminationBuffer);

                main.ShaderMeshChankColor.SetTexture(main._kernelIndexMeshChankColor, "_mainTexture", dataMesh.mainTexture);

                main.ShaderMeshChankColor.SetInt("_mainTextureWight", dataMesh.mainTexture.width);
                main.ShaderMeshChankColor.SetInt("_mainTextureHeight", dataMesh.mainTexture.height);

                //������ ���������� �������
                main.ShaderMeshChankColor.Dispatch(main._kernelIndexMeshChankColor, 1, 1, 1);

                //�������� ������ �� �������
                //verticesBuffer.GetData(dataMesh.vertices);
                normalsBuffer.GetData(dataMesh.normals);
                uvBuffer.GetData(dataMesh.uv);
                uv2Buffer.GetData(dataMesh.uv2);
                trianglesBuffer.GetData(dataMesh.triangles);

                //������� ��� ������ ��������� ���������� � ������ � ������
                dataMesh.calculated = true;

                //����������� ����� ������
                //verticesBuffer.Dispose();
                normalsBuffer.Dispose();
                uvBuffer.Dispose();
                uv2Buffer.Dispose();
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
