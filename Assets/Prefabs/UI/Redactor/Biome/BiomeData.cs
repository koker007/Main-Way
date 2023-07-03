using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class BiomeData
{
    //���� ������������������ ��� ���
    bool isInicialized = false;
    public bool IsInicialized { get { return isInicialized; } }

    public string name;
    public string mod;

    //������ ID ������ � ���� ����� � �� ������� ���������
    public List<GenRule> genRules = new List<GenRule>();


    //�����������
    public BiomeData() {
        //������ ���� ������ ���� ������� ���������
        genRules.Add(new GenRule());
    }

    public virtual void save() {
        //�������� ���� � �����
        string path = getPathFolder();
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        

    }
    static BiomeData load(string pathBiome)
    {
        BiomeData data = null;



        return data;
    }

    /// <summary>
    /// ���������� ���� � ����� �������� �����
    /// </summary>
    protected string getPathFolder() {
        //������� ���� � ����� �����
        string path = GameData.GameData.pathMod + "/" + mod + "/" + StrC.biomes + "/" + name;
        return path;
    }

    //���� ������� ��������� �����
    public class GenRule {
        public int blockID = 0;

        //Perlin
        public float scaleAll = 16;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;
        public float freq = 3;
        public int octaves = 1;
    }

    //����� ������ ��� ��������� ������ ����������� �����
    public class BiomeRuleData
    {
        bool inicialized = false;
        public bool INICIALIZED { get { return inicialized; } }

        //��� ����� � �������� ����������� ������� ���������
        string blockMod;
        string blockName;
        int blockID = -1; //id ����� ����������� � �������� �������� ����

        GenRule genRule; //������� ��������� ����� ����� � �����

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

            inicialized = true;
        }
    }

    
}
