using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovePlanet : MonoBehaviour
{
    [SerializeField]
    GameObject planet;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject SkyBox;
    [SerializeField]
    Material shadowMat;

    [SerializeField]
    float GradX = 180;
    [SerializeField]
    float GradZ = 360;

    Vector3 playerMoveVector = new Vector3(1, 1, 1);

    Texture2D shadowTexture;
    [SerializeField]
    //положение солнца
    Vector2 sunPos = new Vector2(0.0f, 0.5f);

    void Start()
    {
        iniPlayer();
    }

    void iniPlayer() {
        player.transform.localPosition = new Vector3(planet.transform.localScale.x/2, 0, planet.transform.localScale.z / 2);
    }

    // Update is called once per frame
    void Update()
    {
        RotateSun();
        TestSun();

        

        MovePlayer();
        RotateSkyBox();
        GenSunTexture();
    }

    void MovePlayer() {
        if (Input.GetKey(KeyCode.Keypad8)) {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x, 0, player.transform.localPosition.z + Time.unscaledDeltaTime * playerMoveVector.z);
        }
        if (Input.GetKey(KeyCode.Keypad5))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x, 0, player.transform.localPosition.z - Time.unscaledDeltaTime * playerMoveVector.z);
        }
        if (Input.GetKey(KeyCode.Keypad6))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x + Time.unscaledDeltaTime, 0, player.transform.localPosition.z);
        }
        if (Input.GetKey(KeyCode.Keypad4))
        {
            player.transform.localPosition = new Vector3(player.transform.localPosition.x - Time.unscaledDeltaTime, 0, player.transform.localPosition.z);
        }

        Vector3 pos = player.transform.localPosition;

        //Вертикальное движение
        if (pos.z > planet.transform.localScale.z)
        {
            float offset = planet.transform.localScale.z - pos.z;

            pos.z = planet.transform.localScale.z - offset;
            pos.x += planet.transform.localScale.x / 2;
            //Меняем направление движения
            playerMoveVector.z *= -1;
        }
        else if (pos.z < 0)
        {
            float offset = pos.z * -1;
            pos.z += offset;
            pos.x += planet.transform.localScale.x / 2;

            //Меняем направление движения
            playerMoveVector.z *= -1;
        }

        //горизонтальное движение
        if (pos.x > planet.transform.localScale.x)
        {
            pos.x -= planet.transform.localScale.x;
        }
        else if (pos.x < 0)
        {
            pos.x += planet.transform.localScale.x;
        }

        player.transform.localPosition = pos;
    }

    void RotateSkyBox() {

        Vector3 playerPos = player.transform.localPosition;
        Vector3 planetScale = planet.transform.localScale;

        //Позиция игрока в коофиценте
        float coofX = playerPos.x / planetScale.x;
        float coofZ = playerPos.z / planetScale.z;

        float rotX = 0;
        float rotZ = 0;
        float rotY = 0;

        //Вычисляем градусы по x
        //rotX = GradX * coofZ;
        //rotZ = GradZ * coofX + GradX * coofZ;

        //rotY = GradX * coofZ;

        rotX = -GradX * coofZ + GradX/2;
        rotZ = GradZ * coofX;

        //rotY = GradX * coofZ;

        Quaternion rot = Quaternion.Euler(rotX, rotY, rotZ);
        SkyBox.transform.rotation = rot;
    }

    void TestSun() {
        if (sunPos.x < 0)
            sunPos.x += 1.0f;
        else if(sunPos.x > 1.0f)
            sunPos.x -= 1.0f;

        if (sunPos.y < 0)
            sunPos.y = 0;
        else if (sunPos.y > 1)
            sunPos.y = 1;
    }
    void RotateSun()
    {

        sunPos.x = (Time.unscaledTime / 10)%1.0f;

        sunPos.y = (Mathf.Sin(Time.unscaledTime / 30)/2)+0.5f;

    }

    void GenSunTexture() {
        shadowTexture ??= new Texture2D(200, 100);

        //Проходимся по каждому пикселю
        for (int pixX = 0; pixX < shadowTexture.width; pixX++) {
            float pixCoofX = (float)pixX / (float)shadowTexture.width;

            for (int pixY = 0; pixY < shadowTexture.height; pixY++) {
                float pixCoofY = (float)pixY / (float)shadowTexture.height;

                //Вычисляем освещенность пикселя
                //Если коофицент больше 0.5 то это темная сторона солнца
                float lightingCoof = 0;

                float lightX = 1 - Mathf.Abs(sunPos.x - pixCoofX);
                float lightXM = 1 - Mathf.Abs(sunPos.x - (pixCoofX - 1));
                float lightXP = 1 - Mathf.Abs(sunPos.x - (pixCoofX + 1));

                if (lightX < lightXM)
                    lightX = lightXM;
                if (lightX < lightXP)
                    lightX = lightXP;

                lightingCoof = lightX;

                float lightY = 1 - Mathf.Abs(sunPos.y - pixCoofY);
                lightY += 0.25f;

                lightingCoof = lightX;
                if (lightY < lightX)
                    lightingCoof = lightY;

                //Узнаем освещение за пределами полюсов
                float lightYP = 1 - Mathf.Abs(sunPos.y - (1 + (1 - pixCoofY)));
                float lightYM = 1 - Mathf.Abs(sunPos.y - pixCoofY * -1);
                lightYP += 0.25f;
                lightYM += 0.25f;

                if (lightingCoof < lightYM)
                    lightingCoof = lightYM;
                if (lightingCoof < lightYP)
                    lightingCoof = lightYP;


                lightingCoof = lightingCoof - 0.7f;
                lightingCoof *= 5;

                lightingCoof = 1 - lightingCoof;

                shadowTexture.SetPixel(pixX, pixY, new Color(0, 0, 0, lightingCoof));

            }
        }

        shadowTexture.Apply();
        shadowTexture.filterMode = FilterMode.Point;

        shadowMat.mainTexture = shadowTexture;
        
    }
}
