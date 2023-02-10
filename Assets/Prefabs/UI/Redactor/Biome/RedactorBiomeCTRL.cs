using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedactorBiomeCTRL : MonoBehaviour
{
    static public RedactorBiomeCTRL main;

    [SerializeField]
    Material materialNormal;
    [SerializeField]
    Material materialTransparent;
    [SerializeField]
    Material materialCutOff;

    [SerializeField]
    InputFieldCTRL ModName;
    [SerializeField]
    InputFieldCTRL BiomeName;

    // Start is called before the first frame update
    void Start()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        TestOpenGenerator();
    }

    void TestOpenGenerator() {
        if (RedactorBiomeGenerator.main != null && !RedactorBiomeGenerator.main.gameObject.activeSelf)
            RedactorBiomeGenerator.main.gameObject.SetActive(true);
    }

    public void clickButtonSave()
    {

        /*
        //проверяем что имя мода есть
        if (blockDatas[0].mod == null || blockDatas[0].mod.Length == 0)
        {
            Debug.Log("NotSave Need Mod Name");
            return;
        }
        //проверяем что имя мода больше 3
        if (blockDatas[0].mod.Length < 3)
        {
            Debug.Log("NotSave Need Mod Name Lenght > 3");
            return;
        }
        //Проверяем что имя блока есть
        if (blockDatas[0].name == null || blockDatas[0].name.Length == 0)
        {
            Debug.Log("NotSave Need Block Name");
            return;
        }
        //Проверняем что имя больше 3х символов
        if (blockDatas[0].name.Length < 3)
        {
            Debug.Log("NotSave Need Block Name Lenght > 3");
            return;
        }

        //Применяем общие данные на все блоки
        acceptGroupParameters();

        //Сохраняем все блоки
        for (int num = 0; num < blockDatas.Length; num++)
        {
            BlockData.SaveData(blockDatas[num]);
        }
        */
    }

    public void clickButtonLoad()
    {
        WindowMenuCTRL.CloseALL(true);
        //WindowMenuCTRL.ClickRedactorBlockLoad();
    }
}
