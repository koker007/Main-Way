using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorBlocksFormTLiquidColor : MonoBehaviour
{
    static public RedactorBlocksFormTLiquidColor main;

    [SerializeField]
    RawImage ImagePallite;
    [SerializeField]
    RawImage ImagePalliteVelosity;
    [SerializeField]
    RawImage ImageCursor;
    [SerializeField]
    RawImage ImageCursorObvodka;

    [SerializeField]
    SliderCTRL sliderColorH;
    [SerializeField]
    SliderCTRL sliderColorS;
    [SerializeField]
    SliderCTRL sliderColorV;

    [SerializeField]
    SliderCTRL sliderColorHRand;
    [SerializeField]
    SliderCTRL sliderColorSRand;
    [SerializeField]
    SliderCTRL sliderColorVRand;

    [SerializeField]
    SliderCTRL sliderPerlinScale;
    [SerializeField]
    SliderCTRL sliderPerlinOctaves;
    [SerializeField]
    SliderCTRL sliderPerlinScaleX;
    [SerializeField]
    SliderCTRL sliderPerlinScaleY;
    [SerializeField]
    SliderCTRL sliderPerlinScaleZ;
    [SerializeField]
    SliderCTRL sliderAnimationLenght;
    [SerializeField]
    SliderCTRL sliderAnimationSpeedAll;
    [SerializeField]
    SliderCTRL sliderAnimationSpeedX;
    [SerializeField]
    SliderCTRL sliderAnimationSpeedY;
    [SerializeField]
    SliderCTRL sliderAnimationSpeedZ;
    [SerializeField]
    SliderCTRL sliderAnimationSizeX;
    [SerializeField]
    SliderCTRL sliderAnimationSizeY;
    [SerializeField]
    SliderCTRL sliderAnimationSizeZ;

    static public Color getColor()
    {
        //Меняем цвет курсора
        return main.ImageCursor.color;
    }

    void IniSliders() {
        sliderPerlinScale.slider.minValue = 0.1f;
        sliderPerlinScale.slider.maxValue = 50f;
        sliderPerlinScale.slider.value = 16f;
        sliderPerlinOctaves.slider.minValue = 1;
        sliderPerlinOctaves.slider.maxValue = 5;
        sliderPerlinOctaves.slider.value = 2;
        sliderPerlinScaleX.slider.minValue = 0;
        sliderPerlinScaleY.slider.minValue = 0;
        sliderPerlinScaleZ.slider.minValue = 0;
        sliderPerlinScaleX.slider.maxValue = 10;
        sliderPerlinScaleY.slider.maxValue = 10;
        sliderPerlinScaleZ.slider.maxValue = 10;
        sliderPerlinScaleX.slider.value = 1;
        sliderPerlinScaleY.slider.value = 1;
        sliderPerlinScaleZ.slider.value = 1;
        sliderAnimationLenght.slider.minValue = 1;
        sliderAnimationLenght.slider.maxValue = 64;
        sliderAnimationLenght.slider.value = 64;
        sliderAnimationSpeedAll.slider.minValue = 0.1f;
        sliderAnimationSpeedAll.slider.maxValue = 16;
        sliderAnimationSpeedAll.slider.value = 16;
        sliderAnimationSpeedX.slider.minValue = 0.1f;
        sliderAnimationSpeedY.slider.minValue = 0.1f;
        sliderAnimationSpeedZ.slider.minValue = 0.1f;
        sliderAnimationSpeedX.slider.maxValue = 16;
        sliderAnimationSpeedY.slider.maxValue = 16;
        sliderAnimationSpeedZ.slider.maxValue = 16;
        sliderAnimationSpeedX.slider.value = 0.166f;
        sliderAnimationSpeedY.slider.value = 0.166f;
        sliderAnimationSpeedZ.slider.value = 2;
        sliderAnimationSizeX.slider.minValue = 1;
        sliderAnimationSizeY.slider.minValue = 1;
        sliderAnimationSizeZ.slider.minValue = 1;
        sliderAnimationSizeX.slider.maxValue = 8;
        sliderAnimationSizeY.slider.maxValue = 8;
        sliderAnimationSizeZ.slider.maxValue = 8;
        sliderAnimationSizeX.slider.value = 1;
        sliderAnimationSizeY.slider.value = 1;
        sliderAnimationSizeZ.slider.value = 8;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReDrawPalette();
        IniSliders();
    }

    // Update is called once per frame
    void Update()
    {
        main = this;
        //TestVoxSelect();
        reDrawCursor();

        sliderColorH.SetValueText();
        sliderColorS.SetValueText();
        sliderColorV.SetValueText();

        sliderColorHRand.SetValueText();
        sliderColorSRand.SetValueText();
        sliderColorVRand.SetValueText();

        sliderPerlinScale.SetValueText();
        sliderPerlinOctaves.SetValueText();
        sliderPerlinScaleX.SetValueText();
        sliderPerlinScaleY.SetValueText();
        sliderPerlinScaleZ.SetValueText();

        sliderAnimationLenght.SetValueText();
        sliderAnimationSpeedAll.SetValueText();
        sliderAnimationSpeedX.SetValueText();
        sliderAnimationSpeedY.SetValueText();
        sliderAnimationSpeedZ.SetValueText();
        sliderAnimationSizeX.SetValueText();
        sliderAnimationSizeY.SetValueText();
        sliderAnimationSizeZ.SetValueText();
    }

    //Получить дробь от 0 до 1 на слайдере
    float GetFloatFromSlider(SliderCTRL slider)
    {

        float size = slider.slider.maxValue - slider.slider.minValue;
        float maxValue = 0 + size;
        float nowValue = 0 + slider.slider.value;

        return (nowValue / maxValue);
    }

    void ReDrawPalette()
    {

        //Определяемся сколько цветов

        //От красного к красно-зеленому
        //От красно-зеленого к зеленому
        //От зеленого к зелено-синему
        //От зелено-синего к синему
        //От синего к красно-синему

        int sizeX = 100;
        int sizeY = 50;

        Texture2D texture2D = new Texture2D(sizeX, sizeY);

        //Настраиваем текстуру цветовой палитры
        for (int pixX = 0; pixX < sizeX; pixX++)
        {
            for (int pixY = 0; pixY < sizeY; pixY++)
            {
                texture2D.SetPixel(pixX, pixY, getColor((float)pixX / sizeX, (float)pixY / sizeY));
            }
        }

        texture2D.Apply();

        ImagePallite.texture = texture2D;

    }
    void reDrawCursor()
    {
        if (ImageCursor.texture == null && ImagePallite.texture != null)
        {
            Texture2D texture = new Texture2D(ImagePallite.texture.width, ImagePallite.texture.height);
            Texture2D textureObvodka = new Texture2D(ImagePallite.texture.width, ImagePallite.texture.height);

            //Сперва делаем всю текстуру прозрачной
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, Color.clear);
                    textureObvodka.SetPixel(x, y, Color.clear);
                }
            }

            //Теперь ровно по центру создаем курсор
            int cursorSizePix = (int)(texture.width * 0.1f);
            int centerX = texture.width / 2;
            int centerY = texture.height / 2;
            for (int x = -cursorSizePix / 2; x <= cursorSizePix / 2; x++)
            {
                for (int y = -cursorSizePix / 2; y <= cursorSizePix / 2; y++)
                {
                    //По центру белое
                    if (Mathf.Abs(x) < cursorSizePix / 3f && Mathf.Abs(y) < cursorSizePix / 3f ||
                        Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        texture.SetPixel(centerX + x, centerY + y, new Color(1, 1, 1));
                    }
                    else
                    {
                        texture.SetPixel(centerX + x, centerY + y, new Color(0, 0, 0));
                    }

                    //Обводка белая
                    if (Mathf.Abs(x) == cursorSizePix / 2 || Mathf.Abs(y) == cursorSizePix / 2)
                    {
                        textureObvodka.SetPixel(centerX + x, centerY + y, new Color(1, 1, 1));
                    }
                }
            }

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            textureObvodka.filterMode = FilterMode.Point;
            textureObvodka.wrapMode = TextureWrapMode.Clamp;
            textureObvodka.Apply();

            ImageCursor.texture = texture;
            ImageCursorObvodka.texture = textureObvodka;
        }

        //Меняем цвет курсора
        float colorPosX = GetFloatFromSlider(sliderColorH);
        float colorPosY = GetFloatFromSlider(sliderColorS);
        ImageCursor.color = getColor(colorPosX, colorPosY);

        ImageCursor.uvRect = new Rect(new Vector2(0.5f - colorPosX, 0.5f - colorPosY), new Vector2(1, 1));
        ImageCursorObvodka.uvRect = new Rect(new Vector2(0.5f - colorPosX, 0.5f - colorPosY), new Vector2(1, 1));
        ImagePalliteVelosity.color = new Color(0, 0, 0, 1 - GetFloatFromSlider(sliderColorV));
    }

    Color getColor(float valueX, float valueY)
    {

        float partSize = 1 / 6f;

        Color result;

        /////////////////////////////////////////////
        ///Оттенок
        float value = valueX / partSize;
        //От красного к красно-зеленому 100 -> 110
        if (value < 1)
        {
            result = new Color(1, value, 0);
        }
        //От красно-зеленого к зеленому 110 -> 010
        else if (value < 2)
        {
            value = 1 - (value - 1);
            result = new Color(value, 1, 0);
        }
        //От зеленого к зелено-синему 010 -> 011
        else if (value < 3)
        {
            value = (value - 2);
            result = new Color(0, 1, value);
        }
        //От зелено-синего к синему 011 -> 001
        else if (valueX / partSize < 4)
        {
            value = 1 - (value - 3);
            result = new Color(0, value, 1);
        }
        //От синего к красно-синему 001 -> 101
        else if (valueX / partSize < 5)
        {
            value = value - 4;
            result = new Color(value, 0, 1);
        }
        //От красно-синемго к красному 101 -> 100
        else
        {
            value = 1.0f - (value - 5);
            result = new Color(1, 0, value);
        }

        //Debug.Log("X: " + valueX + "val: " + value);

        ///////////////////////////////////////////
        ///Насыщенность
        result = Color.Lerp(new Color(1, 1, 1), result, valueY);

        return result;
    }

    //Установить цвет для палитры
    public void setColor(Color color)
    {
        //https://russianblogs.com/article/69531462555/

        float H = 0;
        float S = 0;
        float V = 0;

        Color.RGBToHSV(color, out H, out S, out V);

        sliderColorH.slider.value = sliderColorH.slider.minValue + H * (sliderColorH.slider.maxValue - sliderColorH.slider.minValue);
        sliderColorS.slider.value = sliderColorS.slider.minValue + S * (sliderColorS.slider.maxValue - sliderColorS.slider.minValue);
        sliderColorV.slider.value = sliderColorV.slider.minValue + V * (sliderColorV.slider.maxValue - sliderColorV.slider.minValue);



        Debug.Log("H" + H + " V" + V + " S" + S);
    }

    public void acceptSliderColor()
    {
        //Изменяем цвет на палитре
        reDrawCursor();

        Color colorStart = ImageCursor.color;

        colorStart = Color.HSVToRGB(GetFloatFromSlider(sliderColorH), GetFloatFromSlider(sliderColorS), GetFloatFromSlider(sliderColorV));

        Vector3 HSVEnd = new Vector3();
        HSVEnd.x = sliderColorHRand.slider.value / sliderColorHRand.slider.maxValue;
        HSVEnd.y = sliderColorSRand.slider.value / sliderColorSRand.slider.maxValue;
        HSVEnd.z = sliderColorVRand.slider.value / sliderColorVRand.slider.maxValue;
        Color colorEnd = Color.HSVToRGB(HSVEnd.x, HSVEnd.y, HSVEnd.z);

        int perlinOctaves = (int)sliderPerlinOctaves.slider.value;
        Vector3 scaleXYZ = new Vector3(sliderPerlinScaleX.slider.value, sliderPerlinScaleY.slider.value, sliderPerlinScaleZ.slider.value);
        float scaleAll = sliderPerlinScale.slider.value;
        int animLenght = (int)sliderAnimationLenght.slider.value;
        Vector3 animSpeed = new Vector3(sliderAnimationSpeedX.slider.value, sliderAnimationSpeedY.slider.value, sliderAnimationSpeedZ.slider.value);
        float animSpeedAll = sliderAnimationSpeedAll.slider.value;
        Vector3 animSizeXYZ = new Vector3(sliderAnimationSizeX.slider.value, sliderAnimationSizeY.slider.value, sliderAnimationSizeZ.slider.value);

        //Применяем изменения
        RedactorBlocksFormTLiquid.main.acceptLiquidColor(colorStart, colorEnd, perlinOctaves, scaleXYZ, scaleAll, animLenght, animSpeed, animSpeedAll, animSizeXYZ);
    }

}
