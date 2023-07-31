using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RedactorBiomeVisualizator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static private RedactorBiomeVisualizator main;

    public bool isMouseOver = false;

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
        //Включить генератор если выключен
        RedactorBiomeGenerator.TestOpen();
        TestRenderTexture();

        SetPlanetHeightMap();

        TestMouseRotate();
    }

    void TestRenderTexture() {
        renderTexture.texture = RedactorBiomeGenerator.GetRender();
    }

    void SetPlanetHeightMap() {
        
        if (RedactorBiomeCTRL.main.planetData == null)
            return;

        Size quality = Calc.GetSize(Calc.GetSizeInt(RedactorBiomeCTRL.main.planetData.size) / Chank.Size);

        //Генерируем глобальную карту высот если ее нет
        if (heightMapAll == null)
        {
            Debug.Log(RedactorBiomeCTRL.main.planetData.size);

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

        RedactorBiomeGenerator.SetHeightMap(heightMapAll, RedactorBiomeCTRL.main.planetData);
    }

    //Проверка вращения мыщью
    void TestMouseRotate() {

        //проверка клика
        bool mouseClick = Input.GetMouseButton(0);

        //Если мышь не наведена на эту панель или кнопка мыши не нажата
        if (!isMouseOver || !mouseClick) {
            //Возвращаем управление мыши
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //Если мышь нажата и наведена на визуализатор
        if (isMouseOver && mouseClick) {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //Проверяем смещение из центра
            int mouseX = (int)Input.mousePosition.x + 1;
            int mouseY = (int)Input.mousePosition.y - 2;

            int screenCenterX = (int)(Screen.width / 2f);
            int screenCenterY = (int)(Screen.height / 2f);

            float deltaX = mouseX - screenCenterX;
            float deltaY = mouseY - screenCenterY;

            if (Mathf.Abs(deltaX) < 1)
                deltaX = 0;
            if (Mathf.Abs(deltaY) < 1)
                deltaY = 0;

            RedactorBiomeGenerator.SetRotate(new Vector2(deltaX, -deltaY));

            Debug.Log("mouseX " + mouseX + " mouseY " + mouseY + "screenCenterX " + screenCenterX + " screenCenterY " + screenCenterY);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}
