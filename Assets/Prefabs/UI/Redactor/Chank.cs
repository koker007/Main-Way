using Cosmos;
using UnityEngine;

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

    abstract public void Generate();
    /// <summary>
    /// ��������� ��������� ���� ��� ������
    /// </summary>
    abstract protected void CalcColor();

}
