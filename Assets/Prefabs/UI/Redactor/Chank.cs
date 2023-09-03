using Cosmos;
using UnityEngine;
using Unity.Jobs;
using System;

/// <summary>
/// ������ ������ �����
/// </summary>
public abstract class Chank
{
    static class DefaultValue {
        static public readonly float[,,] Illumination;

        static DefaultValue(){
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

    //���� ������ ���������� � ������� � ���� 32 ����� �� ������ ������ � �����.
    public const int Size = 32;


    public Size sizeBlock = global::Size.s1; //������ ������ ����� � �����
    public Vector3Int index = new Vector3Int(); //������ �����

    public int[,,] BlocksID = new int[Size, Size, Size]; //ID ������
    public uint[,,] BlocksVariant = new uint[Size, Size, Size]; //������� ������
    public Color[,,] Colors = new Color[Size, Size, Size]; //����� ������
    public float[,,] Illumination = new float[Size, Size, Size]; //���������� ����� � �����

    protected PlanetData planetData;
    public event Action isChange;

    /// <summary>
    /// ��������� �����
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

        //���� ����� �� ������� �����
        if (indexNeighbour.x < 0 || indexNeighbour.y < 0 || indexNeighbour.z < 0 ||
            indexNeighbour.x >= planetData.chanks[indexSize].GetLength(0) ||
            indexNeighbour.y >= planetData.chanks[indexSize].GetLength(1) ||
            indexNeighbour.z >= planetData.chanks[indexSize].GetLength(2))
            return null;

        result = planetData.chanks[indexSize][indexNeighbour.x, indexNeighbour.y, indexNeighbour.z];
        return result;
    }

    //�����, ����, ���� ����� ������� job system ��� �������� �����

    /// <summary>
    /// ������� ���� � ��������� �������� ������, �� ���������� �������, ��� ��������� �������
    /// </summary>
    /// <param name="sizeBlock"></param>
    /// <param name="index"></param>
    /// <param name="planetData"></param>
    public Chank(Size sizeBlock, Vector3Int index, PlanetData planetData) {
        this.Illumination = DefaultValue.Illumination;

        this.sizeBlock = sizeBlock;
        this.index = index;
        this.planetData = planetData;
    }

    abstract protected void JobStartGenerate();
    /// <summary>
    /// ��������� ��������� ���� ��� ������
    /// </summary>
    abstract protected void CalcColor();

}
