using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//’ранит данные вычислений видео карты
public class GraficData : MonoBehaviour
{
    static int LastPerlinID = 0;
    static int LastPerlin2DID = 0;
    static int LastVector3ID = 0;

    const int IDMax = 1000;
    static public Perlin[] dataPerlin = new Perlin[IDMax];
    static public Perlin2D[] dataPerlin2D = new Perlin2D[IDMax];

    //информаци€ типа перлина
    public class Perlin{
        public int id = 0;
        public bool calculated = false;

        public float[,,] result = new float[8, 8, 8];


        public float scale = 1;
        public float frequency = 1;

        public float offsetX = 0;
        public float offsetY = 0;
        public float offsetZ = 0;

        public int octaves = 1;
        public bool best = false;

        public Perlin(float scale, float frequency, float offsetX, float offsetY, float offsetZ, int octaves, bool best) {
            id = LastPerlinID;
            LastPerlinID++;

            //≈сли id стало больше 1000 то заполн€ем данные заного
            if (LastPerlinID >= IDMax)
                LastPerlinID = 0;

            this.scale = scale;
            this.frequency = frequency;

            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.offsetZ = offsetZ;

            this.octaves = octaves;
            this.best = best;

            dataPerlin[id] = this;
        }

        //¬ычислить перлина
        public bool Calculate() {
            if (calculated) return calculated;

            GraficPerlin.main.calculate(this);

            return calculated;
        }
    }

    public class Perlin2D
    {
        public const float factor = 0.875170906246f;

        public int id = 0;
        public bool calculated = false;

        public float[,,] result = new float[32, 32, 1];


        public float scale = 1;
        public float scaleX = 1;
        public float scaleY = 1;
        public float scaleZ = 1;

        public float frequency = 1;

        public float offsetX = 0;
        public float offsetY = 0;
        public float offsetZ = 0;

        public int octaves = 1;

        public int repeatX; //„ерез сколько пикселей текстура повтор€етс€ по X
        public int repeatY; //„ерез сколько пикселей текстура повтор€етс€ по Y
        public float regionX; //регион старта от 0 до 1
        public float regionY;

        public Perlin2D(float scaleX, float scaleY, float scaleZ, float frequency, float offsetX, float offsetY, float offsetZ, int octaves, int repeatX, int repeatY, float regionX, float regionY)
        {
            id = LastPerlin2DID;
            LastPerlin2DID++;

            //≈сли id стало больше 1000 то заполн€ем данные заного
            if (LastPerlin2DID >= IDMax)
                LastPerlin2DID = 0;

            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.scaleZ = scaleZ;
            this.frequency = frequency;

            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.offsetZ = offsetZ;

            this.octaves = octaves;

            this.repeatX = repeatX;
            this.repeatY = repeatY;

            this.regionX = regionX;
            this.regionY = regionY;

            dataPerlin2D[id] = this;
        }

        //¬ычислить перлина
        public bool Calculate()
        {
            if (calculated) return calculated;

            GraficPerlin2D.main.calculate(this);

            return calculated;
        }

