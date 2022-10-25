using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//Отвечает за хранение всех данных о мире
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
                //Ставим мышь в центр
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
        //Выключить гравное меню
        UICTRL.IsOpenMainMenu = false;

        main = this;
        galaxy = null;

        //Потом при создании игры заменить на данные из файла мира
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

        //Если галактики нет, но семя есть то надо создать галактику по семени
        if (galaxy == null && Seed != "") {
            galaxy = new Galaxy(Galaxy.Size.cells61, Seed);
            GalaxyCtrl.ClearBuffer();
        }

        seedOld = Seed;
    }

    /// <summary>
    /// Обновляет информацию наблюдателей, то что им видно
    /// </summary>
    void UpdateObservers() {
    
    }
}
