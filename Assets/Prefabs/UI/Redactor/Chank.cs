using Cosmos;
using UnityEngine;
using Unity.Jobs;
using System;

/// <summary>
/// ������ ������ �����
/// </summary>
public abstract class Chank
{
    //���� ������ ���������� � ������� � ���� 32 ����� �� ������ ������ � �����.
    public const int Size = 32;


    public Size sizeBlock = global::Size.s1; //������ ������ ����� � �����
    public Vector3Int index = new Vector3Int(); //������ �����

    public int[,,] BlocksID = new int[Size, Size, Size]; //ID ������
    public uint[,,] BlocksVariant = new uint[Size, Size, Size]; //������� ������
    public Color[,,] Colors; //����� ������

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

    //�����, ����, ���� ����� ������� job system ��� �������� �����

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
    }

    abstract protected void JobStartGenerate();
    /// <summary>
    /// ��������� ��������� ���� ��� ������
    /// </summary>
    abstract protected void CalcColor();

}
