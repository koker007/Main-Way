using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Паттерн генерации планеты
/// </summary>
public class PatternPlanet
{
    public string name;

    public Terms termsGenerate;

    /// <summary>
    /// Список биомов на планете
    /// </summary>
    public List<BiomeTypeSurface> biomesSurface;
    public List<BiomeTypeUnderground> biomesUnderground;

    /// <summary>
    /// Условия генерации этого паттерна
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


    //Глобальные параметры генерации планеты
    public Parameters parameters;

    public class Parameters {
        //Основная жидкость на планете
        public int LiquidLevelMax;
        public int LiquidLevelMin;

        //Atmosphere
        public int AtmosphereMax;
        public int AtmosphereMin;
    }

    /// <summary>
    /// Конструктор паттерна планеты
    /// </summary>
    /// <param name="name"></param>
    /// <param name="terms"></param>
    /// <param name="parameters"></param>
    /// <param name="listNoisesData"></param>
    public PatternPlanet(string name, Terms terms, Parameters parameters) {
        this.name = name;
        this.termsGenerate = terms;
        this.parameters = parameters;
    }


    static public PatternPlanet GetTestPattern() {
        Terms terms = new Terms();
        terms.sizeMin = Size.s1024;
        terms.sizeMax = Size.s8192;

        terms.starSpectre = StarSpectre.NoMatter;
        terms.temperatuteMin = 0;
        terms.temperatureMax = Calc.GetSizeInt(Size.s65536);

        terms.tidalLocking = TidalLocking.NoMatter;

        Parameters parameters = new Parameters();
        parameters.AtmosphereMin = 0;
        parameters.AtmosphereMax = Calc.GetSizeInt(Size.s65536);

        parameters.LiquidLevelMin = 0;
        parameters.LiquidLevelMax = 100;

        PatternPlanet pattern = new PatternPlanet("Test", terms, parameters);



        return pattern;
    }
}
