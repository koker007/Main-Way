using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//�������� �� �������� ���� ������ � ����
namespace GameData
{
    public class GameData : MonoBehaviour
    {
        //���������� �������� �������������� � ����
        public const int charsMod = 3;
        //���������� �������� �����
        public const int charsName = 3;

        static public string pathMod = "Mods";

        static public string nameBlock = "Blocks"; //��� ����� ������
        static public string nameRecipes = "Recipes"; //��� ����� �������� �������


        // Start is called before the first frame update
        void Start()
        {
            //Debug.Log("char max " + (int)char.MaxValue);
            Blocks.Ini();

            AllReload();
        }

        // Update is called once per frame
        void Update()
        {

        }

        static public uint GetBlockID(string ModName, string BlockName)
        {
            //�������� ��������� ����������
            string abbreviatura = "";
            //������ 3 ������� ����
            for (int num = 0; num < charsMod; num++)
            {
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
            for (int num = 0; num < abbreviatura.Length; num++)
            {
                abbreviaturaID += abbreviatura[num];
            }

            return abbreviaturaID;
        }

        void AllReload()
        {
            //��������� ���� �� ����� ���
            if (!Directory.Exists(pathMod))
            {
                //����� ��� ������ ����
                Directory.CreateDirectory(pathMod);
            }

            //����������� ��� ����� �����
            string[] directoriesMod = Directory.GetDirectories(pathMod);
            foreach (string directoryMod in directoriesMod)
            {
                loadMod(directoryMod);
            }
        }

        void loadMod(string pathMod)
        {
            //��������� ������ ������
            string[] directoriesBlock = Directory.GetDirectories(pathMod);
            foreach (string blockDirectory in directoriesBlock)
            {
                BlockData[] datas = BlockData.LoadDatas(blockDirectory);

                foreach (BlockData data in datas) {
                    Blocks.SetData(data);
                }
            }

            bool test = false;
        }
    }


    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// /// /////////////////////////////////////////////////////      Blocks data      ////////////////////////////////////////////////////////////
    /// /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    static class Blocks {
        const int tryingMAX = 100;
        static int countLoaded = 0;


        static int blockMax = 100000;
        //���� �������� ������
        static BlockData[] blockData;

        static public void Ini()
        {
            //����� ������������ ������� ������ �������
            //M = Mod | N - nameBlock | 
            //[MMMNNN]

            blockMax = char.MaxValue * (GameData.charsMod + GameData.charsName);

            blockData = new BlockData[blockMax];

            //������ ���� ��������� ��� �����
        }

        static private int GetBlockFirstID(string ModName, string BlockName)
        {
            //�������� ��������� ����������
            string abbreviatura = "";
            //������ 3 ������� ����
            for (int num = 0; num < GameData.charsMod; num++)
            {
                //���� ������� ������ ������ ��� ���
                if (num >= ModName.Length)
                    abbreviatura += "_";
                else abbreviatura += ModName[num];
            }

            //������ 3 ������� �����
            for (int num = 0; num < GameData.charsName; num++)
            {
                //���� ������� ������ ������ ��� ���
                if (num >= BlockName.Length)
                    abbreviatura += "_";
                else abbreviatura += BlockName[num];
            }

            //������ ���� ����������� �������� ID
            int abbreviaturaID = 0;
            //���������� ������ ������
            for (int num = 0; num < abbreviatura.Length; num++)
            {
                abbreviaturaID += abbreviatura[num] + (int)Mathf.Pow(4, num);
            }

            return abbreviaturaID;
        }

        //�������� ����������� ID �����, ���� �� ��������
        static public int GetBlockID(string ModName, string BlockName, bool onlyExistData = true) {
            //�������� ��������� id
            int idStart = GetBlockFirstID(ModName, BlockName);

            int idNow = idStart;


            for (int num = 0; num < tryingMAX; num++, idNow++)
            {
                //��������� ��� �� ����� �� ������� �������
                if (idNow >= blockData.Length)
                    idNow = 0;

                //��������� �� id ��� ���� ���� � � ���� ��������� ��� � ���
                if (blockData[idNow] != null && blockData[idNow].mod == ModName && blockData[idNow].name == BlockName)
                    return idNow;


            }

            if (!onlyExistData) {
                for (int num = 0; num < tryingMAX; num++, idNow++)
                {
                    //��������� ��� �� ����� �� ������� �������
                    if (idNow >= blockData.Length)
                        idNow = 0;

                    //��������� �� id ��� ���� ���� � � ���� ��������� ��� � ���
                    if (blockData[idNow] == null || blockData[idNow].mod == ModName && blockData[idNow].name == BlockName)
                        return idNow;

                }
            }

            return -1;
        }

        //�������� ���������� � ����� �� ��� id
        static public BlockData GetData(int blockID) {
            return blockData[blockID];
        }
        static public BlockData GetData(string ModName, string BlockName) {
            //�������� ID
            int id = GetBlockID(ModName, BlockName);
            
            if (id < 0)
                return null;

            return blockData[id];
        }

        //�������� ������ ����� � ������
        static public void SetData(BlockData data) {
            if (data == null)
                return;

            int id = GetBlockID(data.mod, data.name, false);

            if (id < 0)
                Debug.LogError("Not can to load block data: " + data.mod + " " + data.name);

            blockData[id] = data;
            countLoaded++;
        }
    }
}