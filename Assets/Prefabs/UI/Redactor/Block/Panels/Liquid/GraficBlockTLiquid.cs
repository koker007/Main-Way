using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Отвечает за расчет шума парлина на видео карту
public class GraficBlockTLiquid : MonoBehaviour
{
    public static GraficBlockTLiquid main;

    [SerializeField]
    ComputeShader ShaderPerlinLiquid;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndex = 0;

    private void Awake()
    {
        main = this;
        ini();
    }

    void ini() {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndex = ShaderPerlinLiquid.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderPerlinLiquid.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.PerlinCube DataPerlinCube) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlinCube.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlinCube.result);

        //Помещаем буфер данных в шейдер
        ShaderPerlinLiquid.SetBuffer(_kernelIndex, "datas", bufferResult);
        ShaderPerlinLiquid.SetFloat("_offsetX", DataPerlinCube.offsetX);
        ShaderPerlinLiquid.SetFloat("_offsetY", DataPerlinCube.offsetY);
        ShaderPerlinLiquid.SetFloat("_offsetZ", DataPerlinCube.offsetZ);

        float factorX = GraficData.Perlin2D.factor / DataPerlinCube.scaleX;
        float factorY = GraficData.Perlin2D.factor / DataPerlinCube.scaleY;
        float factorZ = GraficData.Perlin2D.factor / DataPerlinCube.scaleZ;

        ShaderPerlinLiquid.SetFloat("_factorX", factorX);
        ShaderPerlinLiquid.SetFloat("_factorY", factorY);
        ShaderPerlinLiquid.SetFloat("_factorZ", factorZ);

        ShaderPerlinLiquid.SetFloat("_frequency", DataPerlinCube.frequency);
        ShaderPerlinLiquid.SetFloat("_octaves", DataPerlinCube.octaves);

        //Начать вычисления шейдера
        ShaderPerlinLiquid.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferResult.GetData(DataPerlinCube.result);

        //Сказать что данные закончили вычисления и готовы к работе
        DataPerlinCube.calculated = true;

        //Высвободить видео память
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
