using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chank
{
    //Если мзменять размер чанка то кратное 2; то есть 10 - 20 - 40 - 80 - 160
    const int MaxSize = 10;

    public uint[,,] BlocksID = new uint[MaxSize, MaxSize, MaxSize];

}
