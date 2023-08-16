using Cosmos;
using UnityEngine;
using Unity.Jobs;

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
    public Color[,,] Colors; //����� ������

    protected PlanetData planetData;

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
