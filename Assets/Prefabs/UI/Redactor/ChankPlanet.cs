using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.Diagnostics;
public class ChankPlanet : Chank
{

    float[][,,,] biomesNoiseSurface;
    float[][,,,] biomesNoiseUnderground;

    public ChankPlanet(Size sizeBlock, Vector3Int index, PlanetData planetData): base(sizeBlock, index, planetData) {

        int countBiomesSurface = 0;
        if (planetData.pattern.biomesSurface != null)
            countBiomesSurface = planetData.pattern.biomesSurface.Count;

        int countBiomesUnderground = 0;
        if (planetData.pattern.biomesUnderground != null)
            countBiomesUnderground = planetData.pattern.biomesUnderground.Count;

        //Создать пространство под шум поверхности
        this.biomesNoiseSurface = new float[countBiomesSurface][,,,];
        //Создать пространство под шум подземлей
        this.biomesNoiseUnderground = new float[countBiomesUnderground][,,,];

        //Начать генерацию чанка
        Generate();
    }

    public override void Generate()
    {
        if (isStartGenerate)
            return;

        JobGenerate jobGenerate = new JobGenerate(this);
        jobGenerate.Execute();
    }
    protected override void JobStartGenerate()
    {

        //нужно узнать по каждому из блоков к какому биому он принадлежит

        Vector2Int index2D = new Vector2Int(index.x, index.z);
        int planetSize = Calc.GetSizeInt(planetData.size);

        //Получаем информацию о высоте поверхности для каждого блока
        HeightMap heightMap = planetData.GetHeightMap(sizeBlock, index2D);

        //Получаем информацию биомов поверхности для каждого блока
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //Перебираем координаты x z чанка
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {

                //Нужно определить по правилам какого биома генерируется область
                int biomeNum = biomeMap.winers[x, z];

                //Вычисляем шумы поверхности (для биома победителя)
                CalcNoisesSurface(biomeNum);
                //Вычисляем шумы подземности
                //CalcNoisesUnderground(biomeNum);

                int heightSurface = (int)(heightMap.map[x, z] * planetSize / 2);

                //Перебираем каждую высоту чанка
                for (int y = 0; y < Size; y++)
                {

                    //ищем блок победитель
                    CalcWiner(new Vector3Int(x, y, z), biomeNum, heightSurface);
                }

            }
        }

        //ID блока уже найденно

        //Теперь если размер чанка больше 1, то нужно еще заполнить цвет
        CalcColor();
    }

    void CalcNoisesSurface(int biomeNum)
    {
        if (biomesNoiseSurface[biomeNum] != null)
        {
            return;
        }

        int sizeBlock = Calc.GetSizeInt(this.sizeBlock);


        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //Получаем шум биома поверхности
        biomesNoiseSurface[biomeNum] = planetData.pattern.biomesSurface[biomeNum].GetBiomeNoise(index, sizeBlock, planetData);

        stopwatch.Stop();
        UnityEngine.Debug.Log("GetBiomeNoise: " + biomeNum +
            " stopwatch: " + stopwatch.ElapsedMilliseconds);

    }
    void CalcNoisesUnderground(int biomeNum)
    {
        if (biomesNoiseUnderground[biomeNum] != null)
        {
            return;
        }

        //Если шума нет, то шумим
        //biomesNoiseUnderground[biomeNum] = planetData.pattern.biomesUnderground[biomeNum].GetBiomeNoise(index, sizeBlock);

        //Это биом поверхности, необходимо сделать поправку на высоту

    }

    void CalcWiner(Vector3Int pos, int biomeNum, int heightSurface)
    {

        //Текущая высота, данного блока
        int heightNow = (index.y * Chank.Size + pos.y) * Calc.GetSizeInt(sizeBlock);

        //Перебираем все блоки в биоме
        int blockWinerID = 0;
        float noiseWiner = 0;


        //Ищем победителя среди поверхности
        for (int num = 0; num < biomesNoiseSurface[biomeNum].GetLength(3); num++)
        {
            if (biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num] < noiseWiner)
                continue;

            noiseWiner = biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num];
            blockWinerID = planetData.pattern.biomesSurface[biomeNum].genRules[num].blockID;
        }

        //Если блок ниже поверхности то проверяем еще срети подземных биомов
        if (heightNow < heightSurface)
        {

        }

        //Если шум выше 0.5 то блок победил
        if (noiseWiner > 0.5f)
            BlocksID[pos.x, pos.y, pos.z] = blockWinerID;
        else BlocksID[pos.x, pos.y, pos.z] = 0;
    }

    protected override void CalcColor()
    {
        if (Colors == null)
            Colors = new Color[Size, Size, Size];

        //Нужно определить коофицент интерполяции цвета между цветом биома и блоком
        float interpolateCoof = ((int)sizeBlock - 1) / 7;


        Vector2Int index2D = new Vector2Int(index.x, index.z);
        //Получаем информацию биомов поверхности для каждого блока
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //Перебираем все блоки
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {
                //Узнаем биом победителя
                int biomeWinerSurface = biomeMap.winers[x, z];

                for (int y = 0; y < Size; y++)
                {

                    //Смотрим по id блоку на его цвет
                    int BID = BlocksID[x, z, y];
                    Color ColorBlock = Game.Blocks.GetColor(BID);

                    Colors[x, y, z] = Color.Lerp(ColorBlock, planetData.pattern.biomesSurface[biomeWinerSurface].color, interpolateCoof);
                }
            }
        }
    }

}
