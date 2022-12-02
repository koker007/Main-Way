using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficCalc : MonoBehaviour
{
    public static GraficCalc main;


    [SerializeField]
    ComputeShader shaderCalcMergeVector2;

    uint mergeVec2LenghtX = 0;
    uint mergeVec2LenghtY = 0;
    uint mergeVec2LenghtZ = 0;

    int mergeVec2KernelIndex = 0;

    [SerializeField]
    ComputeShader shaderCalcMergeVector2_2;

    uint mergeVec2LenghtX_2 = 0;
    uint mergeVec2LenghtY_2 = 0;
    uint mergeVec2LenghtZ_2 = 0;

    int mergeVec2KernelIndex_2 = 0;

    [SerializeField]
    ComputeShader shaderCalcMergeVector3;

    uint mergeVec3LenghtX = 0;
    uint mergeVec3LenghtY = 0;
    uint mergeVec3LenghtZ = 0;

    int mergeVec3KernelIndex = 0;

    [SerializeField]
    ComputeShader shaderCalcMergeVector3_2;

    uint mergeVec3LenghtX_2 = 0;
    uint mergeVec3LenghtY_2 = 0;
    uint mergeVec3LenghtZ_2 = 0;

    int mergeVec3KernelIndex_2 = 0;

    [SerializeField]
    ComputeShader shaderCalcMergeTriangleNum;

    uint mergeTriangleNumLenghtX = 0;
    uint mergeTriangleNumLenghtY = 0;
    uint mergeTriangleNumLenghtZ = 0;

    int mergeTriangleNumKernelIndex = 0;

    [SerializeField]
    ComputeShader shaderCalcMergeTriangleNum_2;

    uint mergeTriangleNumLenghtX_2 = 0;
    uint mergeTriangleNumLenghtY_2 = 0;
    uint mergeTriangleNumLenghtZ_2 = 0;

    int mergeTriangleNumKernelIndex_2 = 0;

    [SerializeField]
    ComputeShader shaderCalcDelZeroTriangleNum;

    uint delTriangleNumLenghtX = 0;
    uint delTriangleNumLenghtY = 0;
    uint delTriangleNumLenghtZ = 0;

    int delTriangleNumKernelIndex = 0;

    [SerializeField]
    ComputeShader shaderCalcAddToInt;

    uint addToIntLenghtX = 0;
    uint addToIntLenghtY = 0;
    uint addToIntLenghtZ = 0;

    int addToIntKernelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniCalcMergeVector2();
        iniCalcMergeVector3();
        iniCalcMergeTriangleNum();

        iniCalcAddToInt();
    }

    void iniCalcMergeVector2()
    {
        //Ищем в шейдере программу по расчету перлина
        mergeVec2KernelIndex = shaderCalcMergeVector2.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderCalcMergeVector2.GetKernelThreadGroupSizes(mergeVec2KernelIndex, out mergeVec2LenghtX, out mergeVec2LenghtY, out mergeVec2LenghtZ);

    }
    void iniCalcMergeVector3()
    {
        //Ищем в шейдере программу по расчету перлина
        mergeVec3KernelIndex = shaderCalcMergeVector3.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderCalcMergeVector3.GetKernelThreadGroupSizes(mergeVec3KernelIndex, out mergeVec3LenghtX, out mergeVec3LenghtY, out mergeVec3LenghtZ);

    }

    void iniCalcMergeTriangleNum()
    {
        //Ищем в шейдере программу по расчету перлина
        mergeTriangleNumKernelIndex = shaderCalcMergeTriangleNum.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderCalcMergeTriangleNum.GetKernelThreadGroupSizes(mergeTriangleNumKernelIndex, out mergeTriangleNumLenghtX, out mergeTriangleNumLenghtY, out mergeTriangleNumLenghtZ);

    }
    void iniCalcAddToInt()
    {
        //Ищем в шейдере программу по расчету перлина
        addToIntKernelIndex = shaderCalcAddToInt.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        shaderCalcAddToInt.GetKernelThreadGroupSizes(addToIntKernelIndex, out addToIntLenghtX, out addToIntLenghtY, out addToIntLenghtZ);

    }


    public Vector2[] mergeVector2(Vector2[] vec0, Vector2[] vec1, Vector2[] vec2, Vector2[] vec3, Vector2[] vec4, Vector2[] vec5)
    {
        Vector2[] result;

        int resultMax = 0;

        if (vec0 != null)
            resultMax += vec0.Length;
        if (vec1 != null)
            resultMax += vec1.Length;
        if (vec2 != null)
            resultMax += vec2.Length;
        if (vec3 != null)
            resultMax += vec3.Length;
        if (vec4 != null)
            resultMax += vec4.Length;
        if (vec5 != null)
            resultMax += vec5.Length;

        result = new Vector2[resultMax];

        int vec2Size = sizeof(float) * 2;

        ComputeBuffer bufferResultVec2 = new ComputeBuffer(result.Length, vec2Size);

        ComputeBuffer bufferVec0 = new ComputeBuffer(vec0.Length, vec2Size);
        ComputeBuffer bufferVec1 = new ComputeBuffer(vec1.Length, vec2Size);
        ComputeBuffer bufferVec2 = new ComputeBuffer(vec2.Length, vec2Size);
        ComputeBuffer bufferVec3 = new ComputeBuffer(vec3.Length, vec2Size);
        ComputeBuffer bufferVec4 = new ComputeBuffer(vec4.Length, vec2Size);
        ComputeBuffer bufferVec5 = new ComputeBuffer(vec5.Length, vec2Size);

        bufferResultVec2.SetData(result);

        bufferVec0.SetData(vec0);
        bufferVec1.SetData(vec1);
        bufferVec2.SetData(vec2);
        bufferVec3.SetData(vec3);
        bufferVec4.SetData(vec4);
        bufferVec5.SetData(vec5);

        //Заряжаем буфер.
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffResult", bufferResultVec2);

        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec0", bufferVec0);
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec1", bufferVec1);
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec2", bufferVec2);
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec3", bufferVec3);
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec4", bufferVec4);
        shaderCalcMergeVector2.SetBuffer(mergeVec2KernelIndex, "_buffVec5", bufferVec5);

        shaderCalcMergeVector2.SetInt("_countResult", result.Length);

        shaderCalcMergeVector2.SetInt("_countVec0", vec0.Length);
        shaderCalcMergeVector2.SetInt("_countVec1", vec1.Length);
        shaderCalcMergeVector2.SetInt("_countVec2", vec2.Length);
        shaderCalcMergeVector2.SetInt("_countVec3", vec3.Length);
        shaderCalcMergeVector2.SetInt("_countVec4", vec4.Length);
        shaderCalcMergeVector2.SetInt("_countVec5", vec5.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeVector2.Dispatch(mergeVec2KernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResultVec2.GetData(result);

        //Высвободить видео память
        bufferResultVec2.Dispose();
        bufferVec0.Dispose();
        bufferVec1.Dispose();
        bufferVec2.Dispose();
        bufferVec3.Dispose();
        bufferVec4.Dispose();
        bufferVec5.Dispose();

        return result;
    }
    public void mergeVector2(out Vector2[] result, in Vector2[] vec0, in Vector2[] vec1)
    {
        int resultMax = 0;

        if (vec0 != null)
            resultMax += vec0.Length;
        if (vec1 != null)
            resultMax += vec1.Length;

        result = new Vector2[resultMax];

        int vec2Size = sizeof(float) * 2;

        ComputeBuffer bufferResultVec2 = new ComputeBuffer(result.Length, vec2Size);

        ComputeBuffer bufferVec0 = new ComputeBuffer(vec0.Length, vec2Size);
        ComputeBuffer bufferVec1 = new ComputeBuffer(vec1.Length, vec2Size);

        bufferResultVec2.SetData(result);

        bufferVec0.SetData(vec0);
        bufferVec1.SetData(vec1);

        //Заряжаем буфер.
        shaderCalcMergeVector2_2.SetBuffer(mergeVec2KernelIndex_2, "_buffResult", bufferResultVec2);

        shaderCalcMergeVector2_2.SetBuffer(mergeVec2KernelIndex_2, "_buffVec0", bufferVec0);
        shaderCalcMergeVector2_2.SetBuffer(mergeVec2KernelIndex_2, "_buffVec1", bufferVec1);

        shaderCalcMergeVector2_2.SetInt("_countResult", result.Length);

        shaderCalcMergeVector2_2.SetInt("_countVec0", vec0.Length);
        shaderCalcMergeVector2_2.SetInt("_countVec1", vec1.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeVector2_2.Dispatch(mergeVec2KernelIndex_2, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResultVec2.GetData(result);

        //Высвободить видео память
        bufferResultVec2.Dispose();
        bufferVec0.Dispose();
        bufferVec1.Dispose();
    }

    public Vector3[] mergeVector3(Vector3[] vec0, Vector3[] vec1, Vector3[] vec2, Vector3[] vec3, Vector3[] vec4, Vector3[] vec5)
    {
        Vector3[] result;

        int resultMax = 0;

        if (vec0 != null)
            resultMax += vec0.Length;
        if (vec1 != null)
            resultMax += vec1.Length;
        if (vec2 != null)
            resultMax += vec2.Length;
        if (vec3 != null)
            resultMax += vec3.Length;
        if (vec4 != null)
            resultMax += vec4.Length;
        if (vec5 != null)
            resultMax += vec5.Length;

        
        result = new Vector3[resultMax];

        int vec3Size = sizeof(float) * 3;

        
        ComputeBuffer bufferResultVec3 = new ComputeBuffer(result.Length, vec3Size);

        ComputeBuffer bufferVec0 = new ComputeBuffer(vec0.Length, vec3Size);
        ComputeBuffer bufferVec1 = new ComputeBuffer(vec1.Length, vec3Size);
        ComputeBuffer bufferVec2 = new ComputeBuffer(vec2.Length, vec3Size);
        ComputeBuffer bufferVec3 = new ComputeBuffer(vec3.Length, vec3Size);
        ComputeBuffer bufferVec4 = new ComputeBuffer(vec4.Length, vec3Size);
        ComputeBuffer bufferVec5 = new ComputeBuffer(vec5.Length, vec3Size);

        bufferResultVec3.SetData(result);

        bufferVec0.SetData(vec0);
        bufferVec1.SetData(vec1);
        bufferVec2.SetData(vec2);
        bufferVec3.SetData(vec3);
        bufferVec4.SetData(vec4);
        bufferVec5.SetData(vec5);

        //Заряжаем буфер.
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffResult", bufferResultVec3);

        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec0", bufferVec0);
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec1", bufferVec1);
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec2", bufferVec2);
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec3", bufferVec3);
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec4", bufferVec4);
        shaderCalcMergeVector3.SetBuffer(mergeVec3KernelIndex, "_buffVec5", bufferVec5);

        shaderCalcMergeVector3.SetInt("_countResult", result.Length);

        shaderCalcMergeVector3.SetInt("_countVec0", vec0.Length);
        shaderCalcMergeVector3.SetInt("_countVec1", vec1.Length);
        shaderCalcMergeVector3.SetInt("_countVec2", vec2.Length);
        shaderCalcMergeVector3.SetInt("_countVec3", vec3.Length);
        shaderCalcMergeVector3.SetInt("_countVec4", vec4.Length);
        shaderCalcMergeVector3.SetInt("_countVec5", vec5.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeVector3.Dispatch(mergeVec3KernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResultVec3.GetData(result);

        //Высвободить видео память
        bufferResultVec3.Dispose();
        bufferVec0.Dispose();
        bufferVec1.Dispose();
        bufferVec2.Dispose();
        bufferVec3.Dispose();
        bufferVec4.Dispose();
        bufferVec5.Dispose();

        return result;
    }
    public void mergeVector3(out Vector3[] result, in Vector3[] vec0, in Vector3[] vec1)
    {
        int resultMax = 0;

        if (vec0 != null)
            resultMax += vec0.Length;
        if (vec1 != null)
            resultMax += vec1.Length;


        result = new Vector3[resultMax];

        int vec3Size = sizeof(float) * 3;


        ComputeBuffer bufferResultVec3 = new ComputeBuffer(result.Length, vec3Size);

        ComputeBuffer bufferVec0 = new ComputeBuffer(vec0.Length, vec3Size);
        ComputeBuffer bufferVec1 = new ComputeBuffer(vec1.Length, vec3Size);

        bufferResultVec3.SetData(result);

        bufferVec0.SetData(vec0);
        bufferVec1.SetData(vec1);

        //Заряжаем буфер.
        shaderCalcMergeVector3_2.SetBuffer(mergeVec3KernelIndex, "_buffResult", bufferResultVec3);

        shaderCalcMergeVector3_2.SetBuffer(mergeVec3KernelIndex, "_buffVec0", bufferVec0);
        shaderCalcMergeVector3_2.SetBuffer(mergeVec3KernelIndex, "_buffVec1", bufferVec1);

        shaderCalcMergeVector3_2.SetInt("_countResult", result.Length);

        shaderCalcMergeVector3_2.SetInt("_countVec0", vec0.Length);
        shaderCalcMergeVector3_2.SetInt("_countVec1", vec1.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeVector3_2.Dispatch(mergeVec3KernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResultVec3.GetData(result);

        //Высвободить видео память
        bufferResultVec3.Dispose();
        bufferVec0.Dispose();
        bufferVec1.Dispose();
    }

    public int[] mergeTriangleNum(int[] tri0, int[] tri1, int[] tri2, int[] tri3, int[] tri4, int[] tri5)
    {
        int[] result;

        int resultMax = 0;

        if (tri0 != null)
            resultMax += tri0.Length;
        if (tri1 != null)
            resultMax += tri1.Length;
        if (tri2 != null)
            resultMax += tri2.Length;
        if (tri3 != null)
            resultMax += tri3.Length;
        if (tri4 != null)
            resultMax += tri4.Length;
        if (tri5 != null)
            resultMax += tri5.Length;

        result = new int[resultMax];

        int intSize = sizeof(int);

        ComputeBuffer bufferResult = new ComputeBuffer(result.Length, intSize);

        ComputeBuffer bufferTri0 = new ComputeBuffer(tri0.Length, intSize);
        ComputeBuffer bufferTri1 = new ComputeBuffer(tri1.Length, intSize);
        ComputeBuffer bufferTri2 = new ComputeBuffer(tri2.Length, intSize);
        ComputeBuffer bufferTri3 = new ComputeBuffer(tri3.Length, intSize);
        ComputeBuffer bufferTri4 = new ComputeBuffer(tri4.Length, intSize);
        ComputeBuffer bufferTri5 = new ComputeBuffer(tri5.Length, intSize);

        bufferResult.SetData(result);

        bufferTri0.SetData(tri0);
        bufferTri1.SetData(tri1);
        bufferTri2.SetData(tri2);
        bufferTri3.SetData(tri3);
        bufferTri4.SetData(tri4);
        bufferTri5.SetData(tri5);

        //Заряжаем буфер.
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffResult", bufferResult);

        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri0", bufferTri0);
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri1", bufferTri1);
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri2", bufferTri2);
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri3", bufferTri3);
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri4", bufferTri4);
        shaderCalcMergeTriangleNum.SetBuffer(mergeTriangleNumKernelIndex, "_buffTri5", bufferTri5);

        shaderCalcMergeTriangleNum.SetInt("_countResult", result.Length);

        shaderCalcMergeTriangleNum.SetInt("_countTria0", tri0.Length);
        shaderCalcMergeTriangleNum.SetInt("_countTria1", tri1.Length);
        shaderCalcMergeTriangleNum.SetInt("_countTria2", tri2.Length);
        shaderCalcMergeTriangleNum.SetInt("_countTria3", tri3.Length);
        shaderCalcMergeTriangleNum.SetInt("_countTria4", tri4.Length);
        shaderCalcMergeTriangleNum.SetInt("_countTria5", tri5.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeTriangleNum.Dispatch(mergeTriangleNumKernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResult.GetData(result);

        //Высвободить видео память
        bufferResult.Dispose();
        bufferTri0.Dispose();
        bufferTri1.Dispose();
        bufferTri2.Dispose();
        bufferTri3.Dispose();
        bufferTri4.Dispose();
        bufferTri5.Dispose();

        return result;
    }

    public int[] mergeTriangleNum(int countVert0, int[] tri0, int[] tri1)
    {
        int[] result;

        int resultMax = 0;

        if (tri0 != null)
            resultMax += tri0.Length;
        if (tri1 != null)
            resultMax += tri1.Length;

        result = new int[resultMax];

        int intSize = sizeof(int);

        ComputeBuffer bufferResult = new ComputeBuffer(result.Length, intSize);

        ComputeBuffer bufferTri0 = new ComputeBuffer(tri0.Length, intSize);
        ComputeBuffer bufferTri1 = new ComputeBuffer(tri1.Length, intSize);

        bufferResult.SetData(result);

        bufferTri0.SetData(tri0);
        bufferTri1.SetData(tri1);

        //Заряжаем буфер.
        shaderCalcMergeTriangleNum_2.SetBuffer(mergeTriangleNumKernelIndex_2, "_buffResult", bufferResult);

        shaderCalcMergeTriangleNum_2.SetBuffer(mergeTriangleNumKernelIndex_2, "_buffTri0", bufferTri0);
        shaderCalcMergeTriangleNum_2.SetBuffer(mergeTriangleNumKernelIndex_2, "_buffTri1", bufferTri1);

        shaderCalcMergeTriangleNum_2.SetInt("_countResult", result.Length);

        shaderCalcMergeTriangleNum_2.SetInt("_countTria0", tri0.Length);
        shaderCalcMergeTriangleNum_2.SetInt("_countTria1", tri1.Length);

        shaderCalcMergeTriangleNum_2.SetInt("_countVert0", countVert0);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcMergeTriangleNum_2.Dispatch(mergeTriangleNumKernelIndex_2, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResult.GetData(result);

        //Высвободить видео память
        bufferResult.Dispose();
        bufferTri0.Dispose();
        bufferTri1.Dispose();

        return result;
    }

    public void delZeroTriangleNum(out int[] result, int[] tested)
    {
        int resultMax = 0;

        if (tested != null)
            resultMax += tested.Length/3;

        result = new int[resultMax];

        int intSize = sizeof(int);

        ComputeBuffer bufferResult = new ComputeBuffer(result.Length, intSize);
        ComputeBuffer bufferTest = new ComputeBuffer(tested.Length, intSize);

        bufferResult.SetData(result);
        bufferTest.SetData(tested);

        //Заряжаем буфер.
        shaderCalcDelZeroTriangleNum.SetBuffer(delTriangleNumKernelIndex, "_buffResult", bufferResult);
        shaderCalcDelZeroTriangleNum.SetBuffer(delTriangleNumKernelIndex, "_buffTest", bufferTest);

        shaderCalcDelZeroTriangleNum.SetInt("_countResult", result.Length);
        shaderCalcDelZeroTriangleNum.SetInt("_countTest", tested.Length);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcDelZeroTriangleNum.Dispatch(delTriangleNumKernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResult.GetData(result);

        //Высвободить видео память
        bufferResult.Dispose();
        bufferTest.Dispose();
    }

    /// <summary>
    /// Прибавить ко всем числам в массиве
    /// </summary>
    public int[] addToInt(int[] array, int addNum)
    {
        int[] result = new int[array.Length];

        int intSize = sizeof(int);

        ComputeBuffer bufferResult = new ComputeBuffer(result.Length, intSize);
        ComputeBuffer bufferArray = new ComputeBuffer(array.Length, intSize);

        bufferResult.SetData(result);
        bufferArray.SetData(array);

        //Заряжаем буфер.
        shaderCalcAddToInt.SetBuffer(addToIntKernelIndex, "_buffResult", bufferResult);
        shaderCalcAddToInt.SetBuffer(addToIntKernelIndex, "_buffArray", bufferArray);

        shaderCalcAddToInt.SetInt("_arrayLenght", result.Length);
        shaderCalcAddToInt.SetInt("_addNum", addNum);


        ////////////////////////////////////////////////
        //Начать вычисления шейдера
        ////////////////////////////////////////////////
        shaderCalcAddToInt.Dispatch(addToIntKernelIndex, 1, 1, 1);


        //Вытащить результат из шейдера
        bufferResult.GetData(result);

        //Высвободить видео память
        bufferResult.Dispose();
        bufferArray.Dispose();

        return result;
    }
}
