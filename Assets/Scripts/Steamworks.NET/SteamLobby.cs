using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

public class SteamLobby : MonoBehaviour
{
    static SteamLobby main;

    static public Lobby lobbyNow = new Lobby(); //������� ����� � ������� �� ���������
    static public List<Lobby> lobbiesList = new List<Lobby>();

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> CallbackGameLobbyJoinRequested;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyUpdate;

    protected Callback<LobbyChatMsg_t> Callback_lobbyChatMsg;
    protected Callback<LobbyChatUpdate_t> Callback_lobbyChatUpdate;

    /// <summary>
    /// ����� ������ �����
    /// </summary>
    public static class LobbyKeys {
        public const string name = "keyName";
        public const string ownerID = "keyOwnerID";
        //public const string hostSteamID = "keyHostSteamID";
        public const string password = "keyPassword";
        public const string seed = "keySeed"; //���� ����
        public const string killMode = "keyKillMode";//����� �� ������� ������ ������� ����� ��� �����
        public const string gameMode = "keyGameMode";//����� ����
        public const string timeWorld = "KeyTimeWorld"; //������� ������� ���������� ���
        public const string timeOnline = "KeyTimeOnline"; //������� ������� ������ ������
    }

    public struct MetaData {
        public string key;
        public string value;
    }

    /// <summary>
    /// ��������� �����
    /// </summary>
    public struct Memders {
        public CSteamID CSteamID; //ID ������������ Steam
        public MetaData[] Data; //������ ������ � ���� ����-��������
    }

    //������ ����� steam
    public class Lobby {
        public CSteamID lobbySID;
        public CSteamID ownerSID;
        public Memders[] members;
        public int memberLimit;
        public MetaData[] metaDatas = new MetaData[50]; //50 ��������� ������ � ������� �����

        public Lobby() {
            int num = 0;
            metaDatas[num].key = LobbyKeys.name;
            num++;
            metaDatas[num].key = LobbyKeys.ownerID;
            //num++;
            //metaDatas[num].key = LobbyKeys.hostSteamID;
            num++;
            metaDatas[num].key = LobbyKeys.password;
            num++;
            metaDatas[num].key = LobbyKeys.seed;
            num++;
            metaDatas[num].key = LobbyKeys.killMode;
            num++;
            metaDatas[num].key = LobbyKeys.gameMode;
            num++;
            metaDatas[num].key = LobbyKeys.timeWorld;
            num++;
            metaDatas[num].key = LobbyKeys.timeOnline;
            num++;
        }

