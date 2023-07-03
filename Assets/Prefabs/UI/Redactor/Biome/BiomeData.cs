using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class BiomeData
{
    //Биом проинициализирован или нет
    bool isInicialized = false;
    public bool IsInicialized { get { return isInicialized; } }

    public string name;
    public string mod;

    //Список ID блоков в этом биоме и их правила генерации
    public List<GenRule> genRules = new List<GenRule>();


    //Конструктор
    public BiomeData() {
        //Должно быть хотябы одно правило генерации
        genRules.Add(new GenRule());
    }

    public virtual void save() {
        //Получаем путь к папке
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
    /// Возвращает путь к папке текущего биома
    /// </summary>
    protected string getPathFolder() {
        //Создаем путь к папке биома
        string path = GameData.GameData.pathMod + "/" + mod + "/" + StrC.biomes + "/" + name;
        return path;
    }

    //одно правило генерации блока
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

    //Набор правил для генерации одного конкретного блока
    public class BiomeRuleData
    {
        bool inicialized = false;
        public bool INICIALIZED { get { return inicialized; } }

        //имя блока к которому применяются правила генерации
        string blockMod;
        string blockName;
        int blockID = -1; //id блока вычисляемый в процессе загрузки игры

        GenRule genRule; //правило генерации этого блока в биоме

        public void ReInicialize() {
            ReInicialize(blockMod, blockName);
        }
        public void ReInicialize(string blockMod, string blockName) {
            //Сброс инициализации
            inicialized = false;

            //Запоминаем имя блока
            this.blockMod = blockMod;
            this.blockName = blockName;

            //Получаем ID блока
            blockID = GameData.Blocks.GetBlockID(this.blockMod, this.blockName);

            //if block not exist - exit
            if (blockID < 0)
                return;

            inicialized = true;
        }
    }

    
}
