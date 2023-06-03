using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenBiome;

public class GenPlanetBehavior : IGenBiome
{
    //������ �� �������������� ������
    static private GraficChankPlanet GenChankShader;

    public Chank GenerateBlock(List<BiomeData.GenRules> rules, Vector3Int chankPosition, SpaceObjData worldData)
    {
        //����� ������� ����
        Chank chank = new Chank();

        //��������� ������� ��������������� �������
        //���� �� ������� �� ��� ������ �� ������ ������.
        if (!(GenChankShader ??= GraficChankPlanet.MAIN))
        {
            Debug.LogError("GraficChankPlanet has null");
            throw new System.NotImplementedException();
        }

        //��������� ���� ������� ��������� ������


        return chank;
    }
}
