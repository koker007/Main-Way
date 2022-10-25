using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�������� �� ���������� ����� ���������� ������� � ���������
public class RedactorBlocksVoxColor : MonoBehaviour
{
    static public RedactorBlocksVoxColor main;

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

    BlockWall wall = null;
    Vector2Int voxelPos = new Vector2Int();

    static public Color getColor()
    {
        //������ ���� �������
        return main.ImageCursor.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReDrawPalette();
    }

    void TestVoxSelect() {

        //���� ����� ���� ����� � �������
        //������ �� ���������� �������
        if (wall == RedactorBlocksColiders.main.selectBlockWall && voxelPos == RedactorBlocksColiders.main.VoxelPos)
            return;

        //���� ���-�� ������� ����������
        wall = RedactorBlocksColiders.main.selectBlockWall;
        voxelPos = RedactorBlocksColiders.main.VoxelPos;

        //������� ���� ����� ���
        if (wall == null)
            return;

        //��������� ������ ���������� ������� �� �������
        setColor(wall.texture.GetPixel(voxelPos.x, voxelPos.y));
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
    }

    //�������� ����� �� 0 �� 1 �� ��������
    float GetFloatFromSlider(SliderCTRL slider) {

        float size = slider.slider.maxValue - slider.slider.minValue;
        float maxValue = 0 + size;
        float nowValue = 0 + slider.slider.value;

        return (nowValue / maxValue);
    }

    void ReDrawPalette() {

        //������������ ������� ������

        //�� �������� � ������-��������
        //�� ������-�������� � ��������
        //�� �������� � ������-������
        //�� ������-������ � ������
        //�� ������ � ������-������

        int sizeX = 100;
        int sizeY = 50;

        Texture2D texture2D = new Texture2D(sizeX, sizeY);

        //����������� �������� �������� �������
        for (int pixX = 0; pixX < sizeX; pixX++) {
            for (int pixY = 0; pixY < sizeY; pixY++) {
                texture2D.SetPixel(pixX, pixY, getColor((float)pixX/sizeX, (float)pixY / sizeY));
            }
        }

        texture2D.Apply();

        ImagePallite.texture = texture2D;

    }
    void reDrawCursor() {
        if (ImageCursor.texture == null && ImagePallite.texture != null) {
            Texture2D texture = new Texture2D(ImagePallite.texture.width, ImagePallite.texture.height);
            Texture2D textureObvodka = new Texture2D(ImagePallite.texture.width, ImagePallite.texture.height);

            //������ ������ ��� �������� ����������
            for (int x = 0; x < texture.width; x++) {
                for (int y = 0; y < texture.height; y++) {
                    texture.SetPixel(x, y, Color.clear);
                    textureObvodka.SetPixel(x, y, Color.clear);
                }
            }

            //������ ����� �� ������ ������� ������
            int cursorSizePix = (int)(texture.width * 0.1f);
            int centerX = texture.width / 2;
            int centerY = texture.height / 2;
            for (int x = -cursorSizePix/2; x <= cursorSizePix/2; x++) {
                for (int y = -cursorSizePix/2; y <= cursorSizePix/2; y++) {
                    //�� ������ �����
                    if (Mathf.Abs(x) < cursorSizePix / 3f && Mathf.Abs(y) < cursorSizePix / 3f ||
                        Mathf.Abs(x) == Mathf.Abs(y))
                    {
                        texture.SetPixel(centerX + x, centerY + y, new Color(1, 1, 1));
                    }
                    else {
                        texture.SetPixel(centerX + x, centerY + y, new Color(0, 0, 0));
                    }
                    
                    //������� �����
                    if(Mathf.Abs(x) == cursorSizePix / 2 || Mathf.Abs(y) == cursorSizePix / 2)
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

        //������ ���� �������
        float colorPosX = GetFloatFromSlider(sliderColorH);
        float colorPosY = GetFloatFromSlider(sliderColorS);
        ImageCursor.color = getColor(colorPosX, colorPosY);

        ImageCursor.uvRect = new Rect(new Vector2(0.5f - colorPosX, 0.5f - colorPosY), new Vector2(1,1));
        ImageCursorObvodka.uvRect = new Rect(new Vector2(0.5f - colorPosX, 0.5f - colorPosY), new Vector2(1, 1));
        ImagePalliteVelosity.color = new Color(0, 0, 0, 1 - GetFloatFromSlider(sliderColorV));
    }

    Color getColor(float valueX, float valueY)
    {

        float partSize = 1 / 6f;

        Color result;

        /////////////////////////////////////////////
        ///�������
        float value = valueX / partSize;
        //�� �������� � ������-�������� 100 -> 110
        if (value < 1)
        {
            result = new Color(1, value, 0);
        }
        //�� ������-�������� � �������� 110 -> 010
        else if (value < 2)
        {
            value = 1 - (value - 1);
            result = new Color(value, 1, 0);
        }
        //�� �������� � ������-������ 010 -> 011
        else if (value < 3)
        {
            value = (value - 2);
            result = new Color(0, 1, value);
        }
        //�� ������-������ � ������ 011 -> 001
        else if (valueX / partSize < 4)
        {
            value = 1 - (value - 3);
            result = new Color(0, value, 1);
        }
        //�� ������ � ������-������ 001 -> 101
        else if (valueX / partSize < 5)
        {
            value = value - 4;
            result = new Color(value, 0, 1);
        }
        //�� ������-������� � �������� 101 -> 100
        else
        {
            value = 1.0f - (value - 5);
            result = new Color(1, 0, value);
        }

        //Debug.Log("X: " + valueX + "val: " + value);

        ///////////////////////////////////////////
        ///������������
        result = Color.Lerp(new Color(1, 1, 1), result, valueY);

        return result;
    }

    //���������� ���� ��� �������
    public void setColor(Color color) {
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
        //�������� ���� �� �������
        reDrawCursor();
        //��������� ���������
        BlockWall wall = RedactorBlocksColiders.main.selectBlockWall;

        //���� ����� ���� - ���� ����� ���� ���������
        if (wall == null)
            return;

        Color color = ImageCursor.color;

        color = Color.HSVToRGB(GetFloatFromSlider(sliderColorH), GetFloatFromSlider(sliderColorS), GetFloatFromSlider(sliderColorV));

        Vector3 HSVrand = new Vector3();
        HSVrand.x = sliderColorHRand.slider.value / sliderColorHRand.slider.maxValue;
        HSVrand.y = sliderColorSRand.slider.value / sliderColorSRand.slider.maxValue;
        HSVrand.z = sliderColorVRand.slider.value / sliderColorVRand.slider.maxValue;

        //��������� ��������� �� ��������� �������
        RedactorBlocksVoxel.main.acceptVoxColorArray(wall, RedactorBlocksColiders.main.VoxelPos, color, HSVrand);
    }


}
