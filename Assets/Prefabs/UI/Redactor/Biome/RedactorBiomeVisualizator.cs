using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBiomeVisualizator : MonoBehaviour
{
    static private RedactorBiomeVisualizator main;

    static public RedactorBiomeVisualizator MAIN { get { return main; }  }

    RawImage renderTexture;

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
        //¬ключить генератор если выключен
        RedactorBiomeGenerator.TestOpen();
        TestRenderTexture();

        //SetPlanetHeightMap();
    }

    void TestRenderTexture() {
        renderTexture.texture = RedactorBiomeGenerator.GetRender();
    }

    void SetPlanetHeightMap() {
        Size quality = Size.s64;
        float[,] heightMap = RedactorBiomeCTRL.main.planetData.GetHeightMap((int)quality);
        RedactorBiomeGenerator.SetHeightMap(heightMap, quality);
    }
}
