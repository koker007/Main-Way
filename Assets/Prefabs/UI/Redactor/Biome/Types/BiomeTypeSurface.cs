using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.Diagnostics;

//Biome type surface
//Определяет поведение генерации для
//поверхностного биома
public class BiomeTypeSurface: BiomeData
{
    /// <summary>
    /// Продолжительность биома подземлей
    /// </summary>
    int underHeightLenght = 5;

    /// <summary>
    /// Уменьшение вероятности спавна за каждый блок высоты над поверхностью
    /// Должно быть отрицательным не равным 0
    /// </summary>
    float surfaceCutting = -0.01f;

    /// <summary>
    /// Получить шум этого биома для каждого вида блока в нем
    /// </summary>
    /// <param name="chankIndex"></param>
    /// <param name="blockSizeInt"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public float[,,,] GetBiomeNoise(Vector3Int chankIndex, int blockSizeInt, PlanetData planetData) {

        int PlanetSize = Calc.GetSizeInt(planetData.size);

        int mapSizeZ = PlanetSize / blockSizeInt; //Полюса - размер планеты
        int mapSizeX = mapSizeZ * 2; //Экватор - два размера планеты
        int mapSizeY = mapSizeZ / 2; //Высота - Половина размера планеты


        float regionX = (chankIndex.x * Chank.Size) / (float)mapSizeX;
        float regionY = (chankIndex.y * Chank.Size) / (float)mapSizeY;
        float regionZ = (chankIndex.z * Chank.Size) / (float)mapSizeZ;

        //Создаем данные
        GraficData.Perlin3DRules perlin3DArray = new GraficData.Perlin3DRules(genRules.ToArray(), mapSizeX, mapSizeY, mapSizeZ, regionX, regionY, regionZ);
        perlin3DArray.Calculate();

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        //Поправка на высоту
        CorrectingHeight(ref perlin3DArray.result, chankIndex, blockSizeInt, planetData);

        stopwatch.Stop();

        UnityEngine.Debug.Log("GetBiomeNoise CorrectingHeight" +
            " stopwatch: " + stopwatch.ElapsedMilliseconds);

        return perlin3DArray.result;
     }

    void CorrectingHeight(ref float[,,,] noise, Vector3Int chankIndex, int blockSizeInt, PlanetData planetData) {

        Size BlockSize = Calc.GetSize(blockSizeInt);
        int PlanetSize = Calc.GetSizeInt(planetData.size);
        HeightMap heightMap = planetData.GetHeightMap(BlockSize, new Vector2Int(chankIndex.x, chankIndex.z));
        BiomeMaps biomeMap = planetData.GetBiomePart(BlockSize, new Vector2Int(chankIndex.x, chankIndex.z));
        float[,] intensive = biomeMap.winersIntensive;

        //Проходимся по всем высотам
        for (int y = 0; y < noise.GetLength(1); y++) {
            int globalY = (chankIndex.y * Chank.Size + y) * blockSizeInt;

            for (int x = 0; x < noise.GetLength(0); x++) {
                for (int z = 0; z < noise.GetLength(2); z++) {

                    //Определяем высоту поверхности
                    int heightY =  (int)(heightMap.map[x, z] * PlanetSize / 2);

                    int changeY = globalY - heightY;

                    float noiseCoof = 0;

                    //Над поверхностью
                    if (changeY >= 0)
                        noiseCoof -= changeY * surfaceCutting * (intensive[x,z]+0.5f);
                    //Под поверхностью и ниже
                    else if(changeY < -underHeightLenght)
                        noiseCoof = (changeY + underHeightLenght) * surfaceCutting * (intensive[x, z] + 0.5f);

                    //Применяем сдвиг на все виды блоков для текущей позиции
                    for (int blockIndex = 0; blockIndex < noise.GetLength(3); blockIndex++) {
                        noise[x, y, z, blockIndex] += noiseCoof;
                    }

                }
            }
        }

    }
}
