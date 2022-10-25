using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyCtrl : MonoBehaviour
{
    static public GalaxyCtrl main;

    static public Galaxy galaxy;

    [Header("Visual")]
    [SerializeField]
    GalaxyObjCtrl GalaxyObjPrefab;


    Vector3Int posCamOld = new Vector3Int(-1, -1, -1);
    Vector3Int bufferCalc = new Vector3Int();

    //����� ������� � ����
    GalaxyObjBuffer galaxyObjBuffer = new GalaxyObjBuffer();

    //������ � ������� �������� �������� ������� �������� �����
    struct GalaxyObjBuffer {
        const int updateCountSecond = 1000;         //������!!    //��� ������� ��������� � �������, ��������� ����������� ������
        int updateCountOld;

        //����� �����
        List<GalaxyObjCtrl> bufferNow;
        List<GalaxyObjCtrl> bufferNew;

        List<GraficData.GalaxyStar> bufferShader;
        int shaderLast;

        bool needRecalcShader;

        //��������� ����� ������
        public void Add(GalaxyObjCtrl galaxyObjBuffer) {
            if (bufferNew == null)
                bufferNew = new List<GalaxyObjCtrl>();

            //��������� ������ � ��������� �����
            int randNum = Random.Range(0, bufferNew.Count);
            bufferNew.Insert(randNum, galaxyObjBuffer);
        }

        public void UpdateBuffer() {

            //���������� ����� ����� ������ �������
            int maxTest = (int)(updateCountSecond * Time.unscaledDeltaTime);

            //���������� ��������� ���������� ��������
            for (int plus = 0; plus < maxTest; plus++) {
                //������� ����� ������������ �������
                int xNow = updateCountOld + plus;


                //���� ����� ����������
                if (bufferNow == null || //���� ������ ������ ���
                    bufferNow.Count == 0 || //���� ������ ������ ������ ���
                    bufferNow.Count != 0 && xNow / bufferNow.Count >= 1) //���� ����� ����� ����������
                {
                    updateCountOld = 0; //���������� �������

                    //���� ���������� ���������� ��������
                    if (bufferNew == null || bufferNow == null || bufferNow.Count != bufferNew.Count)
                        needRecalcShader = true;

                    bufferNow = bufferNew;

                    //���� �������������� ��������� �������� ������
                    if (needRecalcShader)
                    {
                        needRecalcShader = false;

                        DisposeShader();
                        bufferShader = new List<GraficData.GalaxyStar>();
                    }

                    bufferNew = new List<GalaxyObjCtrl>();
                    break;
                }
                //����� ���� ��������� �������� � ���� �����
                else if (plus == maxTest - 1) {
                    //���������� ��������� �������
                    updateCountOld = xNow;
                }

                //���� � ������ ������ ��� - �������
                if (bufferNow.Count == 0)
                    continue;

                //�������� ����� ������
                xNow %= bufferNow.Count;


                if (bufferNow[xNow] == null)
                {
                    needRecalcShader = true;
                    continue;
                }

                bufferNew.Add(bufferNow[xNow]);
            }
        }
        public void UpdateShader() {
            
            if (bufferNow == null ||
                bufferNow.Count == 0)
                return;

            //������ ������������ ���������� ��������
            int bufferShaderMax = bufferNow.Count / GraficData.GalaxyStar.arrayCount;
            if (bufferNow.Count % GraficData.GalaxyStar.arrayCount > 0) 
                bufferShaderMax++;

            //���� ������ ������ ���
            if (bufferShader == null)
                bufferShader = new List<GraficData.GalaxyStar>();

            //���� ����� ������ ������
            if (bufferShader.Count < bufferShaderMax) {
                int start = GraficData.GalaxyStar.arrayCount * bufferShader.Count;

                GraficData.GalaxyStar dataGalaxyStar;
                //������� ����� ���� ������
                if (Gameplay.main == null)
                {
                    //����
                    dataGalaxyStar = new GraficData.GalaxyStar(CameraGalaxy.main.transform.localPosition, Time.time);
                }
                else
                {
                    //����
                    dataGalaxyStar = new GraficData.GalaxyStar(CameraGalaxy.main.transform.localPosition, Gameplay.main.timeWorld);
                }

                for (int offset = 0; offset < GraficData.GalaxyStar.arrayCount; offset++) {
                    int starNum = start + offset;
                    if (starNum >= bufferNow.Count) {
                        break;
                    }

                    dataGalaxyStar.SetData(offset, bufferNow[starNum]) ;
                }

                //����� �������� �������
                bufferShader.Add(dataGalaxyStar);
            }

            if (shaderLast >= bufferShader.Count)
                shaderLast = 0;

            GraficGalaxyStar.main.calculate(bufferShader[shaderLast]);
            shaderLast++;

        }

        public void Clear() {
            if (bufferNow == null) 
                return;

            foreach (GalaxyObjCtrl obj in bufferNow) {
                Destroy(obj.gameObject);
            }

            bufferNow = null;
            bufferNew = null;

            //�������� ����� �������
            DisposeShader();

            bufferShader = null;
        }

        public void DisposeShader() {
            if (bufferShader == null) 
                return;

            //�������� ����� �������
            foreach (GraficData.GalaxyStar galaxyStar in bufferShader)
            {
                galaxyStar.Dispose();
            }
        }
    }


    public static void ClearBuffer() {
        GalaxyObjCtrl[] galaxyObjs = main.GetComponentsInChildren<GalaxyObjCtrl>();
        foreach (GalaxyObjCtrl galaxyObj in galaxyObjs) {
            Destroy(galaxyObj.gameObject);
        }

        //���������� ������
        main.galaxyObjBuffer.Clear();

        main.galaxyObjBuffer = new GalaxyObjBuffer();
        main.isGenCellComplite = false;
        main.GenCellNow = 0;

        main.xNow = 0;
        main.yNow = 0;
        main.zNow = 0;
    }

    //������� ����� ��������� ��������������
    bool isGenCellComplite = false;
    uint GenCellNow = 0;
    int xNow = 0;
    int yNow = 0;
    int zNow = 0;

    // Start is called before the first frame update
    void Start()
    {
        main = this;

        new Galaxy(Galaxy.Size.cells61, "TestGalaxy");

    }

    // Update is called once per frame
    void Update()
    {
        //spaceObjBuffer.UpdateBuff();
        galaxyObjBuffer.UpdateBuffer();
        galaxyObjBuffer.UpdateShader();
        GenVisual();

        //������������� ������� ��������� ����� //������
        GenPlanetsData();
    }

    //����� ��������� ������� ���������, ������ �����, ��� ����������
    void GenVisual() {
        if (isGenCellComplite) 
            return;

        //������� ������� ������ � ������� ���� ���������� ������������
        //int maxYZ = galaxy.cells.GetLength(1) * galaxy.cells.GetLength(2);
        int startX = xNow; //(int)(GenCellNow / maxYZ);
        //int nowYZ = (int)(GenCellNow % maxYZ);
        int startY = yNow; // nowYZ / galaxy.cells.GetLength(1);
        int startZ = zNow; //nowYZ % galaxy.cells.GetLength(2);



        //���������� ����� ������� ����� ���������� �� ���� //��� ������ ���������, ������� ������ ������ ��������� � ����� ������ ����
        int cellmax = (int)(10000 * Time.unscaledDeltaTime);
        int cellNow = 0;
        bool isMaximum = false;

        int lenghtX = galaxy.cells.GetLength(0);
        int lenghtY = galaxy.cells.GetLength(1);
        int lenghtZ = galaxy.cells.GetLength(2);

        for (int x = startX; x < lenghtX && !isMaximum; x++) {
            xNow = x;
            for (int y = startY; y < lenghtY && !isMaximum; y++) {
                yNow = y;
                startY = 0;
                for (int z = startZ; z < lenghtZ && !isMaximum; z++) {
                    cellNow++;

                    zNow = z;
                    startZ = 0;

                    //��������� ����� �������. ��������� �������
                    if (cellNow >= cellmax) {
                        isMaximum = true;
                        //GenCellNow = 0;
                        //GenCellNow += (uint)(x * maxYZ);
                        //GenCellNow += (uint)(y * galaxy.cells.GetLength(2));
                        //GenCellNow += (uint)(z);
                        break;
                    }

                    //������� ���������� ����� ���� ���
                    if (galaxy.cells[x, y, z].mainObjs != null &&
                        galaxy.cells[x, y, z].mainObjs.visual == null)
                    {
                        SpawnGalaxyObj(galaxy.cells[x, y, z].mainObjs);
                        galaxyObjBuffer.Add(galaxy.cells[x, y, z].visual);
                        //spaceObjBuffer.Add(Gameplay.main.galaxy.cells[x, y, z].mainObjs.cell.visual);
                    }

                    //���� ��� ��������� ������
                    if (x == lenghtX - 1 &&
                        y == lenghtY - 1 &&
                        z == lenghtZ - 1) {
                        isMaximum = true;
                        isGenCellComplite = true;

                        Debug.Log("Galaxy gen complite");

                        xNow = 0;
                        yNow = 0;
                        zNow = 0;

                        break;
                    }


                }
            }
        }
    }

    //��������� ������ �� ������ ��������� �������
    void GenPlanetsData() {
        //������� ��������� ��� ����������� ���������
        Vector3 genPosLocal;
        if (PlayerCTRL.local != null)
        {
            genPosLocal = PlayerCTRL.local.NumCell + (PlayerCTRL.local.PosInCell/CellS.size);
        }
        else {
            genPosLocal = SpaceCameraControl.main.spacePosCell;
        }


        //������ ���������� ��� ���������� ������
        GenForPos(genPosLocal);

        //�������
        if (PlayerCTRL.local == null || //���� ���������� ������ ���, ��������� � ���� ���� 
            !PlayerCTRL.local.isServer) //��� ��� ������
            return;

        //����� ������ ���������� ��� ��������� �������
        foreach (PlayerCTRL player in PlayerCTRL.players) {
            if (player == null)
                continue;

            //�������� ������� ������� ��� ���������
            Vector3 genPos = player.NumCell + player.PosInCell / CellS.size;
            GenForPos(genPos);
        }

        void GenForPos(Vector3 position) {
            if (position.x < 0 || position.x > galaxy.cells.GetLength(0) - 1 ||
                position.y < 0 || position.y > galaxy.cells.GetLength(1) - 1 ||
                position.z < 0 || position.z > galaxy.cells.GetLength(2) - 1)
                return;

            //���������� ������� � �������� ������
            galaxy.cells[(int)position.x, (int)position.y, (int)position.z].genPlanets();
        }
    }

    //������� ����������� ������ �� ������ ��� ������
    void SpawnGalaxyObj(SpaceObjData myData) {

        GameObject SpaceObj = Instantiate(GalaxyObjPrefab.gameObject, transform);
        GalaxyObjCtrl spaceObjCtrl = SpaceObj.GetComponent<GalaxyObjCtrl>();
        
        spaceObjCtrl.Ini(myData);

    }

    public void GalaxyClear() {

        isGenCellComplite = false;
        GenCellNow = 0;

        galaxyObjBuffer.Clear();
        galaxyObjBuffer = new GalaxyObjBuffer();

    }

    //�������� ����� ���������
    void OnApplicationQuit() {
        GalaxyClear();
    }
}
