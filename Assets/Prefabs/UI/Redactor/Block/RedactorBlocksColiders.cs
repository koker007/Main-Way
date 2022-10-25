using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� �� �������� ����������� � ��������� �����
public class RedactorBlocksColiders : MonoBehaviour
{
    public static RedactorBlocksColiders main;

    //��� ���������� ����������
    [SerializeField]
    BoxCollider[,] ColidersWallFace;
    [SerializeField]
    BoxCollider[,] ColidersWallBack;
    [SerializeField]
    BoxCollider[,] ColidersWallLeft;
    [SerializeField]
    BoxCollider[,] ColidersWallRight;
    [SerializeField]
    BoxCollider[,] ColidersWallUp;
    [SerializeField]
    BoxCollider[,] ColidersWallDown;

    //��� �������� ��� ���������� ���������� ��������
    [SerializeField]
    GameObject ObjWallFace;
    [SerializeField]
    GameObject ObjWallBack;
    [SerializeField]
    GameObject ObjWallLeft;
    [SerializeField]
    GameObject ObjWallRight;
    [SerializeField]
    GameObject ObjWallUp;
    [SerializeField]
    GameObject ObjWallDown;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestCreateColliders();
    }

    //�������� ����������� �� ������������� � �������� �� ���� �� ���
    void TestCreateColliders() {
        float voxelSize = 0.0625f; //1.0f / 16.0f;

        if (ColidersWallFace == null) {
            //������� �������� ���� ���
            if (ObjWallFace != null)
                Destroy(ObjWallFace);

            //������� ��������
            ObjWallFace = new GameObject("ParentWallFace");
            ObjWallFace.transform.parent = gameObject.transform;
            ObjWallFace.transform.localPosition = new Vector3();
            ObjWallFace.transform.localScale = new Vector3(1,1,1);

            ColidersWallFace = new BoxCollider[16,16];
            for (int x = 0; x < 16; x++) {
                for (int y = 0; y < 16; y++) {
                    GameObject objVoxel = new GameObject("X"+ x + " Y" + y);
                    objVoxel.transform.parent = ObjWallFace.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallFace[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallFace[x, y].center = new Vector3(x * voxelSize + voxelSize/2f, y * voxelSize + voxelSize/2f, RedactorBlocksCTRL.blockData.wallFace.forms.voxel[x,y]*voxelSize * -0.5f);
                    ColidersWallFace[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallFace.forms.voxel[x, y] * voxelSize);
                }
            }
        }
        if (ColidersWallBack == null)
        {
            //������� �������� ���� ���
            if (ObjWallBack != null)
                Destroy(ObjWallBack);

            //������� ��������
            ObjWallBack = new GameObject("ParentWallBack");
            ObjWallBack.transform.parent = gameObject.transform;
            ObjWallBack.transform.localPosition = new Vector3(1,0,1);
            ObjWallBack.transform.localScale = new Vector3(1, 1, 1);

            ColidersWallBack = new BoxCollider[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    GameObject objVoxel = new GameObject("X" + x + " Y" + y);
                    objVoxel.transform.parent = ObjWallBack.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallBack[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallBack[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.wallBack.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallBack[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallBack.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(0, 180.0f, 0);
            ObjWallBack.transform.rotation = rotate;
        }
        if (ColidersWallLeft == null)
        {
            //������� �������� ���� ���
            if (ObjWallLeft != null)
                Destroy(ObjWallLeft);

            //������� ��������
            ObjWallLeft = new GameObject("ParentWallLeft");
            ObjWallLeft.transform.parent = gameObject.transform;
            ObjWallLeft.transform.localPosition = new Vector3(0, 0, 1);
            ObjWallLeft.transform.localScale = new Vector3(1, 1, 1);

            ColidersWallLeft = new BoxCollider[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    GameObject objVoxel = new GameObject("X" + x + " Y" + y);
                    objVoxel.transform.parent = ObjWallLeft.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallLeft[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallLeft[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.wallLeft.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallLeft[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallLeft.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(0, 90.0f, 0);
            ObjWallLeft.transform.rotation = rotate;
        }
        if (ColidersWallRight == null)
        {
            //������� �������� ���� ���
            if (ObjWallRight != null)
                Destroy(ObjWallRight);

            //������� ��������
            ObjWallRight = new GameObject("ParentWallRight");
            ObjWallRight.transform.parent = gameObject.transform;
            ObjWallRight.transform.localPosition = new Vector3(1, 0, 0);
            ObjWallRight.transform.localScale = new Vector3(1, 1, 1);

            ColidersWallRight = new BoxCollider[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    GameObject objVoxel = new GameObject("X" + x + " Y" + y);
                    objVoxel.transform.parent = ObjWallRight.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallRight[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallRight[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.wallRight.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallRight[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallRight.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(0, -90.0f, 0);
            ObjWallRight.transform.rotation = rotate;
        }
        if (ColidersWallUp == null)
        {
            //������� �������� ���� ���
            if (ObjWallUp != null)
                Destroy(ObjWallUp);

            //������� ��������
            ObjWallUp = new GameObject("ParentWallUp");
            ObjWallUp.transform.parent = gameObject.transform;
            ObjWallUp.transform.localPosition = new Vector3(0, 1, 0);
            ObjWallUp.transform.localScale = new Vector3(1, 1, 1);

            ColidersWallUp = new BoxCollider[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    GameObject objVoxel = new GameObject("X" + x + " Y" + y);
                    objVoxel.transform.parent = ObjWallUp.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallUp[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallUp[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.wallUp.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallUp[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallUp.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(90f, 0, 0);
            ObjWallUp.transform.rotation = rotate;

        }
        if (ColidersWallDown == null)
        {
            //������� �������� ���� ���
            if (ObjWallDown != null)
                Destroy(ObjWallDown);

            //������� ��������
            ObjWallDown = new GameObject("ParentWallDown");
            ObjWallDown.transform.parent = gameObject.transform;
            ObjWallDown.transform.localPosition = new Vector3(0, 0, 1);
            ObjWallDown.transform.localScale = new Vector3(1, 1, 1);

            ColidersWallDown = new BoxCollider[16, 16];
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    GameObject objVoxel = new GameObject("X" + x + " Y" + y);
                    objVoxel.transform.parent = ObjWallDown.transform;
                    objVoxel.transform.localPosition = new Vector3();
                    objVoxel.transform.localScale = new Vector3(1, 1, 1);

                    ColidersWallDown[x, y] = objVoxel.AddComponent<BoxCollider>();
                    ColidersWallDown[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.wallDown.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallDown[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.wallDown.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(-90f, 0, 0);
            ObjWallDown.transform.rotation = rotate;
        }
    }

    //��������� �������� ���������
    BoxCollider lastCollider;
    //��������� �������� ������ �����
    GameObject lastObjWall;
    BlockWall lastBlockWall;
    public BlockWall selectBlockWall
    {
        get
        {
            return lastBlockWall;
        }
    }

    Vector2Int voxelPos = new Vector2Int();
    public Vector2Int VoxelPos {
        get {
            return voxelPos;
        }
    }

    //��������� ��������� �����, ���������� ���� �����, ����� �� ����� ���� ������
    public BlockWall GetWall(Collider voxelCollider) {
        //���� ��� ���-�� ������ ��������� ��� ��� ������ � ������� ���
        if (lastBlockWall != null && lastCollider == voxelCollider) {
            return lastBlockWall;    
        }

        //���� ��������� �������

        //�� ����� ��� ����
        if (lastBlockWall != null && voxelCollider.transform.parent.gameObject == lastObjWall)
        {
            //�������������� ������� �� ����� �����
            iniVoxel(voxelCollider);
            return lastBlockWall;
        }

        //���� ���������� ���� �����
        //�������� ����� �����
        iniObjWall(voxelCollider);
        //������ �������� ������� �����
        iniVoxel(voxelCollider);

        //����� ������ ���� ��������
        return lastBlockWall;

        //�������������� ����� �������
        void iniVoxel(Collider voxelCollider)
        {
            //����� ������ ���� � ����� ������� ��������

            if (lastObjWall == ObjWallFace) {
                iniPos(ColidersWallFace);
            }
            else if (lastObjWall == ObjWallBack) {
                iniPos(ColidersWallBack);
            }
            else if (lastObjWall == ObjWallLeft) {
                iniPos(ColidersWallLeft);
            }
            else if (lastObjWall == ObjWallRight) {
                iniPos(ColidersWallRight);
            }
            else if (lastObjWall == ObjWallUp) {
                iniPos(ColidersWallUp);
            }
            else if (lastObjWall == ObjWallDown) {
                iniPos(ColidersWallDown);
            }

            void iniPos(BoxCollider[,] Colliders) {
                //���������� ���������
                for (int x = 0; x < Colliders.GetLength(0); x++) {
                    for (int y = 0; y < Colliders.GetLength(1); y++) {
                        if (Colliders[x, y].gameObject != voxelCollider.gameObject)
                            continue;

                        voxelPos = new Vector2Int(x,y);
                        return;
                    }
                }
            }
        }

        void iniObjWall(Collider voxelCollider)
        {
            //���� �����
            if (voxelCollider.transform.parent.gameObject == ObjWallFace)
            {
                lastObjWall = ObjWallFace;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallFace;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallBack)
            {
                lastObjWall = ObjWallBack;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallBack;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallLeft)
            {
                lastObjWall = ObjWallLeft;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallLeft;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallRight)
            {
                lastObjWall = ObjWallRight;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallRight;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallUp)
            {
                lastObjWall = ObjWallUp;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallUp;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallDown)
            {
                lastObjWall = ObjWallDown;
                lastBlockWall = RedactorBlocksCTRL.blockData.wallDown;
            }
            else
            {
                lastObjWall = null;
                lastBlockWall = null;
            }
        }
    }
    public void UnSelectWall() {
        if (lastBlockWall != null)
            lastBlockWall = null;
    }


    public void delCollidersWall(BlockWall blockWall) {
        if (blockWall == RedactorBlocksCTRL.blockData.wallFace)
        {
            ColidersWallFace = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.wallBack)
        {
            ColidersWallBack = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.wallLeft)
        {
            ColidersWallLeft = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.wallRight)
        {
            ColidersWallRight = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.wallDown)
        {
            ColidersWallDown = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.wallUp)
        {
            ColidersWallUp = null;
        }
    }
}
