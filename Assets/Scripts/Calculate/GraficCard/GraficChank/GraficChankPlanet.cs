using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GraficChankPlanet : MonoBehaviour
{
    static private GraficChankPlanet main;
    static public GraficChankPlanet MAIN { get { return main; }  }

    [SerializeField]
    ComputeShader ShaderGenChankPlanet;

    uint lenghtX = 0;
    uint lenghtY = 0;
    uint lenghtZ = 0;

    int _kernelIndex = 0;

    private void Awake()
    {
        main = this;
        ini();
    }
    void ini()
    {
        //Ищем в шейдере программу по расчету перлина
        _kernelIndex = ShaderGenChankPlanet.FindKernel("CSMain");

        //Получаем информацию из шейдера какая возможная длина за один раcчет
        ShaderGenChankPlanet.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //Установить данные на расчет
    public void calculate(GraficData.ChankWorldData DataWorld)
    {

        int sizeOfBlockID = sizeof(uint);
        int sizeOfGenRules = Marshal.SizeOf(typeof(BiomeData.GenRules));

        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferBlocksIDs = new ComputeBuffer(DataWorld.chankData.BlocksID.Length, sizeOfBlockID);
        bufferBlocksIDs.SetData(DataWorld.chankData.BlocksID);

        ComputeBuffer bufferGenRules = new ComputeBuffer(DataWorld.genRules.Length, sizeOfGenRules);
        bufferGenRules.SetData(DataWorld.genRules);

        //Помещаем буфер данных в шейдер
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "BlocksIDs", bufferBlocksIDs);
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "GenRules", bufferGenRules);

        ShaderGenChankPlanet.SetFloat("_chankPosX", DataWorld.chankPos.x);
        ShaderGenChankPlanet.SetFloat("_chankPosY", DataWorld.chankPos.y);
        ShaderGenChankPlanet.SetFloat("_chankPosZ", DataWorld.chankPos.z);

        //Начать вычисления шейдера
        ShaderGenChankPlanet.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferBlocksIDs.GetData(DataWorld.chankData.BlocksID);

        //Высвободить видео память
        bufferBlocksIDs.Dispose();
        bufferGenRules.Dispose();

    }
}
