using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class TypeLiquid
{

    static public Mesh[,,] meshs;

    public Data data = new Data();
    List<Texture2D[]> textures;

    public class Data
    {
        //Базовый цвет объекта
        public float colorHue = 0.0f;
        public float colorSaturation = 0.0f;
        public float colorValue = 0.0f;
        public float colorHueEnd = 0.0f;
        public float colorSaturationEnd = 0.0f;
        public float colorValueEnd = 0.0f;

        public float perlinScale = 1;
        public int perlinOctaves = 1;
        public float perlinScaleX = 1;
        public float perlinScaleY = 1;
        public float perlinScaleZ = 1;
        public int texturesMax = 10;
        public float animSpeed = 30;
        public float animSpeedX = 1;
        public float animSpeedY = 1;
        public float animSpeedZ = 4;
        public float animSizeX = 0.1f;
        public float animSizeY = 0.1f;
        public float animSizeZ = 0.1f;


    }

    public TypeLiquid()
    {
        iniMeshs();

        void iniMeshs()
        {
            //Если уже проинициализированно
            if (meshs != null)
                return;

            //Up //Down //Side
            meshs = new Mesh[16, 16, 6];

            for (int up = 0; up < 16; up++)
            {
                float voxUp = (up + 1) / 16.0f;
                for (int down = 0; down < 16; down++)
                {
                    if (down > up)
                        continue;

                    float voxDown = down / 16.0f;

                    meshs[up, down, 0] = getFace(voxUp, voxDown);
                    meshs[up, down, 1] = getBack(voxUp, voxDown);
                    meshs[up, down, 2] = getLeft(voxUp, voxDown);
                    meshs[up, down, 3] = getRight(voxUp, voxDown);
                    meshs[up, down, 4] = getUp(voxUp);
                    meshs[up, down, 5] = getDown(voxDown);
                }
            }

            Mesh getFace(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 0.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 0.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 0.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getBack(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(1.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(0.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getLeft(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[3] = new Vector3(0.0f, voxDown, 0.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getRight(float voxUp, float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(1.0f, voxDown, 0.0f);
                vertices[1] = new Vector3(1.0f, voxUp, 0.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, voxDown);
                uv[1] = new Vector2(0.0f, voxUp);
                uv[2] = new Vector2(1.0f, voxUp);
                uv[3] = new Vector2(1.0f, voxDown);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getUp(float voxUp)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxUp, 0.0f);
                vertices[1] = new Vector3(0.0f, voxUp, 1.0f);
                vertices[2] = new Vector3(1.0f, voxUp, 1.0f);
                vertices[3] = new Vector3(1.0f, voxUp, 0.0f);

                uv[0] = new Vector2(0.0f, 0.0f);
                uv[1] = new Vector2(0.0f, 1.0f);
                uv[2] = new Vector2(1.0f, 1.0f);
                uv[3] = new Vector2(1.0f, 0.0f);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
            Mesh getDown(float voxDown)
            {
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[4];
                Vector2[] uv = new Vector2[4];
                int[] triangles = new int[6];

                vertices[0] = new Vector3(0.0f, voxDown, 1.0f);
                vertices[1] = new Vector3(0.0f, voxDown, 0.0f);
                vertices[2] = new Vector3(1.0f, voxDown, 0.0f);
                vertices[3] = new Vector3(1.0f, voxDown, 1.0f);

                uv[0] = new Vector2(0.0f, 0.0f);
                uv[1] = new Vector2(0.0f, 1.0f);
                uv[2] = new Vector2(1.0f, 1.0f);
                uv[3] = new Vector2(1.0f, 0.0f);

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                return mesh;
            }
        }
    }

    public Texture2D GetTexture(int tick, Side side)
    {
        IniTextures();

        //Высчитываем нужный тик
        int numTick = tick % textures.Count;
        if (side == Side.face)
            return textures[numTick][0];
        else if (side == Side.back)
            return textures[numTick][1];
        else if (side == Side.left)
            return textures[numTick][2];
        else if (side == Side.right)
            return textures[numTick][3];
        else if (side == Side.up)
            return textures[numTick][4];
        else
            return textures[numTick][5];


        void IniTextures()
        {
            if (textures != null)
                return;


            textures = new List<Texture2D[]>();

            Color colorStart = Color.HSVToRGB(data.colorHue, data.colorSaturation, data.colorValue);
            Color colorEnd = Color.HSVToRGB(data.colorHueEnd, data.colorSaturationEnd, data.colorValueEnd);

            float offsetXStart = Random.Range(-999999, 999999);
            float offsetYStart = Random.Range(-999999, 999999);
            float offsetZStart = Random.Range(-999999, 999999);

            //Ищем минимальную скорость
            float minSpeed = 1;
            if (data.animSpeedX != 0)
                minSpeed = 1;
            else if (minSpeed > data.animSpeedY && data.animSpeedY != 0)
                minSpeed = data.animSpeedY;
            else if (minSpeed > data.animSpeedZ && data.animSpeedZ != 0)
                minSpeed = data.animSpeedZ;
            //Ищем минимальное время анимации
            float minTime = 1 / minSpeed;

            //Узнаем разницу во времени между 2-мя кадрами
            float timeOneFrame = minTime / data.texturesMax;

            float scaleX = data.perlinScale * data.perlinScaleX;
            float scaleY = data.perlinScale * data.perlinScaleY;
            float scaleZ = data.perlinScale * data.perlinScaleZ;

            //Перебираем все кадры цикла
            for (int frameNum = 0; frameNum < data.texturesMax; frameNum++)
            {
                float timeFrameNow = timeOneFrame * frameNum;

                float offSetX = Mathf.Sin(timeFrameNow * data.animSpeedX * Mathf.PI * 2) / data.animSizeX + offsetXStart;
                float offSetY = Mathf.Cos(timeFrameNow * data.animSpeedY * Mathf.PI * 2) / data.animSizeY + offsetYStart;
                float offSetZ = Mathf.Cos(timeFrameNow * data.animSpeedZ * Mathf.PI * 2) / data.animSizeZ + offsetZStart;

                //Получаем 3д шум
                float[,,] map = GraficData.PerlinCube.GetArrayMap(scaleX, scaleY, scaleZ, 3.0f, offSetX, offSetY, offSetZ, data.perlinOctaves);

                textures.Add(CreateTextures(map));
            }


            Texture2D[] CreateTextures(float[,,] perlin)
            {
                Texture2D[] textures = new Texture2D[6];

                textures[0] = new Texture2D(16, 16);
                textures[1] = new Texture2D(16, 16);
                textures[2] = new Texture2D(16, 16);
                textures[3] = new Texture2D(16, 16);
                textures[4] = new Texture2D(16, 16);
                textures[5] = new Texture2D(16, 16);

                //Face
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[x, y, 0];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[0].SetPixel(x, y, color);
                    }
                }
                //back
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[15 - x, y, 15];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[1].SetPixel(x, y, color);
                    }
                }
                //left
                for (int z = 0; z < perlin.GetLength(0); z++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[0, y, 15 - z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[2].SetPixel(z, y, color);
                    }
                }
                //right
                for (int z = 0; z < perlin.GetLength(0); z++)
                {
                    for (int y = 0; y < perlin.GetLength(1); y++)
                    {
                        float perl = perlin[15, y, z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[3].SetPixel(z, y, color);
                    }
                }
                //down
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int z = 0; z < perlin.GetLength(1); z++)
                    {
                        float perl = perlin[x, 0, 15 - z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[4].SetPixel(x, z, color);
                    }
                }
                //up
                for (int x = 0; x < perlin.GetLength(0); x++)
                {
                    for (int z = 0; z < perlin.GetLength(1); z++)
                    {
                        float perl = perlin[x, 15, z];
                        Color color = Color.Lerp(colorStart, colorEnd, perl);
                        textures[5].SetPixel(x, z, color);
                    }
                }

                //Применение изменений
                for (int num = 0; num < textures.Length; num++)
                {
                    textures[num].filterMode = FilterMode.Point;
                    textures[num].Apply();
                }

                return textures;
            }

            /*
            Texture2D[] CreateTextures() {
                Texture2D[] textures = new Texture2D[6];
                for (int num = 0; num < 6; num++) {
                    textures[num] = new Texture2D(16,16);

                    for (int x = 0; x < 16; x++) {
                        for (int y = 0; y < 16; y++) {
                            textures[num].SetPixel(x, y, Color.Lerp(colorStart, colorMax, Random.Range(0.0f, 1.0f)));
                        }
                    }
                    textures[num].filterMode = FilterMode.Point;
                    textures[num].Apply();
                }
                return textures;
            }
            */
        }
    }
    public void ClearTextures()
    {
        textures = null;
    }
    public Mesh GetMesh(Data data, bool face, bool back, bool left, bool right, bool up, bool down, int lvlUp, int lvlDown)
    {
        this.data = data;

        Mesh meshResult = new Mesh();

        meshResult.vertices = GraficCalc.main.mergeVector3(meshs[lvlUp, lvlDown, 0].vertices, meshs[lvlUp, lvlDown, 1].vertices, meshs[lvlUp, lvlDown, 2].vertices, meshs[lvlUp, lvlDown, 3].vertices, meshs[lvlUp, lvlDown, 4].vertices, meshs[lvlUp, lvlDown, 5].vertices);
        //meshResult.triangles = GraficCalc.main.mergeTriangleNum(wallFace.forms.triangles, wallBack.forms.triangles, wallRight.forms.triangles, wallLeft.forms.triangles, wallUp.forms.triangles, wallDown.forms.triangles);
        meshResult.uv = GraficCalc.main.mergeVector2(meshs[lvlUp, lvlDown, 0].uv, meshs[lvlUp, lvlDown, 1].uv, meshs[lvlUp, lvlDown, 2].uv, meshs[lvlUp, lvlDown, 3].uv, meshs[lvlUp, lvlDown, 4].uv, meshs[lvlUp, lvlDown, 5].uv);


        meshResult.subMeshCount = 6;
        /////////////////////////////////////////////////////////////
        //Нужно в данных треугольников надо сдвигать счет примитивов
        /////////////////////////////////////////////////////////////
        int addNum = 0;
        meshResult.SetTriangles(meshs[lvlUp, lvlDown, 0].triangles, 0);
        addNum += meshs[lvlUp, lvlDown, 0].vertices.Length;

        int[] trianglesBack = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 1].triangles, addNum);
        meshResult.SetTriangles(trianglesBack, 1);
        addNum += meshs[lvlUp, lvlDown, 1].vertices.Length;

        int[] trianglesRight = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 2].triangles, addNum);
        meshResult.SetTriangles(trianglesRight, 2);
        addNum += meshs[lvlUp, lvlDown, 2].vertices.Length;

        int[] trianglesLeft = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 3].triangles, addNum);
        meshResult.SetTriangles(trianglesLeft, 3);
        addNum += meshs[lvlUp, lvlDown, 3].vertices.Length;

        int[] trianglesUp = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 4].triangles, addNum);
        meshResult.SetTriangles(trianglesUp, 4);
        addNum += meshs[lvlUp, lvlDown, 4].vertices.Length;

        int[] trianglesDown = GraficCalc.main.addToInt(meshs[lvlUp, lvlDown, 5].triangles, addNum);
        meshResult.SetTriangles(trianglesDown, 5);

        return meshResult;

    }

    public Mesh GetMesh(bool face, bool back, bool left, bool right, bool up, bool down, int lvlUp, int lvlDown)
    {
        return GetMesh(this.data, face, back, left, right, up, down, lvlUp, lvlDown);
    }

    public void SaveTo(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string pathMainData = path + "/" + StrC.main + StrC.data + StrC.formatTXT;
        string pathPhysics = path + "/" + StrC.physics + StrC.formatTXT;

        saveMainDate();
        savePhysics();

        void saveMainDate()
        {
            //take all data of liquid block and save to string
            List<string> dataMain = new List<string>();

            //Color
            dataMain.Add(StrC.hue + StrC.SEPARATOR + data.colorHue);
            dataMain.Add(StrC.saturation + StrC.SEPARATOR + data.colorSaturation);
            dataMain.Add(StrC.value + StrC.SEPARATOR + data.colorValue);
            dataMain.Add(StrC.hue + StrC.rand + StrC.SEPARATOR + data.colorHue);
            dataMain.Add(StrC.saturation + StrC.rand + StrC.SEPARATOR + data.colorSaturationEnd);
            dataMain.Add(StrC.value + StrC.rand + StrC.SEPARATOR + data.colorValueEnd);

            //Noise
            dataMain.Add(StrC.perlin + StrC.scale + StrC.SEPARATOR + data.perlinScale);
            dataMain.Add(StrC.perlin + StrC.octaves + StrC.SEPARATOR + data.perlinOctaves);
            dataMain.Add(StrC.perlin + StrC.scale + StrC.x + StrC.SEPARATOR + data.perlinScaleX);
            dataMain.Add(StrC.perlin + StrC.scale + StrC.y + StrC.SEPARATOR + data.perlinScaleY);
            dataMain.Add(StrC.perlin + StrC.scale + StrC.z + StrC.SEPARATOR + data.perlinScaleZ);

            //Animation
            dataMain.Add(StrC.animation + StrC.lenght + StrC.SEPARATOR + data.texturesMax);
            dataMain.Add(StrC.animation + StrC.speed + StrC.SEPARATOR + data.animSpeed);
            dataMain.Add(StrC.animation + StrC.speed + StrC.x + StrC.SEPARATOR + data.animSpeedX);
            dataMain.Add(StrC.animation + StrC.speed + StrC.y + StrC.SEPARATOR + data.animSpeedY);
            dataMain.Add(StrC.animation + StrC.speed + StrC.z + StrC.SEPARATOR + data.animSpeedZ);
            dataMain.Add(StrC.animation + StrC.size + StrC.x + StrC.SEPARATOR + data.animSizeX);
            dataMain.Add(StrC.animation + StrC.size + StrC.y + StrC.SEPARATOR + data.animSizeY);
            dataMain.Add(StrC.animation + StrC.size + StrC.z + StrC.SEPARATOR + data.animSizeZ);

            File.WriteAllLines(pathMainData, dataMain.ToArray());

        }
        void savePhysics()
        {
            //take all data of liquid block and save to string
            List<string> dataPhysics = new List<string>();

            //dataPhysics.Add(StrC.viscosity + StrC.SEPARATOR);
        }
    }
    public void LoadFrom(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("Load Error Path not exist: " + path);
            return;
        }


        string pathMainData = path + "/" + StrC.main + StrC.data + StrC.formatTXT;
        string pathPhysics = path + "/" + StrC.physics + StrC.formatTXT;

        loadMainDate();

        void loadMainDate()
        {
            string[] dataMain;

            dataMain = File.ReadAllLines(pathMainData);

            KeyAndText[] keyAndTexts = KeyAndText.GetKATs(dataMain, StrC.SEPARATOR);

            foreach (KeyAndText keyAndText in keyAndTexts)
            {
                loadKaT(keyAndText);
            }

            void loadKaT(KeyAndText keyAndText)
            {
                //Color
                if (keyAndText.key == StrC.hue)
                    data.colorHue = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.saturation)
                    data.colorSaturation = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.value)
                    data.colorValue = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.hue + StrC.rand)
                    data.colorHueEnd = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.saturation + StrC.rand)
                    data.colorSaturationEnd = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.value + StrC.rand)
                    data.colorValueEnd = (float)System.Convert.ToDouble(keyAndText.text);

                //Noise
                else if (keyAndText.key == StrC.perlin + StrC.scale)
                    data.perlinScale = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.perlin + StrC.octaves)
                    data.perlinOctaves = System.Convert.ToInt32(keyAndText.text);
                else if (keyAndText.key == StrC.perlin + StrC.scale + StrC.x)
                    data.perlinScaleX = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.perlin + StrC.scale + StrC.y)
                    data.perlinScaleY = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.perlin + StrC.scale + StrC.z)
                    data.perlinScaleZ = (float)System.Convert.ToDouble(keyAndText.text);

                //Animation
                else if (keyAndText.key == StrC.animation + StrC.lenght)
                    data.texturesMax = System.Convert.ToInt32(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.speed)
                    data.animSpeed = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.speed + StrC.x)
                    data.animSpeedX = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.speed + StrC.y)
                    data.animSpeedY = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.speed + StrC.z)
                    data.animSpeedZ = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.size + StrC.x)
                    data.animSizeX = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.size + StrC.y)
                    data.animSizeY = (float)System.Convert.ToDouble(keyAndText.text);
                else if (keyAndText.key == StrC.animation + StrC.size + StrC.z)
                    data.animSizeZ = (float)System.Convert.ToDouble(keyAndText.text);



            }
        }
    }
}
