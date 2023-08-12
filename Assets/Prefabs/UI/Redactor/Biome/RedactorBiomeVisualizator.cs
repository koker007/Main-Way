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

    [SerializeField]
    TranslaterComp PlanetSize;
    [SerializeField]
    TranslaterComp PlanetSizeResult;
    [SerializeField]
    TranslaterComp CameraPosition;
    [SerializeField]
    TranslaterComp CameraPositionResult;

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

        TestMouseRotate();

        UpdateTextInfo();
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

            Cosmos.HeightMap[,] heightMaps = RedactorBiomeCTRL.main.planetData.GetHeightMaps(quality);

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

    //�������� �������� �����
    void TestMouseRotate() {

        //�������� �����
        bool mouseClick = Input.GetMouseButton(0);

        //���� ���� �� �������� �� ��� ������ ��� ������ ���� �� ������
        if (!mouseClick) {
            //���������� ���������� ����
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //���� ���� ������ � �������� �� ������������
        if (isMouseOver && mouseClick) {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            //��������� �������� �� ������
            float mouseX = Input.GetAxis("Mouse X") * 5;
            float mouseY = Input.GetAxis("Mouse Y") * 5;

            RedactorBiomeGenerator.SetRotate(new Vector2(mouseX, -mouseY));
        }
    }

    void UpdateTextInfo() {
        Cosmos.PlanetData planetData = RedactorBiomeCTRL.main.planetData;
        if (planetData == null)
            return;

        PlanetSizeResult.SetText("" + Calc.GetSizeInt(planetData.size));

        CameraPositionResult.SetText("" + RedactorBiomeGenerator.cameraPositionNeed);
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
