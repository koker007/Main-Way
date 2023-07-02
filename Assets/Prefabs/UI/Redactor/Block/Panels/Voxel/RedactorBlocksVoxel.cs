using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�������� �� ��������� ��������, ���������
public class RedactorBlocksVoxel : MonoBehaviour
{
    static public RedactorBlocksVoxel main;

    [SerializeField]
    SliderCTRL sliderZoneSizeX;
    [SerializeField]
    SliderCTRL sliderZoneSizeY;

    [SerializeField]
    SliderCTRL sliderHeight;
    [SerializeField]
    SliderCTRL sliderHeightPlusRandom;

    float sliderHeightScale = 20;

    [SerializeField]
    RedactorBlocksColiders colliders;

    [Header("Visual help")]
    [SerializeField]
    GameObject pointA;
    [SerializeField]
    GameObject pointB;
    [SerializeField]
    GameObject voxZone;
    [SerializeField]
    GameObject voxPos;


    voxelData[,] voxelBuffer;
    Vector2Int selectSize = new Vector2Int(1,1);
    Vector2Int selectStart = new Vector2Int();

    class voxelData {
        public float height;
        public Color color;
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }
    private void OnEnable()
    {
        main = this;
        iniColliders();
        enableAll();
    }

    void enableAll() {
        voxZone.SetActive(true);
        pointA.SetActive(true);
        pointB.SetActive(true);

        sliderZoneSizeX.slider.maxValue = 16;
        sliderZoneSizeY.slider.maxValue = 16;
        sliderZoneSizeX.slider.minValue = 1;
        sliderZoneSizeY.slider.minValue = 1;
        AcceptSliderZoneSize();
    }

    private void OnDisable()
    {
        disableAll();
    }

    void disableAll() {
        voxZone.SetActive(false);
        pointA.SetActive(false);
        pointB.SetActive(false);
    }

    void iniColliders()
    {
        //���� ���������� ��������� ���� ��������
        if (RedactorBlocksColiders.main != null ||
            !RedactorBlocksColiders.main.enabled)
            RedactorBlocksColiders.main.enabled = true;

        if (colliders != null)
            return;

        colliders = RedactorBlocksColiders.main;
    }

    // Update is called once per frame
    void Update()
    {
        TestHeight();

        UpdateCameraClickLeftIni();
        UpdateCameraClickLeftChange();

        UpdateVoxZoneSelect();

        //���� �� ����������� - �������
        UpdateCopyPast();
    }

    void TestHeight() {
        //��������� ������ � ������ �������� ����� �� �������������� ����������
        TypeBlock.Wall blockWall = RedactorBlocksColiders.main.selectBlockWall;

        //���� ����� �� ������� ������� ��������
        if (blockWall == null) {
            if(sliderHeight.transform.parent.gameObject.activeSelf)
                sliderHeight.transform.parent.gameObject.SetActive(false);
            return;
        }

        if (!sliderHeight.transform.parent.gameObject.activeSelf)
            sliderHeight.transform.parent.gameObject.SetActive(true);

        sliderHeight.slider.minValue = -7 * sliderHeightScale;
        sliderHeight.slider.maxValue = 3 * sliderHeightScale;

        sliderHeightPlusRandom.slider.minValue = 0 * sliderHeightScale;
        sliderHeightPlusRandom.slider.maxValue = 3 * sliderHeightScale;

        sliderHeight.SetValueText((sliderHeight.slider.value / sliderHeightScale) + "");
        sliderHeightPlusRandom.SetValueText((sliderHeightPlusRandom.slider.value / sliderHeightScale) + "");
    }

