using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

//Шейдер по расчету ID блока в чанке
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

    //Передать биом на расчет и получить чанк
    public Chank calculate(GraficData.ChankWorldData DataWorld)
    {
        //Вытаскиваем данные из биома
        BiomeData.GenRule[] GenRules = DataWorld.biomeData.genRules.ToArray();

        int sizeOfBlockID = sizeof(int);
        int sizeOfGenRules = Marshal.SizeOf(typeof(BiomeData.GenRule));

        //Чанк в котором надо хранить данные
        Chank chank = new Chank();
        //Заряжаем буфер данными. первое количество данных, второе размер одной данной в битах
        ComputeBuffer bufferBlocksIDs = new ComputeBuffer(chank.BlocksID.Length, sizeOfBlockID);
        bufferBlocksIDs.SetData(chank.BlocksID);

        ComputeBuffer bufferGenRules = new ComputeBuffer(GenRules.Length, sizeOfGenRules);
        bufferGenRules.SetData(GenRules);

        //Помещаем буфер данных в шейдер
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "BlocksIDs", bufferBlocksIDs);
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "GenRules", bufferGenRules);

        ShaderGenChankPlanet.SetFloat("_chankPosX", DataWorld.chankPos.x);
        ShaderGenChankPlanet.SetFloat("_chankPosY", DataWorld.chankPos.y);
        ShaderGenChankPlanet.SetFloat("_chankPosZ", DataWorld.chankPos.z);

        //Начать вычисления шейдера
        ShaderGenChankPlanet.Dispatch(_kernelIndex, 1, 1, 1);

        //Вытащить данные из шейдера
        bufferBlocksIDs.GetData(chank.BlocksID);

        //Высвободить видео память
        bufferBlocksIDs.Dispose();
        bufferGenRules.Dispose();

        //Возвращаем чанк в виде iD блоков
        return chank;
    }
}
