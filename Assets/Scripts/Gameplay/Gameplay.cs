using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//�������� �� �������� ���� ������ � ����
public class Gameplay: NetworkBehaviour
{
    static public Gameplay main;

    [SyncVar] public string Seed = "";
    [SyncVar] public float timeWorld = 0;
    [SyncVar] public float timeOnline = 0;

    bool isMouseRotate = false;
    public bool IsMouseRotate {
        set {
            isMouseRotate = value;
            if (isMouseRotate && Cursor.lockState != CursorLockMode.Locked)
            {
                //������ ���� � �����
                Cursor.lockState = CursorLockMode.Locked;
                return;
            }
            else if (!isMouseRotate && Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                return;
            }
        }
        get {
            return isMouseRotate;
        }
    }

    static public Galaxy galaxy;


    void Start()
    {
        //��������� ������� ����
        UICTRL.IsOpenMainMenu = false;

        main = this;
        galaxy = null;

        //����� ��� �������� ���� �������� �� ������ �� ����� ����
        Seed = "AbracadabraTest2112";
    }

    private void Update()
    {
        UpdateObservers();
    }

    private void FixedUpdate()
    {
        UpdateGalaxy();
    }

    string seedOld = "";
    void UpdateGalaxy() {
        if (Seed != seedOld) {
            galaxy = null;
        }

        //���� ��������� ���, �� ���� ���� �� ���� ������� ��������� �� ������
        if (galaxy == null && Seed != "") {
            galaxy = new Galaxy(Galaxy.Size.cells61, Seed);
            GalaxyCtrl.ClearBuffer();
        }

        seedOld = Seed;
    }

    /// <summary>
    /// ��������� ���������� ������������, �� ��� �� �����
    /// </summary>
    void UpdateObservers() {
    
    }
}
