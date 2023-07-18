using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

namespace GenBiome
{
    public interface IGenBiome
    {
        //Передаем по ссылке правила генерации и получаем номер блока
        Chank GenerateBlock(List<BiomeData.GenRule> rules, Vector3Int chankPosition, ObjData worldData);

    }

}

