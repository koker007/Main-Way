using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDInventoryCell : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;

    [SerializeField]
    Image itemImage;
    [SerializeField]
    RawImage itemTexture;
    
    //����� ������� ��������� ������ ��������
    //����� ����� ����� ��������� ������������ ��� ������



    // Start is called before the first frame update
    void Start()
    {
        
    }

    void inizialize() {
        rect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
