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
        //���� � ������� ��������� �� ������� �������
        _kernelIndex = ShaderGenChankPlanet.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderGenChankPlanet.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.ChankWorldData DataWorld)
    {

        int sizeOfBlockID = sizeof(uint);
        int sizeOfGenRules = Marshal.SizeOf(typeof(BiomeData.GenRules));

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferBlocksIDs = new ComputeBuffer(DataWorld.chankData.BlocksID.Length, sizeOfBlockID);
        bufferBlocksIDs.SetData(DataWorld.chankData.BlocksID);

        ComputeBuffer bufferGenRules = new ComputeBuffer(DataWorld.genRules.Length, sizeOfGenRules);
        bufferGenRules.SetData(DataWorld.genRules);

        //�������� ����� ������ � ������
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "BlocksIDs", bufferBlocksIDs);
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "GenRules", bufferGenRules);

        ShaderGenChankPlanet.SetFloat("_chankPosX", DataWorld.chankPos.x);
        ShaderGenChankPlanet.SetFloat("_chankPosY", DataWorld.chankPos.y);
        ShaderGenChankPlanet.SetFloat("_chankPosZ", DataWorld.chankPos.z);

        //������ ���������� �������
        ShaderGenChankPlanet.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferBlocksIDs.GetData(DataWorld.chankData.BlocksID);

        //����������� ����� ������
        bufferBlocksIDs.Dispose();
        bufferGenRules.Dispose();

    }
}
