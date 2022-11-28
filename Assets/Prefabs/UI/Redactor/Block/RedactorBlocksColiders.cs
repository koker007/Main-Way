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
        SelectWall(lastSide);
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
                    ColidersWallFace[x, y].center = new Vector3(x * voxelSize + voxelSize/2f, y * voxelSize + voxelSize/2f, RedactorBlocksCTRL.blockData.TBlock.wallFace.forms.voxel[x,y]*voxelSize * -0.5f);
                    ColidersWallFace[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallFace.forms.voxel[x, y] * voxelSize);
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
                    ColidersWallBack[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.TBlock.wallBack.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallBack[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallBack.forms.voxel[x, y] * voxelSize);
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
                    ColidersWallLeft[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.TBlock.wallLeft.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallLeft[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallLeft.forms.voxel[x, y] * voxelSize);
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
                    ColidersWallRight[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.TBlock.wallRight.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallRight[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallRight.forms.voxel[x, y] * voxelSize);
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
                    ColidersWallUp[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.TBlock.wallUp.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallUp[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallUp.forms.voxel[x, y] * voxelSize);
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
                    ColidersWallDown[x, y].center = new Vector3(x * voxelSize + voxelSize / 2, y * voxelSize + voxelSize / 2, RedactorBlocksCTRL.blockData.TBlock.wallDown.forms.voxel[x, y] * voxelSize * -0.5f);
                    ColidersWallDown[x, y].size = new Vector3(voxelSize, voxelSize, RedactorBlocksCTRL.blockData.TBlock.wallDown.forms.voxel[x, y] * voxelSize);
                }
            }

            Quaternion rotate = new Quaternion();
            rotate.eulerAngles = new Vector3(-90f, 0, 0);
            ObjWallDown.transform.rotation = rotate;
        }
    }

    //��������� �������� ���������
    Collider lastCollider;
    //��������� �������� ������ �����
    GameObject lastObjWall;
    TypeBlock.Wall lastBlockWall;
    Side lastSide = Side.face;

    public TypeBlock.Wall selectBlockWall
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
    public void SelectWall(Side selectSide) {
        lastSide = selectSide;

        lastCollider = GetLastCollider();

        GetWall(lastCollider);

        Collider GetLastCollider() {
            //�� ��������� ����� � ������� �������� ���������
            if (selectSide == Side.back)
                return ColidersWallBack[voxelPos.x, voxelPos.y];
            else if (selectSide == Side.left)
                return ColidersWallLeft[voxelPos.x, voxelPos.y];
            else if (selectSide == Side.right)
                return ColidersWallRight[voxelPos.x, voxelPos.y];
            else if (selectSide == Side.up)
                return ColidersWallUp[voxelPos.x, voxelPos.y];
            else if (selectSide == Side.down)
                return ColidersWallDown[voxelPos.x, voxelPos.y];
            else
                return ColidersWallFace[voxelPos.x, voxelPos.y];

        }
    }
    public TypeBlock.Wall GetWall(Collider voxelCollider) {
        if (voxelCollider == null)
            return null;

        lastCollider = voxelCollider;

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
                        if (Colliders[x, y] != voxelCollider)
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
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallFace;
                lastSide = Side.face;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallBack)
            {
                lastObjWall = ObjWallBack;
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallBack;
                lastSide = Side.back;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallLeft)
            {
                lastObjWall = ObjWallLeft;
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallLeft;
                lastSide = Side.left;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallRight)
            {
                lastObjWall = ObjWallRight;
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallRight;
                lastSide = Side.right;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallUp)
            {
                lastObjWall = ObjWallUp;
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallUp;
                lastSide = Side.up;
            }
            else if (voxelCollider.transform.parent.gameObject == ObjWallDown)
            {
                lastObjWall = ObjWallDown;
                lastBlockWall = RedactorBlocksCTRL.blockData.TBlock.wallDown;
                lastSide = Side.down;
            }
            else
            {
                lastObjWall = null;
                lastBlockWall = null;
                lastSide = Side.face;
            }
        }
    }
    public void UnSelectWall() {
        if (lastBlockWall != null)
            lastBlockWall = null;
    }


    public void delCollidersWall(TypeBlock.Wall blockWall) {
        if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallFace)
        {
            ColidersWallFace = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallBack)
        {
            ColidersWallBack = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallLeft)
        {
            ColidersWallLeft = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallRight)
        {
            ColidersWallRight = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallDown)
        {
            ColidersWallDown = null;
        }
        else if (blockWall == RedactorBlocksCTRL.blockData.TBlock.wallUp)
        {
            ColidersWallUp = null;
        }

        TestCreateColliders();
    }
}
