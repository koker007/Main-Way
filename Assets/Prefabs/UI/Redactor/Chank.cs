using Cosmos;
using UnityEngine;
using Unity.Jobs;

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
