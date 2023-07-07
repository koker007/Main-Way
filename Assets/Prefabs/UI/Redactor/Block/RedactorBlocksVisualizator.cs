using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RedactorBlocksVisualizator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static public RedactorBlocksVisualizator main;

    [SerializeField]
    Material materialBlock;
    [SerializeField]
    Material materialVoxels;
    [SerializeField]
    Material materialLiquid;
    [SerializeField]
    MeshRenderer meshRenderer;

    [SerializeField]
    public Camera camera;
    [SerializeField]
    Camera camera2;
    [SerializeField]
    Transform cameraRotateObj;
    [SerializeField]
    RawImage visualizator1;
    [SerializeField]
    RawImage visualizator2;
    RenderTexture renderTexture;
    RenderTexture renderTexture2;

    bool isRotate = false;
    static public bool IsRotate{
        get {
            return main.isRotate;
        }
    }

    bool isRotateTo = true; //�������� � ��������� �����������
    Vector3 targetRot = new Vector3(45, 45, 0);


    bool mouseOnVisualizator = false;
    static public bool MouseOn {
        get {
            return main.mouseOnVisualizator;
        }
    }
    static public MeshRenderer MeshRenderer {
        get { return main.meshRenderer; }
    }

    private void OnEnable()
    {
        main = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Settings.TestUnloadRam();

        //����������� ���������� ��� ����
        UpdateMesh();

        CameraRenderTexture();

        UpdateCameraRotate();
    }

    void UpdateMesh() {
        if (RedactorBlocksCTRL.main == null)
            return;

        //������� ������ ������
        MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
        if (meshFilter.sharedMesh != null) {
            //meshFilter.sharedMesh.Clear(false);
            Destroy(meshFilter.sharedMesh);

            //������� ���������
            for (int num = 0; num < meshRenderer.materials.Length; num++) {
                Destroy(meshRenderer.materials[num]);
            }
        }

        //�������� ����
        BlockData blockData = RedactorBlocksCTRL.blockData;

        TypeBlock typeBlock = blockData as TypeBlock;
        TypeVoxel typeVoxel = blockData as TypeVoxel;
        TypeLiquid typeLiquid = blockData as TypeLiquid;

        if (typeBlock != null)
            MeshBlock();
        else if (typeVoxel != null)
            MeshVoxel();
        else if (typeLiquid != null)
            MeshLiquid();


        ///////////////////////////////////////////
        ////
        void MeshBlock() {
            meshFilter.sharedMesh = typeBlock.GetMesh(true, true, true, true, true, true);
            meshFilter.sharedMesh.Optimize();

            //��������� ��������� � ����
            meshRenderer.materials = new Material[6]{
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
            };

            meshRenderer.materials[0].mainTexture = typeBlock.wallFace.texture;
            meshRenderer.materials[1].mainTexture = typeBlock.wallBack.texture;
            meshRenderer.materials[2].mainTexture = typeBlock.wallRight.texture;
            meshRenderer.materials[3].mainTexture = typeBlock.wallLeft.texture;
            meshRenderer.materials[4].mainTexture = typeBlock.wallUp.texture;
            meshRenderer.materials[5].mainTexture = typeBlock.wallDown.texture;
        }
        void MeshVoxel() {
            meshFilter.sharedMesh = typeVoxel.GetMesh();

            meshRenderer.materials = new Material[1] { 
                materialVoxels
            };

            meshRenderer.materials[0].mainTexture = typeVoxel.GetTexture();
        }
        void MeshLiquid() {

            meshFilter.sharedMesh = typeLiquid.GetMesh(true, true, true, true, true, true, (int)RedactorBlocksFormTLiquid.main.sliderExampleVolumeMax.slider.value, (int)RedactorBlocksFormTLiquid.main.sliderExampleVolumeMin.slider.value);
            meshRenderer.materials = new Material[6] {
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock,
                materialBlock
            };

            long textureNum = (long)(Time.time / (1.0f/ typeLiquid.data.animSpeed));

            int tickNum = (int)(textureNum % typeLiquid.data.texturesMax);

            meshRenderer.materials[0].mainTexture = typeLiquid.GetTexture(tickNum, Side.face);
            meshRenderer.materials[1].mainTexture = typeLiquid.GetTexture(tickNum, Side.back);
            meshRenderer.materials[2].mainTexture = typeLiquid.GetTexture(tickNum, Side.left);
            meshRenderer.materials[3].mainTexture = typeLiquid.GetTexture(tickNum, Side.right);
            meshRenderer.materials[4].mainTexture = typeLiquid.GetTexture(tickNum, Side.down);
            meshRenderer.materials[5].mainTexture = typeLiquid.GetTexture(tickNum, Side.up);
        }
    }

    //����������� �� ��� ����� ������
    void CameraRenderTexture()
    {
        if (renderTexture != null) {
            if (renderTexture.width != Screen.width ||
                renderTexture.height != Screen.height) {
                renderTexture = null;
            }
        }

        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 1);
            renderTexture.filterMode = FilterMode.Point;
            camera.targetTexture = renderTexture;

            visualizator1.texture = renderTexture;
        }

        if (renderTexture2 != null) {
            if (renderTexture2.width != Screen.width ||
                renderTexture2.height != Screen.height) {
                renderTexture2 = null;            
            }
        }
        if (renderTexture2 == null) {
            renderTexture2 = new RenderTexture(Screen.width, Screen.height, 1);
            renderTexture2.filterMode = FilterMode.Point;
            camera2.targetTexture = renderTexture2;

            visualizator2.texture = renderTexture2;
        }
    }

    void UpdateCameraRotate() {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetKey(KeyCode.Mouse1))
                isRotate = true;
        }

        //������ �������� ���� ���������
        if (!Input.GetKey(KeyCode.Mouse1)) {
            isRotate = false;
        }

        //������������ �� ��������� ����
        if (isRotateTo) {
            if (isRotate)
            {
                isRotateTo = false;
            }
            else {
                //�������� ��� �� �������
                cameraRotateObj.eulerAngles = Vector3.Lerp(cameraRotateObj.eulerAngles, targetRot, Time.unscaledDeltaTime * 2);
            }
        }

        //���� ����� ������� ��������� ��������� ���� �� ������
        if (isRotate)
        {
            Rotate();
        }


        if (Input.GetKey(KeyCode.Mouse0))
            return;

        Zoom();


        void Rotate()
        {
            float xRot = Input.GetAxis("Mouse X");
            float yRot = Input.GetAxis("Mouse Y");

            float rotX = cameraRotateObj.eulerAngles.x - yRot * 2;
            float rotY = cameraRotateObj.eulerAngles.y + xRot * 2;

            if (rotX < 180 && rotX > 70)
                rotX = 70;
            else if (rotX > 180 && rotX < 290)
                rotX = 290;

            cameraRotateObj.eulerAngles = new Vector3(rotX, rotY, 0);
        }
        void Zoom()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll == 0)
                return;

            if (scroll > 0.01f || scroll < -0.01f)
            {
                camera.fieldOfView -= scroll;
                camera2.fieldOfView = camera.fieldOfView;
            }

            if (camera.fieldOfView > 25) {
                camera.orthographic = false;
                camera2.orthographic = false;
            }

            if (camera.fieldOfView > 60)
            {
                camera.fieldOfView = 60;
                camera2.fieldOfView = 60;
            }
            else if (camera.fieldOfView < 25)
            {
                camera.orthographic = true;
                camera.orthographicSize = 0.5f;
                camera.fieldOfView = 25;

                camera2.orthographic = true;
                camera2.orthographicSize = 0.5f;
                camera2.fieldOfView = 25;
            }
        }
    }


    void CameraSetTargetRot() {
        CameraSetTargetRot(45, 45);
    }
    void CameraSetTargetRot(float xRot, float yRot)
    {
        isRotateTo = true;
        targetRot = new Vector3(xRot, yRot);
    }


    /// Events
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //Output to console the GameObject's name and the following message
        Debug.Log("Cursor Entering " + name);
        mouseOnVisualizator = true;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //Output the following message with the GameObject's name
        Debug.Log("Cursor Exiting " + name);
        mouseOnVisualizator = false;
    }
}
