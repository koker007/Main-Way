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
    int filterPlayerSlots = 0; //Сколько свободных слотов для игроков
    int filterMaximumLobbyes = 0;


    // Start is called before the first frame update
    void Start()
    {
        main = this;

        //загружаем параметры поиска
        sliderPlayerSlotsCTRL.LoadValue();
        sliderDistanceCTRL.LoadValue();
        sliderMaximumLobbyesCTRL.LoadValue();

        Invoke("SetFilterDistance", 0.1f);
        Invoke("SetFilterPlayerSlots", 0.1f);
        Invoke("SetFilterMaximumLobbyes", 0.1f);

        //Запускаем поиск серверов
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

        //Если время для запроса еще не пришло - выходим
        if (timeToUpdateParameters > Time.unscaledTime)
            return;

        //Следующее обновление через час
        timeToUpdateParameters = Time.unscaledTime + 3600;

        ClearList();
        RequestList();


        //Изменяем слайдер времени обновления
        void timerIndicator() {

            //Время для изменения ползунка
            if (Time.unscaledTime + timeToUpdateScale > timeToUpdateParameters) {
                float value = Time.unscaledTime + timeToUpdateScale - timeToUpdateParameters;

                //Маштабируем до 100
                value = (value / timeToUpdateScale)*100;

                sliderTimeToUpdateCTRL.slider.value = value;

            }

            sliderTimeToUpdateCTRL.SetValueText((timeToUpdateParameters - Time.unscaledTime).ToString());
        }
    }

    //проверяем список серверов на новые
    void testList() {
        //Если список серверов не изменился
        if (lobbyInfos.Count >= SteamLobby.lobbiesList.Count)
            return;

        //узнаем сколько новых серверов
        int plus = SteamLobby.lobbiesList.Count - lobbyInfos.Count;
        int now = lobbyInfos.Count; //Текущий последний добавленный номер

        //Добавляем в список новые сервера
        for (int num = 0; num < plus; num++) {
            //Номер сервера в списке
            int steamNum = now + num;

            AddLobby(SteamLobby.lobbiesList[steamNum]);
        }
    }

    //Добавляет лобби в список
    void AddLobby(SteamLobby.Lobby lobby) {
        //создаем префаб визульной части
        GameObject lobbyInfoObj = Instantiate(lobbyInfoPrefab, content.transform);

        //Заполняем данными
        LobbyInfo lobbyInfo = lobbyInfoObj.GetComponent<LobbyInfo>();
        lobbyInfo.SetData(lobby);

        //Перемещаем на новую позицию
        RectTransform rect = lobbyInfoObj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(rect.pivot.x, lobbyInfos.Count + 1);

        //Добавляем в список
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
    /// Получить новый список с новыми фильтрами
    /// </summary>
    void RequestList() {
        SteamLobby.RecreateLobbyList(filterDistance, filterPlayerSlots, filterMaximumLobbyes);
    }

    //Выделить сервер или попрововать подключиться если уже выбранно
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
