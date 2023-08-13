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
        //���� � ������� ��������� �� ������� �������
        _kernelIndexPerlin3DRules = ShaderPerlin3DRules.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderPerlin3DRules.GetKernelThreadGroupSizes(_kernelIndexPerlin3DRules, out lenghtX, out lenghtY, out lenghtZ);

    }

    static public void calculate(GraficData.Perlin3DRules DataPerlin3DArray)
    {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin3DArray.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin3DArray.result);

        ComputeBuffer bufferRules = new ComputeBuffer(DataPerlin3DArray.genRules.Length, BiomeData.GenRule.SizeOf);
        bufferRules.SetData(DataPerlin3DArray.genRules);

        //�������� ����� ������ � ������
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

        //������ ���������� �������
        main.ShaderPerlin3DRules.Dispatch(main._kernelIndexPerlin3DRules, 1, 1, 1);

        //�������� ������ �� �������
        bufferResult.GetData(DataPerlin3DArray.result);

        //������� ��� ������ ��������� ���������� � ������ � ������
        DataPerlin3DArray.calculated = true;

        //����������� ����� ������
        bufferResult.Dispose();
        bufferRules.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
