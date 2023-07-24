using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public class SpaceObjCTRL2 : MonoBehaviour
    {
        [SerializeField]
        Transform rotateObj;
        [SerializeField]
        Transform sunPos;
        [SerializeField]
        Transform sunRot;
        [SerializeField]
        Transform cameraPos;

        [SerializeField]
        Texture2D shadowTexture;
        [SerializeField]
        Material shadowMat;


        private void Update()
        {
            RotateToCam();
            UpdateTextureShadow();

            AutoRotateSun();
        }

        void RotateToCam() {
            lookAtCamera();

            void lookAtCamera()
            {
                //Смотрим на камеру
                rotateObj.LookAt(cameraPos, transform.up);
            }
        }

        void AutoRotateSun() {
            sunRot.Rotate(Time.unscaledDeltaTime, Time.unscaledDeltaTime * 10, 0);
        }

        void UpdateTextureShadow()
        {
            //Если нет текстуры - создаем
            if (shadowTexture == null)
            {
                //качество текстуры зависит от размера планеты
                int size = Calc.GetSizeInt(Size.s1024) / 32;

                shadowTexture = new Texture2D(size * 2, size);
            }

            //Проходимся по каждому пикселю
            for (int pixX = 0; pixX < shadowTexture.width; pixX++)
            {
                float pixCoofX = (float)pixX / (float)shadowTexture.width;

                for (int pixY = 0; pixY < shadowTexture.height; pixY++)
                {
                    float pixCoofY = (float)pixY / (float)shadowTexture.height;

                    float lightSum = 0;
                    //Вычисляем направление света на планету
                    Vector3 vecLight = this.sunPos.transform.position - rotateObj.position;
                    float distLight = vecLight.magnitude;
                    vecLight.Normalize();

                    Vector3 vecCam = gameObject.transform.position - cameraPos.position;

                    //Находим угл между направлением на камеру и направлением на звезду
                    float AngleForvard = Vector3.Angle(rotateObj.transform.forward, vecLight);

                    float AngleUp = Vector3.Angle(rotateObj.transform.up, vecLight);
                    float AngleR = Vector3.Angle(rotateObj.transform.right, vecLight);

                    Vector2 sunPos = new Vector2();

                    float intensiveR = AngleR / 90;
                    float intensiveUp = 0.5f;

                    float coofU = 90 - AngleUp;
                    float coofF = AngleForvard;

                    //Перед звездой
                    if (AngleForvard < 90)
                    {
                        coofF = AngleForvard;
                        if (coofF < 45)
                            coofF = 45 - coofF;
                        else coofF = coofF - 45;

                        //Сверху
                        if (AngleUp < 90)
                        {
                            if (AngleForvard < 45)
                            {
                                intensiveR = AngleR - (coofF / 45) * AngleForvard;
                                intensiveR += AngleR + (coofF / 45) * AngleForvard;
                                intensiveR /= 2;
                            }
                            else {
                                //Справа
                                if (AngleR < 90)
                                    intensiveR = AngleR - (coofF / 45) * coofU;
                                //Слева
                                else
                                    intensiveR = AngleR + (coofF / 45) * coofU;
                            }
                        }
                        //Снизу
                        else {
                            if (AngleForvard < 45)
                            {
                                intensiveR = AngleR + (coofF / 45) * AngleForvard;
                                intensiveR += AngleR - (coofF / 45) * AngleForvard;
                                intensiveR /= 2;
                            }
                            else {
                                if (AngleR < 90)
                                    intensiveR = AngleR + (coofF / 45) * coofU;
                                else
                                    intensiveR = AngleR - (coofF / 45) * coofU;
                            }
                        }
                        intensiveR /= 90;
                        intensiveR -= 1;
                        intensiveR /= 4;

                        intensiveUp = (AngleUp / 90)-1.0f;
                        intensiveUp /= 2;
                        intensiveUp += 0.5f;
                        intensiveUp = 1 - intensiveUp;

                        //Сверху
                        
                    }
                    //Отвернуты от светила
                    else {
                        float angleF = AngleForvard;
                        angleF = (angleF - 180) * -1;
                        coofF = angleF;
                        if (coofF < 45)
                            coofF = 45 - coofF;
                        else coofF = coofF - 45;
                        

                        //Сверху
                        if (AngleUp < 90)
                        {
                            if (angleF < 45)
                            {
                                intensiveR = AngleR - (coofF / 45) * angleF;
                                intensiveR += AngleR + (coofF / 45) * angleF;
                                intensiveR /= 2;
                            }
                            else {
                                //Справа
                                if (AngleR < 90)
                                    intensiveR = AngleR - (coofF / 45) * coofU;
                                //Слева
                                else
                                    intensiveR = AngleR + (coofF / 45) * coofU;
                            }
                        }
                        //Снизу
                        else
                        {
                            if (angleF < 45)
                            {
                                intensiveR = AngleR + (coofF / 45) * angleF;
                                intensiveR += AngleR - (coofF / 45) * angleF;
                                intensiveR /= 2;
                            }
                            else {
                                if (AngleR < 90)
                                    intensiveR = AngleR + (coofF / 45) * coofU;
                                else
                                    intensiveR = AngleR - (coofF / 45) * coofU;
                            }
                        }
                        intensiveR /= 90;
                        intensiveR += 1;
                        intensiveR /= 4;
                        intensiveR = 1 - intensiveR;

                        intensiveUp = (AngleUp / 90) - 1.0f;
                        intensiveUp /= 2;
                        intensiveUp += 0.5f;
                        intensiveUp = 1 - intensiveUp;
                    }

                    sunPos = new Vector2(intensiveR, intensiveUp);

                    //Debug.Log("Angle: Forvard " + AngleForvard + " UP" + AngleUp + " R" + AngleR + " |sunPos:" + sunPos + "intensiveR: " + intensiveR);

                    lightSum += GetLightingPixel(sunPos, new Vector2(pixCoofX, pixCoofY));

                    lightSum = 1 - lightSum;
                    shadowTexture.SetPixel(pixX, pixY, new Color(0, 0, 0, lightSum));
                }
            }

            shadowTexture.Apply();
            shadowTexture.filterMode = FilterMode.Point;

            shadowMat.mainTexture = shadowTexture;

            float GetLightingPixel(Vector2 sunPos, Vector2 pixCoof)
            {
                //Вычисляем освещенность пикселя
                float lightingCoof = 0;

                float lightX = 1 - Mathf.Abs(sunPos.x - pixCoof.x);
                float lightXM = 1 - Mathf.Abs(sunPos.x - (pixCoof.x - 1));
                float lightXP = 1 - Mathf.Abs(sunPos.x - (pixCoof.x + 1));

                if (lightX < lightXM)
                    lightX = lightXM;
                if (lightX < lightXP)
                    lightX = lightXP;

                //lightX -= 0.05f;

                float lightY = 1 - Mathf.Abs(sunPos.y - pixCoof.y);
                lightY += 0.25f;

                lightingCoof = lightX;
                if (lightY < lightX)
                    lightingCoof = lightY;

                //Узнаем освещение за пределами полюсов
                float lightYP = 1 - Mathf.Abs(sunPos.y - (1 + (1 - pixCoof.y)));
                float lightYM = 1 - Mathf.Abs(sunPos.y - pixCoof.y * -1);
                lightYP += 0.25f;
                lightYM += 0.25f;

                if (lightingCoof < lightYM)
                    lightingCoof = lightYM;
                if (lightingCoof < lightYP)
                    lightingCoof = lightYP;


                lightingCoof = lightingCoof - 0.7f;
                lightingCoof *= 10;

                return lightingCoof;
            }
        }
    }
}
