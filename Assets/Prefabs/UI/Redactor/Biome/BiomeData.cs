using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    public string name;
    public string mod;
    public int variant;

    //������ ������ ��� ��������� ������ � �����
    List<GenRules> ListOfRules = new List<GenRules>();

    public enum Type {
        underground = 0,
        surface = 1
    }

    //������� ��������� �����
    public class GenRules {
        public string blockName;
        public string blockMod;

        //only surface
        public int floorGround;

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

        //������ ������������� ����
        public float distGenGround = 5;
    }
}
