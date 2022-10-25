using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMenuCTRL : MonoBehaviour
{
    static public WindowMenuCTRL main;
    static public List<WindowMenu> buffer = new List<WindowMenu>();


    [SerializeField]
    WindowMenu MainMenu;
    
    [Header("Main Menu")]
    [SerializeField]
    WindowMenu Play;
    [SerializeField]
    WindowMenu Settings;
    [SerializeField]
    WindowMenu Redactor;

    [Header("Play")]
    [SerializeField]
    WindowMenu Create;
    [SerializeField]
    WindowMenu Connect;

    [Header("World")]
    [SerializeField]
    WindowMenu WorldChoose;
    [SerializeField]
    WindowMenu WorldNew;


    [Header("Settings")]
    [SerializeField]
    WindowMenu Audio;
    [SerializeField]
    WindowMenu Controll;
    [SerializeField]
    WindowMenu Graffic;


    public bool FirstPress = false;

    static public void ClickSettings()
    {
        GameObject Obj = Instantiate(main.Settings.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickRedactor() {
        GameObject Obj = Instantiate(main.Redactor.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }

    static public void ClickPlay()
    {
        GameObject Obj = Instantiate(main.Play.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickConnect()
    {
        GameObject Obj = Instantiate(main.Connect.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickServer()
    {
        GameObject Obj = Instantiate(main.Create.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickNewWorld() {
        GameObject Obj = Instantiate(main.WorldNew.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickLoadWorld()
    {
        GameObject Obj = Instantiate(main.WorldChoose.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }

    static public void ClickSound()
    {
        GameObject Obj = Instantiate(main.Audio.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickControl()
    {
        GameObject Obj = Instantiate(main.Controll.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }
    static public void ClickGraffic()
    {
        GameObject Obj = Instantiate(main.Graffic.gameObject, main.gameObject.transform);
        AddInBuffer(Obj.GetComponent<WindowMenu>());
    }

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestBuffer();
        TestFistPress();
    }

    void TestBuffer() {

        //Создаем новый буфер
        List<WindowMenu> bufferNew = new List<WindowMenu>();

        //Перебираем текущий буфер
        foreach (WindowMenu windowMenu in buffer) {
            if (windowMenu == null) continue;

            bufferNew.Add(windowMenu);
        }

        buffer = bufferNew;

        if (buffer.Count > 0)
        {
            buffer[0].PlayOpenLeft();

            if (buffer.Count == 1)
            {
                GameTitleCTRL.ViewFirstMenu();
            }
            else {
                GameTitleCTRL.HideFirstMenu();
            }
        }
        //Иначе вызываем основное меню если что либо было нажало
        else if(FirstPress) {
            GameObject mainMenuObj = Instantiate(MainMenu.gameObject, gameObject.transform);
            AddInBuffer(mainMenuObj.GetComponent<WindowMenu>());
            
        }
    }
    void TestFistPress() {
        if (FirstPress) 
            return;

        if (Input.anyKey) {
            FirstPress = true;
            GameTitleCTRL.MenuIsOpen();
        }
    }

    static void AddInBuffer(WindowMenu windowFunc) {

        List<WindowMenu> bufferNew = new List<WindowMenu>();

        foreach (WindowMenu window in buffer) {
            if (windowFunc == window) {
                return;
            }
        }

        //Если так и не нашли то добавляем в самое начало
        bufferNew.Add(windowFunc);
        foreach (WindowMenu window in buffer) {
            bufferNew.Add(window);
        }

        buffer = bufferNew;
    }

    static public void CloseFirstInBuffer(WindowMenu windowFunc) {
        //Если буфера нет или первый номер не мой
        if (buffer == null || 
            buffer.Count == 0 || 
            buffer[0] != windowFunc) 
            return;

        //Проверка пройдена, удалять можно
        //Создаем новый список
        List<WindowMenu> bufferNew = new List<WindowMenu>();
        bool first = true;
        foreach (WindowMenu windowMenu in buffer) {
            if (first) {
                first = false;
                continue;
            }

            bufferNew.Add(windowMenu);
        }
        buffer = bufferNew;
    }

    static public void CloseALL() {
        buffer = new List<WindowMenu>();
    }
    static public void CloseALL(bool deleteNow) {
        if (!deleteNow)
        {
            CloseALL();
            return;
        }

        foreach (WindowMenu window in buffer) {
            Destroy(window.gameObject);
        }
        buffer = new List<WindowMenu>();
    }
}
