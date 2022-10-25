using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ѕанель инвентар€ быстрого доступа
public class HUDInventoryFast : MonoBehaviour
{
    RectTransform inventoryFastRect;

    int itemNumMax = 9;

    [Header("Prefabs")]
    [SerializeField]
    HUDInventoryCell PrefabCell;

    [SerializeField]
    RectTransform SelectZone;

    //—писок €чеек быстрого инвентар€
    List<HUDInventoryCell> cells = new List<HUDInventoryCell>();



    // Start is called before the first frame update
    void Start()
    {
        inicialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelect();
    }

    void UpdateSelect() {
        if (!PlayerCTRL.local)
            return;

        int itemMax = cells.Count;
        //проверка на следующий или предыдущий предмет
        if (Input.mouseScrollDelta.y > 0 || Input.GetKeyDown(Controls.keyItemNext)) {
            PlayerCTRL.local.ItemSelect++;
            if (PlayerCTRL.local.ItemSelect > itemMax)
                PlayerCTRL.local.ItemSelect = 0;
        }
        else if (Input.mouseScrollDelta.y < 0 || Input.GetKeyDown(Controls.keyItemBack)) {
            PlayerCTRL.local.ItemSelect--;
            if (PlayerCTRL.local.ItemSelect < 0)
                PlayerCTRL.local.ItemSelect = itemMax;
        }

        //проверка на выбор конкретного предмета
        else if (Input.GetKeyDown(Controls.keyItem1))
            PlayerCTRL.local.ItemSelect = 1 -1;
        else if (Input.GetKeyDown(Controls.keyItem2))
            PlayerCTRL.local.ItemSelect = 2 -1;
        else if (Input.GetKeyDown(Controls.keyItem3))
            PlayerCTRL.local.ItemSelect = 3 -1;
        else if (Input.GetKeyDown(Controls.keyItem4))
            PlayerCTRL.local.ItemSelect = 4 -1;
        else if (Input.GetKeyDown(Controls.keyItem5))
            PlayerCTRL.local.ItemSelect = 5 -1;
        else if (Input.GetKeyDown(Controls.keyItem6))
            PlayerCTRL.local.ItemSelect = 6 - 1;
        else if (Input.GetKeyDown(Controls.keyItem7))
            PlayerCTRL.local.ItemSelect = 7 - 1;
        else if (Input.GetKeyDown(Controls.keyItem8))
            PlayerCTRL.local.ItemSelect = 8 - 1;
        else if (Input.GetKeyDown(Controls.keyItem9))
            PlayerCTRL.local.ItemSelect = 9 - 1;

        if (PlayerCTRL.local.ItemSelect > itemMax)
            PlayerCTRL.local.ItemSelect = 0;
        else if (PlayerCTRL.local.ItemSelect < 0)
            PlayerCTRL.local.ItemSelect = itemMax;

        SelectZone.pivot = new Vector2(-PlayerCTRL.local.ItemSelect, 0);
    }

    void inicialize() {
        inventoryFastRect = gameObject.GetComponent<RectTransform>();
    }
}
