using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�������� �� ������ ���� ������� �� ����� �����
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
        //���� � ������� ��������� �� ������� �������
        _kernelIndex = ShaderPerlinLiquid.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderPerlinLiquid.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.PerlinCube DataPerlinCube) {

        int dataPosSize = sizeof(float);
        int sizeOneData = dataPosSize;

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        ComputeBuffer bufferResult = new ComputeBuffer(DataPerlinCube.result.Length, sizeOneData);
        bufferResult.SetData(DataPerlinCube.result);

        //�������� ����� ������ � ������
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

        //������ ���������� �������
        ShaderPerlinLiquid.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        bufferResult.GetData(DataPerlinCube.result);

        //������� ��� ������ ��������� ���������� � ������ � ������
        DataPerlinCube.calculated = true;

        //����������� ����� ������
        bufferResult.Dispose();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
