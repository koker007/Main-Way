using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;


//Игрок на сервере
public class PlayerCTRL : NetworkBehaviour
{
    public static List<PlayerCTRL> players = new List<PlayerCTRL>(); //Список всех игроков на сервере
    public static PlayerCTRL local; //Текуший локальный игрок

    PlayerDataOwner myData;

    [SyncVar] public ulong steamID = 0;
    [SyncVar] public string name = "noName";

    [SyncVar] public int health = 100;
    [SyncVar] public int healthMax = 100;
    [SyncVar] public int oxygen = 100;

    [SyncVar] public int ItemSelect = 0;

    [SyncVar] Vector3Int numCell = new Vector3Int(); //номер галактической ячейки
    [SyncVar] Vector3 posInCell = new Vector3(); //Положение игрока внутри космической ячейки
    [SyncVar] Vector3Int numZone = new Vector3Int(); //Номер сектора внутри ячейки

    Vector3Int numCellLocal = new Vector3Int();
    Vector3 posInCellLocal = new Vector3();
    Vector3Int numZoneLocal = new Vector3Int(); //Номер сектора внутри ячейки для локального клиента

    public Vector3Int NumCell {
        get {
            if (isLocalPlayer) return numCellLocal;
            else return numCell;
        }
    }
    public Vector3 PosInCell
    {
        get
        {
            if (isLocalPlayer) return posInCellLocal;
            else return posInCell;
        }
    }
    public Vector3Int NumZone {
        get {
            if (isLocalPlayer) return numZoneLocal;
            else return numZone;
        }
    }

    [SyncVar] public string posGlobal = ""; //Общее положение игрока 


    //Transform
    /// <summary>
    /// Стоит ли игрок на какой либо поверхности или болтается как говно в космическом пространстве
    /// </summary>
    [SyncVar] bool downSurface = false;
    /// <summary>
    /// Вектор низа в космическом пространстве, направление гравитации
    /// </summary>
    [SyncVar] Vector3 downInSpace = new Vector3(0,-1,0);

    //Камеры игрока
    [SerializeField]
    public GameObject CamPlayer; //Основная планетарная камера, локальная на игроке

