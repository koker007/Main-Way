using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Отвечает за расчет шума парлина на видео карту
public class GraficPerlin : MonoBehaviour
{
    public static GraficPerlin main;

    [SerializeField]
    ComputeShader ShaderPerlin;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        ini();
    }

    void ini() {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndex = ShaderPerlin.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderPerlin.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.Perlin DataPerlin) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin.result);

        //Помещаем буфер данных в шейдер
        ShaderPerlin.SetBuffer(_kernelIndex, "datas", bufferResult);
        ShaderPerlin.SetFloat("_offsetX", DataPerlin.offsetX);
        ShaderPerlin.SetFloat("_offsetY", DataPerlin.offsetY);
        ShaderPerlin.SetFloat("_offsetZ", DataPerlin.offsetZ);

        ShaderPerlin.SetFloat("_scale", DataPerlin.scale);
        ShaderPerlin.SetFloat("_frequency", DataPerlin.frequency);
        ShaderPerlin.SetFloat("_octaves", DataPerlin.octaves);
        ShaderPerlin.SetBool("_best", DataPerlin.best);

        //Начать вычисления шейдера
        ShaderPerlin.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferResult.GetData(DataPerlin.result);

        //Сказать что данные закончили вычисления и готовы к работе
        DataPerlin.calculated = true;

        //Высвободить видео память
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
