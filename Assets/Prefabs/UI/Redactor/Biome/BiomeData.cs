using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    //Ѕиом проинициализирован или нет
    bool isInicialized = false;
    public bool IsInicialized { get { return isInicialized; } }

    public string name;
    public string mod;

    //—писок ID блоков в этом биоме и их правила генерации
    public List<int> blockIDs = new List<int>();
    public List<GenRule> genRules = new List<GenRule>();

    //одно правило генерации блока
    public class GenRule {

        //parameters for world size 4096
        //Perlin
        public float scaleAll = 16;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;
        public float freq = 3;
        public int octaves = 1;
    }

    //Ќабор правил дл€ генерации одного конкретного блока
    public class GenBiomeRule
    {
        bool inicialized = false;
        public bool INICIALIZED { get { return inicialized; } }

        //им€ блока к которому примен€ютс€ правила генерации
        string blockMod;
        string blockName;
        int blockID = -1; //id блока вычисл€емый в процессе загрузки игры

        GenRule genRule; //правило генерации этого блока в биоме

        public void ReInicialize() {
            ReInicialize(blockMod, blockName);
        }
        public void ReInicialize(string blockMod, string blockName) {
            //—брос инициализации
            inicialized = false;

            //«апоминаем им€ блока
            this.blockMod = blockMod;
            this.blockName = blockName;

            //ѕолучаем ID блока
            blockID = GameData.Blocks.GetBlockID(this.blockMod, this.blockName);

            //if block not exist - exit
            if (blockID < 0)
                return;

            inicialized = true;
        }
    }
}