    //Серверные расчеты
    SpaceObjData planetCurrent; //Текущая планета игрока
    public SpaceObjData PlanetCurrent {
        get {
            return planetCurrent;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Inicialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();

        //Серверные проверки растояния
        CalcPosition();

        UpdateRotate();
    }
    private void FixedUpdate()
    {
        testPlayerSteam();
    }

    //Если это локальный игрок
    void Inicialize() {


        if (!isLocalPlayer)
        {
            Destroy(CamPlayer);
            return;
        }

        //Текущий игрока локальный запоминаем
        local = this;

        //Камера
        CamPlayer.SetActive(true);

        //текущий игрок передается камерам
        CameraGalaxy.SetLocalPlayer(this);
        CameraSpaceCell.SetLocalPlayer(this);
    }

    //Возвращяет глобальный космический взгляд игрока
    Quaternion rotateSpaceView;
    float timeCalcSpaceViewLast = 0;
    public Quaternion GetSpaceView() {

        //Если в текущем кадре уже расчитывали взгял игрока
        if (timeCalcSpaceViewLast == Time.unscaledTime) {
            return rotateSpaceView; //Возвращяем расчитанный взгляд
        }

        //Нужно считать новый взгяд
        timeCalcSpaceViewLast = Time.unscaledTime; //Запоминаем время расчета

        //Если игрок в космосе
        if (planetCurrent == null) {
            //Просто возвращяем тот же угл как у планетарной локальной камеры
            rotateSpaceView = CamPlayer.transform.rotation;
            return rotateSpaceView;
        }

        //Иначе игрок на планете

        return rotateSpaceView;
    }

    void UpdateTransform() {
        
    }

    [Command]
    public void CmdSetName(string playerName, ulong cSteamID)
    {
        name = playerName;
        steamID = cSteamID;
    }
    void testPlayerSteam() {
        if (!isLocalPlayer) 
            return;

        string steamName = SteamFriends.GetPersonaName();
        CSteamID cSteamID = SteamUser.GetSteamID();

        if (name == steamName && cSteamID.m_SteamID == steamID)
            return;

        CmdSetName(steamName, cSteamID.m_SteamID);
    }
    
    /// <summary>
    /// Установить текушую планету игрока
    /// </summary>
    /// <param name="spaceObj"></param>
    void ServerSetPlanet(SpaceObjData spaceObj) {
        //только сервер
        if (!isServer)
            return;

        //Присваиваем планету
        planetCurrent = spaceObj;

        //Получаем позицию ячейки
        posGlobal = "X" + (int)numCell.x + "|Y" + (int)numCell.y + "|Z" + (int)numCell.z;

        //Если планеты нет //Выходим
        if (spaceObj == null)
            return;

        //Получаем позицию планеты
        CellS cell = spaceObj.cell;

        //Перебираем все планеты до самой звезды
        SpaceObjData select = spaceObj;
        string planetsInvers = "";
        while (select != null) {
            //если родителя нет - выходим
            if (select.parent == null)
                break;

            //Если родитель есть узнаем номер этого объекта у родителя
            for (int num = 0; num < select.parent.childs.Length; num++) {
                //Нашли текущую планету
                if (select == select.parent.childs[num]) {
                    //прибавляем разделитель и номер
                    if (planetsInvers.Length > 0)
                        planetsInvers += "|";

                    planetsInvers += num;

                    //Переключаемся на родителя
                    select = select.parent;
                    continue;
                }
            }
        }

        //Список планет в инверсии есть
        //Создаем номальный список
        string planets = "";
        string[] planetsMas = planetsInvers.Split("|");

        for (int num = planetsMas.Length - 1; num >= 0; num--) {
            if (planets.Length == 0)
                planets += "P";
            else planets += "M";

            planets += planetsMas[num];
        }

        //Добавляем планеты
        posGlobal += planets;

        //Глобальная позиция готова
    }

    //Перемещение игрока в прстранстве
    void UpdateMove() {
        //Если этот игрок не локальный
        if (!isLocalPlayer)
            return;


        Vector3 move = new Vector3();

        //Меняем положение игрока
        if (Input.GetKey(Controls.keyMoveForward)) {
            move.z += 1;
        }
        if (Input.GetKey(Controls.keyMoveBack)) {
            move.z -= 1;
        }
        if (Input.GetKey(Controls.keyMoveLeft)) {
            move.x -= 1;
        }
        if (Input.GetKey(Controls.keyMoveRight)) {
            move.x += 1;
        }
        if (Input.GetKey(Controls.keyMoveUp)) {
            move.y += 1;
        }
        if (Input.GetKey(Controls.keyMoveDown)) {
            move.y -= 1;
        }
        move.Normalize();

        //Проверка движения
        if (move != new Vector3()) {
            //Debug.Log("Player Move: " + move);
        }

        //Если игрок находится на отталкиваемой поверхности
        if (downSurface) {
        
        }
        //Если игрок не находится на отталкиваемой поверхности
        else
        {

        }

        float speed = 1000000f;
        //Смещение пространстве в текущем кадре
        Vector3 moveOffset = new Vector3(0,0,0);
        //Перемещаем игрока в пространстве
        moveOffset += move.z * CamPlayer.transform.forward * Time.deltaTime * speed;
        moveOffset += move.x * CamPlayer.transform.right * Time.deltaTime * speed;
        moveOffset += move.y * CamPlayer.transform.up * Time.deltaTime * speed;

        if (moveOffset.magnitude > 0) {
            gameObject.transform.position += moveOffset;
        }

        //ВЫчислили позицию игрока в ячейке

        //Нужно поделить ячеку на сектора и смещать игрока локально внутри одного сектора
        //Нужно хранить номер сектора в ячейке

        //Делаем позицию глобальную        
        //posInGalaxy = transform.position / CellS.size;
        //posInCell = transform.position / CellS.sizeVisual;

        

    }

    //Расчет зоны и смещение игрока
    void CalcPosition() {
        if (!isLocalPlayer)
            return;

        //проверяем позицию игрока на выходы за границы
        bool isChange = false;

        //Если вышли за границы зоны локальная
        //По x
        if (gameObject.transform.position.x > CellS.sizeZone) {
            numZoneLocal.x += (int)(transform.position.x / CellS.sizeZone);
            isChange = true;
            gameObject.transform.position = new Vector3(transform.position.x % CellS.sizeZone, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < 0) {
            numZoneLocal.x += (int)(transform.position.x / CellS.sizeZone) - 1;
            isChange = true;
            gameObject.transform.position = new Vector3(CellS.sizeZone + (transform.position.x % CellS.sizeZone), transform.position.y, transform.position.z);
        }

        //По y
        if (gameObject.transform.position.y > CellS.sizeZone) {
            numZoneLocal.y += (int)(transform.position.y / CellS.sizeZone);
            isChange = true;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y % CellS.sizeZone, transform.position.z);
        }
        else if (gameObject.transform.position.y < 0) {
            numZoneLocal.y += (int)(transform.position.y / CellS.sizeZone) - 1;
            isChange = true;
            gameObject.transform.position = new Vector3(transform.position.x, CellS.sizeZone + (transform.position.y % CellS.sizeZone), transform.position.z);
        }

        //По z
        if (gameObject.transform.position.z > CellS.sizeZone) {
            numZoneLocal.z += (int)(transform.position.z / CellS.sizeZone);
            isChange = true;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z % CellS.sizeZone);
        }
        else if (transform.position.z < 0) {
            numZoneLocal.z += (int)(transform.position.z / CellS.sizeZone) - 1;
            isChange = true;
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, CellS.sizeZone + (transform.position.z % CellS.sizeZone));
        }

