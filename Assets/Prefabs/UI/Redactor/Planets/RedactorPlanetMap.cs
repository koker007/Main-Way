using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedactorPlanetMap : MonoBehaviour
{
    [SerializeField]
    RedactorPlanetsCTRL redactor;

    [SerializeField]
    RawImage imageMap;
    [SerializeField]
    Image imageAtmosphere;

    [SerializeField]
    SliderCTRL quarity;
    [SerializeField]
    SliderCTRL PosX;

    // Start is called before the first frame update
    void Start()
    {
        inicialize();
    }

    void inicialize() {
        redactor = gameObject.GetComponentInParent<RedactorPlanetsCTRL>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdatePlanet();
    }

    //�������� �������� �����
    void UpdatePlanet() {
        //���� � ��������� ��� ������ �������, �������
        if (redactor.planetObjData == null)
            return;

        TestAtmosphere();
        TestMap();

        //��������� ���������
        void TestAtmosphere() {
            imageAtmosphere.color = redactor.planetObjData.color;
            RectTransform rectTransform = imageAtmosphere.gameObject.GetComponent<RectTransform>();
            rectTransform.offsetMax = new Vector2(redactor.planetObjData.atmosphere, redactor.planetObjData.atmosphere);
            rectTransform.offsetMin = new Vector2(-redactor.planetObjData.atmosphere, -redactor.planetObjData.atmosphere);
        }

        void TestMap() {
            imageMap.texture = redactor.planetObjData.GetMainTexture((Size)quarity.slider.value);
        }
    }
}
