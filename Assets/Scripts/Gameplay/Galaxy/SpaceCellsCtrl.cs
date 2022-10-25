using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� �� �������� ����� ������ ����������� ������
public class SpaceCellsCtrl : MonoBehaviour
{
    public static SpaceCellsCtrl main;

    [SerializeField]
    SpaceObjCtrl SpaceObjPrefab;

    //������ ��������� ��������
    List<SpaceObjCtrl> spaceObjList = new List<SpaceObjCtrl>();

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }
    // Update is called once per frame
    void Update()
    {
        testVisualize();
        UpdateTransformPlayer();
    }

    public void testVisualize() {
        //���������� ������ ������ ������� � �������
        //�������� ������� ������ � �������

        //������������ ������ ������������ ���������� ������
        Vector3 camPos;
        //���� ���� ����� �����, ��� �������
        if (PlayerCTRL.local) {
            camPos =  PlayerCTRL.local.NumCell + (PlayerCTRL.local.PosInCell / CellS.size);
        }
        //���� ������ ��� ����� ������� �� ��������� ����������� ������
        else
        {
            camPos = SpaceCameraControl.main.spacePosCell;
        }

        int xMax = GalaxyCtrl.galaxy.cells.GetLength(0);
        int yMax = GalaxyCtrl.galaxy.cells.GetLength(1);
        int zMax = GalaxyCtrl.galaxy.cells.GetLength(2);

        //��������� ������ � ������� 3-� ������
        int radius = Settings.main.distance;
        for (int x = -radius; x < radius; x++){
            int xfact = (int)camPos.x + x;
            if (xfact < 0 || xfact >= xMax)
                continue;

            for (int y = -radius; y < radius; y++){
                int yfact = (int)camPos.y + y;
                if (yfact < 0 || yfact >= yMax)
                    continue;

                for (int z = -radius; z < radius; z++){
                    int zfact = (int)camPos.z + z;
                    if (zfact < 0 || zfact >= zMax)
                        continue;

                    //������������� ������� ������ � �� ������� � ����
                    if(GalaxyCtrl.galaxy.cells[xfact, yfact, zfact].visual != null)
                        Visualize(GalaxyCtrl.galaxy.cells[xfact, yfact, zfact].mainObjs, camPos);
                }
            }
        }

        void Visualize(SpaceObjData spaceObj, Vector3 camPos) {
            //������� ���� ������ ���������������
            if (spaceObj == null) 
                return;

            //���� ������������ ��� - �������
            if (spaceObj.visual == null)
            {
                if (canVisualize()) {

                    SpaceObjCtrl spaceObjVisual = GetSpaceObjCTRL();
                    spaceObjVisual.Ini(spaceObj);
                }
            }
            else {
                //��������������� ������ ������� ������
                if (!canVisualize()) {
                    spaceObj.visual.Deactivate();
                }
            }

            //���� ��� ���
            if (spaceObj.childs == null)
                return;

            //���������� ���� �� ������������
            for (int i = 0; i < spaceObj.childs.Length; i++)
            {
                Visualize(spaceObj.childs[i], camPos);
            }

            //����� �� ��������������� ���� ������
            bool canVisualize() {
                bool result = false;

                //���� ����� ��������� ������ ������ �� ������������� ���
                if ((int)camPos.x == (int)spaceObj.cell.pos.x && (int)camPos.y == (int)spaceObj.cell.pos.y && (int)camPos.z == (int)spaceObj.cell.pos.z)
                    return true;

                //������� ��������� �� ������ ������ �� ������
                float dist = Vector3.Distance(spaceObj.cell.pos, camPos);

                //���� ��������� ������ ��� ������ ������
                if (dist < Calc.GetSizeInt(spaceObj.size)/1000f) {
                    return true;
                }


                return result;
            }
        }
    }

    void UpdateTransformPlayer()
    {
        if (!PlayerCTRL.local)
            return;

        //��������� � ���������
        Vector3 posInCell = (PlayerCTRL.local.PosInCell / CellS.size);

        gameObject.transform.localPosition = -(PlayerCTRL.local.NumCell + posInCell) * CellS.sizeVisual;

        //��������� ������ �� ����������� �������� ������
        //gameObject.transform.rotation = PlayerCTRL.local.GetSpaceView();
    }

    //�������� ��������� ����������� ������ ��� ������� �����
    SpaceObjCtrl GetSpaceObjCTRL() {
        //���� ���������
        foreach (SpaceObjCtrl spaceObj in spaceObjList) {
            //����������, ���� ���� ������ ������
            if (spaceObj == null)
                continue;

            //�������� ������ ���� �� �� �������
            if (!spaceObj.gameObject.activeSelf) {
                spaceObj.gameObject.SetActive(true);
                return spaceObj;
            }
        }

        //���� �� ��� ������ ������ �� ��� ������
        //������� �����
        GameObject spaceObjNew = Instantiate(SpaceObjPrefab.gameObject, transform);
        SpaceObjCtrl result = spaceObjNew.GetComponent<SpaceObjCtrl>();
        spaceObjList.Add(spaceObjNew.GetComponent<SpaceObjCtrl>());

        return result;
    }
}
