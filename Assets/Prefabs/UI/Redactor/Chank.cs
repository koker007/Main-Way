using Cosmos;
using UnityEngine;

/// <summary>
/// ������ ������ �����
/// </summary>
public class Chank
{
    //���� ������ ���������� � ������� � ���� 32 ����� �� ������ ������ � �����.
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
    /// ������� ���� � ��������� �������� ������, �� ���������� �������, ��� ��������� �������
    /// </summary>
    /// <param name="sizeBlock"></param>
    /// <param name="index"></param>
    /// <param name="planetData"></param>
    public Chank(Size sizeBlock, Vector3Int index, PlanetData planetData) {
        this.sizeBlock = sizeBlock;
        this.index = index;
        this.planetData = planetData;

        //������� ������������ ��� ��� �����������
        this.biomesNoiseSurface = new float[planetData.pattern.biomesSurface.Count][,,,];
        //������� ������������ ��� ��� ���������
        this.biomesNoiseUnderground = new float[planetData.pattern.biomesUnderground.Count][,,,];
    }

    void GenerateColors() {
        //�������� ������� ������ ����� ������������� ���

        //����� ������ �� ������� �� ������ � ������ ����� �� �����������

        Vector2Int index2D = new Vector2Int(index.x, index.z);
        int planetSize = Calc.GetSizeInt(planetData.size);

        //�������� ���������� � ������ ����������� ��� ������� �����
        HeightMap heightMap = planetData.GetHeightMap(sizeBlock, index2D);

        //�������� ���������� ������ ����������� ��� ������� �����
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //�� ������ ������� ������� ����������� � ������ ������
        int[,] heightMapInt = new int[heightMap.map.GetLength(0), heightMap.map.GetLength(1)];
        for (int x = 0; x < heightMapInt.GetLength(0); x++) {
            for (int z = 0; z < heightMapInt.GetLength(1); z++) {
                heightMapInt[x, z] = (int)(heightMap.map[x, z] * planetSize);
            }
        }

        //������ ����� ������ ���� ������� ���������� ����

        Vector3Int worldPos = new Vector3Int();

        //���������� ���������� x z �����
        for (int x = 0; x < Size; x++) {
            for (int z = 0; z < Size; z++) {

                //����� ���������� �� �������� ������ ����� ������������ �������
                //������� � ����� �����
                int biomeNum = biomeMap.winers[x, z];
                float biomeIntensive = biomeMap.winersIntensive[x, z];
                //����� ����� ����� ����������
                //����� ������������� �����

                //��������� ���� �����������
                CalcNoisesSurface(biomeNum);
                //��������� ���� �����������
                CalcNoisesUnderground(biomeNum);

                int heightSurface = (int)(heightMap.map[x, z] * planetSize/2);

                //���������� ������ ������ �����
                for (int y = 0; y < Size; y++) {

                    //���� ���� ����������
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

        //���� ���� ���, �� �����
        biomesNoiseSurface[biomeNum] = planetData.pattern.biomesSurface[biomeNum].GetBiomeNoise(index, sizeBlock, sizePlanet);

        //��� ���� �����������, ���������� ������� �������� �� ������

    }
    void CalcNoisesUnderground(int biomeNum)
    {
        if (biomesNoiseUnderground[biomeNum] != null)
        {
            return;
        }

        //���� ���� ���, �� �����
        //biomesNoiseUnderground[biomeNum] = planetData.pattern.biomesUnderground[biomeNum].GetBiomeNoise(index, sizeBlock);

        //��� ���� �����������, ���������� ������� �������� �� ������

    }

    void CalcWiner(Vector3Int pos, int biomeNum, int heightSurface) {
        
        //������� ������, ������� �����
        int heightNow = (index.y * Chank.Size + pos.y) * Calc.GetSizeInt(sizeBlock);

        //���������� ��� ����� � �����
        int blockWinerID = 0;
        float noiseWiner = 0;

        //���� ������ ���� �����������
        if (heightNow >= heightSurface)
        {

            //���������� �� �������� ������������� ������
            for (int num = 0; num < biomesNoiseSurface[biomeNum].GetLength(3); num++)
            {
                if (biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num] < noiseWiner)
                    continue;

                noiseWiner = biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num];
                blockWinerID = planetData.pattern.biomesSurface[biomeNum].genRules[num].blockID;
            }
        }
        else {
            //���������� �� �������� ��������� ������

        }

        BlocksID[pos.x, pos.y, pos.z] = blockWinerID;
    }
}