    //������� ��������� ������ �� ��������
    public void AcceptSliderHeight() {
        TypeBlock.Wall blockWall = RedactorBlocksColiders.main.selectBlockWall;
        if (blockWall == null)
            return;


        Vector2Int voxPos = RedactorBlocksColiders.main.VoxelPos;

        //����������� ������ ��� �����
        float height = sliderHeight.slider.value / sliderHeightScale;
        float heightRand = sliderHeightPlusRandom.slider.value / sliderHeightScale;

        //���������� ���������� �������
        for (int x = 0; x < selectSize.x; x++) {
            int xNow = voxPos.x + x;
            if (xNow >= 16)
                continue;

            for (int y = 0; y < selectSize.y; y++) {
                int yNow = voxPos.y + y;

                if (yNow >= 16)
                    continue;



                //����������� ������ ��� �����
                float heightLimited = getVoxelHeightLimit(new Vector2Int(xNow, yNow), Random.Range(height, height + heightRand));

                blockWall.forms.voxel[xNow, yNow] = heightLimited;
            }
        }

        blockWall.calcVertices();
        RedactorBlocksColiders.main.delCollidersWall(blockWall);

        sliderHeight.SetValueText(blockWall.forms.voxel[voxPos.x, voxPos.y].ToString());
    }
    public void AcceptSliderZoneSize() {
        int x = (int)sliderZoneSizeX.slider.value;
        int y = (int)sliderZoneSizeY.slider.value;

        selectSize = new Vector2Int(x, y);

        sliderZoneSizeX.SetValueText();
        sliderZoneSizeY.SetValueText();
    }

    static public float getVoxelHeightLimit(Vector2Int pos, float height) {
        //��������� ����������� ����������� � ��������
        int voxMin = 16 / 2;
        if (pos.x < 8)
        {
            if (voxMin > pos.x)
                voxMin = pos.x;
        }
        else
        {
            if (voxMin > 15 - pos.x)
                voxMin = 15 - pos.x;
        }
        if (pos.y < 8)
        {
            if (voxMin > pos.y)
                voxMin = pos.y;
        }
        else
        {
            if (voxMin > 15 - pos.y)
                voxMin = 15 - pos.y;
        }
        voxMin *= -1;

        //������������
        if (height > 3)
            height = 3;
        else if (height < voxMin)
            height = voxMin;

        return height;
    }



