using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� Hud � ������� ��� ������� ���������� ����
public class HUDMain : MonoBehaviour
{
    static HUDMain main;

    // Start is called before the first frame update
    void Start()
    {
        inicialize();
    }

    public void inicialize() {
        main = this;
    }

    static public void reActive() {
        main.gameObject.SetActive(!main.gameObject.activeSelf);
    }
}
