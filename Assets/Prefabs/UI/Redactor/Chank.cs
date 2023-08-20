using Cosmos;
using UnityEngine;
using Unity.Jobs;
using System;

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
    public uint[,,] BlocksVariant = new uint[Size, Size, Size]; //Вариант блоков
    public Color[,,] Colors; //Цвета блоков

    protected PlanetData planetData;
    public event Action isChange;

    /// <summary>
    /// генерация чанка
    /// </summary>
    protected bool isStartGenerate = false;
    protected bool isDoneGenerate = false;
    protected struct JobGenerate : IJob
    {
        Chank chank;

        public JobGenerate(Chank chank)
        {
            this.chank = chank;
        }

        public void Execute()
        {
            if (chank.isStartGenerate ||
                chank.isDoneGenerate)
                return;

            chank.isStartGenerate = true;
            chank.JobStartGenerate();
            chank.isDoneGenerate = true;
        }
    }
    abstract public void Generate();

    public enum Placement
    {
        Current = 0,
        Left = 1,
        Right = 2,
        Downer = 3,
        Upper = 4,
        Back = 5,
        Front = 6
    }

    abstract public Color GetColor(Vector3Int pos, Placement placement = Placement.Current);
    abstract public BlockData GetBlock(Vector3Int pos, Placement placement = Placement.Current);

    //Потом, ниже, надо будет создать job system для загрузки чанка

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

    abstract protected void JobStartGenerate();
    /// <summary>
    /// Вычислить финальный цвет для блоков
    /// </summary>
    abstract protected void CalcColor();

}
