using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;


//������������� ����������� ������ 
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


        //���������� �� �������
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

        //����������
        renderMain.material.color = data.color;

        //���������� ���������
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
        //������� ��������� ����� ������� � ���� ��������
        dist = Vector3.Distance(CameraSpaceCell.main.transform.position, gameObject.transform.position);
        //��� ��������� ���� �������� � ������� ������
        dist /= CellS.sizeVisual;

        //���� ��������� ��������� ������ ������ �� ���� ������� ���� ������
        if (dist > Settings.main.distance*1.5f) {
            //��������� ������
            Deactivate();
            //Destroy(gameObject);
        }
    }

    void updateTransform()
    {

        lookAtCamera();

        updatePosition();

        //�������� �� ������
        void lookAtCamera() {
            //Vector3 vecToCam = rotateObj.transform.position - CameraSpaceCell.main.transform.position;
            //vecToCam.Normalize();

            //������� �� ������
            rotateObj.LookAt(CameraSpaceCell.main.transform);
            //rotateObj.eulerAngles = new Vector3(rotateObj.eulerAngles.x, rotateObj.eulerAngles.y, 0f);

            int sizeData = Calc.GetSizeInt(data.size);
            float coof = (int)data.size;

            float sizeNow = 1;

                //������� ������ ������
            sizeNow = (dist * 0.3333f * CellS.size) / (100 - coof);

            //����������� ������, �� ���� �����������
            float sizeMin = sizeData;
            if (sizeNow < sizeMin)
                sizeNow = sizeMin;

            sizeNow /= CellS.sizeVisual;

            rotateObj.localScale = new Vector3(sizeNow, sizeNow, sizeNow);
        }
        void updatePosition() {
            //���� �������� ���, �� ����������� �� ���������
            if (data.parent == null) {
                return;
            }

            //���������� ����� ������� �� ������������ ��������
            transform.position = data.parent.visual.rotateObj.position;

            //������ ��������� �� ������� ���� �����������
            rotateObj.localPosition = new Vector3(data.radiusOrbit/(float)CellS.sizeVisual,0,0);
            Quaternion rot = transform.rotation;
            rot.eulerAngles = new Vector3(0, 100000/(float)data.radiusOrbit * (Time.time + 10000), 0);
            transform.rotation = rot;
        }


    }

    //���������� �������
    void updateVisual()
    {
        if (data.parent == null)
            terminator.gameObject.SetActive(false);
        else terminator.gameObject.SetActive(true);
    }

    public void Deactivate() {

        //��������� �� ������� �����
        if (data != null && data.childs != null)
            foreach (ObjData spaceObjData in data.childs)
            {
                if (spaceObjData.visual == null)
                    continue;

                //����� ���� ���������
                spaceObjData.visual.Deactivate();
            }

        data.visual = null;
        data = null;

        transform.parent = SpaceCellsCtrl.main.transform;

        gameObject.SetActive(false);
    }

}
