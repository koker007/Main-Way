using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;


//Визуализирует космический объект 
public class SpaceObjCtrl : MonoBehaviour
{
    [SerializeField]
    public ObjData data;

    [SerializeField]
    public Transform rotateObj;

    [SerializeField]
    MeshRenderer renderMain;

    [SerializeField]
    MeshRenderer renderAtmosUp;
    [SerializeField]
    MeshRenderer renderAtmosDown;
    [SerializeField]
    MeshRenderer renderAtmosLeft;
    [SerializeField]
    MeshRenderer renderAtmosRight;

    [SerializeField]
    TerminatorCTRL terminator;
    [SerializeField]
    ShadowCTRL shadowCTRL;

    float timeTestOld = 0;
    float dist = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Ini(ObjData data)
    {
        this.data = data;
        data.visual = this;

        rotateObj.localPosition = new Vector3(0, 0, 0);


        //Перемещаем на позицию
        gameObject.transform.localPosition = data.cell.pos * (CellS.size / CellS.sizeVisual);

        if (data.parent != null)
        {
            gameObject.name = "PlanetObj";
            gameObject.transform.parent = data.parent.visual.transform;
        }
        else
        {
            gameObject.name = "SpaceObj: " + data.cell.pos;
        }

        //Окрашиваем
        renderMain.material.color = data.color;

        //Окрашиваем атмосферу
        renderAtmosUp.gameObject.SetActive(true);
        renderAtmosDown.gameObject.SetActive(true);
        renderAtmosLeft.gameObject.SetActive(true);
        renderAtmosRight.gameObject.SetActive(true);

        renderAtmosUp.material.color = data.color;
        renderAtmosDown.material.color = data.color;
        renderAtmosLeft.material.color = data.color;
        renderAtmosRight.material.color = data.color;

        terminator.SetAtmosphereFull(renderAtmosLeft, renderAtmosRight, renderAtmosDown, renderAtmosUp);
    }

    // Update is called once per frame
    void Update()
    {

        Test();

        if (data == null)
            return;

        updateTransform();
        updateVisual();
    }

    void Test() {
        if (timeTestOld == Time.unscaledTime) {
            return;
        }

        timeTestOld = Time.unscaledTime;
        //Находим растояние между камерой и этим обьектом
        dist = Vector3.Distance(CameraSpaceCell.main.transform.position, gameObject.transform.position);
        //это растояние надо привести к размеру ячейки
        dist /= CellS.sizeVisual;

        //Если растояние оказалось больше порога то надо удалить этот объект
        if (dist > Settings.main.distance*1.5f) {
            //Выключаем обьект
            Deactivate();
            //Destroy(gameObject);
        }
    }

    void updateTransform()
    {

        lookAtCamera();

        updatePosition();

        //Смотреть на камеру
        void lookAtCamera() {
            //Vector3 vecToCam = rotateObj.transform.position - CameraSpaceCell.main.transform.position;
            //vecToCam.Normalize();

            //Смотрим на камеру
            rotateObj.LookAt(CameraSpaceCell.main.transform);
            //rotateObj.eulerAngles = new Vector3(rotateObj.eulerAngles.x, rotateObj.eulerAngles.y, 0f);

            int sizeData = Calc.GetSizeInt(data.size);
            float coof = (int)data.size;

            float sizeNow = 1;

                //Текущий размер ячейки
            sizeNow = (dist * 0.3333f * CellS.size) / (100 - coof);

            //минимальные размер, то есть фактический
            float sizeMin = sizeData;
            if (sizeNow < sizeMin)
                sizeNow = sizeMin;

            sizeNow /= CellS.sizeVisual;

            rotateObj.localScale = new Vector3(sizeNow, sizeNow, sizeNow);
        }
        void updatePosition() {
            //Если родителя нет, то перемещение не требуется
            if (data.parent == null) {
                return;
            }

            //Перемещаем центр объекта на визуализатор родителя
            transform.position = data.parent.visual.rotateObj.position;

            //Узнаем растояние на которое надо передвинуть
            rotateObj.localPosition = new Vector3(data.radiusOrbit/(float)CellS.sizeVisual,0,0);
            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, 100000/(float)data.radiusOrbit * (Time.time + 10000), 0);
            transform.rotation = rot;
        }


    }

    //Визуальные эффекты
    void updateVisual()
    {
        if (data.parent == null)
            terminator.gameObject.SetActive(false);
        else terminator.gameObject.SetActive(true);
    }

    public void Deactivate() {

        //Проверяем на наличие детей
        if (data != null && data.childs != null)
            foreach (ObjData spaceObjData in data.childs)
            {
                if (spaceObjData.visual == null)
                    continue;

                //Детей тоже отключаем
                spaceObjData.visual.Deactivate();
            }

        data.visual = null;
        data = null;

        transform.parent = SpaceCellsCtrl.main.transform;

        gameObject.SetActive(false);
    }

}
