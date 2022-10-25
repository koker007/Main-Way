using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//������ ������ � �������� � ������� ������ ���-�� ��������� ������� ������� � ����� �������� ���� ������������ ��������
public class GenPositions : MonoBehaviour
{
    public static GenPositions main;

    /*������ ������� ������� ������� ���������*/
    List<GenPos> genPosList = new List<GenPos>();

    class GenPos {
        /*������� � ���������*/
        Vector3 galaxy;
        /*������� ���� � �������� ������ ������� ������� ��������� ���������*/
        string worldNum;
        /*������� ������ ����*/
        Vector3 worldPos;


        public void SetData(Vector3 galaxyPos, string worldNumStr, Vector3 worldPos) {
            galaxy = galaxyPos;
            worldNum = worldNumStr;
            this.worldPos = worldPos;
        }

        /*��������� ������� �� ������������� ���������*/
        public void Generate() {
            //��������� ������� ������� �� ������������� ���������

        }
    }

    [Server]
    void TestServerGenPosList() {
        //������� ���� �� ������
        if (!NetworkCTRL.main.isServer) 
            return;

        List<GenPos> genPosListNew = new List<GenPos>();
        foreach (GenPos genPos in genPosList) {
            if (genPos == null) 
                continue;

            genPos.Generate();

            genPosListNew.Add(genPos);
        }

        genPosList = genPosListNew;
    }

    private void FixedUpdate()
    {
        TestServerGenPosList();
    }

    private void Start()
    {
        main = this;
    }

}
