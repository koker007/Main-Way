using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficPerlin3D : MonoBehaviour
{
    static GraficPerlin3D main;

    [SerializeField]
    ComputeShader ShaderPerlin3DRules;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndexPerlin3DRules;

    

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniPerlin3DRules();
    }

    void iniPerlin3DRules()
    {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndexPerlin3DRules = ShaderPerlin3DRules.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderPerlin3DRules.GetKernelThreadGroupSizes(_kernelIndexPerlin3DRules, out lenghtX, out lenghtY, out lenghtZ);

    }

    static public void calculate(GraficData.Perlin3DRules DataPerlin3DArray)
    {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin3DArray.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin3DArray.result);

        ComputeBuffer bufferRules = new ComputeBuffer(DataPerlin3DArray.genRules.Length, BiomeData.GenRule.SizeOf);
        bufferRules.SetData(DataPerlin3DArray.genRules);

        //Помещаем буфер данных в шейдер
        main.ShaderPerlin3DRules.SetBuffer(main._kernelIndexPerlin3DRules, "_datas", bufferResult);
        main.ShaderPerlin3DRules.SetBuffer(main._kernelIndexPerlin3DRules, "_rules", bufferRules);

        main.ShaderPerlin3DRules.SetInt("_RulesMax", DataPerlin3DArray.genRules.Length);

        main.ShaderPerlin3DRules.SetFloat("_factor", GraficData.Perlin3DRules.factor);

        main.ShaderPerlin3DRules.SetInt("_repeatX", DataPerlin3DArray.repeatX);
        main.ShaderPerlin3DRules.SetInt("_repeatY", DataPerlin3DArray.repeatY);
        main.ShaderPerlin3DRules.SetInt("_repeatZ", DataPerlin3DArray.repeatZ);

        main.ShaderPerlin3DRules.SetFloat("_regionX", DataPerlin3DArray.regionX);
        main.ShaderPerlin3DRules.SetFloat("_regionY", DataPerlin3DArray.regionY);
        main.ShaderPerlin3DRules.SetFloat("_regionZ", DataPerlin3DArray.regionZ);

        //Начать вычисления шейдера
        main.ShaderPerlin3DRules.Dispatch(main._kernelIndexPerlin3DRules, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferResult.GetData(DataPerlin3DArray.result);

        //Сказать что данные закончили вычисления и готовы к работе
        DataPerlin3DArray.calculated = true;

        //Высвободить видео память
        bufferResult.Dispose();
        bufferRules.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
