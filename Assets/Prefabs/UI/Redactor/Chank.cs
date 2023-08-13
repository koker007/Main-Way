using Cosmos;
using UnityEngine;

/// <summary>
/// ’ранит данные чанка
/// </summary>
public class Chank
{
    //„анк всегда кубический и вмещает в себ€ 32 блока по высоте ширене и длине.
    public const int Size = 32;

    public Size sizeBlock = global::Size.s1;
    public Vector3Int index = new Vector3Int();

    public int[,,] BlocksID = new int[Size, Size, Size];
    Color[,,] Colors;
    int[,,] BiomeNum;

    float[][,,,] biomesNoiseSurface;
    float[][,,,] biomesNoiseUnderground;

    PlanetData planetData;


    /// <summary>
    /// —оздать чанк с указанным размером блоков, по указанному индексу, дл€ указанной планеты
    /// </summary>
    /// <param name="sizeBlock"></param>
    /// <param name="index"></param>
    /// <param name="planetData"></param>
    public Chank(Size sizeBlock, Vector3Int index, PlanetData planetData) {
        this.sizeBlock = sizeBlock;
        this.index = index;
        this.planetData = planetData;

        //—оздать пространство под шум поверхности
        this.biomesNoiseSurface = new float[planetData.pattern.biomesSurface.Count][,,,];
        //—оздать пространство под шум подземлей
        this.biomesNoiseUnderground = new float[planetData.pattern.biomesUnderground.Count][,,,];
    }

    void GenerateColors() {
        //”читыва€ текущий размер чанка сгенерировать его

        //нужно узнать по каждому из блоков к какому биому он принадлежит

        Vector2Int index2D = new Vector2Int(index.x, index.z);
        int planetSize = Calc.GetSizeInt(planetData.size);

        //ѕолучаем информацию о высоте поверхности дл€ каждого блока
        HeightMap heightMap = planetData.GetHeightMap(sizeBlock, index2D);

        //ѕолучаем информацию биомов поверхности дл€ каждого блока
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //Ќа основе размера планеты преобразуем в высоту блоков
        int[,] heightMapInt = new int[heightMap.map.GetLength(0), heightMap.map.GetLength(1)];
        for (int x = 0; x < heightMapInt.GetLength(0); x++) {
            for (int z = 0; z < heightMapInt.GetLength(1); z++) {
                heightMapInt[x, z] = (int)(heightMap.map[x, z] * planetSize);
            }
        }

        //“еперь знаем высоту выше которой начинаетс€ биом

        Vector3Int worldPos = new Vector3Int();

        //ѕеребираем координаты x z чанка
        for (int x = 0; x < Size; x++) {
            for (int z = 0; z < Size; z++) {

                //Ќужно определить по правилам какого биома генерируетс€ область
                //—мотрим в карту биома
                int biomeNum = biomeMap.winers[x, z];
                float biomeIntensive = biomeMap.winersIntensive[x, z];
                //«наем номер биома победител€
                //«наем интенсивность биома

                //¬ычисл€ем шумы поверхности
                CalcNoisesSurface(biomeNum);
                //¬ычисл€ем шумы подземности
                CalcNoisesUnderground(biomeNum);

                int heightSurface = (int)(heightMap.map[x, z] * planetSize/2);

                //ѕеребираем каждую высоту чанка
                for (int y = 0; y < Size; y++) {

                    //ищем блок победитель
                    CalcWiner(new Vector3Int(x,y,z), biomeNum, heightSurface);
                }
                
            }
        }

    }

    void CalcNoisesSurface(int biomeNum) {
        if (biomesNoiseSurface[biomeNum] != null) {
            return;
        }

        int sizePlanet = Calc.GetSizeInt(planetData.size);
        int sizeBlock = Calc.GetSizeInt(this.sizeBlock);

        //≈сли шума нет, то шумим
        biomesNoiseSurface[biomeNum] = planetData.pattern.biomesSurface[biomeNum].GetBiomeNoise(index, sizeBlock, sizePlanet);

        //Ёто биом поверхности, необходимо сделать поправку на высоту

    }
    void CalcNoisesUnderground(int biomeNum)
    {
        if (biomesNoiseUnderground[biomeNum] != null)
        {
            return;
        }

        //≈сли шума нет, то шумим
        //biomesNoiseUnderground[biomeNum] = planetData.pattern.biomesUnderground[biomeNum].GetBiomeNoise(index, sizeBlock);

        //Ёто биом поверхности, необходимо сделать поправку на высоту

    }

    void CalcWiner(Vector3Int pos, int biomeNum, int heightSurface) {
        
        //“екуща€ высота, данного блока
        int heightNow = (index.y * Chank.Size + pos.y) * Calc.GetSizeInt(sizeBlock);

        //ѕеребираем все блоки в биоме
        int blockWinerID = 0;
        float noiseWiner = 0;

        //≈сли высота выше поверхности
        if (heightNow >= heightSurface)
        {

            //генерируем по правилам поверхностных биомов
            for (int num = 0; num < biomesNoiseSurface[biomeNum].GetLength(3); num++)
            {
                if (biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num] < noiseWiner)
                    continue;

                noiseWiner = biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num];
                blockWinerID = planetData.pattern.biomesSurface[biomeNum].genRules[num].blockID;
            }
        }
        else {
            //√енерируем по правилам подземных биомов

        }

        BlocksID[pos.x, pos.y, pos.z] = blockWinerID;
    }
}
