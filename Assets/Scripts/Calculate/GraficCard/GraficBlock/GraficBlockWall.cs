using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficBlockWall : MonoBehaviour
{
    public static GraficBlockWall main;

    [SerializeField]
    ComputeShader shaderBlockWall;

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

    void inicialize() {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndex = shaderBlockWall.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderBlockWall.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.BlockWall dataBlockWall)
    {

        int floatSize = sizeof(float);
        int intSize = sizeof(int);
        int vec3Size = sizeof(float) * 3;
        int vec2Size = sizeof(float) * 2;

        ComputeBuffer bufferVoxel = new ComputeBuffer(dataBlockWall.blockForms.voxel.GetLength(0) * dataBlockWall.blockForms.voxel.GetLength(1), floatSize);
        bufferVoxel.SetData(dataBlockWall.blockForms.voxel);

        //Заряжаем буфер. первое количество, второе размер одной в битах
        ComputeBuffer bufferVertices = new ComputeBuffer(dataBlockWall.blockForms.vertices.Length, vec3Size);
        bufferVertices.SetData(dataBlockWall.blockForms.vertices);

        ComputeBuffer bufferTriangles = new ComputeBuffer(dataBlockWall.blockForms.triangles.Length, intSize);
        bufferTriangles.SetData(dataBlockWall.blockForms.triangles);

        ComputeBuffer bufferUV = new ComputeBuffer(dataBlockWall.blockForms.uv.Length, vec2Size);
        bufferUV.SetData(dataBlockWall.blockForms.uv);

        ComputeBuffer bufferUVShadow = new ComputeBuffer(dataBlockWall.blockForms.uvShadow.Length, vec2Size);
        bufferUVShadow.SetData(dataBlockWall.blockForms.uvShadow);

        //Помещаем буфер данных в шейдер
        shaderBlockWall.SetBuffer(_kernelIndex, "_voxel", bufferVoxel);
        shaderBlockWall.SetBuffer(_kernelIndex, "_vertices", bufferVertices);
        shaderBlockWall.SetBuffer(_kernelIndex, "_triangles", bufferTriangles);
        shaderBlockWall.SetBuffer(_kernelIndex, "_uv", bufferUV);
        shaderBlockWall.SetBuffer(_kernelIndex, "_uvShadow", bufferUVShadow);

        shaderBlockWall.SetInt("_typeWall", (int)dataBlockWall.side);

        //Начать вычисления шейдера
        shaderBlockWall.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferVertices.GetData(dataBlockWall.blockForms.vertices);
        bufferTriangles.GetData(dataBlockWall.blockForms.triangles);
        bufferUV.GetData(dataBlockWall.blockForms.uv);
        bufferUVShadow.GetData(dataBlockWall.blockForms.uvShadow);

        //Высвободить видео память
        bufferVoxel.Dispose();
        bufferVertices.Dispose();
        bufferTriangles.Dispose();
        bufferUV.Dispose();
        bufferUVShadow.Dispose();
    }
}