        static public float[,] GetArrayMap(int mapSizeX, int mapSizeY, float ScaleX, float ScaleY, float ScaleZ, float Freq, float OffSetX, float OffSetY, float OffSetZ, int Octaves, bool TimeX, bool TimeZ) {
            //—оздаем новый массив
            float[,] arrayMap = new float[mapSizeX, mapSizeY];

            //»щем фактор чанка
            float FactorChankX = (factor / ScaleX) * 32;
            float FactorChankY = (factor / ScaleY) * 32;
            float FactorChankZ = (factor / ScaleZ) * 32;

            //ќпредел€ем количество чанков
            int chankXMax = mapSizeX / 32;
            int chankXremain = mapSizeX % 32;
            if (chankXremain > 0)
                chankXMax++;

            int chankYMax = mapSizeY / 32;
            int chankYremain = mapSizeY % 32;
            if (chankYremain > 0)
                chankYMax++;

            for (int chankX = 0; chankX < chankXMax; chankX++)
            {
                int chankPixelStartX = chankX * 32;

                for (int chankY = 0; chankY < chankYMax; chankY++)
                {
                    int chankPixelStartY = chankY * 32;

                    float offSetX = OffSetX + FactorChankX * chankX;
                    float offSetY = OffSetY + FactorChankY * chankY;
                    float offSetZ = OffSetZ;

                    if (TimeZ)
                        offSetZ += Time.time * 0.1f;

                    if (TimeX)
                        offSetX += Time.time * 0.1f;

                    float regionX = (chankX * 32) / (float)mapSizeX;
                    float regionY = (chankY * 32) / (float)mapSizeY;

                    Perlin2D dataPerlin2D = new Perlin2D(ScaleX, ScaleY, ScaleZ, Freq, offSetX, offSetY, offSetZ, Octaves, mapSizeX, mapSizeY, regionX, regionY);
                    dataPerlin2D.Calculate();

                    //≈сли крайний чанк с остатком
                    int maxX = 32;
                    int maxY = 32;
                    if (chankX == chankXMax - 1 && chankXremain > 0)
                        maxX = chankXremain;
                    if (chankY == chankYMax - 1 && chankYremain > 0)
                        maxY = chankYremain;

                    //«апихиваем данные в текстуру
                    for (int x = 0; x < maxX; x++)
                    {
                        int posMapX = chankPixelStartX + x;
                        for (int y = 0; y < maxY; y++)
                        {
                            int posMapY = chankPixelStartY + y;
                            arrayMap[posMapX, posMapY] = dataPerlin2D.result[x, y, 0];
                        }
                    }
                }
            }
            return arrayMap;
        }
    }

    //информаци€ о размерах звезд
    public class GalaxyStar {
        public const int arrayCount = 64;
        public int id = 0;
        public bool needBaseCalc = true;
        public float timeLastCalc = 0;

        public GalaxyObjCtrl[] galaxyObjs = new GalaxyObjCtrl[arrayCount];

        public Vector3[] resultSize = new Vector3[arrayCount];
        public Quaternion[] resultRotate = new Quaternion[arrayCount];
        public Color[] resultColor = new Color[arrayCount];

        public Vector3[] basePosition = new Vector3[arrayCount];
        public float[] baseRotateSpeed = new float[arrayCount];
        public Color[] baseColor = new Color[arrayCount];
        public float[] baseStarSize = new float[arrayCount];
        public float[] baseStarBright = new float[arrayCount];

        const int sizeofResultSize = sizeof(float) * 3;
        const int sizeofResultRotate = sizeof(float) * 4;
        const int sizeofResultColor = sizeof(float) * 4;
        const int sizeofBasePos = sizeof(float) * 3;
        const int sizeofBaseSpeedRot360 = sizeof(float);
        const int sizeOfBaseColor = sizeof(float) * 4;
        const int sizeOfBaseStarSize = sizeof(float);
        const int sizeOfBaseStarBright = sizeof(float);

        public ComputeBuffer bufferResultSize = new ComputeBuffer(arrayCount, sizeofResultSize);
        public ComputeBuffer bufferResultRotate = new ComputeBuffer(arrayCount, sizeofResultRotate);
        public ComputeBuffer bufferResultColor = new ComputeBuffer(arrayCount, sizeofResultColor);
        public ComputeBuffer bufferBasePos = new ComputeBuffer(arrayCount, sizeofBasePos);
        public ComputeBuffer bufferBaseSpeedRot360 = new ComputeBuffer(arrayCount, sizeofBaseSpeedRot360);
        public ComputeBuffer bufferBaseColor = new ComputeBuffer(arrayCount, sizeOfBaseColor);
        public ComputeBuffer bufferBaseStarSize = new ComputeBuffer(arrayCount, sizeOfBaseStarSize);
        public ComputeBuffer bufferBaseStarBright = new ComputeBuffer(arrayCount, sizeOfBaseStarBright);


        public Vector3 cameraPos = new Vector3();
        public float time = 0;