    //�������� ���� ���������� �������
    public void acceptVoxColor(TypeBlock.Wall wall, Vector2Int pos, Color color)
    {
        TypeBlock typeBlock = RedactorBlocksCTRL.blockData as TypeBlock;

        //��������� ��� ����� ����������
        if (wall == null)
            return;

        wall.texture.SetPixel(pos.x, pos.y, color);
        wall.texture.Apply();

        //������� ���� �� ���� �����
        if (pos.x != 0 && pos.x != 15 &&
            pos.y != 0 && pos.y != 15)
            return;

        //���� ������� � ������� ������� ���� �������� ���� � �������� ���� ������������ �������



        //���� ��������
        if (pos.x == 0)
        {
            //���� ������� ������� �������
            if (wall == typeBlock.wallFace)
            {
                //�� ����� ����� ����� �������
                typeBlock.wallLeft.texture.SetPixel(15, pos.y, color);
                typeBlock.wallLeft.texture.Apply();
            }
            //���� ������� ������� ����
            else if (wall == typeBlock.wallUp)
            {
                //�� ����� ����� ����� ������� �� ������� Y ����� �� X
                typeBlock.wallLeft.texture.SetPixel(15 - pos.y, 15, color);
                typeBlock.wallLeft.texture.Apply();
            }
            else if (wall == typeBlock.wallDown)
            {
                typeBlock.wallLeft.texture.SetPixel(pos.y, 0, color);
                typeBlock.wallLeft.texture.Apply();
            }
            else if (wall == typeBlock.wallBack)
            {
                typeBlock.wallRight.texture.SetPixel(15, pos.y, color);
                typeBlock.wallRight.texture.Apply();
            }
            else if (wall == typeBlock.wallLeft)
            {
                typeBlock.wallBack.texture.SetPixel(15, pos.y, color);
                typeBlock.wallBack.texture.Apply();
            }
            else if (wall == typeBlock.wallRight)
            {
                typeBlock.wallFace.texture.SetPixel(15, pos.y, color);
                typeBlock.wallFace.texture.Apply();
            }
        }
        //������
        else if (pos.x == 15)
        {
            if (wall == typeBlock.wallFace)
            {
                typeBlock.wallRight.texture.SetPixel(0, pos.y, color);
                typeBlock.wallRight.texture.Apply();
            }
            else if (wall == typeBlock.wallUp)
            {
                //�� ����� ����� ����� ������� �� ������� Y ����� �� X
                typeBlock.wallRight.texture.SetPixel(pos.y, 15, color);
                typeBlock.wallRight.texture.Apply();
            }
            else if (wall == typeBlock.wallDown)
            {
                typeBlock.wallRight.texture.SetPixel(15 - pos.y, 0, color);
                typeBlock.wallRight.texture.Apply();
            }
            else if (wall == typeBlock.wallBack)
            {
                typeBlock.wallLeft.texture.SetPixel(0, pos.y, color);
                typeBlock.wallLeft.texture.Apply();
            }
            else if (wall == typeBlock.wallLeft)
            {
                typeBlock.wallFace.texture.SetPixel(0, pos.y, color);
                typeBlock.wallFace.texture.Apply();
            }
            else if (wall == typeBlock.wallRight)
            {
                typeBlock.wallBack.texture.SetPixel(0, pos.y, color);
                typeBlock.wallBack.texture.Apply();
            }
        }

        //����� ��������
        if (pos.y == 0)
        {
            if (wall == typeBlock.wallFace)
            {
                //�� ����� ���
                typeBlock.wallDown.texture.SetPixel(pos.x, 15, color);
                typeBlock.wallDown.texture.Apply();
            }
            if (wall == typeBlock.wallUp)
            {
                //�� ����� ����
                typeBlock.wallFace.texture.SetPixel(pos.x, 15, color);
                typeBlock.wallFace.texture.Apply();
            }
            else if (wall == typeBlock.wallDown)
            {
                typeBlock.wallBack.texture.SetPixel(15 - pos.x, 0, color);
                typeBlock.wallBack.texture.Apply();
            }
            else if (wall == typeBlock.wallBack)
            {
                typeBlock.wallDown.texture.SetPixel(15 - pos.x, 0, color);
                typeBlock.wallDown.texture.Apply();
            }
            else if (wall == typeBlock.wallLeft)
            {
                typeBlock.wallDown.texture.SetPixel(0, pos.x, color);
                typeBlock.wallDown.texture.Apply();
            }
            else if (wall == typeBlock.wallRight)
            {
                typeBlock.wallDown.texture.SetPixel(15, 15 - pos.x, color);
                typeBlock.wallDown.texture.Apply();
            }
        }
        //������
        else if (pos.y == 15)
        {
            if (wall == typeBlock.wallFace)
            {
                //�� ������ ����
                typeBlock.wallUp.texture.SetPixel(pos.x, 0, color);
                typeBlock.wallUp.texture.Apply();
            }
            if (wall == typeBlock.wallUp)
            {
                //�� ������ ���
                typeBlock.wallBack.texture.SetPixel(15 - pos.x, 15, color);
                typeBlock.wallBack.texture.Apply();
            }
            else if (wall == typeBlock.wallDown)
            {
                typeBlock.wallFace.texture.SetPixel(pos.x, 0, color);
                typeBlock.wallFace.texture.Apply();
            }
            else if (wall == typeBlock.wallBack)
            {
                typeBlock.wallUp.texture.SetPixel(15 - pos.x, 15, color);
                typeBlock.wallUp.texture.Apply();
            }
            else if (wall == typeBlock.wallLeft)
            {
                typeBlock.wallUp.texture.SetPixel(0, 15 - pos.x, color);
                typeBlock.wallUp.texture.Apply();
            }
            else if (wall == typeBlock.wallRight)
            {
                typeBlock.wallUp.texture.SetPixel(15, pos.x, color);
                typeBlock.wallUp.texture.Apply();
            }
        }
    }
    public void acceptVoxColorArray(TypeBlock.Wall wall, Vector2Int pos, Color color, Vector3 plusRand) {

        float H = 0;
        float S = 0;
        float V = 0;
        Color.RGBToHSV(color, out H, out S, out V);

        float Hmax = H + plusRand.x;
        float Smax = S + plusRand.y;
        float Vmax = V + plusRand.z;

        if (Hmax > 1)
            Hmax = 1;
        if (Smax > 1)
            Smax = 1;
        if (Vmax > 1)
            Vmax = 1;

        for (int selectX = 0; selectX < selectSize.x; selectX++) {
            int nowX = pos.x + selectX;
            if (nowX >= 16)
                break;

            for (int selectY = 0; selectY < selectSize.y; selectY++) {
                int nowY = pos.y + selectY;
                if (nowY >= 16)
                    break;

                //������������� ����
                float Hnow = Random.Range(H, Hmax);
                float Snow = Random.Range(S, Smax);
                float Vnow = Random.Range(V, Vmax);

                Color colorNow = Color.HSVToRGB(Hnow, Snow, Vnow);

                acceptVoxColor(wall, new Vector2Int(nowX, nowY), colorNow);
            }
        }
    }

