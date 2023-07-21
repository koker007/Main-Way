using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Отвечает за расчет шума парлина на видео карту
public class GraficPerlin2D : MonoBehaviour
{
    public static GraficPerlin2D main;

    [SerializeField]
    ComputeShader ShaderPerlin2D;
    [SerializeField]
    ComputeShader ShaderPerlin2DArray;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndexPerlin2D = 0;
    int _kernelIndexPerlin2DArray;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniPerlin2D();
        iniPerlin2DArray();
    }

    void iniPerlin2D() {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndexPerlin2D = ShaderPerlin2D.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderPerlin2D.GetKernelThreadGroupSizes(_kernelIndexPerlin2D, out lenghtX, out lenghtY, out lenghtZ);

    }
    void iniPerlin2DArray()
    {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndexPerlin2DArray = ShaderPerlin2DArray.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderPerlin2DArray.GetKernelThreadGroupSizes(_kernelIndexPerlin2DArray, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.Perlin2D DataPerlin2D) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin2D.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin2D.result);

        //Помещаем буфер данных в шейдер
        ShaderPerlin2D.SetBuffer(_kernelIndexPerlin2D, "datas", bufferResult);
        ShaderPerlin2D.SetFloat("_offsetX", DataPerlin2D.offsetX);
        ShaderPerlin2D.SetFloat("_offsetY", DataPerlin2D.offsetY);
        ShaderPerlin2D.SetFloat("_offsetZ", DataPerlin2D.offsetZ);

        float factor = GraficData.Perlin2D.factor / DataPerlin2D.scale;
        float factorX = GraficData.Perlin2D.factor / DataPerlin2D.scaleX;
        float factorY = GraficData.Perlin2D.factor / DataPerlin2D.scaleY;
        float factorZ = GraficData.Perlin2D.factor / DataPerlin2D.scaleZ;
        ShaderPerlin2D.SetFloat("_factor", factor);
        ShaderPerlin2D.SetFloat("_factorX", factorX);
        ShaderPerlin2D.SetFloat("_factorY", factorY);
        ShaderPerlin2D.SetFloat("_factorZ", factorZ);


        ShaderPerlin2D.SetFloat("_frequency", DataPerlin2D.frequency);
        ShaderPerlin2D.SetFloat("_octaves", DataPerlin2D.octaves);

        ShaderPerlin2D.SetInt("_repeatX", DataPerlin2D.repeatX);
        ShaderPerlin2D.SetInt("_repeatY", DataPerlin2D.repeatY);

        ShaderPerlin2D.SetFloat("_regionX", DataPerlin2D.regionX);
        ShaderPerlin2D.SetFloat("_regionY", DataPerlin2D.regionY);

        //Начать вычисления шейдера
        ShaderPerlin2D.Dispatch(_kernelIndexPerlin2D, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferResult.GetData(DataPerlin2D.result);

        //Сказать что данные закончили вычисления и готовы к работе
        DataPerlin2D.calculated = true;

        //Высвободить видео память
        bufferResult.Dispose();

    }

    public void calculate(GraficData.Perlin2DArray DataPerlin2DArray)
    {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin2DArray.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin2DArray.result);

        //Помещаем буфер данных в шейдер
        ShaderPerlin2DArray.SetBuffer(_kernelIndexPerlin2D, "datas", bufferResult);
        ShaderPerlin2DArray.SetFloat("_offsetX", DataPerlin2DArray.offsetX);
        ShaderPerlin2DArray.SetFloat("_offsetY", DataPerlin2DArray.offsetY);
        ShaderPerlin2DArray.SetFloat("_offsetZ", DataPerlin2DArray.offsetZ);
        ShaderPerlin2DArray.SetInt("_ArrayMax", DataPerlin2DArray.result.GetLength(2));

        float factor = GraficData.Perlin2D.factor / DataPerlin2DArray.scale;
        float factorX = GraficData.Perlin2D.factor / DataPerlin2DArray.scaleX;
        float factorY = GraficData.Perlin2D.factor / DataPerlin2DArray.scaleY;
        float factorZ = GraficData.Perlin2D.factor / DataPerlin2DArray.scaleZ;
        ShaderPerlin2DArray.SetFloat("_factor", factor);
        ShaderPerlin2DArray.SetFloat("_factorX", factorX);
        ShaderPerlin2DArray.SetFloat("_factorY", factorY);
        ShaderPerlin2DArray.SetFloat("_factorZ", factorZ);


        ShaderPerlin2DArray.SetFloat("_frequency", DataPerlin2DArray.frequency);
        ShaderPerlin2DArray.SetFloat("_octaves", DataPerlin2DArray.octaves);

        ShaderPerlin2DArray.SetInt("_repeatX", DataPerlin2DArray.repeatX);
        ShaderPerlin2DArray.SetInt("_repeatY", DataPerlin2DArray.repeatY);

        ShaderPerlin2DArray.SetFloat("_regionX", DataPerlin2DArray.regionX);
        ShaderPerlin2DArray.SetFloat("_regionY", DataPerlin2DArray.regionY);

        //Начать вычисления шейдера
        ShaderPerlin2DArray.Dispatch(_kernelIndexPerlin2D, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferResult.GetData(DataPerlin2DArray.result);

        //Сказать что данные закончили вычисления и готовы к работе
        DataPerlin2DArray.calculated = true;

        //Высвободить видео память
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
