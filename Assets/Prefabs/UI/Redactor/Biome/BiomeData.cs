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

    public static string GetDataPath(string mod, string name)
    {
        //������� ���� � ����� �����
        string path = GameData.GameData.pathMod + "/" + mod + "/" + StrC.biomes + "/" + name;

        return path;
    }
    public string GetDataPath()
    {
        return GetDataPath(mod, name);
    }

    static public void SaveData(BiomeData biomeData) {
        string path = biomeData.GetDataPath();

        //��������� ���� �� �����
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        SaveMain();
        SaveRules();

        void SaveMain() {
            string pathMain = path + "/" + StrC.main + StrC.formatTXT;

            //��������� ��� �����
            //��������� ���� � ��������� ����
            //������� ������ ���� ��� ���� ���������
            List<string> dataList = new List<string>();

            string dataOne = "";
            //���������� ���
            dataOne = StrC.type + StrC.SEPARATOR;
            if (biomeData as BiomeTypeSurface != null)
                dataOne += StrC.TSurface;
            else if (biomeData as BiomeTypeUnderground != null)
                dataOne += StrC.TUnderground;
            else if (biomeData as BiomeTypeDwarf != null)
                dataOne += StrC.TDwarf;
            else if (biomeData as BiomeTypeRings != null)
                dataOne += StrC.TRings;

            dataList.Add(dataOne);

            //���� ����� ��� - �������
            if (!File.Exists(pathMain))
            {
                FileStream fileStream;
                fileStream = File.Create(pathMain);
                fileStream.Close();
            }
            //��������� � ����
            File.WriteAllLines(pathMain, dataList.ToArray());
        }
        void SaveRules() {
            //������� ���� � ����� ������
            string pathRules = path + "/" + StrC.rules;

            //���� ���������� ����, ���������� �� ������� ������� ������
            if (!Directory.Exists(pathRules))
            {
                Directory.CreateDirectory(pathRules);
            }

            //�������� ��� ����� � ������� ��
            string[] files = Directory.GetFiles(pathRules);
            foreach (string file in files)
                File.Delete(file);


            //������������ ��� �������
            for (int num = 0; num < biomeData.genRules.Count; num++) {
                SetRule(biomeData.genRules[num], num);
            }

            void SetRule(GenRule genRule, int num) {
                //������� ���� � �������
                string pathRule = pathRules + "/" + num + StrC.formatTXT;
                if (File.Exists(pathRule))
                    File.Delete(pathRule);

                //�������� ��� �����
                BlockData blockData = GameData.Blocks.GetData(genRule.blockID, 0);
                if (blockData == null)
                {
                    //���� ����� ��� ������ �������
                    blockData = new TypeBlock();
                    blockData.mod = "";
                    blockData.name = "";
                }

                //��������� ���� � ��������� ����
                //������� ������ ���� ��� ���� ���������
                List<string> dataList = new List<string>();

                string data = "";
                //���������� ��� � ��� �����
                data = StrC.blocks + StrC.mod + StrC.SEPARATOR + blockData.mod;
                dataList.Add(data);
                data = StrC.blocks + StrC.name + StrC.SEPARATOR + blockData.name;
                dataList.Add(data);

                data = StrC.perlin + StrC.scale + StrC.SEPARATOR + genRule.scaleAll;
                dataList.Add(data);
                data = StrC.perlin + StrC.octaves + StrC.SEPARATOR + genRule.octaves;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.x + StrC.SEPARATOR + genRule.scaleX;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.y + StrC.SEPARATOR + genRule.scaleY;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.z + StrC.SEPARATOR + genRule.scaleZ;
                dataList.Add(data);
                data = StrC.perlin + StrC.scale + StrC.frequency + StrC.SEPARATOR + genRule.freq;
                dataList.Add(data);

                //��������� � ����
                File.WriteAllLines(pathRule, dataList.ToArray());
            }
        }
    }
    static public BiomeData LoadData(string pathBiome) {

        //��������� ���������� �� �����
        if (!Directory.Exists(pathBiome))
        {
            Debug.Log(pathBiome + " Not exist");
            return null;
        }

        string mod = "";
        string name = "";

        BiomeData biomeData = null;

        LoadMain();

        //����������� ������ ���� ��� ��������� ��� �����
        if (biomeData == null)
            return null;

        biomeData.mod = mod;
        biomeData.name = name;

        //���� ���� ���-�� �� �����������, �� ������
        if (!LoadRules())
            return null;

        return biomeData;

        void LoadMain() {
            //����������� ����
            string[] pathParts1 = pathBiome.Split("/");

            List<string> pathList = new List<string>();
            foreach (string pathCut in pathParts1)
            {
                string[] pathParts2 = pathCut.Split("\\");
                foreach (string part in pathParts2)
                {
                    pathList.Add(part);
                }
            }

            string[] pathMass = pathList.ToArray();


            if (pathMass.Length <= 3)
            {
                pathMass = pathBiome.Split("\\");
            }

            if (pathMass.Length <= 3)
            {
                Debug.LogError(pathBiome + " load name error");
                return;
            }

            mod = pathMass[pathMass.Length - 4];
            name = pathMass[pathMass.Length - 1];

            //����� ��������� ���� � ��������� �������
            loadMainTXT();

            void loadMainTXT()
            {
                string pathMainStr = pathBiome + "/" + StrC.main + StrC.formatTXT;

                //��������� ������������� �����
                if (!File.Exists(pathMainStr))
                {
                    //����� ���, ������
                    Debug.LogError("File main.txt not exist " + pathMainStr);
                    return;
                }

                //����������� ������ �����
                string[] datasStr = File.ReadAllLines(pathMainStr);

                //��������� ��� ������ �� ������
                foreach (string dataStr in datasStr)
                {
                    string[] data = dataStr.Split(StrC.SEPARATOR);

                    if (data.Length > 2)
                    {
                        Debug.LogError("Bad parametr: " + dataStr + " in " + pathMainStr);
                        continue;
                    }

                    GetType(data[0], data[1]);
                }
                //////////////////////////////////////////////////////////////////////////////////////
                ///

                void GetType(string name, string value)
                {
                    if (name == StrC.type)
                    {
                        if (value == StrC.TSurface)
                            biomeData = new BiomeTypeSurface();
                        else if (value == StrC.TUnderground)
                            biomeData = new BiomeTypeUnderground();
                        else if (value == StrC.TDwarf)
                            biomeData = new BiomeTypeDwarf();
                        else if (value == StrC.TDwarf)
                            biomeData = new BiomeTypeRings();
                        else
                            Debug.LogError("Bad parametr of " + name + ": " + value);

                    }
                }
            }
        }
        bool LoadRules() {
            //������� ���� � ����� ������
            string pathRule = pathBiome + "/" + StrC.rules;

            if (!Directory.Exists(pathRule))
                return false;

            //�������� ��� ����� � ���������
            string[] pathRulesAll = Directory.GetFiles(pathRule);

            //������������ ���
            List<GenRule> ruleDatas = new List<GenRule>();
            foreach (string path in pathRulesAll) {
                GetRule(path);
            }

            biomeData.genRules = ruleDatas;

            return true;

            void GetRule(string path) {
                //����������� ����� �������
                string[] spliters = new string[3];
                spliters[0] = "/";
                spliters[1] = "\\";
                spliters[2] = StrC.formatTXT;

                string[] pathArray = path.Split(spliters, System.StringSplitOptions.RemoveEmptyEntries);

                string[] DatasStr = File.ReadAllLines(path);
                GenRule genRule = new GenRule();

                string mod = "";
                string name = "";

                foreach (string DataStr in DatasStr) {
                    SetData(DataStr);
                }

                genRule.blockID = GameData.Blocks.GetBlockID(mod, name);

                //������� �������, ���������
                ruleDatas.Add(genRule);

                void SetData(string DataStr) {
                    string[] data = DataStr.Split(StrC.SEPARATOR);

                    if (data.Length != 2) {
                        Debug.LogError("Bad biome data:" + path);
                        return;
                    }

                    switch (data[0]) {
                        case StrC.blocks + StrC.mod:
                            mod = data[1];
                            break;
                        case StrC.blocks + StrC.name:
                            name = data[1];
                            break;
                        case StrC.perlin + StrC.scale:
                            genRule.scaleAll = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.octaves:
                            genRule.octaves = System.Convert.ToInt32(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.x:
                            genRule.scaleX = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.y:
                            genRule.scaleY = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.scale + StrC.z:
                            genRule.scaleZ = (float)System.Convert.ToDouble(data[1]);
                            break;
                        case StrC.perlin + StrC.frequency:
                            genRule.freq = (float)System.Convert.ToDouble(data[1]);
                            break;
                    }
                }
            }
        }
    }
}