    //�������� ������� ����� ������
    struct ClickLeft
    {
        public bool stopRotate;
        public float timeStart;
        public Vector3 directionStartRay;
        public Vector3 voxelStartRay;
        public Vector2Int mouseScreenPosStart;

        public bool canLockZero;
        public bool canLockLeft;
        public bool canLockRight;
        public bool canLockUp;
        public bool canLockDown;

        public bool isLockZero;
        public bool isLockLeft;
        public bool isLockRight;
        public bool isLockUp;
        public bool isLockDown;

    }
    ClickLeft clickLeft;

    void UpdateCameraClickLeftIni()
    {
        //������ ���� ������ �� �������������
        if (!RedactorBlocksVisualizator.MouseOn)
            return;

        //������� ���� ������ �� ������ ��� ������
        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        //���� ���������� �������� ������ �� ������
        if (RedactorBlocksVisualizator.IsRotate)
            return;

        //���� ��� �������� � ������ ������ //������� ���
        RaycastHit hitInfo;
        Ray ray = RedactorBlocksVisualizator.main.camera.ScreenPointToRay(Input.mousePosition);
        bool rayCollision = Physics.Raycast(ray, out hitInfo, 15);

        //������� ���� ��������������� �� ����
        if (!rayCollision)
        {
            colliders.UnSelectWall();
            return;
        }
        //����������� ���� ��������


        //��������� � ����� �������� ���� ������������
        TypeBlock.Wall blockWall = colliders.GetWall(hitInfo.collider);
        //���� �� ������� �����
        if (blockWall == null)
            return;

        //����� ������ �����
        clickLeft.timeStart = Time.unscaledTime;
        clickLeft.stopRotate = false;
        clickLeft.directionStartRay = ray.direction; //����������� ���� � ������� �����������
        clickLeft.voxelStartRay = hitInfo.point; //���������� ����� ������ ����

        clickLeft.isLockZero = false;
        clickLeft.isLockDown = false;
        clickLeft.isLockUp = false;
        clickLeft.isLockLeft = false;
        clickLeft.isLockRight = false;

        clickLeft.canLockZero = true;
        clickLeft.canLockDown = true;
        clickLeft.canLockLeft = true;
        clickLeft.canLockRight = true;
        clickLeft.canLockUp = true;

        Debug.Log("Block redactor voxel: " + colliders.VoxelPos);

    }
    void UpdateCameraClickLeftChange()
    {
        //���� ��� �� �� ������������� �������
        if (!RedactorBlocksVisualizator.MouseOn)
            return;

        //�������� �������� ������ �������

        //���������� ��������� ����
        //���� ������ �� ������
        if (!Input.GetKey(KeyCode.Mouse0))
            return;

        //���� ���� ��� ���� ��������
        if (RedactorBlocksVisualizator.IsRotate ||
            clickLeft.stopRotate)
        {
            //���� �������� ������� ����
            //��������� ���������� ���������
            clickLeft.stopRotate = true;
            return;
        }

        TypeBlock.Wall blockWall = colliders.selectBlockWall;
        //���� �� ������� �����
        //��� ����� ��� ���������� ��������� � ���� ��� �� ���������
        if (colliders.selectBlockWall == null ||
            clickLeft.timeStart + 0.5f > Time.unscaledTime)
            return;

        Vector3 voxPointStart = new Vector3();
        Vector3 voxDirection = new Vector3(0, 0, -1);
        Vector3 voxPointCollision = new Vector3();

        //���� ����� ������������
        CalcPointColision();

        void CalcPointColision()
        {
            MeshRenderer meshRenderer = RedactorBlocksVisualizator.MeshRenderer;
            Ray cameraR = RedactorBlocksVisualizator.main.camera.ScreenPointToRay(Input.mousePosition);

            //������� ��������� �� ������� ������ �������� ���
            //���� ����� � ����� ����� ����, 2 � ����� ����� + �����������, 3 ����� ����� + ����� ������������ ������
            //Plane targetPlane = new Plane(cameraR.origin + cameraR.direction, cameraR.origin, cameraR.origin + camera.gameObject.transform.up);
            float vox = 0;

            TypeBlock typeBlock = RedactorBlocksCTRL.blockData as TypeBlock;

            //���� ��� ������� ������� �����
            if (blockWall == typeBlock.wallFace)
            {
                voxPointStart += new Vector3(
                    colliders.VoxelPos.x,
                    colliders.VoxelPos.y,
                    typeBlock.wallFace.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * -1);
                voxDirection = new Vector3(0, 0, -1);
            }
            //������ ������� �����
            else if (blockWall == typeBlock.wallBack)
            {
                voxPointStart += new Vector3(
                    16 - colliders.VoxelPos.x,
                    colliders.VoxelPos.y,
                    16 + typeBlock.wallBack.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * 1);
                voxDirection = new Vector3(0, 0, 1);
            }
            //����� �������
            else if (blockWall == typeBlock.wallLeft)
            {
                voxPointStart += new Vector3(
                    typeBlock.wallLeft.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * -1,
                    colliders.VoxelPos.y,
                    16 - colliders.VoxelPos.x);
                voxDirection = new Vector3(-1, 0, 0);
            }
            //������ �������
            else if (blockWall == typeBlock.wallRight)
            {
                voxPointStart += new Vector3(
                    16 + typeBlock.wallRight.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * 1,
                    colliders.VoxelPos.y,
                    colliders.VoxelPos.x);
                voxDirection = new Vector3(1, 0, 0);
            }
            //������ �������
            else if (blockWall == typeBlock.wallDown)
            {
                voxPointStart += new Vector3(
                    colliders.VoxelPos.x,
                    typeBlock.wallDown.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * -1,
                    16 - colliders.VoxelPos.y);
                voxDirection = new Vector3(0, -1, 0);
            }
            //������� �������
            else if (blockWall == typeBlock.wallUp)
            {
                voxPointStart += new Vector3(
                    colliders.VoxelPos.x,
                    16 + typeBlock.wallUp.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] * 1,
                    colliders.VoxelPos.y);
                voxDirection = new Vector3(0, 1, 0);
            }

            //�������� ������ 16 � 1
            voxPointStart *= RedactorBlocksCTRL.voxSize;
            //���������� ��������� � ������� �����������
            voxPointStart += meshRenderer.transform.position;

            voxPointStart = clickLeft.voxelStartRay;
            selectStart = colliders.VoxelPos;

            Ray voxR = new Ray(voxPointStart, voxDirection);

            Line line1 = new Line(voxR.origin, voxR.origin + voxR.direction);
            Line line2 = new Line(cameraR.origin, cameraR.origin + cameraR.direction * 4);

            //�������� ����� ��������� �����. ����� ��������
            Line lineNearest = Line.GetNearestLine(line1, line2, 100);

            pointA.transform.position = lineNearest.PointA;
            pointB.transform.position = lineNearest.PointB;


            //����� ����� ���������
            float voxNeed = 0.0f;
            if (blockWall == typeBlock.wallFace)
            {
                voxNeed = (lineNearest.PointA.z - meshRenderer.transform.position.z) * -1;
            }
            else if (blockWall == typeBlock.wallBack)
            {
                voxNeed = (lineNearest.PointA.z - meshRenderer.transform.position.z) - 1;
            }
            else if (blockWall == typeBlock.wallLeft)
            {
                voxNeed = (lineNearest.PointA.x - meshRenderer.transform.position.x) * -1;
            }
            else if (blockWall == typeBlock.wallRight)
            {
                voxNeed = (lineNearest.PointA.x - meshRenderer.transform.position.x) - 1;
            }
            else if (blockWall == typeBlock.wallDown)
            {
                voxNeed = (lineNearest.PointA.y - meshRenderer.transform.position.y) * -1;
            }
            else if (blockWall == typeBlock.wallUp)
            {
                voxNeed = (lineNearest.PointA.y - meshRenderer.transform.position.y) - 1;
            }


            //��������� ����������� ����������� � ��������
            int voxMin = 16 / 2;
            if (colliders.VoxelPos.x < 8)
            {
                if (voxMin > colliders.VoxelPos.x)
                    voxMin = colliders.VoxelPos.x;
            }
            else
            {
                if (voxMin > 15 - colliders.VoxelPos.x)
                    voxMin = 15 - colliders.VoxelPos.x;
            }
            if (colliders.VoxelPos.y < 8)
            {
                if (voxMin > colliders.VoxelPos.y)
                    voxMin = colliders.VoxelPos.y;
            }
            else
            {
                if (voxMin > 15 - colliders.VoxelPos.y)
                    voxMin = 15 - colliders.VoxelPos.y;
            }
            voxMin *= -1;

            //�������� � �������� ��������
            voxNeed /= RedactorBlocksCTRL.voxSize;

            //������������
            if (voxNeed > 3)
                voxNeed = 3;
            else if (voxNeed < voxMin)
                voxNeed = voxMin;

            //////////////////////////////////////////////////
            //����������
            float distLock = 0.3f;
            //����� �� ��������� �� ����
            if (clickLeft.canLockZero &&
                Mathf.Abs(0 - voxNeed) < distLock)
            {
                clickLeft.isLockZero = true;
                voxNeed = 0;
            }
            else if (clickLeft.isLockZero)
            {
                //��� ������ �� ����� ����������
                clickLeft.canLockZero = false;
                //������ ���������� ���
                clickLeft.isLockZero = false;
            }

            //����� �� ��������� � ����
            if (!clickLeft.isLockZero &&
                clickLeft.canLockLeft && colliders.VoxelPos.x > 0 &&
                Mathf.Abs(blockWall.forms.voxel[colliders.VoxelPos.x - 1, colliders.VoxelPos.y] - voxNeed) < distLock)
            {
                clickLeft.isLockLeft = true;
                //��������� ������ �����
                voxNeed = blockWall.forms.voxel[colliders.VoxelPos.x - 1, colliders.VoxelPos.y];
                Debug.Log("Left");
            }
            else if (clickLeft.isLockLeft)
            {
                //��� ������ �� ����� ����������
                clickLeft.canLockLeft = false;
                //������ ���������� ���
                clickLeft.isLockLeft = false;
            }

            //���� ���������� ����� ������ ���
            if (!clickLeft.isLockZero && !clickLeft.isLockLeft &&
                clickLeft.canLockRight && colliders.VoxelPos.x < 15 &&
                Mathf.Abs(blockWall.forms.voxel[colliders.VoxelPos.x + 1, colliders.VoxelPos.y] - voxNeed) < distLock)
            {
                clickLeft.isLockRight = true;
                voxNeed = blockWall.forms.voxel[colliders.VoxelPos.x + 1, colliders.VoxelPos.y];
                Debug.Log("Right");
            }
            else if (clickLeft.isLockRight)
            {
                //��� ������ �� ����� ����������
                clickLeft.canLockRight = false;
                //������ ���������� ���
                clickLeft.isLockRight = false;
            }

            if (!clickLeft.isLockZero && !clickLeft.isLockLeft && !clickLeft.isLockRight &&
                clickLeft.canLockUp && colliders.VoxelPos.y < 15 &&
                Mathf.Abs(blockWall.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y + 1] - voxNeed) < distLock)
            {
                clickLeft.isLockUp = true;
                voxNeed = blockWall.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y + 1];
                Debug.Log("Up");
            }
            else if (clickLeft.isLockUp)
            {
                //��� ������ �� ����� ����������
                clickLeft.canLockUp = false;
                //������ ���������� ���
                clickLeft.isLockUp = false;
            }


            if (!clickLeft.isLockZero && !clickLeft.isLockLeft && !clickLeft.isLockRight && !clickLeft.isLockUp &&
                clickLeft.canLockDown && colliders.VoxelPos.y > 0 &&
                Mathf.Abs(blockWall.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y - 1] - voxNeed) < distLock)
            {
                clickLeft.isLockDown = true;
                voxNeed = blockWall.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y - 1];
                Debug.Log("Down");
            }
            else if (clickLeft.isLockDown)
            {
                //��� ������ �� ����� ����������
                clickLeft.canLockDown = false;
                //������ ���������� ���
                clickLeft.isLockDown = false;
            }

            //������ ����� ��������� ������ �������
            //��������� ��
            blockWall.forms.voxel[colliders.VoxelPos.x, colliders.VoxelPos.y] = voxNeed;

            blockWall.calcVertices();
            colliders.delCollidersWall(blockWall);

            //Debug.Log("Vox:" + vox + " voxNeed: " + voxNeed + " PointA: " + lineNearest.PointA + " PointB: " + lineNearest.PointB + " Dist: " + lineNearest.vector.magnitude);
        }
    }

    void UpdateVoxZoneSelect()
    {
        //������������ � ��������� ������
        TypeBlock.Wall blockWall = colliders.selectBlockWall;

        if (blockWall == null)
        {
            voxZone.SetActive(false);
            return;
        }

        voxZone.SetActive(true);
        voxPos.transform.localPosition = new Vector3(colliders.VoxelPos.x / 16.0f, colliders.VoxelPos.y / 16.0f, 0);

        TypeBlock typeBlock = RedactorBlocksCTRL.blockData as TypeBlock;

        if (blockWall == typeBlock.wallFace)
        {
            voxZone.transform.localPosition = new Vector3(0, 0, 0);
            voxZone.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (blockWall == typeBlock.wallBack)
        {
            voxZone.transform.localPosition = new Vector3(1, 0, 1);
            voxZone.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (blockWall == typeBlock.wallLeft)
        {
            voxZone.transform.localPosition = new Vector3(0, 0, 1);
            voxZone.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else if (blockWall == typeBlock.wallRight)
        {
            voxZone.transform.localPosition = new Vector3(1, 0, 0);
            voxZone.transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else if (blockWall == typeBlock.wallUp)
        {
            voxZone.transform.localPosition = new Vector3(0, 1, 0);
            voxZone.transform.eulerAngles = new Vector3(90, 0, 0);
        }
        else if (blockWall == typeBlock.wallDown)
        {
            voxZone.transform.localPosition = new Vector3(0, 0, 1);
            voxZone.transform.eulerAngles = new Vector3(-90, 0, 0);
        }

        int sizeX = selectSize.x;
        int sizeY = selectSize.y;

        if (colliders.VoxelPos.x + sizeX >= 16)
            sizeX += 16 - (colliders.VoxelPos.x + sizeX);
        if (colliders.VoxelPos.y + sizeY >= 16)
            sizeY += 16 - (colliders.VoxelPos.y + sizeY);

        voxPos.transform.localScale = new Vector3(sizeX, sizeY, 1);
    }

    void UpdateCopyPast() {
        //������������ �����������
        bool keyCTRLnow = false;
        bool keyCnow = false;
        bool keyVnow = false;

        bool keyCTRLlong = false;
        bool keyClong = false;
        bool keyVlong = false;

        keyVnow = Input.GetKeyDown(KeyCode.V);
        keyCnow = Input.GetKeyDown(KeyCode.C);
        keyVlong = Input.GetKey(KeyCode.V);
        keyClong = Input.GetKey(KeyCode.C);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            keyCTRLlong = true;

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            keyCTRLnow = true;

        //���� ���� ���������
        if (keyCTRLlong && keyCnow || keyCTRLnow && keyClong) {
            selectCopy();
        }
        else if (keyCTRLlong && keyVnow || keyCTRLnow && keyVlong) {
            past();
        }


        void selectCopy() {
            TypeBlock.Wall blockWall = colliders.selectBlockWall;
            if (blockWall == null)
                return;

            Debug.Log("Voxel Copy");

            //������� �����
            voxelBuffer = new voxelData[selectSize.x, selectSize.y];

            //��������� ����� �������
            for (int x = 0; x < voxelBuffer.GetLength(0); x++) {
                int resultX = colliders.VoxelPos.x + x;

                //���� ����� �� ������� 16 ������� �������
                if (resultX >= 16)
                    break;

                for (int y = 0; y < voxelBuffer.GetLength(1); y++) {
                    int resultY = colliders.VoxelPos.y + y;

                    if (resultY >= 16)
                        break;

                    voxelBuffer[x, y] = new voxelData();
                    voxelBuffer[x, y].height = blockWall.forms.voxel[resultX, resultY];
                    voxelBuffer[x, y].color = blockWall.texture.GetPixel(resultX, resultY);
                }
            }

            //���� �������� � �������
            RedactorBlocksVoxColor.main.setColor(blockWall.texture.GetPixel(colliders.VoxelPos.x, colliders.VoxelPos.y));
        }
        void past() {
            TypeBlock.Wall blockWall = colliders.selectBlockWall;
            if (blockWall == null || voxelBuffer == null)
                return;

            Debug.Log("Voxel Past");

            //��������� � ����� �� ��� ����
            for (int x = 0; x < voxelBuffer.GetLength(0) && x < selectSize.x; x++) {
                int resultX = colliders.VoxelPos.x + x;
                if (resultX >= 16)
                    break;
                
                for (int y = 0; y < voxelBuffer.GetLength(1) && y < selectSize.y; y++) {
                    int resultY = colliders.VoxelPos.y + y;
                    if (resultY >= 16)
                        break;

                    if (voxelBuffer[x, y] == null)
                        continue;

                    blockWall.forms.voxel[resultX, resultY] = voxelBuffer[x, y].height;
                    //blockWall.texture.SetPixel(resultX, resultY, voxelBuffer[x,y].color);
                    acceptVoxColor(blockWall, new Vector2Int(resultX, resultY), voxelBuffer[x, y].color);
                }
            }

            blockWall.calcVertices();
            blockWall.texture.Apply();
        }
    }
}
