using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Game.Space;

//�������� �� �������� ���� ������ � ����
namespace Game
{
    public class GameData : MonoBehaviour
    {
        static public GameData main;
        //���������� �������� �������������� � ����
        public const int charsMod = 3;
        //���������� �������� �����
        public const int charsName = 3;

        static public string pathMod = "Mods";

        static public string nameBlock = "Blocks"; //��� ����� ������
        static public string nameRecipes = "Recipes"; //��� ����� �������� �������

        [SerializeField]
        public ChankGO prefabChankGO;
        [SerializeField]
        public PlanetGO prefabPlanetGO;

        // Start is called before the first frame update
        void Start()
        {
            main = this;
            Blocks.Ini();

            AllReload();
        }

        // Update is called once per frame
        void Update()
        {
            TestGarbageCollector();
        }
        /*
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
        */
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
            //��������� �����
            loadModBlocks(pathMod);

            //��������� �����

        }

        void loadModBlocks(string pathMod) {

            //��������� ������ ����������� ����
            string pathModBlocks = pathMod + "\\" + StrC.blocks;

            //��������� ��� ����� ������ ����������
            if (!Directory.Exists(pathModBlocks))
                return;

            //�������� ������ ������
            string[] pathsBlockdata = Directory.GetDirectories(pathModBlocks);

            //��������� ��� �����
            foreach (string pathBlockData in pathsBlockdata) {
                BlockData[] BlockVariants = BlockData.LoadDatas(pathBlockData);

                Blocks.SetData(BlockVariants);
            }
        }


        void TestGarbageCollector() {
            if (!GarbageCollector.isTimeToClear())
                return;

            GarbageCollector.GC.Execute();
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
        static BlockData[][] blockData;

        static public void Ini()
        {
            //����� ������������ ������� ������ �������
            //M = Mod | N - nameBlock | 
            //[MMMNNN]

            blockMax = char.MaxValue * (GameData.charsMod + GameData.charsName);

            blockData = new BlockData[blockMax][];

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
                abbreviaturaID += abbreviatura[num] * (int)Mathf.Pow(5, num);
            }

            while (abbreviaturaID >= blockData.Length) {
                abbreviaturaID -= blockData.Length;
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
                    idNow -= blockData.Length;

                //��������� �� id ��� ���� ���� � � ���� ��������� ��� � ���
                if (blockData[idNow] != null && blockData[idNow][0].mod == ModName && blockData[idNow][0].name == BlockName)
                    return idNow;


            }

            if (!onlyExistData) {
                //���������� � ������
                idNow = idStart;
                for (int num = 0; num < tryingMAX; num++, idNow++)
                {
                    //��������� ��� �� ����� �� ������� �������
                    if (idNow >= blockData.Length)
                        idNow = 0;

                    //��������� �� id ��� ���� ���� � � ���� ��������� ��� � ���
                    if (blockData[idNow] == null || blockData[idNow][0].mod == ModName && blockData[idNow][0].name == BlockName)
                        return idNow;

                }
            }

            return -1;
        }

        static public BlockData GetData(string ModName, string BlockName, uint variant)
        {
            //�������� ID
            int id = GetBlockID(ModName, BlockName);

            return GetData(id, variant);
        }
        //�������� ���������� � ����� �� ��� id
        static public BlockData GetData(int blockID, uint variant) {
            BlockData[] blockVariants = GetDatas(blockID);

            //���� ����� �� ����������, �������
            if (blockVariants == null)
                return null;

            //���� ������� �� ����������
            if (variant >= blockVariants.Length)
                variant = 0;

            return blockVariants[variant];
        }
        static public BlockData[] GetDatas(int blockID)
        {
            return blockData[blockID];
        }
        static public BlockData[] GetDatas(string ModName, string BlockName) {
            //�������� ID
            int id = GetBlockID(ModName, BlockName);
            
            if (id < 0)
                return null;

            return blockData[id];
        }

        //�������� ������ ����� � ������
        static public void SetData(BlockData[] data) {
            if (data == null)
                return;

            int id = GetBlockID(data[0].mod, data[0].name, false);

            if (id < 0)
                Debug.LogError("Not can to load block data: " + data[0].mod + " " + data[0].name);

            blockData[id] = data;
            countLoaded++;
        }

        static public Color GetColor(int blockID) {
            Color result = new Color(1,1,1,0);

            BlockData block = GetData(blockID, 0);

            //���� ������ 10 �� ��� �������� �����
            if(blockID < 10)
               return result;

            return block.GetColor();
        }
    }
}