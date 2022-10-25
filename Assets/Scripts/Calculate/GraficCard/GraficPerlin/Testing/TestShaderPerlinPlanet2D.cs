using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestShaderPerlinPlanet2D : MonoBehaviour
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
    [SerializeField]
    bool TimeX = false;
    [SerializeField]
    bool TimeZ = false;

    void UpdateImage()
    {
        //Создаем новую текстуру
        Texture2D texture;

        int sizeY = 128;
        int sizeX = sizeY * 2;

        texture = new Texture2D(sizeX, sizeY * 2);

        float[,] map = GraficData.Perlin2D.GetArrayMap(sizeX, sizeY, Scale, Scale, Scale, Freq, OffSetX, OffSetY, OffSetZ, Octaves, TimeX, TimeZ);

        //рисование основной текстуры - нижняя половина
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Color color = new Color(map[x, y], map[x, y], map[x, y]);
                if (map[x, y] < 0.5f)
                    color = Color.blue;

                texture.SetPixel(x, y, color);
            }
        }
        //Рисование отзеркаленной текстуры верхняя половина
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {

                //По х смещяем на половину
                int mirrorX = x - (map.GetLength(0) / 2);
                //По у отзеркаливаем
                int mirrorY = map.GetLength(1) - 1 - y;

                if (mirrorX < 0)
                    mirrorX += map.GetLength(0);

                Color color = texture.GetPixel(mirrorX, mirrorY);

                texture.SetPixel(x, y + map.GetLength(1), color);
            }
        }

        /*
        float FactorChank = (GraficData.Perlin2D.factor / Scale) * 32;

        for (int chankX = 0; chankX < sizeX / 32; chankX++) {
            int chankPixelStartX = chankX * 32;

            for (int chankY = 0; chankY < sizeY / 32; chankY++) {
                int chankPixelStartY = chankY * 32;

                float offSetX = OffSetX + FactorChank * chankX;
                float offSetY = OffSetY + FactorChank * chankY;
                float offSetZ = OffSetZ;

                if (TimeZ)
                    offSetZ += Time.time * 0.1f;

                if (TimeX)
                    offSetX += Time.time * 0.1f;

                float regionX = (chankX * 32)/ (float)sizeX;
                float regionY = (chankY * 32)/ (float)sizeY;

                GraficData.Perlin2D dataPerlin2D = new GraficData.Perlin2D(Scale, Freq, offSetX, offSetY, offSetZ, Octaves, Best, sizeX, sizeY, regionX, regionY);
                dataPerlin2D.Calculate();

                //Запихиваем данные в текстуру
                for (int x = 0; x < 32; x++) {
                    for (int y = 0; y < 32; y++) {
                        float result = dataPerlin2D.result[x, y, 0];
                        Color color = new Color(result, result, result);

                        //присваиваем пикселю цвет
                        texture.SetPixel(chankPixelStartX + x, chankPixelStartY + y, color);
                    }
                }
            }
        }
        */

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
