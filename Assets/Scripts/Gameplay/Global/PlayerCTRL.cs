using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;


//����� �� �������
public class PlayerCTRL : NetworkBehaviour
{
    public static List<PlayerCTRL> players = new List<PlayerCTRL>(); //������ ���� ������� �� �������
    public static PlayerCTRL local; //������� ��������� �����

    PlayerDataOwner myData;

    [SyncVar] public ulong steamID = 0;
    [SyncVar] public string name = "noName";

    [SyncVar] public int health = 100;
    [SyncVar] public int healthMax = 100;
    [SyncVar] public int oxygen = 100;

    [SyncVar] public int ItemSelect = 0;

    [SyncVar] Vector3Int numCell = new Vector3Int(); //����� ������������� ������
    [SyncVar] Vector3 posInCell = new Vector3(); //��������� ������ ������ ����������� ������
    [SyncVar] Vector3Int numZone = new Vector3Int(); //����� ������� ������ ������

    Vector3Int numCellLocal = new Vector3Int();
    Vector3 posInCellLocal = new Vector3();
    Vector3Int numZoneLocal = new Vector3Int(); //����� ������� ������ ������ ��� ���������� �������

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

    [SyncVar] public string posGlobal = ""; //����� ��������� ������ 


    //Transform
    /// <summary>
    /// ����� �� ����� �� ����� ���� ����������� ��� ��������� ��� ����� � ����������� ������������
    /// </summary>
    [SyncVar] bool downSurface = false;
    /// <summary>
    /// ������ ���� � ����������� ������������, ����������� ����������
    /// </summary>
    [SyncVar] Vector3 downInSpace = new Vector3(0,-1,0);

    //������ ������
    [SerializeField]
    public GameObject CamPlayer; //�������� ����������� ������, ��������� �� ������

    //��������� �������
    SpaceObjData planetCurrent; //������� ������� ������
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

        //��������� �������� ���������
        CalcPosition();

