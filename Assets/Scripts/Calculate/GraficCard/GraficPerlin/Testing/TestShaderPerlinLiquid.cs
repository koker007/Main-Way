using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TestShaderPerlinLiquid : MonoBehaviour
{

    [SerializeField]
    RawImage imageFace;
    [SerializeField]
    RawImage imageLeft;
    [SerializeField]
    RawImage imageRight;
    [SerializeField]
    RawImage imageBack;
    [SerializeField]
    RawImage imageUp;
    [SerializeField]
    RawImage imageDown;


    [Header("Parametrs")]
    [SerializeField]
    float Scale = 1;
    [SerializeField]
    float ScaleX = 1;
    [SerializeField]
    float ScaleY = 1;
    [SerializeField]
    float ScaleZ = 1;
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
        //—оздаем новую текстуру
        Texture2D textureFace;
        Texture2D textureLeft;
        Texture2D textureRight;
        Texture2D textureBack;
        Texture2D textureUp;
        Texture2D textureDown;

        int sizeY = 16;
        int sizeX = 16;

        textureFace = new Texture2D(sizeX, sizeY);
        textureLeft = new Texture2D(sizeX, sizeY);
        textureRight = new Texture2D(sizeX, sizeY);
        textureBack = new Texture2D(sizeX, sizeY);
        textureUp = new Texture2D(sizeX, sizeY);
        textureDown = new Texture2D(sizeX, sizeY);

        textureFace.filterMode = FilterMode.Point;
        textureLeft.filterMode = FilterMode.Point;
        textureRight.filterMode = FilterMode.Point;
        textureBack.filterMode = FilterMode.Point;
        textureUp.filterMode = FilterMode.Point;
        textureDown.filterMode = FilterMode.Point;

        float[,,] map = GraficData.PerlinCube.GetArrayMap(Scale * ScaleX, Scale * ScaleY, Scale * ScaleZ, Freq, OffSetX, OffSetY, OffSetZ, Octaves);

        //рисование основной текстуры - нижн€€ половина
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                //Face
                Color color = new Color(map[x, y, 0], map[x, y ,0], map[x, y ,0]);
                if (map[x, y, 0] < 0.5f)
                    color = Color.blue;

                textureFace.SetPixel(x, y, color);
            }
        }

        //left
        for (int z = 0; z < map.GetLength(0); z++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                float perlin = map[0, y, 15 - z];
                Color color = new Color(perlin, perlin, perlin);
                if (perlin < 0.5f)
                    color = Color.blue;

                textureLeft.SetPixel(z, y, color);
            }
        }
        //right
        for (int z = 0; z < map.GetLength(0); z++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                float perlin = map[15, y, z];
                Color color = new Color(perlin, perlin, perlin);
                if (perlin < 0.5f)
                    color = Color.blue;

                textureRight.SetPixel(z, y, color);
            }
        }
        //back
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                float perlin = map[15 -x, y, 15];
                Color color = new Color(perlin, perlin, perlin);
                if (perlin < 0.5f)
                    color = Color.blue;

                textureBack.SetPixel(x, y, color);
            }
        }
        //up
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                float perlin = map[x, 15, z];
                Color color = new Color(perlin, perlin, perlin);
                if (perlin < 0.5f)
                    color = Color.blue;

                textureUp.SetPixel(x, z, color);
            }
        }
        //down
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                float perlin = map[x, 0, 15 - z];
                Color color = new Color(perlin, perlin, perlin);
                if (perlin < 0.5f)
                    color = Color.blue;

                textureDown.SetPixel(x, z, color);
            }
        }

        textureFace.Apply(false);
        textureLeft.Apply(false);
        textureRight.Apply(false);
        textureBack.Apply(false);
        textureUp.Apply(false);
        textureDown.Apply(false);

        //«амен€ем старую текстуру новой
        imageFace.texture = textureFace;
        imageLeft.texture = textureLeft;
        imageRight.texture = textureRight;
        imageBack.texture = textureBack;
        imageUp.texture = textureUp;
        imageDown.texture = textureDown;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateImage();

        OffSetX = Mathf.Sin(Time.time/6);
        OffSetY = Mathf.Cos(Time.time/6);
        OffSetZ = Mathf.Cos(Time.time*2)/8;
    }
}
