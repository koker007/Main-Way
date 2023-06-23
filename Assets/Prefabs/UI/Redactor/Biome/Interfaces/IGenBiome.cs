using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GenBiome
{
    public interface IGenBiome
    {
        //�������� �� ������ ������� ��������� � �������� ����� �����
        Chank GenerateBlock(List<BiomeData.GenRule> rules, Vector3Int chankPosition, SpaceObjData worldData);

    }

}

