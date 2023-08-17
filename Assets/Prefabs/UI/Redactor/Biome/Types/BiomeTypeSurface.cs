using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

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

        //Поправка на высоту
        CorrectingHeight(ref perlin3DArray.result, chankIndex, blockSizeInt, planetData);

        return perlin3DArray.result;
     }

    /// <summary>
    /// получить тестовый биом, только для редактора биомов
    /// </summary>
    /// <returns></returns>
    static public BiomeTypeSurface GetTestBiome()
    {
        BiomeTypeSurface biome = new BiomeTypeSurface();

        //цвет всегда серый но с разной интенсивностью\
        float colorIntensive = Random.Range(0.25f, 0.75f);
        biome.color = new Color(colorIntensive, colorIntensive, colorIntensive);
        biome.coofHeightMax = 100;
        biome.coofHeightMin = 0;

        //Расположение биома
        biome.coofPolus = Random.Range(-0.3f, 0.3f);
        biome.coofZeroX = Random.Range(-0.1f, 0.1f);

        biome.seaPriority = SeaPriority.everywhere;
        
        //поверхностное обрезание
        biome.surfaceCutting = Random.Range(0.001f, 0.02f);
        //Подземная продолжительность
        biome.underHeightLenght = Random.Range(1, 20);

        biome.genRules = new List<GenRule>();
        for (int num = 0; num < 1; num++) {
            GenRule genRule = GenRule.GetTest();
            biome.genRules.Add(genRule);
        }

        return biome;
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
                        noiseCoof = (changeY + underHeightLenght) * surfaceCutting * (intensive[x, z] + 1f);

                    float test = 0;
                    //Применяем сдвиг на все виды блоков для текущей позиции
                    for (int blockIndex = 0; blockIndex < noise.GetLength(3); blockIndex++) {
                        test = noise[x, y, z, blockIndex];
                        noise[x, y, z, blockIndex] += noiseCoof;
                    }

                }
            }
        }

    }
}
