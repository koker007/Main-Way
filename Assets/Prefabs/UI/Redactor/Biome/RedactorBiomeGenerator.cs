using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gererate the example biome, use chanks
public class RedactorBiomeGenerator : MonoBehaviour
{
    static private RedactorBiomeGenerator main;
    static public RedactorBiomeGenerator MAIN { get { return main; } }


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
        Generate();


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

    void Generate() {
        
    }
}
