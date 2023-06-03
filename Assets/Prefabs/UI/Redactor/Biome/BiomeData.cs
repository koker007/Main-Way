using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    public string name;
    public string mod;
    public int variant;

    //������ ������ ��� ��������� ������ � �����
    public List<GenRules> ListOfRules = new List<GenRules>();

    //������� ��������� �����
    public class GenRules {

        //parameters for world size 4096
        //Perlin
        public float scaleAll = 16;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;
        public float freq = 3;
        public int octaves = 1;

        //������ ��������� �� ������ ���� 0% ���� � 100% ������ �����������.
        public float distGenCoreMax = 100;
        public float distGenCoreMin = 0;
    }

    public class GenBlockRules
    {
        //���� ����� � �������� ����������� ������� ���������
        string blockName;
        string blockMod;

        List<GenRules> genRules;

        public uint GetBlockID() {
            //����� �������� ID ����� �� ����������, �� ���� ������������ ������ � �����.

            throw new System.NotImplementedException();

            
        }
    }
}