        UpdateRotate();
    }
    private void FixedUpdate()
    {
        testPlayerSteam();
    }

    //���� ��� ��������� �����
    void Inicialize() {


        if (!isLocalPlayer)
        {
            Destroy(CamPlayer);
            return;
        }

        //������� ������ ��������� ����������
        local = this;

        //������
        CamPlayer.SetActive(true);

        //������� ����� ���������� �������
        CameraGalaxy.SetLocalPlayer(this);
        CameraSpaceCell.SetLocalPlayer(this);
    }

    //���������� ���������� ����������� ������ ������
    Quaternion rotateSpaceView;
    float timeCalcSpaceViewLast = 0;
    public Quaternion GetSpaceView() {

        //���� � ������� ����� ��� ����������� ����� ������
        if (timeCalcSpaceViewLast == Time.unscaledTime) {
            return rotateSpaceView; //���������� ����������� ������
        }

        //����� ������� ����� �����
        timeCalcSpaceViewLast = Time.unscaledTime; //���������� ����� �������

        //���� ����� � �������
        if (planetCurrent == null) {
            //������ ���������� ��� �� ��� ��� � ����������� ��������� ������
            rotateSpaceView = CamPlayer.transform.rotation;
            return rotateSpaceView;
        }

        //����� ����� �� �������

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
    /// ���������� ������� ������� ������
    /// </summary>
    /// <param name="spaceObj"></param>
    void ServerSetPlanet(SpaceObjData spaceObj) {
        //������ ������
        if (!isServer)
            return;

        //����������� �������
        planetCurrent = spaceObj;

        //�������� ������� ������
        posGlobal = "X" + (int)numCell.x + "|Y" + (int)numCell.y + "|Z" + (int)numCell.z;

        //���� ������� ��� //�������
        if (spaceObj == null)
            return;

        //�������� ������� �������
        CellS cell = spaceObj.cell;

        //���������� ��� ������� �� ����� ������
        SpaceObjData select = spaceObj;
        string planetsInvers = "";
        while (select != null) {
            //���� �������� ��� - �������
            if (select.parent == null)
                break;

            //���� �������� ���� ������ ����� ����� ������� � ��������
            for (int num = 0; num < select.parent.childs.Length; num++) {
                //����� ������� �������
                if (select == select.parent.childs[num]) {
                    //���������� ����������� � �����
                    if (planetsInvers.Length > 0)
                        planetsInvers += "|";

                    planetsInvers += num;

                    //������������� �� ��������
                    select = select.parent;
                    continue;
                }
            }
        }

        //������ ������ � �������� ����
        //������� ��������� ������
        string planets = "";
        string[] planetsMas = planetsInvers.Split("|");

        for (int num = planetsMas.Length - 1; num >= 0; num--) {
            if (planets.Length == 0)
                planets += "P";
            else planets += "M";

            planets += planetsMas[num];
        }

        //��������� �������
        posGlobal += planets;

        //���������� ������� ������
    }

    //����������� ������ � �����������
    void UpdateMove() {
        //���� ���� ����� �� ���������
        if (!isLocalPlayer)
            return;


        Vector3 move = new Vector3();

        //������ ��������� ������
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

        //�������� ��������
        if (move != new Vector3()) {
            //Debug.Log("Player Move: " + move);
        }

        //���� ����� ��������� �� ������������� �����������
        if (downSurface) {
        
        }
        //���� ����� �� ��������� �� ������������� �����������
        else
        {

        }

        float speed = 1000000f;
        //�������� ������������ � ������� �����
        Vector3 moveOffset = new Vector3(0,0,0);
        //���������� ������ � ������������
        moveOffset += move.z * CamPlayer.transform.forward * Time.deltaTime * speed;
        moveOffset += move.x * CamPlayer.transform.right * Time.deltaTime * speed;
        moveOffset += move.y * CamPlayer.transform.up * Time.deltaTime * speed;

        if (moveOffset.magnitude > 0) {
            gameObject.transform.position += moveOffset;
        }

        //��������� ������� ������ � ������

        //����� �������� ����� �� ������� � ������� ������ �������� ������ ������ �������
        //����� ������� ����� ������� � ������

        //������ ������� ����������        
        //posInGalaxy = transform.position / CellS.size;
        //posInCell = transform.position / CellS.sizeVisual;

        

    }

    //������ ���� � �������� ������
    void CalcPosition() {
        if (!isLocalPlayer)
            return;

        //��������� ������� ������ �� ������ �� �������
        bool isChange = false;

        //���� ����� �� ������� ���� ���������
        //�� x
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

        //�� y
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

        //�� z
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

        //���������� �� ������ ������� � ��������� ����
        //������ ������ ������� ������� ��� ������� ����
        if (isChange)
            SetZone(numZoneLocal.x, numZoneLocal.y, numZoneLocal.z);

        //�������� ������ �� ����������� ������
        int maxZonesInCell = CellS.size / CellS.sizeZone;
        //�� x
        if (numZoneLocal.x > maxZonesInCell) {
            numCellLocal.x += numZoneLocal.x / maxZonesInCell;
            numZoneLocal.x = numZoneLocal.x % maxZonesInCell;
        }
        else if (numZoneLocal.x < 0) {
            numCellLocal.x += (numZoneLocal.x / maxZonesInCell) - 1;
            numZoneLocal.x = maxZonesInCell + (numZoneLocal.x % maxZonesInCell);
        }

        //�� �
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
        //�� z
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

        //������� ��������� �������� � ����������� ������
        posInCellLocal = numZoneLocal * CellS.sizeZone + gameObject.transform.position;

    }

    [Command]
    void SetZone(int zoneX, int zoneY, int zoneZ) {
        //������ ������� ����� � ���� ������� ����, ������ �������� ������
        numZone.x = zoneX;
        numZone.y = zoneY;
        numZone.z = zoneZ;

        //����� ������� ����
        ServerSetPlanet(null);

        //�������� ������ �� ����������� ������
        int maxZonesInCell = CellS.size / CellS.sizeZone;
        //�� x
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

        //�� �
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
        //�� z
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

    //�������� ������� ������ � ������������
    void UpdateRotate() {

        if (!isLocalPlayer ||
            !Gameplay.main.IsMouseRotate) {
            return;
        }

        //������ ��������� �� ������
        Vector2 mouseOffSet = new Vector2();
        mouseOffSet.x = Input.GetAxis("Mouse X");
        mouseOffSet.y = Input.GetAxis("Mouse Y") * -1;

        float mouseSpeed = 100;
        mouseOffSet = mouseOffSet * Time.unscaledDeltaTime * mouseSpeed;
        Vector3 rotate = new Vector3(mouseOffSet.y, mouseOffSet.x, 0);

        gameObject.transform.Rotate(rotate, Space.Self);


    }

}
