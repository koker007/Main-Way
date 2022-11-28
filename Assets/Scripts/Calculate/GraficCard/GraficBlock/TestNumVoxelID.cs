using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNumVoxelID : MonoBehaviour
{
    [SerializeField]
    int sectorX = 0;
    [SerializeField]
    int sectorY = 0;
    [SerializeField]
    int sectorZ = 0;
    [SerializeField]
    int idX = 0;
    [SerializeField]
    int idY = 0;
    [SerializeField]
    int idZ = 0;

    [SerializeField]
    int idGlobal = 0;

    [SerializeField]
    public int formIDMaximum = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        CalcIDGlobal();
    }

    void CalcIDGlobal() {
        idGlobal = idX + (sectorX * 8) + idY * 16 + (sectorY * 16 * 8) + idZ * 16 * 16 + (sectorZ * 16 * 16 * 8);
    }
}
