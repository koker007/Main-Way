using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextures : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField]
    Camera cameraGalaxy;
    [SerializeField]
    Camera cameraCell;
    [SerializeField]
    Camera cameraNearest;

    [Header("Renders")]
    [SerializeField]
    RenderTexture renderGalaxy;
    [SerializeField]
    RenderTexture renderCell;
    [SerializeField]
    RenderTexture renderNearest;

    [Header("RawImages")]
    [SerializeField]
    RawImage RawGalaxy;
    [SerializeField]
    RawImage RawCell;
    [SerializeField]
    RawImage RawNearest;


    //Качество отрисовки
    float quality = 1;
    float qualityOld = 0;
    //Размер экрана предыдущего кадра
    Vector2Int screenSizeOld = new Vector2Int();
    bool reDraw = false;



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTestCam();
        UpdateRender();
    }

    void UpdateTestCam() {
        if (cameraGalaxy == null || cameraCell == null)
        {
            cameraGalaxy = CameraGalaxy.main.GetComponent<Camera>();
            cameraCell = CameraSpaceCell.main.GetComponent<Camera>();
            reDraw = true;
        }


        //Камера глаза игрока
        if (cameraNearest == null && CameraNearest.main)
        {
            cameraNearest = CameraNearest.main.GetComponent<Camera>();
            reDraw = true;
        }
    }

    void UpdateRender()
    {

        //Создать рендер двух камер
        //Если размер экрана не изменился и качество осталось прежним выходим
        if (screenSizeOld.x == Screen.width && screenSizeOld.y == Screen.height && quality == qualityOld && reDraw == false)
        {
            return;
        }

        //что-то изменилось
        screenSizeOld.x = Screen.width;
        screenSizeOld.y = Screen.height;
        qualityOld = quality;
        reDraw = false;

        CreateNewTextureGalaxy();
        CreateNewTextureCell();
        CreateNewTextureNearest();

        void CreateNewTextureGalaxy()
        {
            int wight = (int)(Screen.width * quality);
            int height = (int)(Screen.height * quality);

            renderGalaxy = new RenderTexture(wight, height, 1);
            renderGalaxy.filterMode = FilterMode.Point;

            cameraGalaxy.targetTexture = renderGalaxy;
            RawGalaxy.texture = renderGalaxy;
        }
        void CreateNewTextureCell()
        {
            int wight = (int)(Screen.width * quality);
            int height = (int)(Screen.height * quality);

            renderCell = new RenderTexture(wight, height, 1);
            renderCell.filterMode = FilterMode.Point;

            cameraCell.targetTexture = renderCell;
            RawCell.texture = renderCell;
        }

        void CreateNewTextureNearest() {
            if (cameraNearest != null)
            {
                RawNearest.color = new Color(1,1,1,1);

                int wight = (int)(Screen.width * quality);
                int height = (int)(Screen.height * quality);

                renderNearest = new RenderTexture(wight, height, 1);
                renderNearest.filterMode = FilterMode.Point;

                cameraNearest.targetTexture = renderNearest;
                RawNearest.texture = renderNearest;

            }
            else {
                RawNearest.color = new Color(1,1,1,0);
            }
        }
    }
}
