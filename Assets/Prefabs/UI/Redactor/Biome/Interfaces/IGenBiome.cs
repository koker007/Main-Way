using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenBiome
{
    public interface IGenBiome
    {
        //�������� �� ������ ������� ��������� � �������� ����� �����
        Chank GenerateBlock(List<BiomeData.GenRules> rules, Vector3Int chankPosition, SpaceObjData worldData);

    }

}

