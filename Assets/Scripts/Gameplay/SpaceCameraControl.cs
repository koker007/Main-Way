using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCameraControl : MonoBehaviour
{
    public static SpaceCameraControl main;

    [SerializeField]
    float mouseSpeed = 1000;

    Vector3 SpacePos = new Vector3(15.0f, 15.0f, 15.0f);
    Quaternion SpaceRot = new Quaternion();

    //Получить текущую позицию камеры в размерах где целое - ячейка
    public Vector3 spacePosCell {
        get {
            return new Vector3(
                SpacePos.x/CellS.size, SpacePos.y/CellS.size, SpacePos.z/CellS.size);
        }
    }
    public Vector3 spacePos
    {
        get
        {
            return SpacePos;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestKey();
        TestMouse();

        UpdateCameras();
    }

    [SerializeField]
    float speed = 1000000f;

    void TestKey() {
        if (Input.GetKey(KeyCode.W)) {
            SpacePos += CameraGalaxy.main.transform.forward * Time.unscaledDeltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S)) {
            SpacePos -= CameraGalaxy.main.transform.forward * Time.unscaledDeltaTime * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            SpacePos -= CameraGalaxy.main.transform.right * Time.unscaledDeltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            SpacePos += CameraGalaxy.main.transform.right * Time.unscaledDeltaTime * speed;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            SpacePos -= CameraGalaxy.main.transform.up * Time.unscaledDeltaTime * speed;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            SpacePos += CameraGalaxy.main.transform.up * Time.unscaledDeltaTime * speed;
        }
    }

    void TestMouse() {
        //Вращяем эти камеры только если нет игрока
        if (PlayerCTRL.local)
            return;

        //Если не управляем мышью
        if (!Input.GetKey(KeyCode.Mouse1)) {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        //Если в предыдущем кадре не управляли мыщью
        if (Cursor.lockState != CursorLockMode.Locked) {
            //Ставим мышь в центр
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        //Узнаем положение на экране
        Vector2 mouseOffSet = new Vector2();
        mouseOffSet.x = Input.GetAxis("Mouse X");
        mouseOffSet.y = Input.GetAxis("Mouse Y") * -1;
        mouseOffSet = mouseOffSet * Time.unscaledDeltaTime * mouseSpeed;

        Vector3 spaceRotNow = SpaceRot.eulerAngles;
        SpaceRot.eulerAngles = new Vector3(spaceRotNow.x + mouseOffSet.y * Time.unscaledDeltaTime, spaceRotNow.y + mouseOffSet.x * Time.unscaledDeltaTime, spaceRotNow.z);

        //Debug.Log(mouseOffSet + " rot " + SpaceRot.eulerAngles);
    }

    void UpdateCameras()
    {
        //Перемещаем эти камеры только если нет игрока
        if (PlayerCTRL.local)
            return;

        CameraGalaxy.main.transform.localPosition = SpacePos / CellS.size;

        SpaceCellsCtrl.main.transform.position = -SpacePos / CellS.sizeVisual;
        CameraSpaceCell.main.transform.localPosition = SpacePos / CellS.sizeVisual;

        CameraGalaxy.main.transform.rotation = SpaceRot;
        CameraSpaceCell.main.transform.rotation = SpaceRot;
    }
}
