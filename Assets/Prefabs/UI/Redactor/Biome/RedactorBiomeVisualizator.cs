using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBiomeVisualizator : MonoBehaviour
{
    static private RedactorBiomeVisualizator main;

    static public RedactorBiomeVisualizator MAIN { get { return main; }  }



    [SerializeField]
    RedactorBiomeGenerator Generator;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Tests();
    }

    void Tests() {

        generatorTest();


        //¬ключить если выключеннр
        void generatorTest()
        {

            if (!(Generator ??= RedactorBiomeGenerator.MAIN))
                return;

            if (!Generator.gameObject.activeSelf)
                Generator.gameObject.SetActive(true);


        }
    }
}