        public static Lobby SetParametrs(CSteamID lobbySID) {
            Lobby lobby = null;

            if (lobbySID == null)
                return null;

            lobby = new Lobby();
            lobby.lobbySID = lobbySID;
            //lobby.ownerSID = SteamMatchmaking.GetLobbyOwner(lobbySID);
            lobby.memberLimit = SteamMatchmaking.GetLobbyMemberLimit(lobbySID);

            int membersFound = SteamMatchmaking.GetNumLobbyMembers(lobbySID);

            //�������� ������������� ����� ������ ���� ���� ��������� � ���� �����
            if (lobbyNow != null && lobbyNow.lobbySID != null && lobby.lobbySID == lobbyNow.lobbySID) {
                lobby.members = new Memders[membersFound];
                for (int num = 0; num < membersFound; num++) {
                    lobby.members[num].CSteamID = SteamMatchmaking.GetLobbyMemberByIndex(lobbySID, num);
                }

            }

            //�������� ������ �����
            for (int num = 0; num < lobby.metaDatas.Length; num++) {
                if (lobby.metaDatas[num].key == LobbyKeys.name) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.name);
                else if (lobby.metaDatas[num].key == LobbyKeys.ownerID)
                {
                    lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.ownerID);
                    if (lobby.metaDatas[num].value != null && lobby.metaDatas[num].value.Length > 0)
                        lobby.ownerSID.m_SteamID = (ulong)System.Convert.ToInt64(lobby.metaDatas[num].value);
                    else {
                        lobby.ownerSID.m_SteamID = 0;
                    }
                }
                //else if (lobby.metaDatas[num].key == LobbyKeys.hostSteamID) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.hostSteamID);
                else if (lobby.metaDatas[num].key == LobbyKeys.password) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.password);
                else if (lobby.metaDatas[num].key == LobbyKeys.seed) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.seed);
                else if (lobby.metaDatas[num].key == LobbyKeys.killMode) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.killMode);
                else if (lobby.metaDatas[num].key == LobbyKeys.gameMode) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.gameMode);
                else if (lobby.metaDatas[num].key == LobbyKeys.timeWorld) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.timeWorld);
                else if (lobby.metaDatas[num].key == LobbyKeys.timeOnline) lobby.metaDatas[num].value = SteamMatchmaking.GetLobbyData(lobby.lobbySID, LobbyKeys.timeOnline);
            }

            return lobby;
        }

        public static int GetPlayersLimit(Lobby lobby) {
            int result = 0;
            if (lobby == null)
                return result;

            result = SteamMatchmaking.GetLobbyMemberLimit(lobby.lobbySID);

            return result;
        }

        public static int GetPlayerCount(Lobby lobby) {
            int result = 0;
            
            if (lobby == null)
                return 0;

            result = SteamMatchmaking.GetNumLobbyMembers(lobby.lobbySID);
            return result;
        }
    }

    static public void AddFavorite(Lobby lobby, bool onlyHistory)
    {
        uint ip = 0;
        ushort port = 0;
        uint flag = 1;
        if (onlyHistory) 
            flag = 2;

        DateTime now = DateTime.Now;
        DateTime nowFrom1970 = new DateTime(now.Year - 1970, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        uint nowUnix = (uint)(now.Ticks / (10000 * 1000));

        AppId_t appId_T;
        appId_T.m_AppId = NetworkCTRL.SteamAppID;

        SteamMatchmaking.AddFavoriteGame(appId_T, ip, port, port, flag, nowUnix);
    }
    static public void RemoveFavorite(Lobby lobby)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        iniCallback();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///�������������� � ��� ������������� �����
    public static void JoinLobby(Lobby lobby) {
        SteamMatchmaking.JoinLobby(lobby.lobbySID);
    }

    /// <summary>
    /// ������: ��� 1
    /// </summary>
    public static void CreateLobby(ELobbyType eLobbyType, int playersMax)
    {
        SteamMatchmaking.CreateLobby(eLobbyType, playersMax);
    }
    /// <summary>
    /// ������: ��� 2 ���������� ����� �� ������ ������ ������������ � ����� �����
    /// </summary>
    public static void SetLobbyJoinable(bool isJoinable)
    {
        SteamMatchmaking.SetLobbyJoinable(lobbyNow.lobbySID, isJoinable);
    }
    /// <summary>
    /// ������: ��� 3 ���������� �������
    /// </summary>
    public static void SetDataMyLobby(string key, string value)
    {
        //���� ����� �������� � � ��������
        if (lobbyNow.lobbySID.m_SteamID != 0 && SteamMatchmaking.GetLobbyOwner(lobbyNow.lobbySID) == SteamCTRL.MySteamID)
        {
            SteamMatchmaking.SetLobbyData(lobbyNow.lobbySID, key, value);
        }
    }


    /// <summary>
    /// ������������� ������� �������� ������
    /// </summary>
    void iniCallback() {
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated); // ��� �������� �����
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList); // ��� ��������� ������ �����
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered); // ��� ����� � �����
        Callback_lobbyUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyUpdate);  // ��� ���������� ����-������ �����
        CallbackGameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested); //��� �������� �� ���� � �����

        Callback_lobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg); // ��� ��������� ��������� � �����
        Callback_lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate); // ��� ��������� ������ ������� � ����� (����� �����-���� ����� ������ � ����� ��� �������)

        if (SteamAPI.Init())
            Debug.Log("Steam API init -- SUCCESS!");
        else
            Debug.Log("Steam API init -- failure ...");
    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
            Debug.Log("Lobby created -- SUCCESS!");
        else
            Debug.Log("Lobby created -- failure ...");

        //���� ��������. ��������� �������
        if (result.m_eResult == EResult.k_EResultOK)
        {
            //� ������
            NetworkCTRL.main.isServer = true;

            //��������� ������
            NetworkCTRL.ServerStart(WindowCreateGame.playersMax);
        }
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {

        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyUpdate(LobbyDataUpdate_t result)
    {
        //��������� ����� � ������
        CSteamID cSteamID;
        cSteamID.m_SteamID = result.m_ulSteamIDLobby;
        Lobby lobby = new Lobby();
        lobby.lobbySID = cSteamID;


        //���� ��� ����� � ������� � ��������
        if (lobbyNow != null &&
            lobbyNow.lobbySID != null &&
            result.m_ulSteamIDLobby == lobbyNow.lobbySID.m_SteamID)
        {
            //��������� ������ � �����
            lobbyNow = Lobby.SetParametrs(lobbyNow.lobbySID);
        }

        //���� � ������ ��� �����
        for (int i = 0; i < lobbiesList.Count; i++)
        {
            //���� ����� ����, � ��� ����� � ������� ���������� ������
            if (lobbiesList[i] != null && lobbiesList[i].lobbySID.m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbiesList[i].lobbySID.m_SteamID, "name"));

                //��������� ������� ��� �����
                lobbiesList[i] = Lobby.SetParametrs(lobbiesList[i].lobbySID);

                //����� ���� �������� � ������ ���� ��������, �������
                return;
            }
        }

        //��������� ����� � ������ � ��������� ������� ��� �����
        Debug.Log("Lobby Add " + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobby.lobbySID.m_SteamID, "name"));
        lobby = Lobby.SetParametrs(lobby.lobbySID);
        lobbiesList.Add(lobby);

    }

    void OnLobbyEntered(LobbyEnter_t result)
    {

        //������ ������� ����� �� �����
        lobbyNow = Lobby.SetParametrs((CSteamID)result.m_ulSteamIDLobby);

        //���� �������� ����� �, �� ������ ��� ��� ������ ������
        if (SteamMatchmaking.GetLobbyOwner(lobbyNow.lobbySID) == SteamCTRL.MySteamID) {
            IniServerData();
            //����������� �������� �� ���������
            return;
        }

        //����� ����������� � �������
        string hostAddress = lobbyNow.ownerSID.ToString(); //SteamMatchmaking.GetLobbyData(new CSteamID(result.m_ulSteamIDLobby), LobbyKeys.hostSteamID);
        NetworkCTRL.ConnectStart(hostAddress);

        if (result.m_EChatRoomEnterResponse == 1)
            Debug.Log("Lobby joined!");
        else
            Debug.Log("Failed to join lobby.");

        //������������� �������� ������ ��������� �� ����� � ����������� ����� � ������ ������ ��� ������� ���
        void IniServerData() {
            NetworkCTRL.main.isServer = true;

            //��������� �����������
            SetLobbyJoinable(true);
            SetDataMyLobby(LobbyKeys.name, WindowCreateGame.serverName);
            SetDataMyLobby(LobbyKeys.ownerID, SteamCTRL.MySteamID.ToString());
            if (WindowCreateGame.password != null && WindowCreateGame.password != "" && WindowCreateGame.password.Length > 0)
            {
                SetDataMyLobby(LobbyKeys.password, "YES");
            }
            else {
                SetDataMyLobby(LobbyKeys.password, "");
            }

            //��� �������� ����������� �� ����� �������� ����
            //SteamLobby.SetDataMyLobby(SteamLobby.LobbyKeys.timeOnline, "0");
            //SteamLobby.SetDataMyLobby(SteamLobby.LobbyKeys.timeWorld, "0");
        }
    }

    void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t result)
    {

    }

    void OnLobbyChatMsg(LobbyChatMsg_t result)
    {

    }
    void OnLobbyChatUpdate(LobbyChatUpdate_t result)
    {

    }

    /// <summary>
    /// ������� ������ ������ � ��������� �� ����� ����� ������ �������� � ����� ��������
    /// </summary>
    static public void RecreateLobbyList(ELobbyDistanceFilter filterDist, int filterSlots, int maximumLobbyes) {
        //������� ������ ������
        lobbiesList = new List<Lobby>();

        //������ ���������
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(filterDist);
        //������� ���������� ����� ������ ����
        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(filterSlots);
        //������� �������� ����� ������ ����
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(maximumLobbyes);

        //�������� ������ �������� � ���������� ���������
        SteamMatchmaking.RequestLobbyList();
    }
}
