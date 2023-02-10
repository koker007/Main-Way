using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gererate the example biome, use chanks
public class RedactorBiomeGenerator : MonoBehaviour
{
    static public RedactorBiomeGenerator main;

    [Header("Obj")]
    [SerializeField]
    GameObject chanksParent;
    [SerializeField]
    GameObject cameraObj;

    [SerializeField]
    GameObject chankPrefab;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestClose();
    }

    void TestClose() {
        //Если меню нет или оно не активно
        if (RedactorBiomeCTRL.main == null ||
            !RedactorBiomeCTRL.main.gameObject.activeSelf) {
            //закрываем генератор
            gameObject.SetActive(false);
        }
        
    }
}
