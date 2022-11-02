using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Список сериализованных классов

[System.Serializable]
public struct Vector3S {
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3() {
        return new Vector3(x,y,z);
    }
}

