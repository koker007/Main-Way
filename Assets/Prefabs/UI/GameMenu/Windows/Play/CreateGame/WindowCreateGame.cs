using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class WindowCreateGame : MonoBehaviour
{
    [SerializeField]
    SliderCTRL PlayersMaxSlider;
    [SerializeField]
    InputFieldCTRL ServerNameInputField;
    [SerializeField]
    InputFieldCTRL PasswordInputField;
    [SerializeField]
    SliderCTRL LobbyTypeSlider;

    
    static public int playersMax = 1;
    static public string serverName = "";
    static public string password = "";
    static ELobbyType lobbyType;


    // Start is called before the first frame update
    void Start()
    {
        PlayersMaxSlider.LoadValue();
        Invoke("SetPlayers", 0.1f);

        LobbyTypeSlider.LoadValue();
        Invoke("SetLobbyType", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayers() {
        playersMax = (int)PlayersMaxSlider.slider.value;
        PlayersMaxSlider.SaveValue();

        SetText();

        void SetText() {
            if (playersMax == 1)
            {
                PlayersMaxSlider.SetValueText("Single", "CreateGamePlayersMax1");
            }
            else {
                PlayersMaxSlider.SetValueText(playersMax.ToString());
            }

            PlayersMaxSlider.SetDefaultText("CreateGamePlayers");
        }
    }
    public void SetServerName()
    {
        serverName = ServerNameInputField.inputField.text;
    }
    public void SetPassword() {
        password = PasswordInputField.inputField.text;

    }
    public void SetLobbyType() {
        int lobbyTypeInt = (int)LobbyTypeSlider.slider.value;
        lobbyType = (ELobbyType)lobbyTypeInt;

        LobbyTypeSlider.SaveValue();

        SetText();

        void SetText() {
            if (lobbyType == ELobbyType.k_ELobbyTypePrivate)
            {
                LobbyTypeSlider.SetValueText("Private", "CreateGameLobbyTypePrivate");
            }
            else if (lobbyType == ELobbyType.k_ELobbyTypeFriendsOnly)
            {
                LobbyTypeSlider.SetValueText("Friends Only", "CreateGameLobbyTypeFriendsOnly");
            }
            else
            {
                LobbyTypeSlider.SetValueText("Public", "CreateGameLobbyTypePublic");
            }
        }
    }


    //������� ������
    public void ClickButtonCreateServer() {

        if (playersMax <= 1) {
            CreateServerSingle();
        }
        else {
            CreateServerPublic();
        }

        //������� ����� ��ee���� ����
        void CreateServerSingle()
        {
            //������� ����� � �����
            SteamLobby.CreateLobby(ELobbyType.k_ELobbyTypePrivate, 1);
            //��������� �����������
            SteamLobby.SetLobbyJoinable(false);

            //��������� ������
            NetworkCTRL.ServerStart(1);
        }
        //������� ��������� ������
        void CreateServerPublic()
        {
            //�������������� ��������� � ����������
            SetServerName();
            SetPassword();

            //��������� ������������ ����������
            if (serverName.Length < 1) {
                //����������� ��� �������
                return;
            }

            //�������
            SteamLobby.CreateLobby(lobbyType, playersMax);
        }

        //������� ��� ����
        WindowMenuCTRL.CloseALL();
    }
}
