using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Biome type surface
//���������� ��������� ��������� ���
//�������������� �����
public class BiomeTypeSurface: BiomeData
{
    float distGenSeaLevelMax = 100;
    float distGenSeaLevelMin = -100;


    /// <summary>
    /// �������� ��� ����� ����� ��� ������� ���� ����� � ���
    /// </summary>
    /// <param name="chankIndex"></param>
    /// <param name="sizeBlock"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public float[,,,] GetBiomeNoise(Vector3Int chankIndex, Size sizeBlock) {
        float[,,,] result = new float[0,0,0,0];



        return result;
     }
}
