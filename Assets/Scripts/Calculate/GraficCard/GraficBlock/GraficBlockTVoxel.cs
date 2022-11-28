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
        //Ищем в шейдере программу по расчету перлина
        _kernelIndex = shaderBlockTVoxels.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderBlockTVoxels.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.BlockVoxelPart dataBlockVoxel, int[] exist)
    {

        int intSize = sizeof(int);
        int vec3Size = sizeof(float) * 3;
        int vec2Size = sizeof(float) * 2;

        //Заряжаем буфер. первое количество, второе размер одной в битах
        ComputeBuffer bufferVertices = new ComputeBuffer(dataBlockVoxel.vertices.Length, vec3Size);
        bufferVertices.SetData(dataBlockVoxel.vertices);

        ComputeBuffer bufferTriangles = new ComputeBuffer(dataBlockVoxel.triangles.Length, intSize);
        bufferTriangles.SetData(dataBlockVoxel.triangles);

        ComputeBuffer bufferUV = new ComputeBuffer(dataBlockVoxel.uv.Length, vec2Size);
        bufferUV.SetData(dataBlockVoxel.uv);

        ComputeBuffer bufferExist = new ComputeBuffer(exist.Length, intSize);
        bufferExist.SetData(exist);

        //Помещаем буфер данных в шейдер
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_vertices", bufferVertices);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_triangles", bufferTriangles);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_uv", bufferUV);
        shaderBlockTVoxels.SetBuffer(_kernelIndex, "_exist", bufferExist);

        shaderBlockTVoxels.SetInt("_sectorX", (int)dataBlockVoxel.sectorX);
        shaderBlockTVoxels.SetInt("_sectorY", (int)dataBlockVoxel.sectorY);
        shaderBlockTVoxels.SetInt("_sectorZ", (int)dataBlockVoxel.sectorZ);

        //Начать вычисления шейдера
        shaderBlockTVoxels.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferVertices.GetData(dataBlockVoxel.vertices);
        bufferTriangles.GetData(dataBlockVoxel.triangles);
        bufferUV.GetData(dataBlockVoxel.uv);

        //Высвободить видео память
        bufferVertices.Dispose();
        bufferTriangles.Dispose();
        bufferUV.Dispose();
        bufferExist.Dispose();
    }

    public void mergeVectors3(Vector3[] vec1, Vector3[] vec2, Vector3[] vec3, Vector3[] vec4, Vector3[] vec5, Vector3[] vec6, Vector3[] vec7, Vector3[] vec8) {
    
    }
}

