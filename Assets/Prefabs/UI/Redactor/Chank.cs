using Cosmos;
using UnityEngine;
using Unity.Jobs;
using System;

/// <summary>
/// ’ранит данные чанка
/// </summary>
public abstract class Chank
{
    static class DefaultValue {
        static public readonly float[,,] Illumination;

        static DefaultValue() {
            Illumination = new float[Size, Size, Size];

            for (int x = 0; x < Illumination.GetLength(0); x++) {
                for (int y = 0; y < Illumination.GetLength(1); y++) {
                    for (int z = 0; z < Illumination.GetLength(2); z++) {
                        Illumination[x, y, z] = 0.99f;
                    }
                }
            }
        }
    }

    //„анк всегда кубический и вмещает в себ€ 32 блока по высоте ширене и длине.
    public const int Size = 32;


    public Size sizeBlock = global::Size.s1; //размер одного блока в чанке
    public Vector3Int index = new Vector3Int(); //индекс чанка

    public int[,,] BlocksID = new int[Size, Size, Size]; //ID Ѕлоков
    public uint[,,] BlocksVariant = new uint[Size, Size, Size]; //¬ариант блоков
    public Color[,,] Colors = new Color[Size, Size, Size]; //÷вета блоков
    public float[,,] Light = new float[Size, Size, Size]; // оличество света в блоке

    protected PlanetData planetData;
    public event Action isChange;
    public Times times;

    /// <summary>
    /// генераци€ чанка
    /// </summary>
    protected bool isStartGenerate = false;
    protected bool isLocalGenerateDone = false; // огда генераци€ в этом чанке завершена и данными могут пользоватьс€ другие чанки, но сам чанк еще должен проверить соседей
    protected bool isDoneGenerate = false;


    public struct Times {
        public double lastChange;

    }

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

    virtual public Color[,,] GetColors() {
        return Colors;
    }
    virtual public Chank GetNeighbour(Side side) {
        Chank result = null;

        Vector3Int indexNeighbour = index;
        switch (side) {
            case Side.left:
                indexNeighbour.x--;
                break;
            case Side.right:
                indexNeighbour.x++;
                break;
            case Side.down:
                indexNeighbour.y--;
                break;
            case Side.up:
                indexNeighbour.y++;
                break;
            case Side.back:
                indexNeighbour.z--;
                break;
            case Side.face:
                indexNeighbour.z++;
                break;
        }

        int indexSize = (int)sizeBlock - 1;

        //≈сли вышли за пределы карты
        if (indexNeighbour.x < 0 || indexNeighbour.y < 0 || indexNeighbour.z < 0 ||
            indexNeighbour.x >= planetData.chanks[indexSize].GetLength(0) ||
            indexNeighbour.y >= planetData.chanks[indexSize].GetLength(1) ||
            indexNeighbour.z >= planetData.chanks[indexSize].GetLength(2))
            return null;

        result = planetData.chanks[indexSize][indexNeighbour.x, indexNeighbour.y, indexNeighbour.z];
        return result;
    }

    /// <summary>
    /// —оздать чанк с указанным размером блоков, по указанному индексу, дл€ указанной планеты
    /// </summary>
    /// <param name="sizeBlock"></param>
    /// <param name="index"></param>
    /// <param name="planetData"></param>
    public Chank(Size sizeBlock, Vector3Int index, PlanetData planetData) {
        this.Light = DefaultValue.Illumination;

        this.sizeBlock = sizeBlock;
        this.index = index;
        this.planetData = planetData;

        iniActIsChange();

        void iniActIsChange()
        {
            isChange += fixChangeTime;
        }
    }

    abstract protected void JobStartGenerate();
    /// <summary>
    /// ¬ычислить финальный цвет дл€ блоков
    /// </summary>
    abstract protected void CalcColor();

    void fixChangeTime() {
        times.lastChange = Time.unscaledTimeAsDouble;
    }

}
