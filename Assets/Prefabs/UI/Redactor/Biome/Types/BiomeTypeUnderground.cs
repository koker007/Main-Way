using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BiomeTypeUnderground data
//���������� ��������� ���
//���������� �����
public class BiomeTypeUnderground : BiomeData
{
    //������ ��������� �� ������ ���� 0% ���� � 100% ������ �����������.
    public float distGenCoreMax = 100;
    public float distGenCoreMin = 0;

    public float tempGenMax = 100;
    public float tempGenMin = 0;

    static public BiomeTypeUnderground GetTestBiome()
    {
        throw new System.NotImplementedException();
    }
}
