using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.FizzySteam;

public class NetworkCTRL : MonoBehaviour
{
    public static NetworkCTRL main;

    public bool isServer = false;

    [SerializeField]
    NetworkManager networkManager;
    [SerializeField]
    FizzySteamworks TransportSteam;

    static public uint SteamAppID = 1985400;

    void iniNetWork() {
        networkManager = gameObject.GetComponent<NetworkManager>();
    }

    private void Start()
    {
        main = this;
        iniNetWork();
    }

    
    static public bool isNetrorkActive() {
        return main.networkManager.isNetworkActive;
    }

    static public void ConnectStart(string addressIP) {
        if (main.networkManager.isNetworkActive)
            return;

        main.networkManager.networkAddress = addressIP;
        main.networkManager.StartClient();
    }

    static public void ServerStart(int PlayersMax) {
        if (main.networkManager.isNetworkActive)
            return;

        main.networkManager.maxConnections = PlayersMax;
        main.networkManager.StartHost();
    }

    static public void Disconnect() {
        if (main.networkManager.isNetworkActive) {
            StopAll();
        }

        void StopAll() {
            main.networkManager.StopClient();
            main.networkManager.StopHost();
            main.networkManager.StopServer();
        }
    }
}
