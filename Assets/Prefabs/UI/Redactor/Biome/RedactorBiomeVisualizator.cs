using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBiomeVisualizator : MonoBehaviour
{
    static private RedactorBiomeVisualizator main;

    static public RedactorBiomeVisualizator MAIN { get { return main; }  }

    RawImage renderTexture;

    public float[,] heightMapAll;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniRawImage();

    }

    void iniRawImage() {
        renderTexture = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        //�������� ��������� ���� ��������
        RedactorBiomeGenerator.TestOpen();
        TestRenderTexture();

        SetPlanetHeightMap();
    }

    void TestRenderTexture() {
        renderTexture.texture = RedactorBiomeGenerator.GetRender();
    }

    void SetPlanetHeightMap() {
        
        if (RedactorBiomeCTRL.main.planetData == null)
            return;

        Size quality = Calc.GetSize(Calc.GetSizeInt(RedactorBiomeCTRL.main.planetData.size) / Chank.Size);

        //���������� ���������� ����� ����� ���� �� ���
        if (heightMapAll == null)
        {
            Debug.Log(RedactorBiomeCTRL.main.planetData.size);

            Cosmos.HeightMap[,] heightMaps = RedactorBiomeCTRL.main.planetData.GetHeightMap(quality);

            //������������� � �������� ����� �����
            heightMapAll = new float[heightMaps.GetLength(0) * Chank.Size, heightMaps.GetLength(1) * Chank.Size];

            //���������� ��� �����
            for (int chx = 0; chx < heightMaps.GetLength(0); chx++)
            {
                for (int chy = 0; chy < heightMaps.GetLength(1); chy++)
                {
                    float[,] chank = heightMaps[chx, chy].map;

                    //����������� ���������� ������� � ��������� ������
                    for (int x = 0; x < chank.GetLength(0); x++)
                    {
                        int globalPosX = chx * Chank.Size + x;
                        for (int y = 0; y < chank.GetLength(1); y++)
                        {
                            int globalPosY = chy * Chank.Size + y;
                            heightMapAll[globalPosX, globalPosY] = chank[x, y];
                        }
                    }
                }
            }
        }

        RedactorBiomeGenerator.SetHeightMap(heightMapAll, RedactorBiomeCTRL.main.planetData);
    }
}
