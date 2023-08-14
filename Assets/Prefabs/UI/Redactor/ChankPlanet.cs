using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;

public class ChankPlanet : Chank
{

    float[][,,,] biomesNoiseSurface;
    float[][,,,] biomesNoiseUnderground;

    public ChankPlanet(Size sizeBlock, Vector3Int index, PlanetData planetData): base(sizeBlock, index, planetData) {

        //������� ������������ ��� ��� �����������
        this.biomesNoiseSurface = new float[planetData.pattern.biomesSurface.Count][,,,];
        //������� ������������ ��� ��� ���������
        this.biomesNoiseUnderground = new float[planetData.pattern.biomesUnderground.Count][,,,];
    }

    public override void Generate()
    {

        //����� ������ �� ������� �� ������ � ������ ����� �� �����������

        Vector2Int index2D = new Vector2Int(index.x, index.z);
        int planetSize = Calc.GetSizeInt(planetData.size);

        //�������� ���������� � ������ ����������� ��� ������� �����
        HeightMap heightMap = planetData.GetHeightMap(sizeBlock, index2D);

        //�������� ���������� ������ ����������� ��� ������� �����
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //���������� ���������� x z �����
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {

                //����� ���������� �� �������� ������ ����� ������������ �������
                int biomeNum = biomeMap.winers[x, z];

                //��������� ���� ����������� (��� ����� ����������)
                CalcNoisesSurface(biomeNum);
                //��������� ���� �����������
                //CalcNoisesUnderground(biomeNum);

                int heightSurface = (int)(heightMap.map[x, z] * planetSize / 2);

                //���������� ������ ������ �����
                for (int y = 0; y < Size; y++)
                {

                    //���� ���� ����������
                    CalcWiner(new Vector3Int(x, y, z), biomeNum, heightSurface);
                }

            }
        }

        //ID ����� ��� ��������

        //������ ���� ������ ����� ������ 1, �� ����� ��� ��������� ����
        CalcColor();


    }

    void CalcNoisesSurface(int biomeNum)
    {
        if (biomesNoiseSurface[biomeNum] != null)
        {
            return;
        }

        int sizeBlock = Calc.GetSizeInt(this.sizeBlock);

        //�������� ��� ����� �����������
        biomesNoiseSurface[biomeNum] = planetData.pattern.biomesSurface[biomeNum].GetBiomeNoise(index, sizeBlock, planetData);



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

    void CalcWiner(Vector3Int pos, int biomeNum, int heightSurface)
    {

        //������� ������, ������� �����
        int heightNow = (index.y * Chank.Size + pos.y) * Calc.GetSizeInt(sizeBlock);

        //���������� ��� ����� � �����
        int blockWinerID = 0;
        float noiseWiner = 0;


        //���� ���������� ����� �����������
        for (int num = 0; num < biomesNoiseSurface[biomeNum].GetLength(3); num++)
        {
            if (biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num] < noiseWiner)
                continue;

            noiseWiner = biomesNoiseSurface[biomeNum][pos.x, pos.y, pos.z, num];
            blockWinerID = planetData.pattern.biomesSurface[biomeNum].genRules[num].blockID;
        }

        //���� ���� ���� ����������� �� ��������� ��� ����� ��������� ������
        if (heightNow < heightSurface)
        {

        }

        //���� ��� ���� 0.5 �� ���� �������
        if (noiseWiner > 0.5f)
            BlocksID[pos.x, pos.y, pos.z] = blockWinerID;
        else BlocksID[pos.x, pos.y, pos.z] = 0;
    }

    protected override void CalcColor()
    {
        if (Colors == null)
            Colors = new Color[Size, Size, Size];

        //����� ���������� ��������� ������������ ����� ����� ������ ����� � ������
        float interpolateCoof = ((int)sizeBlock - 1) / 7;


        Vector2Int index2D = new Vector2Int(index.x, index.z);
        //�������� ���������� ������ ����������� ��� ������� �����
        BiomeMaps biomeMap = planetData.GetBiomePart(sizeBlock, index2D);

        //���������� ��� �����
        for (int x = 0; x < Size; x++)
        {
            for (int z = 0; z < Size; z++)
            {
                //������ ���� ����������
                int biomeWinerSurface = biomeMap.winers[x, z];

                for (int y = 0; y < Size; y++)
                {

                    //������� �� id ����� �� ��� ����
                    int BID = BlocksID[x, z, y];
                    Color ColorBlock = GameData.Blocks.GetColor(BID);

                    Colors[x, y, z] = Color.Lerp(ColorBlock, planetData.pattern.biomesSurface[biomeWinerSurface].color, interpolateCoof);
                }
            }
        }
    }
}
