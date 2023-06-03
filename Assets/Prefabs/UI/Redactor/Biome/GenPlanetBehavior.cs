using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenBiome;

public class GenPlanetBehavior : IGenBiome
{
    //—сылка на вычислительный шейдер
    static private GraficChankPlanet GenChankShader;

    public Chank GenerateBlock(List<BiomeData.GenRules> rules, Vector3Int chankPosition, SpaceObjData worldData)
    {
        //Ќужно создать чанк
        Chank chank = new Chank();

        //ѕровер€ем наличие вычислительного шейдера
        //≈сли он всетаки не был найден то играть нельз€.
        if (!(GenChankShader ??= GraficChankPlanet.MAIN))
        {
            Debug.LogError("GraficChankPlanet has null");
            throw new System.NotImplementedException();
        }

        //«аполн€ем чанк данными использу€ шейдер


        return chank;
    }
}
