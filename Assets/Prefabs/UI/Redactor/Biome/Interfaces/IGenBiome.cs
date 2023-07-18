using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace GenBiome
{
    public interface IGenBiome
    {
        //�������� �� ������ ������� ��������� � �������� ����� �����
        Chank GenerateBlock(List<BiomeData.GenRule> rules, Vector3Int chankPosition, ObjData worldData);

    }

}

