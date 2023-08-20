using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Cosmos;
using Game.Space;

//Gererate the example biome, use chanks
public class RedactorBiomeGenerator : MonoBehaviour
{
    static private RedactorBiomeGenerator main;
    static public RedactorBiomeGenerator MAIN { get { return main; } }


    [Header("Obj")]
    [SerializeField]
    GameObject chanksParent;
    [SerializeField]
    Camera cameraPlane;
    [SerializeField]
    Camera cameraChanks;


    [SerializeField]
    GameObject chankPrefab;

    [Header("Parameters")]
    [SerializeField]
    float[,] heightMap;
    [SerializeField]
    MeshRenderer PlanetPlane;
    MeshFilter PlanetPlaneFilter;
    [SerializeField]
    MeshRenderer PlanetLiquid;
    MeshFilter PlanetLiquidFilter;
    [SerializeField]
    MeshRenderer PlanetGroundZero;
    MeshFilter PlanetGroundZeroFilter;

    PlanetData planetData;
    PlanetGO planetGO;

    [Header("CameraControls")]
    [SerializeField]
    float CameraHeight = 10;
    Vector3 CameraPosNeed = new Vector3();
    float CamerHeightLast = 0;

    Vector3Int ChankPosGen = new Vector3Int(); 

    static public Vector3 cameraPositionNeed { get {
            Vector3 positionNeed = main.CameraPosNeed;
            positionNeed.y = main.CamerHeightLast;
            return positionNeed; } }

