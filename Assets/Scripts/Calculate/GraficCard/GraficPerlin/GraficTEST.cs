using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

namespace Testing {
    public class GraficTEST : MonoBehaviour
    {
        [SerializeField]
        Texture2D texture2D;
        [SerializeField]
        RawImage TestRawImage;

        [SerializeField]
        int visualY = 0;
        [SerializeField]
        float scaleAll = 1;
        [SerializeField]
        float scaleX = 32;
        [SerializeField]
        float scaleY = 32;
        [SerializeField]
        float scaleZ = 32;

        [SerializeField]
        int octaves = 5;
        [SerializeField]
        float freq = 2;

        [SerializeField]
        float offsetX = 25423;
        [SerializeField]
        float offsetY = 13247;
        [SerializeField]
        float offsetZ = 32154;

        [SerializeField]
        float intensive = 0;

        [SerializeField]
        bool NeedPerlin3dRules = false;


        // Start is called before the first frame update
        void Start()
        {
            //InvokeRepeating("NeedTest", 1.0f, 1.0f);
        }

        // Update is called once per frame
        void Update()
        {
            NeedTest();
        }

        void NeedTest() {
            if (NeedPerlin3dRules) {
                GenPerlin3DRule();
            }

            if (TestRawImage != null) {
                TestRawImage.texture = texture2D;
            }
        }

        void GenPerlin3DRule() {
            //NeedPerlin3dRules = false;

            //Создаем правило блока
            BiomeData.GenRule[] genRule = new BiomeData.GenRule[2];

            genRule[0].blockID = 1;
            genRule[0].scaleX = scaleX;
            genRule[0].scaleY = scaleY;
            genRule[0].scaleZ = scaleZ;

            genRule[0].scaleAll = scaleAll;
            genRule[0].octaves = octaves;
            genRule[0].freq = freq;

            genRule[0].intensive = intensive;

            genRule[0].offsetX = offsetX;
            genRule[0].offsetY = offsetY;
            genRule[0].offsetZ = offsetZ;

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            texture2D = new Texture2D(Chank.Size * 2, Chank.Size * 2);

            float[,] perlinY = new float[texture2D.width, texture2D.height];

            int chankXMax = texture2D.width / Chank.Size;
            int chankZMax = texture2D.height / Chank.Size;

            GraficData.Perlin3DRules[,] perlin3DRules = new GraficData.Perlin3DRules[chankXMax, chankZMax];
            for (int chankX = 0; chankX < perlin3DRules.GetLength(0); chankX++) {
                float regionX = chankX / (float)chankXMax;

                for (int chankZ = 0; chankZ < perlin3DRules.GetLength(1); chankZ++) {
                    float regionZ = chankZ / (float)chankZMax;

                    perlin3DRules[chankX, chankZ] = new GraficData.Perlin3DRules(genRule, texture2D.width, 0, texture2D.height, regionX, 0, regionZ);
                    perlin3DRules[chankX, chankZ].Calculate();

                    for (int x = 0; x < Chank.Size; x++)
                    {
                        int pixX = chankX * Chank.Size + x;
                        for (int z = 0; z < Chank.Size; z++)
                        {
                            int pixZ = chankZ * Chank.Size + z;

                            float perlin = perlin3DRules[chankX, chankZ].result[x, visualY, z, 0];

                            perlinY[pixX, pixZ] = perlin;

                            if (perlin > 0.5f)
                            {
                                texture2D.SetPixel(pixX, pixZ, new Color(1, 1, 1));
                            }
                            else
                            {
                                texture2D.SetPixel(pixX, pixZ, new Color(0, 0, 0));
                            }

                            if (x == texture2D.width - 1 && z == 0)
                                texture2D.SetPixel(pixX, pixZ, new Color(1, 0, 0));
                        }
                    }
                }
            }

            stopwatch.Stop();

            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();

            UnityEngine.Debug.Log("Testing Perlin3DRule" +
                " stopwatch: " + stopwatch.ElapsedMilliseconds +
                " RulesMax:" + genRule.Length +
                " blockID:" + genRule[0].blockID +
                " scaleX:" + genRule[0].scaleX +
                " scaleY:" + genRule[0].scaleY +
                " scaleZ:" + genRule[0].scaleZ +
                " scaleALL:" + genRule[0].scaleAll +
                " octaves:" + genRule[0].octaves +
                " freq:" + genRule[0].freq) ;
        }
    }
}
