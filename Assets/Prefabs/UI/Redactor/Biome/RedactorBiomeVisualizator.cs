using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBiomeVisualizator : MonoBehaviour
{
    static private RedactorBiomeVisualizator main;

    static public RedactorBiomeVisualizator MAIN { get { return main; }  }

    RawImage renderTexture;

    float[,] heightMapAll;

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
        //Включить генератор если выключен
        RedactorBiomeGenerator.TestOpen();
        TestRenderTexture();

        SetPlanetHeightMap();
    }

    void TestRenderTexture() {
        renderTexture.texture = RedactorBiomeGenerator.GetRender();
    }

    void SetPlanetHeightMap() {
        Size quality = Size.s64;

        if (RedactorBiomeCTRL.main.planetData == null)
            return;

        //Генерируем глобальную карту высот если ее нет
        if (heightMapAll == null)
        {
            Cosmos.HeightMap[,] heightMaps = RedactorBiomeCTRL.main.planetData.GetHeightMap(quality);

            //Опеределяемся с размером карты высот
            heightMapAll = new float[heightMaps.GetLength(0) * Chank.Size, heightMaps.GetLength(1) * Chank.Size];

            //Перебираем все чанки
            for (int chx = 0; chx < heightMaps.GetLength(0); chx++)
            {
                for (int chy = 0; chy < heightMaps.GetLength(1); chy++)
                {
                    float[,] chank = heightMaps[chx, chy].map;

                    //Расчитываем глобальную позицию и добавляем данные
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

        RedactorBiomeGenerator.SetHeightMap(heightMapAll, quality, RedactorBiomeCTRL.main.planetData);
    }
}
