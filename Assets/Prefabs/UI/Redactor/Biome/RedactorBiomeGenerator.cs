using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Cosmos;

//Gererate the example biome, use chanks
public class RedactorBiomeGenerator : MonoBehaviour
{
    static private RedactorBiomeGenerator main;
    static public RedactorBiomeGenerator MAIN { get { return main; } }


    [Header("Obj")]
    [SerializeField]
    GameObject chanksParent;
    [SerializeField]
    Camera camera;

    [SerializeField]
    GameObject chankPrefab;

    [Header("Parameters")]
    [SerializeField]
    float[,] heightMap;
    Size quarityMap = Size.s1;
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
        Generate();
        TestCamera();

        TestClose();
    }

    static public RenderTexture GetRender() {

        if (!main || !main.camera)
            return null;

        return main.camera.targetTexture;
    }

    void iniCamera()
    {
        camera = gameObject.GetComponentInChildren<Camera>();
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
        if (camera == null)
            return;

        //проинициалзировать камеру
        if (camera.targetTexture != null && camera.targetTexture.width != Screen.width && camera.targetTexture.height != Screen.height) {
            camera.targetTexture = null;
        }

        camera.targetTexture ??= new RenderTexture(Screen.width, Screen.height, 32);
    }

    static public void TestOpen() {
        //открыть генератор биомов
        if (main == null)
            return;

        main.gameObject.SetActive(true);

    }
    void TestClose() {
        //Если меню нет или оно не активно
        if (RedactorBiomeCTRL.main == null ||
            !RedactorBiomeCTRL.main.gameObject.activeInHierarchy) {
            //закрываем генератор
            gameObject.SetActive(false);
        }
        
    }

    static public void SetHeightMap(float[,] heightMap, Size quarity, PlanetData planetData) {
        main.heightMap = heightMap;
        main.quarityMap = quarity;
        main.planetData = planetData;
    }

    void Generate() {

        RandomizeHeightMap();

        ReGeneratePlanetPlane();
        ReGeneratePlanetLiquid();
        ReGeneratePlanetGroundZero();

        void ReGeneratePlanetPlane() {
            if (!PlanetPlane || !PlanetPlaneFilter || planetData == null)
                return;

            Mesh mesh = PlanetPlaneFilter.mesh;
            mesh ??= new Mesh();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();

            mesh.RecalculateNormals();

            PlanetPlaneFilter.mesh = mesh;

            int SizePlanet = Calc.GetSizeInt(planetData.size);
            int SizeMapPixel = Calc.GetSizeInt(quarityMap);
            float scale = SizePlanet / SizeMapPixel;
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
        }
        void ReGeneratePlanetLiquid() {
            if (!PlanetLiquid || !PlanetLiquidFilter)
                return;

            Mesh mesh = new Mesh();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();

            mesh.RecalculateNormals();

            PlanetLiquidFilter.mesh = mesh;

            float scale = Calc.GetSizeInt(quarityMap);
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

            float scale = Calc.GetSizeInt(quarityMap);
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

    }
}