        public GalaxyStar(Vector3 cameraPos, float time) {
            id = LastPerlinID;
            LastPerlinID++;

            //≈сли id стало больше 1000 то заполн€ем данные заного
            if (LastPerlinID >= IDMax)
                LastPerlinID = 0;

            this.cameraPos = cameraPos;
            this.time = time;

        }
        ~GalaxyStar() {
            //¬ысвободить видео пам€ть
            Dispose();
        }

        //¬ысвободить видео пам€ть
        public void Dispose() {
            bufferResultSize.Dispose();
            bufferResultRotate.Dispose();
            bufferResultColor.Dispose();
            bufferBasePos.Dispose();
            bufferBaseSpeedRot360.Dispose();
            bufferBaseColor.Dispose();
            bufferBaseStarSize.Dispose();
            bufferBaseStarBright.Dispose();
        }

        //»нициализировать данные перед использованием
        public void iniData() {

            //≈сли базовые данные требуют загрузки
            if (needBaseCalc)
            {
                needBaseCalc = false;

                IniBase();
            }

            IniResult();


            void IniBase() {
                for(int x = 0; x < galaxyObjs.Length; x++) {
                    if (galaxyObjs[x] == null) 
                        continue;

                    basePosition[x] = galaxyObjs[x].data.cell.pos;
                    baseRotateSpeed[x] = galaxyObjs[x].data.time360Rotate;
                    baseColor[x] = galaxyObjs[x].data.color;
                    baseStarSize[x] = Calc.GetSizeInt(galaxyObjs[x].data.size);
                    baseStarBright[x] = galaxyObjs[x].data.bright;
                }

                bufferBasePos.SetData(basePosition);
                bufferBaseSpeedRot360.SetData(baseRotateSpeed);
                bufferBaseColor.SetData(baseColor);
                bufferBaseStarSize.SetData(baseStarSize);
            }

            void IniResult() {
                for (int x = 0; x < galaxyObjs.Length; x++) {
                    if (galaxyObjs[x] == null)
                        continue;

                    resultColor[x] = galaxyObjs[x].data.color;
                    resultRotate[x] = galaxyObjs[x].transform.rotation;
                    resultSize[x] = galaxyObjs[x].transform.localScale;

                }

                cameraPos = CameraGalaxy.main.transform.localPosition;
            }
        }

        //–аставить результаты вычислений
        public void SetResult() {
            for (int x = 0; x < galaxyObjs.Length; x++)
            {
                if (galaxyObjs[x] == null)
                    continue;

                galaxyObjs[x].Pixel.color = resultColor[x];
                galaxyObjs[x].transform.rotation = resultRotate[x];
                galaxyObjs[x].PixelMain.transform.localScale = resultSize[x];
            }
        }

        public void SetData(int index, GalaxyObjCtrl galaxyObj)
        {
            this.galaxyObjs[index] = galaxyObj;
        }
        

    }

    //—оздает меш стены
    public class BlockWall {
        public BlockForms blockForms;
        public Side side;
        //16*16 количество вокселей
        //5 количество сторон (лицева€ и боковые)
        //2 количество треугольников на каждую сторону
        //3 количество вершин на каждый треугольник
        //const int verticesCount = 16 * 16 * 5 * 2 * 3;


        //16*16 количество вокселей
        //3 количество сторон у воксел€ (лицева€, право, вверх)
        //2 количество треугольников на каждую сторону

        //прибавл€ем 16 нижних сторон + 16 левых сторон + общее количество сторон и умножаем это все на 2 количество треугольников в каждой стороне
        const int verticesCount = (16 + 16 + (16 * 16 * 3)) * 2 * 3;

        public BlockWall(BlockForms blockForms, Side side)
        {
            this.blockForms = blockForms;
            this.side = side;

            //создаем вершин на каждый воксель
            this.blockForms.vertices = new Vector3[verticesCount];
            this.blockForms.triangles = new int[verticesCount];

            //—оздаем тексель на каждый воксель
            this.blockForms.uv = new Vector2[verticesCount]; 
        }


    }
}