        //Отправляем на сервер команду о изменении зоны
        //Клиент должен сказать серверу что поменял зону
        if (isChange)
            SetZone(numZoneLocal.x, numZoneLocal.y, numZoneLocal.z);

        //Проверка выхода из космической ячейки
        int maxZonesInCell = CellS.size / CellS.sizeZone;
        //по x
        if (numZoneLocal.x > maxZonesInCell) {
            numCellLocal.x += numZoneLocal.x / maxZonesInCell;
            numZoneLocal.x = numZoneLocal.x % maxZonesInCell;
        }
        else if (numZoneLocal.x < 0) {
            numCellLocal.x += (numZoneLocal.x / maxZonesInCell) - 1;
            numZoneLocal.x = maxZonesInCell + (numZoneLocal.x % maxZonesInCell);
        }

        //по у
        if (numZoneLocal.y > maxZonesInCell)
        {
            numCellLocal.y += numZoneLocal.y / maxZonesInCell;
            numZoneLocal.y = numZoneLocal.y % maxZonesInCell;
        }
        else if (numZoneLocal.y < 0)
        {
            numCellLocal.y += (numZoneLocal.y / maxZonesInCell) - 1;
            numZoneLocal.y = maxZonesInCell + (numZoneLocal.y % maxZonesInCell);
        }
        //по z
        if (numZoneLocal.z > maxZonesInCell)
        {
            numCellLocal.z += numZoneLocal.z / maxZonesInCell;
            numZoneLocal.z = numZoneLocal.z % maxZonesInCell;
        }
        else if (numZoneLocal.z < 0)
        {
            numCellLocal.z += (numZoneLocal.z / maxZonesInCell) - 1;
            numZoneLocal.z = maxZonesInCell + (numZoneLocal.z % maxZonesInCell);
        }

        //Считаем локальное значение в космической ячейке
        posInCellLocal = numZoneLocal * CellS.sizeZone + gameObject.transform.position;

    }

    [Command]
    void SetZone(int zoneX, int zoneY, int zoneZ) {
        //Клиент говорит какая у него игровая зона, сервер послушно ставит
        numZone.x = zoneX;
        numZone.y = zoneY;
        numZone.z = zoneZ;

        //Новая игровая зона
        ServerSetPlanet(null);

        //Проверка выхода из космической ячейки
        int maxZonesInCell = CellS.size / CellS.sizeZone;
        //по x
        if (numZone.x > maxZonesInCell)
        {
            numCell.x += numZone.x / maxZonesInCell;
            numZone.x = numZone.x % maxZonesInCell;
        }
        else if (numZone.x < 0)
        {
            numCell.x += (numZone.x / maxZonesInCell) - 1;
            numZone.x = maxZonesInCell + (numZone.x % maxZonesInCell);
        }

        //по у
        if (numZone.y > maxZonesInCell)
        {
            numCell.y += numZone.y / maxZonesInCell;
            numZone.y = numZone.y % maxZonesInCell;
        }
        else if (numZone.y < 0)
        {
            numCell.y += (numZone.y / maxZonesInCell) - 1;
            numZone.y = maxZonesInCell + (numZone.y % maxZonesInCell);
        }
        //по z
        if (numZone.z > maxZonesInCell)
        {
            numCell.z += numZone.z / maxZonesInCell;
            numZone.z = numZone.z % maxZonesInCell;
        }
        else if (numZone.z < 0)
        {
            numCell.z += (numZone.z / maxZonesInCell) - 1;
            numZone.z = maxZonesInCell + (numZone.z % maxZonesInCell);
        }
    }

    //Вращение взгляда игрока в пространстве
    void UpdateRotate() {

        if (!isLocalPlayer ||
            !Gameplay.main.IsMouseRotate) {
            return;
        }

        //Узнаем положение на экране
        Vector2 mouseOffSet = new Vector2();
        mouseOffSet.x = Input.GetAxis("Mouse X");
        mouseOffSet.y = Input.GetAxis("Mouse Y") * -1;

        float mouseSpeed = 100;
        mouseOffSet = mouseOffSet * Time.unscaledDeltaTime * mouseSpeed;
        Vector3 rotate = new Vector3(mouseOffSet.y, mouseOffSet.x, 0);

        gameObject.transform.Rotate(rotate, Space.Self);


    }

}
