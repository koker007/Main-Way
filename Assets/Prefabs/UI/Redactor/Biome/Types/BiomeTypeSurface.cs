using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Biome type surface
//ќпредел€ет поведение генерации дл€
//поверхностного биома
public class BiomeTypeSurface: BiomeData
{
    float distGenSeaLevelMax = 100;
    float distGenSeaLevelMin = -100;


    /// <summary>
    /// ѕолучить шум этого биома дл€ каждого вида блока в нем
    /// </summary>
    /// <param name="chankIndex"></param>
    /// <param name="sizeBlock"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public float[,,,] GetBiomeNoise(Vector3Int chankIndex, int sizeBlock, int PlanetSize) {

        int mapSizeZ = PlanetSize / sizeBlock;
        int mapSizeX = mapSizeZ * 2;
        int mapSizeY = mapSizeZ / 2;


        float regionX = (chankIndex.x * Chank.Size) / (float)mapSizeX;
        float regionY = (chankIndex.y * Chank.Size) / (float)mapSizeY;
        float regionZ = (chankIndex.z * Chank.Size) / (float)mapSizeZ;

        //—оздаем данные
        GraficData.Perlin3DRules perlin3DArray = new GraficData.Perlin3DRules(genRules.ToArray(), mapSizeX, mapSizeY, mapSizeZ, regionX, regionY, regionZ);
        perlin3DArray.Calculate();

        return perlin3DArray.result;
     }
}
