using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//�������� �� �������� ���� ������ � ����
public class GameData : MonoBehaviour
{
    int blockMax = 100000;
    //���������� �������� �������������� � ����
    const int charsMod = 3;
    //���������� �������� �����
    const int charsName = 3;

    static public string pathMod = "Mods";

    static public string nameBlock = "Blocks"; //��� ����� ������
    static public string nameRecipes = "Recipes"; //��� ����� �������� �������

    //���� �������� ������
    BlockData[] blockData;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("char max " + (int)char.MaxValue);
        iniBlockData();

        AllReload();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void iniBlockData() {
        //����� ������������ ������� ������ �������
        //M = Mod | N - nameBlock | 
        //[MMMNNN]

        blockMax = char.MaxValue * (charsMod + charsName);

        blockData = new BlockData[blockMax];

        //������ ���� ��������� ��� �����
    }

    static public uint GetBlockID(string ModName, string BlockName) {
        //�������� ��������� ����������
        string abbreviatura = "";
        //������ 3 ������� ����
        for (int num = 0; num < charsMod; num++) {
            //���� ������� ������ ������ ��� ���
            if (num >= ModName.Length)
                abbreviatura += "_";
            else abbreviatura += ModName[num];
        }

        //������ 3 ������� �����
        for (int num = 0; num < charsMod; num++)
        {
            //���� ������� ������ ������ ��� ���
            if (num >= BlockName.Length)
                abbreviatura += "_";
            else abbreviatura += BlockName[num];
        }

        //������ ���� ����������� �������� ID
        uint abbreviaturaID = 0;
        //���������� ������ ������
        for (int num = 0; num < abbreviatura.Length; num++) {
            abbreviaturaID += abbreviatura[num];
        }

        return abbreviaturaID;
    }

    void AllReload()
    {
        //��������� ���� �� ����� ���
        if (!Directory.Exists(pathMod)) {
            //����� ��� ������ ����
            Directory.CreateDirectory(pathMod);
        }

        //����������� ��� ����� �����
        string[] directoriesMod = Directory.GetDirectories(pathMod);
        foreach (string directoryMod in directoriesMod) {
            loadMod(directoryMod);
        }
    }

    void loadMod(string pathMod) {
        //��������� ������ ������
        string[] directoriesBlock = Directory.GetDirectories(pathMod);
        foreach (string blockDirectory in directoriesBlock) {
            BlockData.LoadDatas(blockDirectory);
        }

        bool test = false;
    }
}
