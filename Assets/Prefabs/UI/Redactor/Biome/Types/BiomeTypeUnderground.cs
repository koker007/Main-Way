using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BiomeTypeUnderground data
//ќпредел€ет поведение дл€
//подземного биома
public class BiomeTypeUnderground : BiomeData
{
    //высота генерации от уровн€ €дра 0% €дро и 100% высота поверхности.
    public float distGenCoreMax = 100;
    public float distGenCoreMin = 0;

    public float tempGenMax = 100;
    public float tempGenMin = 0;

    static public BiomeTypeUnderground GetTestBiome()
    {
        throw new System.NotImplementedException();
    }
}
