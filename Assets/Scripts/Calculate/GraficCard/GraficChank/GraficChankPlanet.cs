using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

//������ �� ������� ID ����� � �����
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

    //�������� ���� �� ������ � �������� ����
    public Chank calculate(GraficData.ChankWorldData DataWorld)
    {
        //����������� ������ �� �����
        BiomeData.GenRule[] GenRules = DataWorld.biomeData.genRules.ToArray();

        int sizeOfBlockID = sizeof(int);
        int sizeOfGenRules = Marshal.SizeOf(typeof(BiomeData.GenRule));

        //���� � ������� ���� ������� ������
        Chank chank = new Chank();
        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferBlocksIDs = new ComputeBuffer(chank.BlocksID.Length, sizeOfBlockID);
        bufferBlocksIDs.SetData(chank.BlocksID);

        ComputeBuffer bufferGenRules = new ComputeBuffer(GenRules.Length, sizeOfGenRules);
        bufferGenRules.SetData(GenRules);

        //�������� ����� ������ � ������
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "BlocksIDs", bufferBlocksIDs);
        ShaderGenChankPlanet.SetBuffer(_kernelIndex, "GenRules", bufferGenRules);

        ShaderGenChankPlanet.SetFloat("_chankPosX", DataWorld.chankPos.x);
        ShaderGenChankPlanet.SetFloat("_chankPosY", DataWorld.chankPos.y);
        ShaderGenChankPlanet.SetFloat("_chankPosZ", DataWorld.chankPos.z);

        //������ ���������� �������
        ShaderGenChankPlanet.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferBlocksIDs.GetData(chank.BlocksID);

        //����������� ����� ������
        bufferBlocksIDs.Dispose();
        bufferGenRules.Dispose();

        //���������� ���� � ���� iD ������
        return chank;
    }
}
