using Cosmos;
using UnityEngine;

/// <summary>
/// Хранит данные чанка
/// </summary>
public abstract class Chank
{
    //Чанк всегда кубический и вмещает в себя 32 блока по высоте ширене и длине.
    public const int Size = 32;

    public Size sizeBlock = global::Size.s1; //размер одного блока в чанке
    public Vector3Int index = new Vector3Int(); //индекс чанка

    public int[,,] BlocksID = new int[Size, Size, Size]; //ID Блоков
    public Color[,,] Colors; //Цвета блоков

    protected PlanetData planetData;


    /// <summary>
    /// Создать чанк с указанным размером блоков, по указанному индексу, для указанной планеты
    /// </summary>
    /// <param name="sizeBlock"></param>
    /// <param name="index"></param>
    /// <param name="planetData"></param>
    public Chank(Size sizeBlock, Vector3Int index, PlanetData planetData) {
        this.sizeBlock = sizeBlock;
        this.index = index;
        this.planetData = planetData;
    }

    abstract public void Generate();
    /// <summary>
    /// Вычислить финальный цвет для блоков
    /// </summary>
    abstract protected void CalcColor();

}
