using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using UnityEngine.EventSystems;

//���������� ����� �� ����� ����������� � ��������
public class LobbyInfo : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    SteamLobby.Lobby lobby;

    [Header("Main")]

    [SerializeField]
    Image[] ramki;
    [SerializeField]
    Sprite ramkaNormal;
    [SerializeField]
    Sprite ramkaSelected;
    [SerializeField]
    Image[] backgroungs;
    [SerializeField]
    Color backgroundNormal;
    [SerializeField]
    Color backgroundSelected;

    [Header("Parameters")]
    [SerializeField]
    RawImage Avatar;
    Texture2D downloadedAvatar;

    [SerializeField]
    TextMeshProUGUI[] Name;
    [SerializeField]
    TextMeshProUGUI[] Players;
    [SerializeField]
    GameObject passwordIcon;

    string nameStr = "";
    //string hostAddress = "";
    string password = "";
    string seed = ""; //���� ����
    string killMode = "";//����� �� ������� ������ ������� ����� ��� �����
    string gameMode = "";//����� ����
    string timeWorld = ""; //������� ������� ���������� ���
    string timeOnline = ""; //������� ������� ������ ������

    int playersNow = 0;
    int playersMax = 0;

    bool isSelected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        testSelect();
    }

    public void SetData(SteamLobby.Lobby lobby) {

        this.lobby = lobby;

        //������������� ������
        foreach (SteamLobby.MetaData data in lobby.metaDatas) {
            if (data.key == SteamLobby.LobbyKeys.name) nameStr = data.value;
            //else if (data.key == SteamLobby.LobbyKeys.hostSteamID) hostAddress = data.value;
            else if (data.key == SteamLobby.LobbyKeys.password) password = data.value;
            else if (data.key == SteamLobby.LobbyKeys.seed) seed = data.value;
            else if (data.key == SteamLobby.LobbyKeys.killMode) killMode = data.value;
            else if (data.key == SteamLobby.LobbyKeys.gameMode) gameMode = data.value;
            else if (data.key == SteamLobby.LobbyKeys.timeWorld) timeWorld = data.value;
            else if (data.key == SteamLobby.LobbyKeys.timeOnline) timeOnline = data.value;
        }

        //����� �������� ������������ ���������� ������� ��������� �� �������
        playersNow = SteamMatchmaking.GetNumLobbyMembers(lobby.lobbySID);
        playersMax = SteamMatchmaking.GetLobbyMemberLimit(lobby.lobbySID);

        //��������� ���������� �����
        updateVisual();

    }

    void updateVisual() {

        //��������� ������ ���������
        downloadedAvatar = SteamCTRL.GetPlayerIcon(lobby.ownerSID, SteamCTRL.IconSize.Small);
        downloadedAvatar.filterMode = FilterMode.Point;
        Avatar.texture = downloadedAvatar;
        Avatar.rectTransform.localScale = new Vector2(1, -1);

        //��������� ��� �������
        foreach (TextMeshProUGUI text in Name) {
            text.text = nameStr;
        }
        //��������� ���������� �������
        foreach (TextMeshProUGUI text in Players)
        {
            text.text = ((int)playersNow).ToString() + "/" + ((int)playersMax).ToString();
        }

        //��������� ������
        if (password != null && password != "")
            passwordIcon.SetActive(true);
        else
            passwordIcon.SetActive(false);

    }

    public void OnPointerClick(PointerEventData pointerData) {
        Debug.Log("click");
        Select();
    }
    public void OnPointerExit(PointerEventData pointerData) {
        SetRamki(ramkaNormal);
    }
    public void OnPointerEnter(PointerEventData pointerData) {
        SetRamki(ramkaSelected);
    }

    void Select() {
        WindowConnect.SelectLobby(lobby);
    }

    //�������� ���������
    void testSelect() {
        if (WindowConnect.LobbySelect != lobby && isSelected)
        {
            isSelected = false;
            SetBackground(backgroundNormal);
        }
        else if(WindowConnect.LobbySelect == lobby && !isSelected) {
            isSelected = true;
            SetBackground(backgroundSelected);
        }
    }

    //���������� ���� �����
    void SetRamki(Sprite sprite) {
        foreach (Image image in ramki) {
            image.sprite = sprite;
        }
    }
    //���������� ���� ����
    void SetBackground(Color color) {
        foreach (Image image in backgroungs)
        {
            image.color = color;
        }
    }
}
