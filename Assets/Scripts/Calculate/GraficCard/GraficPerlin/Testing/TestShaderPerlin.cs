using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestShaderPerlin : MonoBehaviour
{

    [SerializeField]
    RawImage rawImage;

    [Header("Parametrs")]
    [SerializeField]
    float Scale = 1;
    [SerializeField]
    float Freq = 1;
    [SerializeField]
    float OffSetX = 0;
    [SerializeField]
    float OffSetY = 0;
    [SerializeField]
    float OffSetZ = 0;
    [SerializeField]
    int Octaves = 0;
    [SerializeField]
    bool Best = false;

    void UpdateImage()
    {
        //Создаем новую текстуру
        Texture2D texture;
        texture = new Texture2D(8,8);

        GraficData.Perlin dataPerlin = new GraficData.Perlin(Scale, Freq, OffSetX, OffSetY, OffSetZ, Octaves, Best);
        dataPerlin.Calculate();

        //ВЫчисления перлиа произведены
        //заносим данные в текстуру
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                float result = dataPerlin.result[x, y, 0];
                Color color = new Color(result, result, result);
                texture.SetPixel(x,y, color);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply(false);

        //Заменяем старую текстуру новой
        rawImage.texture = texture;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImage();
    }
}
