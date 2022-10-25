using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollContentScaler : MonoBehaviour
{
    [SerializeField]
    RectTransform Content;
    [SerializeField]
    int space = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateContent();
    }

    void UpdateContent() {
        //Получаем детей
        RectTransform[] childs = Content.GetComponentsInChildren<RectTransform>();

        float heightNow = 0;

        //Перебираем детей
        for (int num = 0; num < childs.Length; num++) {
            if (childs[num].transform.parent != Content.transform)
                continue;

            heightNow -= space/2;

            childs[num].localPosition = new Vector3(childs[num].localPosition.x, heightNow, 0);

            heightNow -= childs[num].sizeDelta.y;

            heightNow -= space/2;
        }

        Content.sizeDelta = new Vector2(Content.sizeDelta.x, Mathf.Abs(heightNow));
    }
}
