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

    //Буфер звеньев в цепи
    GalaxyObjBuffer galaxyObjBuffer = new GalaxyObjBuffer();

    //Буффер в котором хранятся основные объекты звездных ячеек
    struct GalaxyObjBuffer {
        const int updateCountSecond = 1000;         //Ошибка!!    //при больших значениях в секунду, перестают обновляться вообще
        int updateCountOld;

        //Буфер звезд
        List<GalaxyObjCtrl> bufferNow;
        List<GalaxyObjCtrl> bufferNew;

        List<GraficData.GalaxyStar> bufferShader;
        int shaderLast;

        bool needRecalcShader;

        //Добавляем новый обьект
        public void Add(GalaxyObjCtrl galaxyObjBuffer) {
            if (bufferNew == null)
                bufferNew = new List<GalaxyObjCtrl>();

            //Добавляем объект в случайное место
            int randNum = Random.Range(0, bufferNew.Count);
            bufferNew.Insert(randNum, galaxyObjBuffer);
        }

        public void UpdateBuffer() {

            //Перебираем буфер чтобы убрать пустоты
            int maxTest = (int)(updateCountSecond * Time.unscaledDeltaTime);

            //Перебираем некоторое количество объектов
            for (int plus = 0; plus < maxTest; plus++) {
                //находим номер обновляемого объекта
                int xNow = updateCountOld + plus;


                //Если буфер закончился
                if (bufferNow == null || //Если буфера вообще нет
                    bufferNow.Count == 0 || //Если внутри бувера ничего нет
                    bufferNow.Count != 0 && xNow / bufferNow.Count >= 1) //Если буфер прсто закончился
                {
                    updateCountOld = 0; //Сбрасываем счетчик

                    //Если изменилось количество объектов
                    if (bufferNew == null || bufferNow == null || bufferNow.Count != bufferNew.Count)
                        needRecalcShader = true;

                    bufferNow = bufferNew;

                    //Если зафиксированно изменение обнуляем шейдер
                    if (needRecalcShader)
                    {
                        needRecalcShader = false;

                        DisposeShader();
                        bufferShader = new List<GraficData.GalaxyStar>();
                    }

                    bufferNew = new List<GalaxyObjCtrl>();
                    break;
                }
                //иначе Если последняя проверка в этом кадре
                else if (plus == maxTest - 1) {
                    //Запоминаем последнюю позицию
                    updateCountOld = xNow;
                }

                //если в буфере ничего нет - выходим
                if (bufferNow.Count == 0)
                    continue;

                //Получаем номер буфера
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

            //Узнаем максимальное количество шейдеров
            int bufferShaderMax = bufferNow.Count / GraficData.GalaxyStar.arrayCount;
            if (bufferNow.Count % GraficData.GalaxyStar.arrayCount > 0) 
                bufferShaderMax++;

            //Если буфера вообще нет
            if (bufferShader == null)
                bufferShader = new List<GraficData.GalaxyStar>();

            //Если буфер шейдра меньше
            if (bufferShader.Count < bufferShaderMax) {
                int start = GraficData.GalaxyStar.arrayCount * bufferShader.Count;

                GraficData.GalaxyStar dataGalaxyStar;
                //Создаем новый дата шейдер
                if (Gameplay.main == null)
                {
                    //Меню
                    dataGalaxyStar = new GraficData.GalaxyStar(CameraGalaxy.main.transform.localPosition, Time.time);
                }
                else
                {
                    //Игра
                    dataGalaxyStar = new GraficData.GalaxyStar(CameraGalaxy.main.transform.localPosition, Gameplay.main.timeWorld);
                }

                for (int offset = 0; offset < GraficData.GalaxyStar.arrayCount; offset++) {
                    int starNum = start + offset;
                    if (starNum >= bufferNow.Count) {
                        break;
                    }

                    dataGalaxyStar.SetData(offset, bufferNow[starNum]) ;
                }

                //Буфер заполнен данными
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

            //Очистить буфер шейдера
            DisposeShader();

            bufferShader = null;
        }

        public void DisposeShader() {
            if (bufferShader == null) 
                return;

            //Очистить буфер шейдера
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

        //Подчистить шейдер
        main.galaxyObjBuffer.Clear();

        main.galaxyObjBuffer = new GalaxyObjBuffer();
        main.isGenCellComplite = false;
        main.GenCellNow = 0;

        main.xNow = 0;
        main.yNow = 0;
        main.zNow = 0;
    }

    //Сколько ячеек галактики сгенерированно
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

        //Сгенерировать планеты ближайщих ячеек //Данные
        GenPlanetsData();
    }

    //Здесь создаются объекты галактики, только спавн, без обновления
    void GenVisual() {
        if (isGenCellComplite) 
            return;

        //Находим позицию ячейки с которой надо продолжить генерировать
        //int maxYZ = galaxy.cells.GetLength(1) * galaxy.cells.GetLength(2);
        int startX = xNow; //(int)(GenCellNow / maxYZ);
        //int nowYZ = (int)(GenCellNow % maxYZ);
        int startY = yNow; // nowYZ / galaxy.cells.GetLength(1);
        int startZ = zNow; //nowYZ % galaxy.cells.GetLength(2);



        //Количество ячеек которое можно обработать за кадр //Это только генерация, которая должна быстро произойти в самом начале игры
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

                    //Достигнут лимит расчета. сохраняем выходим
                    if (cellNow >= cellmax) {
                        isMaximum = true;
                        //GenCellNow = 0;
                        //GenCellNow += (uint)(x * maxYZ);
                        //GenCellNow += (uint)(y * galaxy.cells.GetLength(2));
                        //GenCellNow += (uint)(z);
                        break;
                    }

                    //создаем визуальную часть если нет
                    if (galaxy.cells[x, y, z].mainObjs != null &&
                        galaxy.cells[x, y, z].mainObjs.visual == null)
                    {
                        SpawnGalaxyObj(galaxy.cells[x, y, z].mainObjs);
                        galaxyObjBuffer.Add(galaxy.cells[x, y, z].visual);
                        //spaceObjBuffer.Add(Gameplay.main.galaxy.cells[x, y, z].mainObjs.cell.visual);
                    }

                    //Если это последняя ячейка
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

    //Генерация планет на основе положений игроков
    void GenPlanetsData() {
        //Находим положение для обязателной генерации
        Vector3 genPosLocal;
        if (PlayerCTRL.local != null)
        {
            genPosLocal = PlayerCTRL.local.NumCell + (PlayerCTRL.local.PosInCell/CellS.size);
        }
        else {
            genPosLocal = SpaceCameraControl.main.spacePosCell;
        }


        //Сперва генерируем для локального игрока
        GenForPos(genPosLocal);

        //Выходим
        if (PlayerCTRL.local == null || //Если локального игрока нет, генерация в меню игры 
            !PlayerCTRL.local.isServer) //Или это сервер
            return;

        //Далее сервер генерирует для остальных игроков
        foreach (PlayerCTRL player in PlayerCTRL.players) {
            if (player == null)
                continue;

            //Получаем позицию игроков для генерации
            Vector3 genPos = player.NumCell + player.PosInCell / CellS.size;
            GenForPos(genPos);
        }

        void GenForPos(Vector3 position) {
            if (position.x < 0 || position.x > galaxy.cells.GetLength(0) - 1 ||
                position.y < 0 || position.y > galaxy.cells.GetLength(1) - 1 ||
                position.z < 0 || position.z > galaxy.cells.GetLength(2) - 1)
                return;

            //Генерируем планеты в звездной ячейке
            galaxy.cells[(int)position.x, (int)position.y, (int)position.z].genPlanets();
        }
    }

    //Создаем космический объект на основе его данных
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

    //Очистить буфер галактики
    void OnApplicationQuit() {
        GalaxyClear();
    }
}
