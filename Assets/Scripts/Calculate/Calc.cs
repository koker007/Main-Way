using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calc
{
    public class Array {
        public static int[] unite(int[] a, int[] b) {
            int[] result = new int[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++) {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }

        public static Vector2[] unite(Vector2[] a, Vector2[] b)
        {
            Vector2[] result = new Vector2[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++)
            {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }
        public static Vector3[] unite(Vector3[] a, Vector3[] b)
        {
            Vector3[] result = new Vector3[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++)
            {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }

        public static void changeEvery(ref int[] array, int valueAdd) {
            if (array == null)
                return;

            for (int num = 0; num > array.Length; num++) {
                array[num] += valueAdd;
            }
        }

        public static int[] changeEvery(int[] array, int valueAdd)
        {
            if (array == null)
                return array;

            int[] arrayNew = new int[array.Length];

            for (int num = 0; num < array.Length; num++)
            {
                arrayNew[num] = array[num] + valueAdd;
            }

            return arrayNew;
        }
    }

    public class Mesh {
        public static int[] DelZeroTriangles(int[] trianglesTest) {
            List<int> resultList = new List<int>();

            for (int id = 0; id < trianglesTest.Length; id += 3) {
                if (id == 73710) {
                    float test = 0;
                }

                if (trianglesTest[id + 0] == trianglesTest[id + 1] ||
                    trianglesTest[id + 1] == trianglesTest[id + 2] ||
                    trianglesTest[id + 2] == trianglesTest[id + 0]) {
                    continue;
                }

                resultList.Add(trianglesTest[id + 0]);
                resultList.Add(trianglesTest[id + 1]);
                resultList.Add(trianglesTest[id + 2]);

            }

            return resultList.ToArray();
        }

        public static Vector2[] MergeVec2(Vector2[] vec2_0, Vector2[] vec2_1, Vector2[] vec2_2, Vector2[] vec2_3, Vector2[] vec2_4, Vector2[] vec2_5, Vector2[] vec2_6, Vector2[] vec2_7) {
            List<Vector2> resultList = new List<Vector2>();

            if (vec2_0 != null) {
                for (int x = 0; x < vec2_0.Length; x++)
                    resultList.Add(vec2_0[x]);
            }
            if (vec2_1 != null)
            {
                for (int x = 0; x < vec2_1.Length; x++)
                    resultList.Add(vec2_1[x]);
            }
            if (vec2_2 != null)
            {
                for (int x = 0; x < vec2_2.Length; x++)
                    resultList.Add(vec2_2[x]);
            }
            if (vec2_3 != null)
            {
                for (int x = 0; x < vec2_3.Length; x++)
                    resultList.Add(vec2_3[x]);
            }
            if (vec2_4 != null)
            {
                for (int x = 0; x < vec2_4.Length; x++)
                    resultList.Add(vec2_4[x]);
            }
            if (vec2_5 != null)
            {
                for (int x = 0; x < vec2_5.Length; x++)
                    resultList.Add(vec2_5[x]);
            }
            if (vec2_6 != null)
            {
                for (int x = 0; x < vec2_6.Length; x++)
                    resultList.Add(vec2_6[x]);
            }
            if (vec2_7 != null)
            {
                for (int x = 0; x < vec2_7.Length; x++)
                    resultList.Add(vec2_7[x]);
            }

            return resultList.ToArray();
        }
        public static Vector3[] MergeVec3(Vector3[] vec3_0, Vector3[] vec3_1, Vector3[] vec3_2, Vector3[] vec3_3, Vector3[] vec3_4, Vector3[] vec3_5, Vector3[] vec3_6, Vector3[] vec3_7) {
            List<Vector3> resultList = new List<Vector3>();

            if (vec3_0 != null)
            {
                for (int x = 0; x < vec3_0.Length; x++)
                    resultList.Add(vec3_0[x]);
            }
            if (vec3_1 != null)
            {
                for (int x = 0; x < vec3_1.Length; x++)
                    resultList.Add(vec3_1[x]);
            }
            if (vec3_2 != null)
            {
                for (int x = 0; x < vec3_2.Length; x++)
                    resultList.Add(vec3_2[x]);
            }
            if (vec3_3 != null)
            {
                for (int x = 0; x < vec3_3.Length; x++)
                    resultList.Add(vec3_3[x]);
            }
            if (vec3_4 != null)
            {
                for (int x = 0; x < vec3_4.Length; x++)
                    resultList.Add(vec3_4[x]);
            }
            if (vec3_5 != null)
            {
                for (int x = 0; x < vec3_5.Length; x++)
                    resultList.Add(vec3_5[x]);
            }
            if (vec3_6 != null)
            {
                for (int x = 0; x < vec3_6.Length; x++)
                    resultList.Add(vec3_6[x]);
            }
            if (vec3_7 != null)
            {
                for (int x = 0; x < vec3_7.Length; x++)
                    resultList.Add(vec3_7[x]);
            }

            return resultList.ToArray();
        }

        public static int[] MergeTriangles(
            int[] tri0, int vertCount0,
            int[] tri1, int vertCount1, 
            int[] tri2, int vertCount2, 
            int[] tri3, int vertCount3, 
            int[] tri4, int vertCount4, 
            int[] tri5, int vertCount5, 
            int[] tri6, int vertCount6, 
            int[] tri7, int vertCount7) {
            List<int> resultList = new List<int>();

            int olderCount = 0;
            if (tri0 != null) {
                for (int x = 0; x < tri0.Length; x++)
                {
                    resultList.Add(tri0[x] + olderCount);
                }
                olderCount += vertCount0;
            }
            if (tri1 != null) {
                for (int x = 0; x < tri1.Length; x++) {
                    resultList.Add(tri1[x] + olderCount);
                }
                olderCount += vertCount1;
            }

            if (tri2 != null)
            {
                for (int x = 0; x < tri2.Length; x++)
                {
                    resultList.Add(tri2[x] + olderCount);
                }
                olderCount += vertCount2;
            }

            if (tri3 != null)
            {
                for (int x = 0; x < tri3.Length; x++)
                {
                    resultList.Add(tri3[x] + olderCount);
                }
                olderCount += vertCount3;
            }

            if (tri4 != null)
            {
                for (int x = 0; x < tri4.Length; x++)
                {
                    resultList.Add(tri4[x] + olderCount);
                }
                olderCount += vertCount4;
            }

            if (tri5 != null)
            {
                for (int x = 0; x < tri5.Length; x++)
                {
                    resultList.Add(tri5[x] + olderCount);
                }
                olderCount += vertCount5;
            }

            if (tri6 != null)
            {
                for (int x = 0; x < tri6.Length; x++)
                {
                    resultList.Add(tri6[x] + olderCount);
                }
                olderCount += vertCount6;
            }

            if (tri7 != null)
            {
                for (int x = 0; x < tri7.Length; x++)
                {
                    resultList.Add(tri7[x] + olderCount);
                }
                olderCount += vertCount7;
            }

            return resultList.ToArray();
        }
    }

    public static int Nolmalize(float value) {
        int ret = 0;
        if (value > 0) {
            ret = 1;
        }
        else if (value < 0) {
            ret = -1;
        }
        return ret;
    }
    public static float CutFrom0To1(float value) {
        float ret = value;
        if (ret > 1) {
            ret = 1;
        }
        else if (ret < 0) {
            ret = 0;
        }

        return ret;
    }

    public static float FixRandom(float fixNum, float min, float max) {
        float scale = Mathf.Abs(max - min);
        float perlinPos = fixNum / 3;
        float ret = Mathf.PerlinNoise(perlinPos, Mathf.PI);

        ret *= 100;
        ret = ret % 1;
        ret = ret * scale;
        ret += fixNum % scale;
        if (ret > scale) {
            ret -= scale;
        }

        if (min < max)
        {
            ret = min + ret;
        }
        else {
            ret = max + ret;
        }

        return ret;
    }


    //Получить число на основе сида и второго компонента
    public static float GetSeedNum(string Seed, int part)
    {
        float result = 0;

        float numerator =  100;
        float denominator = 1;

        bool isSum = true;

        float plus = Mathf.Sin(Mathf.PI * part) * 43758.5453123f;

        //получить рандомное число на основе сида
        for (int x = 0; x < Seed.Length; x++) {

            int num = part + x;

            num += (int)(Mathf.Sin(num * 747)*43758.5453123f);

            //число должно быть положительным
            //чтобы не вышли за рамки массива
            num = Mathf.Abs(num);
            num = num % Seed.Length;

            //Делитель равняется новому значению
            denominator = Seed[num];

            //Вычисляем
            float division = numerator / denominator;

            if (isSum)
                result += (Mathf.Sin(Mathf.PI * division) * 43758.5453123f)/10000;
            else
                result -= (Mathf.Sin(Mathf.PI * division) * 43758.5453123f)/10000;

            //убираем целое
            result %= 1;

            //Меняем переключаетель вычитания или прибавления
            isSum = !isSum;

            //Вычисления выполненны
            //Делитель становится числителем
            numerator = denominator;
        }

        result += plus;
        result %= 1;

        //Когда результат есть
        return result;
    }

    public static Size GetSize(float size) {
        if (size >= 81920) return Size.s8192o;
        if (size >= 40960) return Size.s4096o;
        if (size >= 20480) return Size.s2048o;
        if (size >= 10240) return Size.s1024o;
        if (size >= 5120) return Size.s512o;
        if (size >= 2560) return Size.s256o;
        if (size >= 1280) return Size.s128o;
        if (size >= 640) return Size.s64o;
        if (size >= 320) return Size.s32o;
        if (size >= 160) return Size.s16o;
        if (size >= 80) return Size.s8o;
        if (size >= 40) return Size.s4o;
        if (size >= 20) return Size.s2o;
        else return Size.s1o;
    }

    public static int GetSizeInt(Size size)
    {
        if ((int)size >= 14) return 81920;
        if ((int)size >= 13) return 40960;
        if ((int)size >= 12) return 20480;
        if ((int)size >= 11) return 10240;
        if ((int)size >= 10) return 5120;
        if ((int)size >= 9) return 2560;
        if ((int)size >= 8) return 1280;
        if ((int)size >= 7) return 640;
        if ((int)size >= 6) return 320;
        if ((int)size >= 5) return 160;
        if ((int)size >= 4) return 80;
        if ((int)size >= 3) return 40;
        if ((int)size >= 2) return 20;
        else return 10;
    }
}

//Класс линии
public class Line {
    Vector3 pointA = new Vector3();
    Vector3 pointB = new Vector3();

    public Vector3 PointA {
        get
        {
            return pointA;
        }
    }
    public Vector3 PointB {
        get {
            return pointB;
        }
    }

    public Line(Vector3 pointA, Vector3 pointB) {
        this.pointA = pointA;
        this.pointB = pointB;
    }
    public Line(Vector3 pointB) {
        this.pointA = new Vector3();
        this.pointB = pointB;
    }

    public Vector3 vector {
        get
        {
            return pointB - pointA;
        }
    }

    /// <summary>
    /// Получить кратчайшие точки между прямыми
    /// </summary>
    /// <returns></returns>
    static public Line GetNearestLine(Line line1, Line line2) {


        Vector3 vec1 = line1.vector;
        Vector3 vec2 = line2.vector;

        //Создаем плоскость паралельную 1 линии
        Plane plane2 = new Plane(line2.pointA, line2.pointB, line2.pointA + vec1);
        //Получаем нормаль плоскости 2
        Vector3 plane2Normal = plane2.normal;

        //Нужно найти точку соприкосновения от линии1А по нормали к плоскости2
        //Создаем луч направленный на плоскость
        Ray rayToPlane2Normal = new Ray(line1.pointA, plane2Normal);
        float distEnderNormalPlane2 = 0;
        bool enter = plane2.Raycast(rayToPlane2Normal, out distEnderNormalPlane2);

        //Узнаем координаты точки соприкосновения на плоскости plane2
        Vector3 pointF2 = line1.pointA + plane2Normal * distEnderNormalPlane2;

        Debug.Log("Point: " + pointF2);

        //Теперь создаем плоскость где линия 1 может быть нормалью
        Plane plane2normLine1 = new Plane(line2.pointA, line2.pointB, line2.pointA + plane2Normal);
        //Создаем луч направленный на плоскость
        Ray rayToPlane2line1 = new Ray(pointF2, pointF2 + vec1);
        float distEnderPlane2line = 0;
        bool enter2 = plane2.Raycast(rayToPlane2Normal, out distEnderPlane2line);

        //Смещаем координаты F2 на line2
        pointF2 = pointF2 + distEnderPlane2line * vec1;

        Vector3 pointF1 = line1.pointA + distEnderPlane2line * vec1;


        Debug.Log("PointF1: " + pointF1 + " PointF2: " + pointF2);

        Line resultLine = new Line(new Vector3(0,0,1));
        return resultLine;
    }

    static public Line GetNearestLine(Line line1, Line line2, int cycle)
    {

        //Берем на этих двух прямых по 2 рандомные точки
        Vector3 vec1 = line1.vector;
        Vector3 vec2 = line2.vector;

        Vector3 pointA = line1.PointA;
        Vector3 pointB = line2.PointA;

        float stepA = line1.vector.magnitude;
        float stepB = line2.vector.magnitude;

        //Уменьшаем растояние через цикл
        for (int num = cycle; num > 0; num--) {
            stepA *= 0.90f;
            stepB *= 0.90f;

            Vector3 pointAP = pointA + vec1 * stepA;
            Vector3 pointAM = pointA - vec1 * stepA;
            Vector3 pointBP = pointB + vec2 * stepB;
            Vector3 pointBM = pointB - vec2 * stepB;

            //Узнаем растояние каждого из 4-х движений
            float distPP = Vector3.Distance(pointAP, pointBP);
            float distPM = Vector3.Distance(pointAP, pointBM);
            float distMP = Vector3.Distance(pointAM, pointBP);
            float distMM = Vector3.Distance(pointAM, pointBM);

            //ищем лучшее
            float best = distPP;
            if (best > distPM) best = distPM;
            if (best > distMP) best = distMP;
            if (best > distMM) best = distMM;

            //Применяем изменения лучшего
            if (best == distPP) {
                pointA = pointAP;
                pointB = pointBP;
            }
            else if (best == distPM)
            {
                pointA = pointAP;
                pointB = pointBM;
            }
            else if (best == distMP)
            {
                pointA = pointAM;
                pointB = pointBP;
            }
            else if (best == distMM)
            {
                pointA = pointAM;
                pointB = pointBM;
            }
        }

        Line resultLine = new Line(pointA, pointB);

        return resultLine;
    }
}

public class Boolean
{
    public bool value;
}
