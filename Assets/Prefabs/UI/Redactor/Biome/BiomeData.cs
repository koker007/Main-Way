using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData
{
    public string name;
    public string mod;
    public int variant;

    //Список правил для генерации блоков в биоме
    List<GenRules> ListOfRules = new List<GenRules>();

    public enum Type {
        underground = 0,
        surface = 1
    }

    //правила генерации блока
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

        //высота генерации от уровня ядра 0% ядро и 100% высота поверхности.
        public float distGenCoreMax = 100;
        public float distGenCoreMin = 0;

        //только поверхностный биом
        public float distGenGround = 5;
    }
}
