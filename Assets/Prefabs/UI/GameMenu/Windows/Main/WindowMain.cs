using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowMain : MonoBehaviour
{
    [SerializeField]
    ButtonCTRL buttonPlay;
    [SerializeField]
    ButtonCTRL buttonSettings;
    [SerializeField]
    ButtonCTRL buttonRedactor;

    [SerializeField]
    ButtonCTRL ButtonExit;

    string keyQuit;
    string keyDisconnect;
    string keySaveAndExit;

    string strQuit = "Quit";
    string strDisconnect = "Disconnect";
    string strSaveAndExit = "Save and Exit";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExitText();

        UpdateActiveButton();
    }

    void UpdateActiveButton() {
        //���� ��������
        if (RedactorUICTRL.main.gameObject.activeSelf) {
            buttonPlay.gameObject.SetActive(false);
            buttonSettings.gameObject.SetActive(false);
            buttonRedactor.gameObject.SetActive(false);
        }
        //���� ��������
        else if (Gameplay.main != null && Gameplay.main.gameObject.activeSelf) {
            buttonRedactor.gameObject.SetActive(false);
        }
        else {
            buttonPlay.gameObject.SetActive(true);
            buttonSettings.gameObject.SetActive(true);
            buttonRedactor.gameObject.SetActive(true);
        }
    }

    void UpdateExitText() {
        if (NetworkCTRL.isNetrorkActive())
        {
            if (NetworkCTRL.main.isServer)
                ButtonExit.SetText(strSaveAndExit);
            else ButtonExit.SetText(strDisconnect);
        }
        else {
            ButtonExit.SetText(strQuit);
        }
    }

    public void ClickPlay() {
        WindowMenuCTRL.ClickPlay();
    }
    public void ClickSettings() {
        WindowMenuCTRL.ClickSettings();
    }
    public void ClickRedactor() {
        WindowMenuCTRL.ClickRedactor();
    }

    public void ClickExit() {

        //���� ��������� �������� ������ ���� ���������
        //��� �������� ��������
        if (Gameplay.main != null && Gameplay.main.gameObject.activeSelf ||
            RedactorUICTRL.main.gameObject.activeSelf)
        {
            Debug.Log("Disconnect");

            //����� ��������� ����
            //Gameplay
            //����� �������������
            NetworkCTRL.Disconnect();

            GalaxyCtrl.ClearBuffer();

            //����� ������������� �����
            SceneManager.LoadScene(0);

        }
        else {
            //�������� �� �������� ������ ������� �� ����
            Debug.Log("Game quit");

            GalaxyCtrl.ClearBuffer();

            //System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Quit();

            //����� ������������� ����� ���� ��������� ������
            SceneManager.LoadScene(0);
        }
    }
}
