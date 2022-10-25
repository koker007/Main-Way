using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowConnect : MonoBehaviour
{
    static WindowConnect main;

    [SerializeField]
    GameObject content;
    [SerializeField]
    GameObject lobbyInfoPrefab;
    [SerializeField]
    SteamLobby.Lobby lobbySelect;
    static public SteamLobby.Lobby LobbySelect {
        get {
            return main.lobbySelect;
        }
    }

    List<LobbyInfo> lobbyInfos = new List<LobbyInfo>();

    [SerializeField]
    SliderCTRL sliderPlayerSlotsCTRL;
    [SerializeField]
    SliderCTRL sliderDistanceCTRL;
    [SerializeField]
    SliderCTRL sliderMaximumLobbyesCTRL;

    [SerializeField]
    SliderCTRL sliderTimeToUpdateCTRL;


    Steamworks.ELobbyDistanceFilter filterDistance;
    int filterPlayerSlots = 0; //������� ��������� ������ ��� �������
    int filterMaximumLobbyes = 0;


    // Start is called before the first frame update
    void Start()
    {
        main = this;

        //��������� ��������� ������
        sliderPlayerSlotsCTRL.LoadValue();
        sliderDistanceCTRL.LoadValue();
        sliderMaximumLobbyesCTRL.LoadValue();

        Invoke("SetFilterDistance", 0.1f);
        Invoke("SetFilterPlayerSlots", 0.1f);
        Invoke("SetFilterMaximumLobbyes", 0.1f);

        //��������� ����� ��������
        Invoke("RequestList", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        testParameters();
        testList();
    }

    float timeToUpdateParameters = 0;
    float timeToUpdateScale = 1;
    void testParameters() {

        timerIndicator();

        //���� ����� ��� ������� ��� �� ������ - �������
        if (timeToUpdateParameters > Time.unscaledTime)
            return;

        //��������� ���������� ����� ���
        timeToUpdateParameters = Time.unscaledTime + 3600;

        ClearList();
        RequestList();


        //�������� ������� ������� ����������
        void timerIndicator() {

            //����� ��� ��������� ��������
            if (Time.unscaledTime + timeToUpdateScale > timeToUpdateParameters) {
                float value = Time.unscaledTime + timeToUpdateScale - timeToUpdateParameters;

                //����������� �� 100
                value = (value / timeToUpdateScale)*100;

                sliderTimeToUpdateCTRL.slider.value = value;

            }

            sliderTimeToUpdateCTRL.SetValueText((timeToUpdateParameters - Time.unscaledTime).ToString());
        }
    }

    //��������� ������ �������� �� �����
    void testList() {
        //���� ������ �������� �� ���������
        if (lobbyInfos.Count >= SteamLobby.lobbiesList.Count)
            return;

        //������ ������� ����� ��������
        int plus = SteamLobby.lobbiesList.Count - lobbyInfos.Count;
        int now = lobbyInfos.Count; //������� ��������� ����������� �����

        //��������� � ������ ����� �������
        for (int num = 0; num < plus; num++) {
            //����� ������� � ������
            int steamNum = now + num;

            AddLobby(SteamLobby.lobbiesList[steamNum]);
        }
    }

    //��������� ����� � ������
    void AddLobby(SteamLobby.Lobby lobby) {
        //������� ������ ��������� �����
        GameObject lobbyInfoObj = Instantiate(lobbyInfoPrefab, content.transform);

        //��������� �������
        LobbyInfo lobbyInfo = lobbyInfoObj.GetComponent<LobbyInfo>();
        lobbyInfo.SetData(lobby);

        //���������� �� ����� �������
        RectTransform rect = lobbyInfoObj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(rect.pivot.x, lobbyInfos.Count + 1);

        //��������� � ������
        lobbyInfos.Add(lobbyInfo);
    }


    public void SetFilterDistance() {
        filterDistance = (Steamworks.ELobbyDistanceFilter)sliderDistanceCTRL.slider.value;

        if (filterDistance == Steamworks.ELobbyDistanceFilter.k_ELobbyDistanceFilterClose)
        {
            sliderDistanceCTRL.SetValueText("Close1", "ConnectSliderDistanceClose");
        }
        else if (filterDistance == Steamworks.ELobbyDistanceFilter.k_ELobbyDistanceFilterDefault)
        {
            sliderDistanceCTRL.SetValueText("Region1", "ConnectSliderDistanceRegion");
        }
        else if (filterDistance == Steamworks.ELobbyDistanceFilter.k_ELobbyDistanceFilterFar)
        {
            sliderDistanceCTRL.SetValueText("Half Planet1", "ConnectSliderDistanceHalfPlanet");
        }
        else {
            sliderDistanceCTRL.SetValueText("All planet1", "ConnectSliderDistanceAllPlanet");
        }

        sliderDistanceCTRL.SaveValue();

        timeToUpdateParameters = Time.unscaledTime + timeToUpdateScale;
    }
    public void SetFilterPlayerSlots() {
        filterPlayerSlots = (int)sliderPlayerSlotsCTRL.slider.value;

        if(filterPlayerSlots == 0)
            sliderPlayerSlotsCTRL.SetValueText("ALL", "ConnectSliderPlayersVacanciesALL");
        else sliderPlayerSlotsCTRL.SetValueText(filterPlayerSlots.ToString());

        sliderPlayerSlotsCTRL.SaveValue();

        timeToUpdateParameters = Time.unscaledTime + timeToUpdateScale;
    }
    public void SetFilterMaximumLobbyes() {
        int lobbyesCount = (int)sliderMaximumLobbyesCTRL.slider.value;
        if (lobbyesCount == 1) filterMaximumLobbyes = 10;
        else if (lobbyesCount == 2) filterMaximumLobbyes = 50;
        else if (lobbyesCount == 3) filterMaximumLobbyes = 100;
        else filterMaximumLobbyes = 500;

        sliderMaximumLobbyesCTRL.SetValueText(filterMaximumLobbyes.ToString());
        sliderMaximumLobbyesCTRL.SaveValue();

        timeToUpdateParameters = Time.unscaledTime + timeToUpdateScale;
    }


    void ClearList()
    {
        foreach (LobbyInfo lobbyInfo in lobbyInfos)
        {
            Destroy(lobbyInfo.gameObject);
        }
        lobbyInfos = new List<LobbyInfo>();
    }
    /// <summary>
    /// �������� ����� ������ � ������ ���������
    /// </summary>
    void RequestList() {
        SteamLobby.RecreateLobbyList(filterDistance, filterPlayerSlots, filterMaximumLobbyes);
    }

    //�������� ������ ��� ����������� ������������ ���� ��� ��������
    static public void SelectLobby(SteamLobby.Lobby lobbySelect) {
        if (main.lobbySelect == lobbySelect)
        {
            if (main.lobbySelect.ownerSID == SteamCTRL.MySteamID)
                return;


            Debug.Log("Join to lobby");
            SteamLobby.JoinLobby(lobbySelect);

            return;
        }

        main.lobbySelect = lobbySelect;
        Debug.Log("Select Lobby: " + lobbySelect.ownerSID);
    }

}
