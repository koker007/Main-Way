using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//�������� �� �������� ���� ������ � ����
public class GameData : MonoBehaviour
{
    int blockMax = 100000;
    //���������� �������� �������������� � ����
    int charsMod = 3;
    //���������� �������� �����
    int charsName = 3;
    //������ ����� �������� �������
    int charsNameSum = 1;

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
        //[MMMNNNn]

        blockMax = char.MaxValue * charsMod * charsName * charsNameSum;

        blockData = new BlockData[blockMax];

        //������ ���� ��������� ��� �����
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
        
        }

        bool test = false;
    }
}
