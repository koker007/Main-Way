using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�������� �� ������ ���� ������� �� ����� �����
public class GraficPerlin2D : MonoBehaviour
{
    public static GraficPerlin2D main;

    [SerializeField]
    ComputeShader ShaderPerlin2D;

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
        _kernelIndex = ShaderPerlin2D.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderPerlin2D.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.Perlin2D DataPerlin2D) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlin2D.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlin2D.result);

        //�������� ����� ������ � ������
        ShaderPerlin2D.SetBuffer(_kernelIndex, "datas", bufferResult);
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

        //������ ���������� �������
        ShaderPerlin2D.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferResult.GetData(DataPerlin2D.result);

        //������� ��� ������ ��������� ���������� � ������ � ������
        DataPerlin2D.calculated = true;

        //����������� ����� ������
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