    Quaternion CameraRotNeed = new Quaternion();
    float CameraSpeed = 0;
    float CameraHeightMin = 10;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
        iniCamera();
        IniPlanetVisualPlane();
    }
    // Update is called once per frame
    void Update()
    {
        TestInput();

        Generate();
        TestCamera();
        TestCameraMove();

        TestClose();
    }

    static public RenderTexture GetRender() {

        if (!main || !main.cameraPlane)
            return null;

        return main.cameraPlane.targetTexture;
    }

    void iniCamera()
    {
        cameraPlane = gameObject.GetComponentInChildren<Camera>();
    }

    void IniPlanetVisualPlane() {
        iniPlanetPlane();
        iniPlanetLiquid();
        iniPlanetZeroGround();

        void iniPlanetPlane()
        {
            if (PlanetPlane == null)
                return;

            PlanetPlaneFilter = PlanetPlane.GetComponent<MeshFilter>();
        }


        void iniPlanetLiquid()
        {
            if (PlanetLiquid == null)
                return;

            PlanetLiquidFilter = PlanetLiquid.GetComponent<MeshFilter>();
        }

        void iniPlanetZeroGround()
        {
            if (PlanetGroundZero == null)
                return;

            PlanetGroundZeroFilter = PlanetGroundZero.GetComponent<MeshFilter>();
        }
    }

    void TestCamera() {
        if (cameraPlane == null)
            return;

        //проинициалзировать камеру
        if (cameraPlane.targetTexture != null && cameraPlane.targetTexture.width != Screen.width && cameraPlane.targetTexture.height != Screen.height) {
            cameraPlane.targetTexture = null;
        }

        cameraPlane.targetTexture ??= new RenderTexture(Screen.width, Screen.height, 32);
    }

    static public void TestOpen() {
        //открыть генератор биомов
        if (main == null)
            return;

        main.gameObject.SetActive(true);

    }
    void TestClose() {
        //≈сли меню нет или оно не активно
        if (RedactorBiomeCTRL.main == null ||
            !RedactorBiomeCTRL.main.gameObject.activeInHierarchy) {
            //закрываем генератор
            gameObject.SetActive(false);
        }
        
    }

    static public void SetHeightMap(float[,] heightMap, PlanetData planetData) {
        main.heightMap = heightMap;
        main.planetData = planetData;
    }

    void Generate() {

        if (planetData == null)
            return;

        RandomizeHeightMap();

        ReGeneratePlanetPlane();
        ReGeneratePlanetLiquid();
        ReGeneratePlanetGroundZero();

        VisualizeChanksSurface();

        void ReGeneratePlanetPlane() {
            if (!PlanetPlane || !PlanetPlaneFilter || planetData == null)
                return;

            Mesh mesh = PlanetPlaneFilter.mesh;
            mesh ??= new Mesh();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();
            mesh.uv = GetUV();

            mesh.RecalculateNormals();

            PlanetPlaneFilter.mesh = mesh;

            SetTexture();

            int SizePlanet = Calc.GetSizeInt(planetData.size);
            float scale = SizePlanet / Chank.Size;
            PlanetPlane.gameObject.transform.localScale = new Vector3(scale, SizePlanet/2, scale);

            Vector3[] GetVertices() {
                List<Vector3> vertices = new List<Vector3>();

                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    for (int x = 0; x < heightMap.GetLength(0); x++)
                    {
                        vertices.Add(new Vector3(x, heightMap[x, y], y));
                    }
                }

                return vertices.ToArray();
            }
            Vector2[] GetUV() {
                List<Vector2> UVs = new List<Vector2>();

                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    float coofY = (float)y / heightMap.GetLength(1);
                    for (int x = 0; x < heightMap.GetLength(0); x++)
                    {
                        float coofX = (float)x / heightMap.GetLength(0);
                        UVs.Add(new Vector2(coofX, coofY));
                    }
                }

                return UVs.ToArray();
            }
            int[] GetTriangles() {
                List<int> triangles = new List<int>();

                for (int x = 0; x < heightMap.GetLength(0) - 1; x++) {
                    for (int y = 0; y < heightMap.GetLength(1) - 1; y++) {

                        int smeshenieX = heightMap.GetLength(0);

                        int vert00 = y * smeshenieX + x;
                        int vert01 = vert00 + smeshenieX;
                        int vert11 = vert00 + smeshenieX + 1;
                        int vert10 = vert00 + 1;

                        triangles.Add(vert00);
                        triangles.Add(vert01);
                        triangles.Add(vert11);

                        triangles.Add(vert11);
                        triangles.Add(vert10);
                        triangles.Add(vert00);

                    }
                }
                
                return triangles.ToArray();
            }


            void SetTexture(){
                PlanetPlane.material.mainTexture = planetData.GetMainTexture(Calc.GetSize(Chank.Size));
            }
        }
        void ReGeneratePlanetLiquid() {
            if (!PlanetLiquid || !PlanetLiquidFilter)
                return;

            Mesh mesh = new Mesh();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();

            mesh.RecalculateNormals();

            PlanetLiquidFilter.mesh = mesh;

            float scale = Calc.GetSizeInt(planetData.size) / Chank.Size;
            PlanetLiquid.gameObject.transform.localPosition = new Vector3(0, 0.5f, 0);

            Vector3[] GetVertices()
            {
                List<Vector3> vertices = new List<Vector3>();

                int xMax = heightMap.GetLength(0);
                int yMax = heightMap.GetLength(1);

                vertices.Add(new Vector3(0, 0, 0));
                vertices.Add(new Vector3(0, 0, yMax));
                vertices.Add(new Vector3(xMax, 0, yMax));
                vertices.Add(new Vector3(xMax, 0, 0));

                return vertices.ToArray();
            }
            int[] GetTriangles()
            {
                List<int> triangles = new List<int>();

                int vert00 = 0;
                int vert01 = 1;
                int vert11 = 2;
                int vert10 = 3;

                triangles.Add(vert00);
                triangles.Add(vert01);
                triangles.Add(vert11);

                triangles.Add(vert11);
                triangles.Add(vert10);
                triangles.Add(vert00);

                return triangles.ToArray();
            }
        }
        void ReGeneratePlanetGroundZero()
        {
            if (!PlanetGroundZero || !PlanetGroundZeroFilter)
                return;

            Mesh mesh = new Mesh();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();

            mesh.RecalculateNormals();

            PlanetGroundZeroFilter.mesh = mesh;

            float scale = Calc.GetSizeInt(planetData.size) / Chank.Size;
            PlanetGroundZero.gameObject.transform.localPosition = new Vector3(0, 0.0f, 0);

            Vector3[] GetVertices()
            {
                List<Vector3> vertices = new List<Vector3>();

                int xMax = heightMap.GetLength(0);
                int yMax = heightMap.GetLength(1);

                vertices.Add(new Vector3(0, 0, 0));
                vertices.Add(new Vector3(0, 0, yMax));
                vertices.Add(new Vector3(xMax, 0, yMax));
                vertices.Add(new Vector3(xMax, 0, 0));

                return vertices.ToArray();
            }
            int[] GetTriangles()
            {
                List<int> triangles = new List<int>();

                int vert00 = 0;
                int vert01 = 1;
                int vert11 = 2;
                int vert10 = 3;

                triangles.Add(vert00);
                triangles.Add(vert01);
                triangles.Add(vert11);

                triangles.Add(vert11);
                triangles.Add(vert10);
                triangles.Add(vert00);

                return triangles.ToArray();
            }
        }

        void RandomizeHeightMap() {
            if (heightMap != null)
                return;

            heightMap = new float[100, 100];

            for (int x = 0; x < heightMap.GetLength(0); x++) {
                for (int y = 0; y < heightMap.GetLength(1); y++) {
                    heightMap[x, y] = UnityEngine.Random.Range(0.0f, 5.0f);
                }
            }

        }

        void VisualizeChanksSurface() {

            Size sizeVisualize = Size.s1;
            int sizeVisualizeInt = Calc.GetSizeInt(sizeVisualize);
            int chankSize = Chank.Size * sizeVisualizeInt;

            int SizePlanet = Calc.GetSizeInt(planetData.size);

            ChankPosGen.x = (int)(CameraPosNeed.x / chankSize);
            ChankPosGen.z = (int)(CameraPosNeed.z / chankSize);

            //высоту необходимо узнать через карту высот
            //ѕолучаем карту высот текущего чанка
            HeightMap heightMapChank = planetData.GetHeightMap(sizeVisualize, new Vector2Int(ChankPosGen.x, ChankPosGen.z));

            ChankPosGen.y = (int)(heightMapChank.MeanValue * SizePlanet * 0.5f / chankSize + 1);

            planetData.GetChank(sizeVisualize, ChankPosGen);

            if (planetGO == null)
            {
                planetGO = PlanetGO.GetPlanetGO();
                planetGO.transform.SetParent(transform);
                planetGO.Inizialize(planetData, true);
            }
        }
    }

    void TestInput() {

        if (planetData == null)
            return;

        bool moving = false;

        Vector3 MoveVec = new Vector3();

        //Forvard
        if (Input.GetKey(KeyCode.W)) {
            moving = true;

            MoveVec += cameraPlane.transform.forward;
        }
        //Back
        if (Input.GetKey(KeyCode.S)) {
            moving = true;

            MoveVec -= cameraPlane.transform.forward;
        }
        //Left
        if (Input.GetKey(KeyCode.A)) {
            moving = true;

            MoveVec -= cameraPlane.transform.right;
        }
        //Right
        if (Input.GetKey(KeyCode.D)) {
            moving = true;

            MoveVec += cameraPlane.transform.right;
        }

        if (moving)
        {
            CameraSpeed += CameraSpeed * Time.unscaledDeltaTime + Time.unscaledDeltaTime;

            MoveVec.y = 0;
            MoveVec.Normalize();

            CameraPosNeed += MoveVec * CameraSpeed;
        }
        else {
            CameraSpeed = 0;
        }

        float planetSize = Calc.GetSizeInt(planetData.size);
        float CameraHeightMax = planetSize;

        if (Input.mouseScrollDelta.y != 0) {
            CameraHeight += Input.mouseScrollDelta.y * CameraHeight * 0.1f;
            if (CameraHeight < CameraHeightMin)
                CameraHeight = CameraHeightMin;
            else if (CameraHeight > CameraHeightMax)
                CameraHeight = CameraHeightMax;
        }

        //ѕроверка границ
        if (CameraPosNeed.x < 0)
            CameraPosNeed.x = 0;
        else if (CameraPosNeed.x > (heightMap.GetLength(0) -1) * Chank.Size * (planetSize / 1024))
            CameraPosNeed.x = (heightMap.GetLength(0) -1) * Chank.Size * (planetSize / 1024);

        if (CameraPosNeed.z < 0)
            CameraPosNeed.z = 0;
        else if (CameraPosNeed.z > (heightMap.GetLength(1) -1) * Chank.Size * (planetSize/1024))
            CameraPosNeed.z = (heightMap.GetLength(1) -1) * Chank.Size * (planetSize / 1024);

        //ѕроверка требуемой высоты
        Vector2Int indexHeightMap = new Vector2Int((int)CameraPosNeed.x, (int)CameraPosNeed.z)/Chank.Size/ (int)(planetSize / 1024);

        //ѕроверка высоты с соседними €чейками
        int distTest = 1;
        CamerHeightLast = 0;

        for (int x = -distTest; x <= distTest; x++) {
            int xNow = indexHeightMap.x + x;
            if (xNow < 0 || xNow >= heightMap.GetLength(0))
                continue;
            for (int y = -distTest; y <= distTest; y++) {
                int yNow = indexHeightMap.y + y;
                if (yNow < 0 || yNow >= heightMap.GetLength(1))
                    continue;

                float heightThis = heightMap[xNow, yNow];
                if (CamerHeightLast < heightThis)
                    CamerHeightLast = heightThis;
            }
        }


        CamerHeightLast = (CamerHeightLast * planetSize) / 2 + CameraHeight;
        CameraPosNeed.y = CamerHeightLast;
    }

    /// <summary>
    /// ѕринимает смещение мыши и прибавл€ет к текущему вращению камеры
    /// </summary>
    /// <param name="vecRot"></param>
    static public void SetRotate(Vector2 vecRot) {
        //¬ытаскиваем вращение камеры
        Quaternion rotNow = main.cameraPlane.transform.localRotation;
        Vector3 rotEuler = rotNow.eulerAngles;
        rotEuler.x += vecRot.y;
        rotEuler.y += vecRot.x;

        //ќграничиваем по верх низ
        if (rotEuler.x > 90 && rotEuler.x < 180)
            rotEuler.x = 90;
        else if (rotEuler.x < -90 && rotEuler.x > -180 ||
            rotEuler.x > 180 && rotEuler.x < 270)
            rotEuler.x = -90;

        rotNow.eulerAngles = rotEuler;
        main.cameraPlane.transform.localRotation = rotNow;


    }
    void TestCameraMove() {
        cameraPlane.transform.localPosition += (CameraPosNeed - cameraPlane.transform.localPosition) * Time.unscaledDeltaTime * 10;
    }
}
