using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ��������� �������
/// </summary>
public class PatternPlanet
{
    public string name;

    public Terms termsGenerate;

    /// <summary>
    /// ������� ��������� ����� ��������
    /// </summary>
    public class Terms
    {
        public Size sizeMax;
        public Size sizeMin;

        public float temperatureMax;
        public float temperatuteMin;

        public StarSpectre starSpectre;
        public TidalLocking tidalLocking;
    }

    public enum StarSpectre
    {
        NoMatter,
        Blue,
        BlueWhite,
        White,
        WhiteYellow,
        Yellow,
        Orange,
        Red,
        Brown,
        BlackHole
    }
    public enum TidalLocking
    {
        No,
        NoMatter,
        Yes
    }


    //���������� ��������� ��������� �������
    public Parameters parameters;

    public class Parameters {
        //�������� �������� �� �������
        public int LiquidLevelMax;
        public int LiquidLevelMin;

        //Atmosphere
        public int AtmosphereMax;
        public int AtmosphereMin;
    }

    //������ ����� ������������ � ��������� �������
    public List<NoisePlanetData> listNoisesData = new List<NoisePlanetData>();


    /// <summary>
    /// ����������� �������� �������
    /// </summary>
    /// <param name="name"></param>
    /// <param name="terms"></param>
    /// <param name="parameters"></param>
    /// <param name="listNoisesData"></param>
    public PatternPlanet(string name, Terms terms, Parameters parameters, List<NoisePlanetData> listNoisesData) {
        this.name = name;
        this.termsGenerate = terms;
        this.parameters = parameters;

        this.listNoisesData = listNoisesData;
    }

}

/// <summary>
/// ������ ���� �������
/// </summary>
public class NoisePlanetData : NoiseData
{
    public TypeNoise type;

    /// <summary>
    /// ��� ���� ��� ���� ����� ������ ��� ��� ������������
    /// </summary>
    public enum TypeNoise
    {
        heightMap = 0,
        biomeMap = 1
    }

    public NoisePlanetData(NoiseData noiseData, TypeNoise type)
    {
        scaleX = noiseData.scaleX;
        scaleY = noiseData.scaleY;
        scaleZ = noiseData.scaleZ;

        freq = noiseData.freq;
        octaves = noiseData.octaves;

        polusFactor = noiseData.polusFactor;

        this.type = type;
    }
}
