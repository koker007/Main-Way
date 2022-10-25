using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraficGalaxyStar : MonoBehaviour
{
    public static GraficGalaxyStar main;

    [SerializeField]
    ComputeShader ShaderGalaxyStar;

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

    void ini()
    {
        //���� � ������� ��������� �� ������� �������
        _kernelIndex = ShaderGalaxyStar.FindKernel("CSMain");

        //�������� ���������� �� ������� ����� ��������� ����� �� ���� ��c���
        ShaderGalaxyStar.GetKernelThreadGroupSizes(_kernelIndex, out lenghtX, out lenghtY, out lenghtZ);

    }

    //���������� ������ �� ������
    public void calculate(GraficData.GalaxyStar data)
    {
        data.iniData();

        //�������� ����� �������. ������ ���������� ������, ������ ������ ����� ������ � �����
        data.bufferResultSize.SetData(data.resultSize);
        data.bufferResultRotate.SetData(data.resultRotate);
        data.bufferResultColor.SetData(data.resultColor);
        data.bufferBasePos.SetData(data.basePosition);
        data.bufferBaseSpeedRot360.SetData(data.baseRotateSpeed);
        data.bufferBaseColor.SetData(data.baseColor);
        data.bufferBaseStarSize.SetData(data.baseStarSize);
        data.bufferBaseStarBright.SetData(data.baseStarBright);

        //�������� ����� ������ � ������
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_resultSize", data.bufferResultSize);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_resultRotate", data.bufferResultRotate);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_resultColor", data.bufferResultColor);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_basePosition", data.bufferBasePos);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_baseRotateSpeed", data.bufferBaseSpeedRot360);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_baseColor", data.bufferBaseColor);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_baseStarSize", data.bufferBaseStarSize);
        ShaderGalaxyStar.SetBuffer(_kernelIndex, "_baseStarBright", data.bufferBaseStarBright);

        ShaderGalaxyStar.SetFloat("_time", Time.unscaledTime);
        ShaderGalaxyStar.SetFloat("_camPosX", data.cameraPos.x);
        ShaderGalaxyStar.SetFloat("_camPosY", data.cameraPos.y);
        ShaderGalaxyStar.SetFloat("_camPosZ", data.cameraPos.z);

        //������ ���������� �������
        ShaderGalaxyStar.Dispatch(_kernelIndex, 1, 1, 1);

        //�������� ������ �� �������
        data.bufferResultSize.GetData(data.resultSize);
        data.bufferResultRotate.GetData(data.resultRotate);
        data.bufferResultColor.GetData(data.resultColor);

        //������������ ������
        data.SetResult();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
