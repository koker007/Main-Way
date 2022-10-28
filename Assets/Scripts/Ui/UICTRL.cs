using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICTRL : MonoBehaviour
{
    static public UICTRL main;

    [SerializeField]
    GameObject redactor;



    bool isOpenMainMenu = true;

    static public bool IsOpenMainMenu{
        set {
            main.isOpenMainMenu = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestMainMenu();
    }

    //Проверка открытия или закрытия главного меню игры Принажатии ESC
    void TestMainMenu() {
        //Если игра активна
        if (Gameplay.main != null && Gameplay.main.gameObject.activeSelf ||
            RedactorUICTRL.main != null && RedactorUICTRL.main.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (WindowMenuCTRL.main.gameObject.activeSelf)
                {
                    isOpenMainMenu = false;
                }
                else
                {
                    isOpenMainMenu = true;
                }
            }
        }
        //Если игра не активна открываем меню
        else
        {
            isOpenMainMenu = true;
        }

        //изменение статуса
        if (isOpenMainMenu != WindowMenuCTRL.main.gameObject.activeSelf)
            SetMainMenu(isOpenMainMenu);
    }

    void SetMainMenu(bool isActive) {
        if (WindowMenuCTRL.main.gameObject.activeSelf != isActive) {
            if (Gameplay.main != null) {
                if (!isActive) {
                    Gameplay.main.IsMouseRotate = true;
                }
                else {
                    Gameplay.main.IsMouseRotate = false;
                }
            }
            

            WindowMenuCTRL.main.gameObject.SetActive(isActive);
            WindowMenuCTRL.CloseALL(true);

            GameTitleCTRL.main.gameObject.SetActive(isActive);
            GameTitleCTRL.MenuIsOpen();
        }
    }
}
