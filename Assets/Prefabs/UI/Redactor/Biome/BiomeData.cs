using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    public string name;
    public string mod;

    //������ ������ ��� ��������� ������ � �����
    public List<GenBiomeRules> ListOfRules = new List<GenBiomeRules>();

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
    }

    public class GenBiomeRules
    {
        bool inicialized = false;
        public bool INICIALIZED { get { return inicialized; } }

        //���� ����� � �������� ����������� ������� ���������
        string blockMod;
        string blockName;
        int blockID = -1; //id ����� ����������� � �������� �������� ����

        List<GenRules> genRules; //������ ������ ��������� ����� ����� � �����

        public void ReInicialize() {
            ReInicialize(blockMod, blockName);
        }
        public void ReInicialize(string blockMod, string blockName) {
            //����� �������������
            inicialized = false;

            //���������� ��� �����
            this.blockMod = blockMod;
            this.blockName = blockName;

            //�������� ID �����
            blockID = GameData.Blocks.GetBlockID(this.blockMod, this.blockName);

            //if block not exist - exit
            if (blockID < 0)
                return;

            //if have 1 or more gen rules
            if (genRules != null && genRules.Count > 0)
                return;

            inicialized = true;
        }
    }
}
