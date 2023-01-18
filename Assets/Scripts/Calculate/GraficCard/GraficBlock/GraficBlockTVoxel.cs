using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficBlockTVoxel : MonoBehaviour
{
    public static GraficBlockTVoxel main;

    [SerializeField]
    ComputeShader shaderBlockTVoxels;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        inicialize();
    }

    void inicialize()
    {
        //���� � ������� ��������� �� ������� �������
        _kernelIndex = shaderBlockTVoxels.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        shaderBlockTVoxels.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.BlockVoxelPart dataBlockVoxel, int[] exist)
    {

        int intSize = sizeof(int);
        int vec3Size = sizeof(float) * 3;
        int vec2Size = sizeof(float) * 2;

        //�������� �����. ������ ����������, ������ ������ ����� � �����
        ComputeBuffer bufferVertices = new ComputeBuffer(dataBlockVoxel.vertices.Length, vec3Size);
        bufferVertices.SetData(dataBlockVoxel.vertices);

        ComputeBuffer bufferTriangles = new ComputeBuffer(dataBlockVoxel.triangles.Length, intSize);
        bufferTriangles.SetData(dataBlockVoxel.triangles);

        ComputeBuffer bufferUV = new ComputeBuffer(dataBlockVoxel.uv.Length, vec2Size);
        bufferUV.SetData(dataBlockVoxel.uv);

        ComputeBuffer bufferNormals = new ComputeBuffer(dataBlockVoxel.normals.Length, vec3Size);
        bufferNormals.SetData(dataBlockVoxel.normals);

        ComputeBuffer bufferExist = new ComputeBuffer(exist.Length, intSize);
        bufferExist.SetData(exist);

        //�������� ����� ������ � ������
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_vertices", bufferVertices);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_triangles", bufferTriangles);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_uv", bufferUV);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_normals", bufferNormals);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_exist", bufferExist);

        shaderBlockTVoxels.SetInt("_sectorX", (int)dataBlockVoxel.sectorX);
        shaderBlockTVoxels.SetInt("_sectorY", (int)dataBlockVoxel.sectorY);
        shaderBlockTVoxels.SetInt("_sectorZ", (int)dataBlockVoxel.sectorZ);

        //������ ���������� �������
        shaderBlockTVoxels.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferVertices.GetData(dataBlockVoxel.vertices);
        bufferTriangles.GetData(dataBlockVoxel.triangles);
        bufferUV.GetData(dataBlockVoxel.uv);
        bufferNormals.GetData(dataBlockVoxel.normals);

        //����������� ����� ������
        bufferVertices.Dispose();
        bufferTriangles.Dispose();
        bufferUV.Dispose();
        bufferNormals.Dispose();
        bufferExist.Dispose();
    }

    public void calculate(GraficData.BlockVoxel dataBlockVoxel, int[] exist)
    {

        int intSize = sizeof(int);
        int vec3Size = sizeof(float) * 3;
        int vec2Size = sizeof(float) * 2;

        //�������� �����. ������ ����������, ������ ������ ����� � �����
        ComputeBuffer bufferVertices = new ComputeBuffer(dataBlockVoxel.vertices.Length, vec3Size);
        bufferVertices.SetData(dataBlockVoxel.vertices);

        ComputeBuffer bufferTriangles = new ComputeBuffer(dataBlockVoxel.triangles.Length, intSize);
        bufferTriangles.SetData(dataBlockVoxel.triangles);

        ComputeBuffer bufferUV = new ComputeBuffer(dataBlockVoxel.uv.Length, vec2Size);
        bufferUV.SetData(dataBlockVoxel.uv);

        ComputeBuffer bufferNormals = new ComputeBuffer(dataBlockVoxel.normals.Length, vec3Size);
        bufferNormals.SetData(dataBlockVoxel.normals);

        ComputeBuffer bufferExist = new ComputeBuffer(exist.Length, intSize);
        bufferExist.SetData(exist);

        //�������� ����� ������ � ������
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_vertices", bufferVertices);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_triangles", bufferTriangles);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_uv", bufferUV);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_normals", bufferNormals);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_exist", bufferExist);

        //������ ���������� �������
        shaderBlockTVoxels.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferVertices.GetData(dataBlockVoxel.vertices);
        bufferTriangles.GetData(dataBlockVoxel.triangles);
        bufferUV.GetData(dataBlockVoxel.uv);
        bufferNormals.GetData(dataBlockVoxel.normals);

        //����������� ����� ������
        bufferVertices.Dispose();
        bufferTriangles.Dispose();
        bufferUV.Dispose();
        bufferNormals.Dispose();
        bufferExist.Dispose();
    }

    public void mergeVectors3(Vector3[] vec1, Vector3[] vec2, Vector3[] vec3, Vector3[] vec4, Vector3[] vec5, Vector3[] vec6, Vector3[] vec7, Vector3[] vec8) {

        int vec3Size = sizeof(float) * 3;

        ComputeBuffer bufferVertices1 = new ComputeBuffer(vec1.Length, vec3Size);
        bufferVertices1.SetData(vec1);
        ComputeBuffer bufferVertices2 = new ComputeBuffer(vec2.Length, vec3Size);
        bufferVertices2.SetData(vec1);
        ComputeBuffer bufferVertices3 = new ComputeBuffer(vec3.Length, vec3Size);
        bufferVertices3.SetData(vec1);
        ComputeBuffer bufferVertices4 = new ComputeBuffer(vec4.Length, vec3Size);
        bufferVertices4.SetData(vec1);
        ComputeBuffer bufferVertices5 = new ComputeBuffer(vec5.Length, vec3Size);
        bufferVertices5.SetData(vec1);
        ComputeBuffer bufferVertices6 = new ComputeBuffer(vec6.Length, vec3Size);
        bufferVertices6.SetData(vec1);
        ComputeBuffer bufferVertices7 = new ComputeBuffer(vec7.Length, vec3Size);
        bufferVertices7.SetData(vec1);
        ComputeBuffer bufferVertices8 = new ComputeBuffer(vec8.Length, vec3Size);
        bufferVertices8.SetData(vec1);

        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_vertices", bufferVertices1);
    }
}

