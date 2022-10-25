using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�������� �� ������ ���� ������� �� ����� �����
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
        //���� � ������� ��������� �� ������� �������
        _kernelIndex = ShaderPerlin.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderPerlin.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.Perlin DataPerlin) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin.result);

        //�������� ����� ������ � ������
        ShaderPerlin.SetBuffer(_kernelIndex, "datas", bufferResult);
        ShaderPerlin.SetFloat("_offsetX", DataPerlin.offsetX);
        ShaderPerlin.SetFloat("_offsetY", DataPerlin.offsetY);
        ShaderPerlin.SetFloat("_offsetZ", DataPerlin.offsetZ);

        ShaderPerlin.SetFloat("_scale", DataPerlin.scale);
        ShaderPerlin.SetFloat("_frequency", DataPerlin.frequency);
        ShaderPerlin.SetFloat("_octaves", DataPerlin.octaves);
        ShaderPerlin.SetBool("_best", DataPerlin.best);

        //������ ���������� �������
        ShaderPerlin.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferResult.GetData(DataPerlin.result);

        //������� ��� ������ ��������� ���������� � ������ � ������
        DataPerlin.calculated = true;

        //����������� ����� ������
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
